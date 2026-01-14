using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

/// <summary>
///     Main menu screen
/// </summary>
public static class MenuPrincipal
{
    public static void Mostrar(string version)
    {
        var content = new List<IRenderable>();

        Helpers.CentrarVert(content, 12);
        content.Add(new FigletText("Calculadora IMC")
            .Color(Tema.Atual.Titulo)
            .Centered());

        Helpers.CentrarVert(content, 10);

        content.Add(Align.Center(new Markup(
            $"[{Tema.Atual.Texto.ToMarkup()}]" +
            "1) Definir dados\n" +
            "2) Obter IMC\n" +
            "3) Definições\n" +
            "4) Status IMC\n" +
            "5) Sair\n" +
            "7) Selecionar Utilizador\n" +
            "8) Criar Utilizador" +
            "[/]"
        )));

        Helpers.CentrarVert(content, 7);
        content.Add(new Markup($"[dim]{version}[/]"));

        Helpers.Render(content, "Calculadora IMC");
    }
}