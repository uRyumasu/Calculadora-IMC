using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Core;

// Funções auxiliares para renderização de UI
public static class HelpersUI
{
    // Adiciona linhas vazias para centrar conteúdo verticalmente
    public static void CentrarVertical(List<IRenderable> conteudo, int offset = Constantes.OFFSET_VERTICAL_PEQUENO)
    {
        int linhas = Console.WindowHeight / 2 - offset;
        for (int i = 0; i < linhas; i++)
        {
            conteudo.Add(new Text(""));
        }
    }

    // Renderiza o conteúdo dentro de um painel com título e borda
    public static void Render(List<IRenderable> conteudo, string titulo)
    {
        Console.Clear();

        var linhas = new Rows(conteudo);
        var painel = new Panel(linhas)
            .Header($"[bold {Tema.Atual.Cabecalho.ToMarkup()}] {titulo} [/]")
            .RoundedBorder()
            .Expand()
            .BorderColor(Tema.Atual.Borda);

        painel.Height = Console.WindowHeight - 1;

        AnsiConsole.Write(painel);
        Console.SetCursorPosition(0, 0);
    }

    // Mostra uma mensagem temporária ao utilizador
    public static void MostrarMensagem(string mensagem, Color cor)
    {
        var conteudo = new List<IRenderable>();
        CentrarVertical(conteudo);
        conteudo.Add(new Markup($"[{cor.ToMarkup()}]{mensagem}[/]").Centered());
        Render(conteudo, "Mensagem");
        Thread.Sleep(Constantes.TEMPO_MENSAGEM); // Thread.Sleep serve para o programa "dormir" 
    }                                                            // (parar) por determinado tempo

    // Pede confirmação ao utilizador (S/N)
    public static bool ConfirmarAcao(string mensagem)
    {
        var conteudo = new List<IRenderable>();
        CentrarVertical(conteudo);

        var painel = new Panel(
            new Markup(
                $"[{Color.Yellow.ToMarkup()} bold]{mensagem}[/]\n\n" +
                $"[{Tema.Atual.Normal.ToMarkup()}]S[/] = Sim    " +
                $"[{Color.Red.ToMarkup()}]N[/] = Não"
            ).Centered()
        ).RoundedBorder().BorderColor(Color.Yellow);

        conteudo.Add(painel);
        Render(conteudo, "Confirmação");

        while (true)
        {
            var tecla = Console.ReadKey(true).Key;
            if (tecla == ConsoleKey.S) return true;
            if (tecla == ConsoleKey.N || tecla == ConsoleKey.Escape) return false;
        }
    }
}