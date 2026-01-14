using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;


public static class MenuTemas
{
    public static void Mostrar(Program.Pessoa pessoa)
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

            Helpers.Render(content, "Configurações de Tema");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.LeftArrow: CycleTheme("backwards");
                    pessoa.NomeTema = Tema.Atual.Nome; break;
                case ConsoleKey.RightArrow: CycleTheme();
                    pessoa.NomeTema = Tema.Atual.Nome; break;
                case ConsoleKey.Enter: running = false;
                    UserDataManager.SaveUser(pessoa); break;
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