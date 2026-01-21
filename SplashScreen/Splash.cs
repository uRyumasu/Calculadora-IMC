using System.Diagnostics;
using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC;

public static class Splash
{
    // Mostra a tela de saída 
    public static void Mostrar()
    {
        var figletText = new FigletText("Daniel Matseiko")
            .Centered()
            .Color(Color.Cyan1);

        var panel = 
                Align.Center(
                    new Rows(
                        figletText,
                        new Text(""),
                        new Markup($"[bold {Tema.Atual.Texto}]Nº3 | 10ºH - GPSI[/]"),
                        new Rule($"[{Tema.Atual.Borda}]PSI - M3 PROGRAMAÇÃO ESTRUTURADA[/]").RuleStyle(Style.Parse("dim")),
                        new Text(""),
                        new Markup($"[italic dim]Escola Secundária Adolfo Portela[/]")
                    ),
                    VerticalAlignment.Middle
                );

        HelpersUI.Render(new List<IRenderable> { panel }, "Saída");
        Thread.Sleep(3000);
    }
}