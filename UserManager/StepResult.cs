namespace CalculadoraIMC.UserManager;

// Resultado de um passo no wizard de criação de utilizador
public class StepResult
{
    // Açao executada apos cada passo
    public StepAction Action { get; set; }
    
    // Dados retornados do passo 
    public object? Data { get; set; }

    // Avançar 
    public static StepResult Next(object? data = null)
    {
        return new StepResult { Action = StepAction.Next, Data = data };
    }

    // Retrodecer
    public static StepResult Back()
    {
        return new StepResult { Action = StepAction.Back };
    }

    // Cancelar
    public static StepResult Cancel()
    {
        return new StepResult { Action = StepAction.Cancel };
    }
}

// Açoes possiveis
public enum StepAction
{
    // Avançar para o próximo passo
    Next,
    
    // Voltar ao passo anterior
    Back,
    
    // Cancelar o wizard completamente
    Cancel,
}