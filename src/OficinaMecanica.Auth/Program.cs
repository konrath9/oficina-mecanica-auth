using Amazon.Lambda.AspNetCoreServer.Hosting;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OficinaMecanica.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("DB_CONNECTION_STRING nao configurada");
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException("JWT_SECRET_KEY nao configurada");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "OficinaMecanicaAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "OficinaMecanicaClients";

app.MapPost("/auth/login", async (LoginRequest request) =>
{
    if (!CpfValidator.TryNormalizeEValidar(request.Cpf, out var cpfNormalizado))
        return Results.Unauthorized();

    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(
        "SELECT id, ativo FROM clientes WHERE regexp_replace(documento, '[^0-9]', '', 'g') = @cpf",
        connection);
    command.Parameters.AddWithValue("cpf", cpfNormalizado);

    await using var reader = await command.ExecuteReaderAsync();
    if (!await reader.ReadAsync())
        return Results.Unauthorized();

    var clienteId = reader.GetGuid(0);
    var ativo = reader.GetBoolean(1);

    if (!ativo)
        return Results.Unauthorized();

    var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
    var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, clienteId.ToString()),
        new Claim(ClaimTypes.Role, "Cliente"),
        new Claim("perfil", "Cliente"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credenciais);

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new LoginResponse(tokenString));
});

app.Run();

public record LoginRequest(string Cpf);
public record LoginResponse(string Token);

// Necessario para o WebApplicationFactory dos testes de integracao.
public partial class Program { }
