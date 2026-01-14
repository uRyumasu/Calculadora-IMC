using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Core;

public static class Helpers
{
    public static void CentrarVert(List<IRenderable> content, int offset = 8)
    {
        var linhasCentrar = Console.WindowHeight / 2 - offset;
        for (var i = 0; i < linhasCentrar; i++)
            content.Add(new Text(""));
    }

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

    public static Color IMCtoColor(float imc)
    {
        return imc switch
        {
            < 18.5f => Tema.Atual.Magreza,
            < 25 => Tema.Atual.Normal,
            < 30f => Tema.Atual.Sobrepeso,
            < 35f => Tema.Atual.ObesidadeI,
            < 40f => Tema.Atual.ObesidadeII,
            _ => Tema.Atual.ObesidadeIII
        };
    }

    public static string IMCtoString(float imc)
    {
        return imc switch
        {
            < 18.5f => "Magreza",
            < 25 => "Normal",
            < 29.9f => "Sobrepeso",
            _ => "Obesidade"
        };
    }

    public static float CalcularIMC(float peso, float altura)
    {
        if (altura <= 0) return 0;
        return peso / (altura * altura);
    }

    public static void MostrarMensagem(string mensagem, Color cor)
    {
        var content = new List<IRenderable>();
        CentrarVert(content);
        content.Add(new Markup($"[{cor.ToMarkup()}]{mensagem}[/]").Centered());
        Render(content, "Mensagem");
        Thread.Sleep(1500);
    }


    public static bool ConfirmarAcao(string mensagem)
    {
        var content = new List<IRenderable>();
        CentrarVert(content);

        var confirmPanel = new Panel(
            new Markup(
                $"[{Color.Yellow.ToMarkup()} bold]{mensagem}[/]\n\n" +
                $"[{Tema.Atual.Normal.ToMarkup()}]S[/] = Sim    " +
                $"[{Color.Red.ToMarkup()}]N[/] = Não"
            ).Centered()
        ).RoundedBorder().BorderColor(Color.Yellow);

        content.Add(confirmPanel);
        Render(content, "Confirmação");

        while (true)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.S) return true;
            if (key == ConsoleKey.N || key == ConsoleKey.Escape) return false;
        }
    }
}