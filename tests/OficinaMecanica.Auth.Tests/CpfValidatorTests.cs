namespace OficinaMecanica.Auth.Tests;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData("52998224725", "52998224725")]
    [InlineData("222.333.444-05", "22233344405")]
    public void TryNormalizeEValidar_CpfValido_DeveRetornarTrueENormalizado(string cpf, string esperado)
    {
        var valido = CpfValidator.TryNormalizeEValidar(cpf, out var normalizado);

        Assert.True(valido);
        Assert.Equal(esperado, normalizado);
    }

    [Theory]
    [InlineData("111.111.111-11")]
    [InlineData("123.456.789-00")]
    [InlineData("529.982.247-26")]
    [InlineData("1234567890")]
    [InlineData("")]
    [InlineData(null)]
    public void TryNormalizeEValidar_CpfInvalido_DeveRetornarFalse(string? cpf)
    {
        var valido = CpfValidator.TryNormalizeEValidar(cpf, out var normalizado);

        Assert.False(valido);
        Assert.Equal(string.Empty, normalizado);
    }
}
