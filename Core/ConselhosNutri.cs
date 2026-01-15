namespace CalculadoraIMC.Core;

// Calcula e fornece conselhos nutricionais personalizados
public static class ConselhosNutri
{
    // Retorna conselhos completos de calorias e macros
    public static string ObterConselhosCalorias(Program.Pessoa pessoa)
    {
        float tmb = CalcularTMB(pessoa);
        float tdee = tmb * ObterMultiplicadorAtividade(pessoa.NivelAtividade);
        
        var (ajuste, textoAjuste) = ObterAjusteCalorico(pessoa.Objetivo, pessoa.Sexo);
        float caloriasAlvo = tdee + ajuste;
        
        var (proteina, carboidratos, gorduras) = CalcularMacros(pessoa, caloriasAlvo);
        
        return FormatarConselho(pessoa, tdee, caloriasAlvo, textoAjuste, 
                               proteina, carboidratos, gorduras);
    }
    
    // Calcula a Taxa Metabólica Basal usando fórmula de Mifflin-St Jeor
    private static float CalcularTMB(Program.Pessoa pessoa)
    {
        float tmb;
        
        if (pessoa.Sexo == Program.Sexo.Masculino)
        {
            tmb = (10 * pessoa.Peso) + (6.25f * pessoa.Altura * 100) - (5 * pessoa.Idade) + 5;
        }
        else if (pessoa.Sexo == Program.Sexo.Feminino)
        {
            tmb = (10 * pessoa.Peso) + (6.25f * pessoa.Altura * 100) - (5 * pessoa.Idade) - 161;
        }
        else
        {
            // Média entre masculino e feminino para sexo não definido
            tmb = (10 * pessoa.Peso) + (6.25f * pessoa.Altura * 100) - (5 * pessoa.Idade) - 78;
        }
        
        return tmb;
    }
    
    // Retorna o multiplicador de atividade física
    private static float ObterMultiplicadorAtividade(Program.NivelAtividade nivel)
    {
        return nivel switch
        {
            Program.NivelAtividade.Sedentario => Constantes.MULTIPLICADOR_SEDENTARIO,
            Program.NivelAtividade.Leve => Constantes.MULTIPLICADOR_LEVE,
            Program.NivelAtividade.Moderado => Constantes.MULTIPLICADOR_MODERADO,
            Program.NivelAtividade.Ativo => Constantes.MULTIPLICADOR_ATIVO,
            Program.NivelAtividade.MuitoAtivo => Constantes.MULTIPLICADOR_MUITO_ATIVO,
            _ => Constantes.MULTIPLICADOR_SEDENTARIO
        };
    }
    
    // Calcula o ajuste calórico baseado no objetivo
    private static (int ajuste, string texto) ObterAjusteCalorico(Program.Objetivo objetivo, 
                                                                   Program.Sexo sexo)
    {
        return objetivo switch
        {
            Program.Objetivo.PerderPeso => 
                (Constantes.AJUSTE_DEFICIT_GRANDE, "défice de 500 kcal"),
            
            Program.Objetivo.ManterPeso => 
                (0, "manutenção"),
            
            Program.Objetivo.GanharMassa => sexo == Program.Sexo.Feminino 
                ? (Constantes.AJUSTE_SUPERAVIT_MULHER, "superavit de 200 kcal") 
                : (Constantes.AJUSTE_SUPERAVIT_HOMEM, "superavit de 300 kcal"),
            
            Program.Objetivo.Definicao => 
                (Constantes.AJUSTE_DEFICIT_MEDIO, "défice moderado de 300 kcal"),
            
            Program.Objetivo.Recomposicao => 
                (Constantes.AJUSTE_DEFICIT_PEQUENO, "défice ligeiro de 100 kcal"),
            
            _ => (0, "manutenção")
        };
    }
    
    // Calcula as quantidades de macronutrientes
    private static (float proteina, float carboidratos, float gorduras) CalcularMacros(
        Program.Pessoa pessoa, float calorias)
    {
        float proteina, carboidratos, gorduras;
        
        switch (pessoa.Objetivo)
        {
            case Program.Objetivo.PerderPeso:
            case Program.Objetivo.Definicao:
                proteina = pessoa.Peso * Constantes.PROTEINA_PERDA_PESO;
                gorduras = pessoa.Peso * Constantes.GORDURA_PERDA_PESO;
                carboidratos = (calorias - (proteina * Constantes.CALORIAS_POR_GRAMA_PROTEINA) - 
                               (gorduras * Constantes.CALORIAS_POR_GRAMA_GORDURA)) / 
                               Constantes.CALORIAS_POR_GRAMA_CARBOIDRATO;
                break;
                
            case Program.Objetivo.GanharMassa:
                proteina = pessoa.Peso * Constantes.PROTEINA_GANHO_MASSA;
                gorduras = pessoa.Peso * Constantes.GORDURA_GANHO_MASSA;
                carboidratos = (calorias - (proteina * Constantes.CALORIAS_POR_GRAMA_PROTEINA) - 
                               (gorduras * Constantes.CALORIAS_POR_GRAMA_GORDURA)) / 
                               Constantes.CALORIAS_POR_GRAMA_CARBOIDRATO;
                break;
                
            case Program.Objetivo.Recomposicao:
                proteina = pessoa.Peso * Constantes.PROTEINA_RECOMPOSICAO;
                gorduras = pessoa.Peso * Constantes.GORDURA_RECOMPOSICAO;
                carboidratos = (calorias - (proteina * Constantes.CALORIAS_POR_GRAMA_PROTEINA) - 
                               (gorduras * Constantes.CALORIAS_POR_GRAMA_GORDURA)) / 
                               Constantes.CALORIAS_POR_GRAMA_CARBOIDRATO;
                break;
                
            case Program.Objetivo.ManterPeso:
            default:
                proteina = pessoa.Peso * Constantes.PROTEINA_MANUTENCAO;
                gorduras = pessoa.Peso * Constantes.GORDURA_MANUTENCAO;
                carboidratos = (calorias - (proteina * Constantes.CALORIAS_POR_GRAMA_PROTEINA) - 
                               (gorduras * Constantes.CALORIAS_POR_GRAMA_GORDURA)) / 
                               Constantes.CALORIAS_POR_GRAMA_CARBOIDRATO;
                break;
        }
        
        return (proteina, carboidratos, gorduras);
    }
    
    // Formata o conselho final para exibição
    private static string FormatarConselho(Program.Pessoa pessoa, float tdee, float caloriasAlvo, 
        string textoAjuste, float proteina, float carboidratos, float gorduras)
    {
        string textoObjetivo = pessoa.Objetivo switch
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
               $"• Proteína: {proteina:F0}g ({proteina * Constantes.CALORIAS_POR_GRAMA_PROTEINA:F0} kcal)\n" +
               $"• Carboidratos: {Math.Max(0, carboidratos):F0}g ({Math.Max(0, carboidratos) * Constantes.CALORIAS_POR_GRAMA_CARBOIDRATO:F0} kcal)\n" +
               $"• Gorduras: {gorduras:F0}g ({gorduras * Constantes.CALORIAS_POR_GRAMA_GORDURA:F0} kcal)";
    }
}