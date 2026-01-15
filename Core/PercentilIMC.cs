namespace CalculadoraIMC.Core;

/// <summary>
/// Calcula percentis de IMC para crianças e adolescentes (2-18 anos) usando o método LMS da OMS
/// </summary>
public static class PercentilIMC
{
    // Verifica se devemos usar percentil
    public static bool DeveUsarPercentil(Program.Pessoa pessoa)
    {
        return pessoa.Idade < Constantes.IDADE_MAXIMA_PERCENTIL && 
               pessoa.Idade >= Constantes.IDADE_MINIMA_PERCENTIL;
    }
    
    // Calcula o percentil e classificação
    public static (double percentil, string classificacao) CalcularPercentil(Program.Pessoa pessoa, float imc)
    {
        if (!DeveUsarPercentil(pessoa))
            return (-1, "N/A");
        
        // Calcula Z-score usando método LMS
        double zScore = CalcularZScore(pessoa.Idade, pessoa.Sexo, imc);
        
        // Converte Z-score para percentil
        double percentil = ZScoreParaPercentil(zScore);
        
        // Classifica baseado no percentil
        string classificacao = ClassificarPercentil(percentil);
        
        return (percentil, classificacao);
    }
    
    // Calcula o Z-score usando o método LMS (Lambda-Mu-Sigma)
    private static double CalcularZScore(int idade, Program.Sexo sexo, float imc)
    {
        var (l, m, s) = ObterParametrosLMS(idade, sexo);
        
        // Se L ≈ 0, usa fórmula logarítmica (distribuição normal)
        if (Math.Abs(l) < 0.01) 
        {
            return Math.Log(imc / m) / s;
        }
        
        // Fórmula padrão do método LMS (distribuição Box-Cox)
        return (Math.Pow(imc / m, l) - 1) / (l * s);
    }
    
    // Obtem parametros da tabela WHO para idade e genero especifico
    private static (double l, double m, double s) ObterParametrosLMS(int idade, Program.Sexo sexo)
    {
        // Tabela LMS da OMS para rapazes
        if (sexo == Program.Sexo.Masculino)
        {
            return idade switch
            {
                2 => (-1.8608, 16.5, 0.08929), // 2 anos
                3 => (-1.9449, 16.0, 0.09182), // 3 anos
                4 => (-2.0120, 15.6, 0.09428), // 4 anos
                5 => (-2.0568, 15.4, 0.09630), // 5 anos
                6 => (-2.0767, 15.3, 0.09816), // 6 anos
                7 => (-2.0738, 15.3, 0.10023), // 7 anos
                8 => (-2.0508, 15.4, 0.10270), // 8 anos
                9 => (-2.0122, 15.6, 0.10562), // 9 anos
                10 => (-1.9623, 15.9, 0.10889), // 10 anos
                11 => (-1.9047, 16.3, 0.11239), // 11 anos
                12 => (-1.8426, 16.7, 0.11595), // 12 anos
                13 => (-1.7788, 17.3, 0.11942), // 13 anos
                14 => (-1.7157, 17.9, 0.12267), // 14 anos
                15 => (-1.6551, 18.6, 0.12563), // 15 anos
                16 => (-1.5983, 19.2, 0.12824), // 16 anos
                17 => (-1.5458, 19.8, 0.13049), // 17 anos
                _ => (-1.5000, 20.5, 0.13244) // 18+ anos
            };
        }
        else // Tabela LMS da OMS para raparigas
        {
            return idade switch
            {
                2 => (-1.6473, 16.2, 0.09025), // 2 anos
                3 => (-1.7526, 15.7, 0.09328), // 3 anos
                4 => (-1.8440, 15.4, 0.09634), // 4 anos
                5 => (-1.9186, 15.2, 0.09930), // 5 anos
                6 => (-1.9738, 15.1, 0.10216), // 6 anos
                7 => (-2.0085, 15.1, 0.10504), // 7 anos
                8 => (-2.0228, 15.2, 0.10809), // 8 anos
                9 => (-2.0179, 15.4, 0.11143), // 9 anos
                10 => (-1.9958, 15.7, 0.11517), // 10 anos
                11 => (-1.9590, 16.1, 0.11932), // 11 anos
                12 => (-1.9104, 16.6, 0.12380), // 12 anos
                13 => (-1.8530, 17.1, 0.12847), // 13 anos
                14 => (-1.7897, 17.6, 0.13313), // 14 anos
                15 => (-1.7236, 18.0, 0.13760), // 15 anos
                16 => (-1.6573, 18.5, 0.14175), // 16 anos
                17 => (-1.5929, 18.8, 0.14550), // 17 anos
                _ => (-1.5324, 19.2, 0.14881) // 18+ anos
            };
        }
    }
    
    // Converte Z-score para percentil
    private static double ZScoreParaPercentil(double zScore)
    {
        // Limita o Z-score a ±3.5 desvios padrão (percentis 0.02 a 99.98)
        zScore = Math.Max(-3.5, Math.Min(3.5, zScore));
        
        // Usa função de erro (erf) para calcular percentil da distribuição normal
        // Fórmula: P = 50 * (1 + erf(z / √2))
        double percentil = 0.5 * (1 + Erf(zScore / Math.Sqrt(2))) * 100;
        
        return Math.Round(percentil, 1);
    }
    
    // Aproximação da função de erro (erf) usando método de Abramowitz e Stegun
    private static double Erf(double x)
    {
        // Constantes da aproximação de Abramowitz e Stegun
        const double a1 = 0.254829592;
        const double a2 = -0.284496736;
        const double a3 = 1.421413741;
        const double a4 = -1.453152027;
        const double a5 = 1.061405429;
        const double p = 0.3275911;
        
        int sinal = x < 0 ? -1 : 1;
        x = Math.Abs(x);
        
        double t = 1.0 / (1.0 + p * x);
        double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);
        
        return sinal * y;
    }
    
    // Classifica o percentil
    private static string ClassificarPercentil(double percentil)
    {
        return percentil switch
        {
            < Constantes.PERCENTIL_MAGREZA => "Magreza",
            < Constantes.PERCENTIL_ABAIXO => "Abaixo",
            < Constantes.PERCENTIL_NORMAL => "Normal",
            < Constantes.PERCENTIL_SOBREPESO => "Sobrepeso",
            _ => "Obesidade"
        };
    }
}