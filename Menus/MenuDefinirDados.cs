using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

public static class MenuDefinirDados
{
    public static void Mostrar(Program.Pessoa pessoa)
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
                new FigletText(UnitConverter.FormatWeight(pessoa.peso, pessoa.unidadeSistema).PadLeft(8)).Color(Tema.Atual.Peso).LeftJustified(),
                new FigletText(UnitConverter.FormatHeight(pessoa.altura, pessoa.unidadeSistema).PadRight(8)).Color(Tema.Atual.Altura).RightJustified()
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

            Helpers.Render(content, "Definir Dados");

            var opcao = Console.ReadKey(true);

            bool dadosAlterados = false;

            switch (opcao.KeyChar)
            {
                case '1': 
                    var novoPeso = EditarPeso(pessoa);
                    if (novoPeso != pessoa.peso)
                    {
                        pessoa.peso = novoPeso;
                        dadosAlterados = true;
                    }
                    break;
                case '2': 
                    var novaAltura = EditarAltura(pessoa);
                    if (novaAltura != pessoa.altura)
                    {
                        pessoa.altura = novaAltura;
                        dadosAlterados = true;
                    }
                    break;
            }

            
            if (dadosAlterados)
            {
                if (UserDataManager.SaveUser(pessoa))
                {
                    Helpers.MostrarMensagem("Dados guardados com sucesso!", Color.Green);
                }
                else
                {
                    Helpers.MostrarMensagem("Erro ao guardar dados!", Color.Red);
                }
                running = false; 
            }

            if (opcao.Key == ConsoleKey.Enter) running = false;
        }
    }

    private static float EditarAltura(Program.Pessoa pessoa)
    {
        var selecionando = true;
        var altura = pessoa.altura;

        while (selecionando)
        {
            var content = new List<IRenderable>();
            Helpers.CentrarVert(content);
            content.Add(new FigletText(UnitConverter.FormatHeight(altura, pessoa.unidadeSistema)).Color(Tema.Atual.Altura).Centered());

            string controlosText;
            if (pessoa.unidadeSistema == Program.UnidadeSistema.Metrico)
            {
                controlosText = $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                                "  [dim]↑[/] Aumentar 10cm\n" +
                                "  [dim]↓[/] Diminuir 10cm\n" +
                                " [dim]→[/] Aumentar 1cm\n" +
                                " [dim]←[/] Diminuir 1cm\n\n" +
                                $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar";
            }
            else
            {
                controlosText = $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                                "  [dim]↑[/] Aumentar 1 inch\n" +
                                "  [dim]↓[/] Diminuir 1 inch\n\n" +
                                $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar";
            }

            var controlos = new Panel(new Markup(controlosText).Centered())
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda);

            content.Add(Align.Center(controlos));

            Helpers.Render(content, "Altura");

            var key = Console.ReadKey(true).Key;

            if (pessoa.unidadeSistema == Program.UnidadeSistema.Metrico)
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow: altura = Math.Min(altura + 0.1f, 2.5f); break;
                    case ConsoleKey.DownArrow: altura = Math.Max(altura - 0.1f, 0.5f); break;
                    case ConsoleKey.RightArrow: altura = Math.Min(altura + 0.01f, 2.5f); break;
                    case ConsoleKey.LeftArrow: altura = Math.Max(altura - 0.01f, 0.5f); break;
                    case ConsoleKey.Enter: selecionando = false; break;
                }
            }
            else 
            {
                var (currentFeet, currentInches) = UnitConverter.MetersToFeetInches(altura);
                switch (key)
                {
                    case ConsoleKey.UpArrow: 
                        currentInches++;
                        if (currentInches >= 12)
                        {
                            currentFeet++;
                            currentInches = 0;
                        }
                        altura = UnitConverter.FeetInchesToMeters(currentFeet, currentInches);
                        altura = Math.Min(altura, 2.5f);
                        break;
                    case ConsoleKey.DownArrow: 
                        currentInches--;
                        if (currentInches < 0)
                        {
                            currentFeet--;
                            currentInches = 11;
                        }
                        if (currentFeet < 0) currentFeet = 0;
                        altura = UnitConverter.FeetInchesToMeters(currentFeet, currentInches);
                        altura = Math.Max(altura, 0.5f);
                        break;
                    case ConsoleKey.Enter: selecionando = false; break;
                }
            }
        }

        return altura;
    }

    private static float EditarPeso(Program.Pessoa pessoa)
    {
        var selecionando = true;
        var peso = pessoa.peso;

        while (selecionando)
        {
            var content = new List<IRenderable>();
            Helpers.CentrarVert(content);
            content.Add(new FigletText(UnitConverter.FormatWeight(peso, pessoa.unidadeSistema)).Color(Tema.Atual.Peso).Centered());

            string controlosText;
            float largeIncrement;
            float smallIncrement;
            
            if (pessoa.unidadeSistema == Program.UnidadeSistema.Metrico)
            {
                largeIncrement = 1f;      
                smallIncrement = 0.1f;    
                controlosText = $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                                "  [dim]↑[/] Aumentar 1kg\n" +
                                "  [dim]↓[/] Diminuir 1kg\n" +
                                " [dim]→[/] Aumentar 100g\n" +
                                " [dim]←[/] Diminuir 100g\n\n" +
                                $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar";
            }
            else
            {
                largeIncrement = UnitConverter.LbsToKg(1f);    
                smallIncrement = UnitConverter.LbsToKg(0.1f);  
                controlosText = $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                                "  [dim]↑[/] Aumentar 1lb\n" +
                                "  [dim]↓[/] Diminuir 1lb\n" +
                                " [dim]→[/] Aumentar 0.1lb\n" +
                                " [dim]←[/] Diminuir 0.1lb\n\n" +
                                $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar";
            }

            var controlos = new Panel(new Markup(controlosText).Centered())
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda);

            content.Add(Align.Center(controlos));

            Helpers.Render(content, "Peso");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow: peso = Math.Min(peso + largeIncrement, 300f); break;
                case ConsoleKey.DownArrow: peso = Math.Max(peso - largeIncrement, 30f); break;
                case ConsoleKey.RightArrow: peso = Math.Min(peso + smallIncrement, 300f); break;
                case ConsoleKey.LeftArrow: peso = Math.Max(peso - smallIncrement, 30f); break;
                case ConsoleKey.Enter: selecionando = false; break;
            }
        }

        return peso;
    }
}