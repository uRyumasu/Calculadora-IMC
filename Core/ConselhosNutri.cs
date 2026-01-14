namespace CalculadoraIMC.Core;

public static class ConselhosNutri
{
    
    public static string ObterConselhosCalorias(Program.Pessoa pessoa)
    {
        float tmb = CalcularTMB(pessoa);
        
        
        float tdee = tmb * ObterMultiplicadorAtividade(pessoa.nivelAtividade);
        
        
        var (ajuste, textoAjuste) = ObterAjusteCalorico(pessoa.objetivo, pessoa.sexo);
        float caloriasAlvo = tdee + ajuste;
        
        
        var (proteina, carboidratos, gorduras) = CalcularMacros(pessoa, caloriasAlvo);
        
        return FormatarConselho(pessoa, tdee, caloriasAlvo, textoAjuste, proteina, carboidratos, gorduras);
    }
    
    private static float CalcularTMB(Program.Pessoa pessoa)
    {
        
        float tmb;
        
        if (pessoa.sexo == Program.Sexo.Masculino)
        {
            
            tmb = (10 * pessoa.peso) + (6.25f * pessoa.altura * 100) - (5 * pessoa.Idade) + 5;
        }
        else if (pessoa.sexo == Program.Sexo.Feminino)
        {
            
            tmb = (10 * pessoa.peso) + (6.25f * pessoa.altura * 100) - (5 * pessoa.Idade) - 161;
        }
        else
        {
            
            tmb = (10 * pessoa.peso) + (6.25f * pessoa.altura * 100) - (5 * pessoa.Idade) - 78;
        }
        
        return tmb;
    }
    
    private static float ObterMultiplicadorAtividade(Program.NivelAtividade nivel)
    {
        return nivel switch
        {
            Program.NivelAtividade.Sedentario => 1.2f,
            Program.NivelAtividade.Leve => 1.375f,
            Program.NivelAtividade.Moderado => 1.55f,
            Program.NivelAtividade.Ativo => 1.725f,
            Program.NivelAtividade.MuitoAtivo => 1.9f,
            _ => 1.2f
        };
    }
    
    private static (int ajuste, string texto) ObterAjusteCalorico(Program.Objetivo objetivo, Program.Sexo sexo)
    {
        return objetivo switch
        {
            Program.Objetivo.PerderPeso => (-500, "défice de 500 kcal"),
            Program.Objetivo.ManterPeso => (0, "manutenção"),
            Program.Objetivo.GanharMassa => sexo == Program.Sexo.Feminino 
                ? (200, "superavit de 200 kcal") 
                : (300, "superavit de 300 kcal"),
            Program.Objetivo.Definicao => (-300, "défice moderado de 300 kcal"),
            Program.Objetivo.Recomposicao => (-100, "défice ligeiro de 100 kcal"),
            _ => (0, "manutenção")
        };
    }
    
    private static (float proteina, float carboidratos, float gorduras) CalcularMacros(Program.Pessoa pessoa, float calorias)
    {
        float proteina, carboidratos, gorduras;
        
        switch (pessoa.objetivo)
        {
            case Program.Objetivo.PerderPeso:
            case Program.Objetivo.Definicao:
                
                proteina = pessoa.peso * 2.2f; 
                gorduras = pessoa.peso * 0.8f;    
                carboidratos = (calorias - (proteina * 4) - (gorduras * 9)) / 4;
                break;
                
            case Program.Objetivo.GanharMassa:
                
                proteina = pessoa.peso * 2.0f;
                gorduras = pessoa.peso * 1.0f;
                carboidratos = (calorias - (proteina * 4) - (gorduras * 9)) / 4;
                break;
                
            case Program.Objetivo.Recomposicao:
                
                proteina = pessoa.peso * 2.4f;
                gorduras = pessoa.peso * 0.9f;
                carboidratos = (calorias - (proteina * 4) - (gorduras * 9)) / 4;
                break;
                
            case Program.Objetivo.ManterPeso:
            default:
                
                proteina = pessoa.peso * 1.8f;
                gorduras = pessoa.peso * 1.0f;
                carboidratos = (calorias - (proteina * 4) - (gorduras * 9)) / 4;
                break;
        }
        
        return (proteina, carboidratos, gorduras);
    }
    
    private static string FormatarConselho(Program.Pessoa pessoa, float tdee, float caloriasAlvo, 
        string textoAjuste, float proteina, float carboidratos, float gorduras)
    {
        string textoObjetivo = pessoa.objetivo switch
        {
            Program.Objetivo.PerderPeso => "Para perder peso",
            Program.Objetivo.ManterPeso => "Para manter o peso",
            Program.Objetivo.GanharMassa => "Para ganhar massa muscular",
            Program.Objetivo.Definicao => "Para definição muscular",
            Program.Objetivo.Recomposicao => "Para recomposição corporal",
            _ => "Recomendação"
        };
        
        return $"{textoObjetivo}, consome cerca de {caloriasAlvo:F0} kcal/dia ({textoAjuste}).\n" +
               $"TDEE estimado: {tdee:F0} kcal/dia\n\n" +
               $"Macros sugeridos:\n" +
               $"• Proteína: {proteina:F0}g ({proteina * 4:F0} kcal)\n" +
               $"• Carboidratos: {Math.Max(0, carboidratos):F0}g ({Math.Max(0, carboidratos) * 4:F0} kcal)\n" +
               $"• Gorduras: {gorduras:F0}g ({gorduras * 9:F0} kcal)";
    }
}