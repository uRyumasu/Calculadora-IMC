using System.Diagnostics;

namespace CalculadoraIMC;

public class Splash
{
    public static void ExecutarSplashNoCmd()
    {
        try
        {
            var splashPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                "SplashScreen.exe"
            );
            
            if (!File.Exists(splashPath))
            {
                Console.WriteLine("Splash não encontrado!");
                return;
            }
            
            // Força execução no cmd.exe clássico
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C \"{splashPath}\"",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            };
            
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao executar splash: {ex.Message}");
        }
    }
}