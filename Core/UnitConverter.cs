namespace CalculadoraIMC.Core;

// Conversões entre sistemas de unidades (métrico e imperial)
public static class UnitConverter
{
    private const float KG_PARA_LBS = 2.20462f;
    private const float METROS_PARA_POLEGADAS = 39.3701f;
    private const float POLEGADAS_PARA_METROS = 0.0254f;

    // Converte quilogramas para libras
    public static float KgParaLbs(float kg)
    {
        return kg * KG_PARA_LBS;
    }

    // Converte libras para quilogramas
    public static float LbsParaKg(float lbs)
    {
        return lbs / KG_PARA_LBS;
    }

    // Converte metros para pés e polegadas
    public static (int pes, int polegadas) MetrosParaPesPolegadas(float metros)
    {
        float totalPolegadas = metros * METROS_PARA_POLEGADAS;
        int pes = (int)(totalPolegadas / Constantes.POLEGADAS_POR_PE);
        int polegadas = (int)(totalPolegadas % Constantes.POLEGADAS_POR_PE);
        return (pes, polegadas);
    }

    // Converte pés e polegadas para metros
    public static float PesPolegadasParaMetros(int pes, int polegadas)
    {
        return (pes * Constantes.POLEGADAS_POR_PE + polegadas) * POLEGADAS_PARA_METROS;
    }

    // Retorna a categoria do IMC
    public static string ObterCategoriaIMC(float imc)
    {
        return CalcIMC.ObterCategoriaDetalhada(imc);
    }

    // Formata o peso de acordo com o sistema de unidades
    public static string FormatarPeso(float pesoKg, Program.UnidadeSistema sistema)
    {
        return sistema == Program.UnidadeSistema.Metrico
            ? $"{pesoKg:F1}kg"
            : $"{KgParaLbs(pesoKg):F1}lbs";
    }

    // Formata a altura de acordo com o sistema de unidades
    public static string FormatarAltura(float alturaMetros, Program.UnidadeSistema sistema)
    {
        if (sistema == Program.UnidadeSistema.Metrico)
            return $"{alturaMetros:F2}m";

        var (pes, polegadas) = MetrosParaPesPolegadas(alturaMetros);
        return $"{pes}'{polegadas}\"";
    }

    // Formata a diferença de peso
    public static string FormatarDiferencaPeso(float pesoKg, Program.UnidadeSistema sistema)
    {
        return sistema == Program.UnidadeSistema.Metrico
            ? $"{pesoKg:F1} kg"
            : $"{KgParaLbs(pesoKg):F1} lbs";
    }
}