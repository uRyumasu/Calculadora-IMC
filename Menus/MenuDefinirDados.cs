using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;


public static class MenuDefinirDados
{
    public static void Mostrar(Program.Pessoa pessoa, string version)
    {
        var running = true;
        while (running)
        {
            var content = new List<IRenderable>();

            Helpers.CentrarVert(content);
            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow(
                new FigletText($"{pessoa.peso:F1}kg".PadLeft(8)).Color(Tema.Atual.Peso).LeftJustified(),
                new FigletText($"{pessoa.altura:F2}m".PadRight(8)).Color(Tema.Atual.Altura).RightJustified()
            );
            grid.AddRow(
                new Markup($"[{Tema.Atual.Fundo.ToMarkup()}](1)[/]").Centered(),
                new Markup($"[{Tema.Atual.Fundo.ToMarkup()}](2)[/]").Centered()
            );

            content.Add(grid);

            Helpers.CentrarVert(content, 10);
            content.Add(
                new Markup(
                        $"\n[dim]Pressione [{Tema.Atual.Cabecalho.ToMarkup()}]1[/] ou [{Tema.Atual.Cabecalho.ToMarkup()}]2[/] para editar[/]")
                    .Centered());
            content.Add(new Markup("[dim]Pressione ENTER para voltar ao menu[/]").Centered());

            Helpers.CentrarVert(content, 11);
            content.Add(new Markup($"[dim]{version}[/]"));

            Helpers.Render(content, "Definir Dados");

            var opcao = Console.ReadKey(true);

            switch (opcao.KeyChar)
            {
                case '1': pessoa.peso = EditarPeso(pessoa, version); break;
                case '2': pessoa.altura = EditarAltura(pessoa, version); break;
            }

            if (opcao.Key == ConsoleKey.Enter) running = false;
        }
    }

    private static float EditarAltura(Program.Pessoa pessoa, string version)
    {
        var selecionando = true;
        var altura = pessoa.altura;

        while (selecionando)
        {
            var content = new List<IRenderable>();
            Helpers.CentrarVert(content);
            content.Add(new FigletText($"{altura:F2} m").Color(Tema.Atual.Altura).Centered());

            var controlos = new Panel(
                    new Markup(
                        $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                        "  [dim]↑[/] Aumentar 10cm\n" +
                        "  [dim]↓[/] Diminuir 10cm\n" +
                        " [dim]→[/] Aumentar 1cm\n" +
                        " [dim]←[/] Diminuir 1cm\n\n" +
                        $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar"
                    ).Centered()
                )
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda);

            content.Add(Align.Center(controlos));

            Helpers.CentrarVert(content, 12);
            content.Add(new Markup($"[dim]{version}[/]"));

            Helpers.Render(content, "Altura");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow: altura = Math.Min(altura + 0.1f, 2.5f); break;
                case ConsoleKey.DownArrow: altura = Math.Max(altura - 0.1f, 0.5f); break;
                case ConsoleKey.RightArrow: altura = Math.Min(altura + 0.01f, 2.5f); break;
                case ConsoleKey.LeftArrow: altura = Math.Max(altura - 0.01f, 0.5f); break;
                case ConsoleKey.Enter: selecionando = false; break;
            }
        }

        return altura;
    }

    private static float EditarPeso(Program.Pessoa pessoa, string version)
    {
        var selecionando = true;
        var peso = pessoa.peso;

        while (selecionando)
        {
            var content = new List<IRenderable>();
            Helpers.CentrarVert(content);
            content.Add(new FigletText($"{peso:F1} kg").Color(Tema.Atual.Peso).Centered());

            var controlos = new Panel(
                    new Markup(
                        $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                        "  [dim]↑[/] Aumentar 1kg\n" +
                        "  [dim]↓[/] Diminuir 1kg\n" +
                        " [dim]→[/] Aumentar 100g\n" +
                        " [dim]←[/] Diminuir 100g\n\n" +
                        $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar"
                    ).Centered()
                )
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda);

            content.Add(Align.Center(controlos));

            Helpers.CentrarVert(content, 12);
            content.Add(new Markup($"[dim]{version}[/]"));

            Helpers.Render(content, "Peso");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow: peso = Math.Min(peso + 1f, 300f); break;
                case ConsoleKey.DownArrow: peso = Math.Max(peso - 1f, 30f); break;
                case ConsoleKey.RightArrow: peso = Math.Min(peso + 0.1f, 300f); break;
                case ConsoleKey.LeftArrow: peso = Math.Max(peso - 0.1f, 30f); break;
                case ConsoleKey.Enter: selecionando = false; break;
            }
        }

        return peso;
    }
}