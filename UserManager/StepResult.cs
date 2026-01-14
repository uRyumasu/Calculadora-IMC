namespace CalculadoraIMC.UserManager;

public class StepResult
{
    public StepAction Action { get; set; }
    public object? Data { get; set; }

    public static StepResult Next(object? data = null)
    {
        return new StepResult { Action = StepAction.Next, Data = data };
    }

    public static StepResult Back()
    {
        return new StepResult { Action = StepAction.Back };
    }

    public static StepResult Cancel()
    {
        return new StepResult { Action = StepAction.Cancel };
    }

    public static StepResult Skip()
    {
        return new StepResult { Action = StepAction.Skip };
    }
}

/// <summary>
///     Actions that can be taken from a wizard step
/// </summary>
public enum StepAction
{
    Next, // Move to next step
    Back, // Go back to previous step
    Cancel, // Cancel entire wizard
    Skip // Skip current step (for optional fields)
}