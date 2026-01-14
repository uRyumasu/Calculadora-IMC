using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

/// <summary>
///     Theme selection menu
/// </summary>
public static class MenuTemas
{
    public static void Mostrar(string version)
    {
        var running = true;
        Console.CursorVisible = false;

        while (running)
        {
            var content = new List<IRenderable>();

            Helpers.CentrarVert(content, 10);
            content.Add(new FigletText("TEMAS")
                .Color(Tema.Atual.Titulo)
                .Centered());

            Helpers.CentrarVert(content);

            var linha1 = new Columns(
                new Markup($"[{Tema.Atual.Peso.ToMarkup()}]◄[/]").LeftJustified(),
                new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()} bold]{Tema.Atual.Nome}[/]").Centered(),
                new Markup($"[{Tema.Atual.Altura.ToMarkup()}]►[/]").RightJustified()
            ).Expand();

            content.Add(linha1);

            Helpers.CentrarVert(content, 10);

            content.Add(
                new Markup("[dim]Use ← → para mudar de tema[/]\n[dim]Pressione ENTER para voltar[/]").Centered());

            content.Add(new Markup($"[dim]{version}[/]"));

            Helpers.Render(content, "Configurações de Tema");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.LeftArrow: CycleTheme("backwards"); break;
                case ConsoleKey.RightArrow: CycleTheme(); break;
                case ConsoleKey.Enter: running = false; break;
            }
        }
    }

    private static void CycleTheme(string direction = "forwards")
    {
        var indiceAtual = Tema.Todos.FindIndex(t => t.Nome == Tema.Atual.Nome);

        if (direction == "forwards")
        {
            var proximoIndice = (indiceAtual + 1) % Tema.Todos.Count;
            Tema.Atual = Tema.Todos[proximoIndice];
        }
        else
        {
            var indiceAnterior = (indiceAtual - 1 + Tema.Todos.Count) % Tema.Todos.Count;
            Tema.Atual = Tema.Todos[indiceAnterior];
        }
    }
}