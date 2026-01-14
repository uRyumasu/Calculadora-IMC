namespace CalculadoraIMC.Core;

/// <summary>
///     Handles conversions between metric and imperial units
/// </summary>
public static class UnitConverter
{
    // Weight conversions
    public static float KgToLbs(float kg)
    {
        return kg * 2.20462f;
    }

    public static float LbsToKg(float lbs)
    {
        return lbs / 2.20462f;
    }

    // Height conversions
    public static (int feet, int inches) MetersToFeetInches(float meters)
    {
        var totalInches = meters * 39.3701f;
        var feet = (int)(totalInches / 12);
        var inches = (int)(totalInches % 12);
        return (feet, inches);
    }

    public static float FeetInchesToMeters(int feet, int inches)
    {
        return (feet * 12 + inches) * 0.0254f;
    }

    // BMI calculation
    public static float CalculateBMI(float weightKg, float heightMeters)
    {
        if (heightMeters <= 0) return 0;
        return weightKg / (heightMeters * heightMeters);
    }

    public static string GetBMICategory(float bmi)
    {
        return bmi switch
        {
            < 18.5f => "Magreza",
            < 25f => "Normal",
            < 30f => "Sobrepeso",
            < 35f => "Obesidade I",
            < 40f => "Obesidade II",
            _ => "Obesidade III"
        };
    }

    // Formatting helpers
    public static string FormatWeight(float weightKg, Program.UnidadeSistema sistema)
    {
        return sistema == Program.UnidadeSistema.Metrico
            ? $"{weightKg:F1}kg"
            : $"{KgToLbs(weightKg):F1}lbs";
    }

    public static string FormatHeight(float heightMeters, Program.UnidadeSistema sistema)
    {
        if (sistema == Program.UnidadeSistema.Metrico)
            return $"{heightMeters:F2}m";
        var (feet, inches) = MetersToFeetInches(heightMeters);
        return $"{feet}'{inches}\"";
    }
}