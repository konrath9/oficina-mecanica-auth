namespace OficinaMecanica.Auth;

// Reimplementacao isolada do algoritmo de validacao de CPF: este repositorio e um deploy
// independente (Lambda) e nao referencia o projeto Domain do repositorio principal.
public static class CpfValidator
{
    public static bool TryNormalizeEValidar(string? cpf, out string cpfNormalizado)
    {
        cpfNormalizado = string.Empty;

        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var digitos = new string(cpf.Where(char.IsDigit).ToArray());

        if (digitos.Length != 11 || digitos.Distinct().Count() == 1)
            return false;

        var numeros = digitos.Select(c => c - '0').ToArray();

        if (CalculaDigitoVerificador(numeros, 9) != numeros[9])
            return false;

        if (CalculaDigitoVerificador(numeros, 10) != numeros[10])
            return false;

        cpfNormalizado = digitos;
        return true;
    }

    private static int CalculaDigitoVerificador(int[] numeros, int quantidade)
    {
        var soma = 0;
        var multiplicador = quantidade + 1;

        for (var i = 0; i < quantidade; i++)
        {
            soma += numeros[i] * multiplicador;
            multiplicador--;
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
