using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Core;

/// <summary>
/// Classe com diferentes funções "ajudantes"
/// </summary>
public static class Helpers
{
    // Centrar verticalmente com offset
    public static void CentrarVert(List<IRenderable> content, int offset = 8)
    {
        var linhasCentrar = Console.WindowHeight / 2 - offset;
        for (var i = 0; i < linhasCentrar; i++)
            content.Add(new Text(""));
    }

    // Apresentar conteudo com borda e titulo
    public static void Render(List<IRenderable> content, string title)
    {
        Console.Clear();

        var rows = new Rows(content);

        var frame = new Panel(rows)
            .Header($"[bold {Tema.Atual.Cabecalho.ToMarkup()}] {title} [/]")
            .RoundedBorder()
            .Expand()
            .BorderColor(Tema.Atual.Borda);
        frame.Height = Console.WindowHeight - 1;

        AnsiConsole.Write(frame);
        Console.SetCursorPosition(0, 0);
    }
}