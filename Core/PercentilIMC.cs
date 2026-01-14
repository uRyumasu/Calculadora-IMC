namespace CalculadoraIMC.Core;

public static class PercentilIMC
{
    public static bool DeveUsarPercentil(Program.Pessoa pessoa)
    {
        return pessoa.Idade < 18 && pessoa.Idade >= 2;
    }
    
    public static (double percentil, string classificacao) CalcularPercentil(Program.Pessoa pessoa, float imc)
    {
        if (!DeveUsarPercentil(pessoa))
            return (-1, "N/A");
        
        
        double zScore = CalcularZScore(pessoa.Idade, pessoa.sexo, imc);
        
        
        double percentil = ZScoreParaPercentil(zScore);
        
        
        string classificacao = ClassificarPercentil(percentil);
        
        return (percentil, classificacao);
    }
    
    private static double CalcularZScore(int idade, Program.Sexo sexo, float imc)
    {
        
        
        var (l, m, s) = ObterParametrosLMS(idade, sexo);
        
        
        if (Math.Abs(l) < 0.01) 
        {
            return Math.Log(imc / m) / s;
        }
        
        return (Math.Pow(imc / m, l) - 1) / (l * s);
    }
    
    private static (double l, double m, double s) ObterParametrosLMS(int idade, Program.Sexo sexo)
    {
        
        
        
        if (sexo == Program.Sexo.Masculino)
        {
            return idade switch
            {
                2 => (-1.8608, 16.5, 0.08929),
                3 => (-1.9449, 16.0, 0.09182),
                4 => (-2.0120, 15.6, 0.09428),
                5 => (-2.0568, 15.4, 0.09630),
                6 => (-2.0767, 15.3, 0.09816),
                7 => (-2.0738, 15.3, 0.10023),
                8 => (-2.0508, 15.4, 0.10270),
                9 => (-2.0122, 15.6, 0.10562),
                10 => (-1.9623, 15.9, 0.10889),
                11 => (-1.9047, 16.3, 0.11239),
                12 => (-1.8426, 16.7, 0.11595),
                13 => (-1.7788, 17.3, 0.11942),
                14 => (-1.7157, 17.9, 0.12267),
                15 => (-1.6551, 18.6, 0.12563),
                16 => (-1.5983, 19.2, 0.12824),
                17 => (-1.5458, 19.8, 0.13049),
                _ => (-1.5000, 20.5, 0.13244)
            };
        }
        else 
        {
            return idade switch
            {
                2 => (-1.6473, 16.2, 0.09025),
                3 => (-1.7526, 15.7, 0.09328),
                4 => (-1.8440, 15.4, 0.09634),
                5 => (-1.9186, 15.2, 0.09930),
                6 => (-1.9738, 15.1, 0.10216),
                7 => (-2.0085, 15.1, 0.10504),
                8 => (-2.0228, 15.2, 0.10809),
                9 => (-2.0179, 15.4, 0.11143),
                10 => (-1.9958, 15.7, 0.11517),
                11 => (-1.9590, 16.1, 0.11932),
                12 => (-1.9104, 16.6, 0.12380),
                13 => (-1.8530, 17.1, 0.12847),
                14 => (-1.7897, 17.6, 0.13313),
                15 => (-1.7236, 18.0, 0.13760),
                16 => (-1.6573, 18.5, 0.14175),
                17 => (-1.5929, 18.8, 0.14550),
                _ => (-1.5324, 19.2, 0.14881)
            };
        }
    }
    
    private static double ZScoreParaPercentil(double zScore)
    {
        
        
        
        
        zScore = Math.Max(-3.5, Math.Min(3.5, zScore));
        
        
        double percentil = 0.5 * (1 + Erf(zScore / Math.Sqrt(2))) * 100;
        
        return Math.Round(percentil, 1);
    }
    
    private static double Erf(double x)
    {
        
        const double a1 = 0.254829592;
        const double a2 = -0.284496736;
        const double a3 = 1.421413741;
        const double a4 = -1.453152027;
        const double a5 = 1.061405429;
        const double p = 0.3275911;
        
        int sign = x < 0 ? -1 : 1;
        x = Math.Abs(x);
        
        double t = 1.0 / (1.0 + p * x);
        double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);
        
        return sign * y;
    }
    
    private static string ClassificarPercentil(double percentil)
    {
        
        return percentil switch
        {
            < 3 => "Magreza",
            < 15 => "Abaixo",
            < 85 => "Normal",
            < 97 => "Sobrepeso",
            _ => "Obesidade"
        };
    }
    
    public static string ObterDescricaoCompleta(Program.Pessoa pessoa, float imc)
    {
        if (!DeveUsarPercentil(pessoa))
            return "Percentil não aplicável (usar IMC padrão)";
        
        var (percentil, classificacao) = CalcularPercentil(pessoa, imc);
        
        string genero = pessoa.sexo == Program.Sexo.Masculino ? "rapazes" : "raparigas";
        
        return $"Percentil {percentil:F1} para {genero} de {pessoa.Idade} anos - {classificacao}";
    }
}