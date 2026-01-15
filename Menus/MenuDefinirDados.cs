using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

// Menu para definir/editar peso e altura do utilizador
public static class MenuDefinirDados
{
    // Mostra o menu principal de definição de dados
    public static void Mostrar(Program.Pessoa pessoa)
    {
        var emExecucao = true;
        while (emExecucao)
        {
            var conteudo = new List<IRenderable>();

            HelpersUI.CentrarVertical(conteudo);
            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow(
                new FigletText(UnitConverter.FormatarPeso(pessoa.Peso, pessoa.UnidadeSistema).PadLeft(8))
                    .Color(Tema.Atual.Peso).LeftJustified(),
                new FigletText(UnitConverter.FormatarAltura(pessoa.Altura, pessoa.UnidadeSistema).PadRight(8))
                    .Color(Tema.Atual.Altura).RightJustified()
            );
            grid.AddRow(
                new Markup($"[{Tema.Atual.Fundo.ToMarkup()}](1)[/]").Centered(),
                new Markup($"[{Tema.Atual.Fundo.ToMarkup()}](2)[/]").Centered()
            );

            conteudo.Add(grid);

            HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_MEDIO);
            conteudo.Add(
                new Markup(
                        $"\n[dim]Pressione [{Tema.Atual.Cabecalho.ToMarkup()}]1[/] ou [{Tema.Atual.Cabecalho.ToMarkup()}]2[/] para editar[/]")
                    .Centered());
            conteudo.Add(new Markup("[dim]Pressione ENTER para voltar ao menu[/]").Centered());

            HelpersUI.Render(conteudo, "Definir Dados");

            var opcao = Console.ReadKey(true);
            bool dadosAlterados = false;

            switch (opcao.KeyChar)
            {
                case '1': 
                    var novoPeso = EditarPeso(pessoa);
                    if (novoPeso != pessoa.Peso)
                    {
                        pessoa.Peso = novoPeso;
                        dadosAlterados = true;
                    }
                    break;
                case '2': 
                    var novaAltura = EditarAltura(pessoa);
                    if (novaAltura != pessoa.Altura)
                    {
                        pessoa.Altura = novaAltura;
                        dadosAlterados = true;
                    }
                    break;
            }

            // Guarda os dados se foram alterados
            if (dadosAlterados)
            {
                if (UserDataManager.SaveUser(pessoa))
                {
                    HelpersUI.MostrarMensagem("Dados guardados com sucesso!", Color.Green);
                }
                else
                {
                    HelpersUI.MostrarMensagem("Erro ao guardar dados!", Color.Red);
                }
                emExecucao = false; 
            }

            if (opcao.Key == ConsoleKey.Enter) emExecucao = false;
        }
    }

    // Editar a altura do utilizador
    private static float EditarAltura(Program.Pessoa pessoa)
    {
        var selecionando = true;
        var altura = pessoa.Altura;

        while (selecionando)
        {
            var conteudo = new List<IRenderable>();
            HelpersUI.CentrarVertical(conteudo);
            conteudo.Add(new FigletText(UnitConverter.FormatarAltura(altura, pessoa.UnidadeSistema))
                .Color(Tema.Atual.Altura).Centered());

            string textoControlos;
            if (pessoa.UnidadeSistema == Program.UnidadeSistema.Metrico)
            {
                textoControlos = $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                                "  [dim]↑[/] Aumentar 10cm\n" +
                                "  [dim]↓[/] Diminuir 10cm\n" +
                                " [dim]→[/] Aumentar 1cm\n" +
                                " [dim]←[/] Diminuir 1cm\n\n" +
                                $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar";
            }
            else
            {
                textoControlos = $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                                "  [dim]↑[/] Aumentar 1 inch\n" +
                                "  [dim]↓[/] Diminuir 1 inch\n\n" +
                                $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar";
            }

            var controlos = new Panel(new Markup(textoControlos).Centered())
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda);

            conteudo.Add(Align.Center(controlos));

            HelpersUI.Render(conteudo, "Altura");

            var tecla = Console.ReadKey(true).Key;

            if (pessoa.UnidadeSistema == Program.UnidadeSistema.Metrico)
            {
                switch (tecla)
                {
                    case ConsoleKey.UpArrow: 
                        altura = Math.Min(altura + Constantes.AJUSTE_ALTURA_GRANDE, Constantes.ALTURA_MAXIMA); 
                        break;
                    case ConsoleKey.DownArrow: 
                        altura = Math.Max(altura - Constantes.AJUSTE_ALTURA_GRANDE, Constantes.ALTURA_MINIMA); 
                        break;
                    case ConsoleKey.RightArrow: 
                        altura = Math.Min(altura + Constantes.AJUSTE_ALTURA_PEQUENO, Constantes.ALTURA_MAXIMA); 
                        break;
                    case ConsoleKey.LeftArrow: 
                        altura = Math.Max(altura - Constantes.AJUSTE_ALTURA_PEQUENO, Constantes.ALTURA_MINIMA); 
                        break;
                    case ConsoleKey.Enter: 
                        selecionando = false; 
                        break;
                }
            }
            else 
            {
                var (pesAtual, polegadasAtual) = UnitConverter.MetrosParaPesPolegadas(altura);
                switch (tecla)
                {
                    case ConsoleKey.UpArrow: 
                        polegadasAtual++;
                        if (polegadasAtual >= Constantes.POLEGADAS_POR_PE)
                        {
                            pesAtual++;
                            polegadasAtual = 0;
                        }
                        altura = UnitConverter.PesPolegadasParaMetros(pesAtual, polegadasAtual);
                        altura = Math.Min(altura, Constantes.ALTURA_MAXIMA);
                        break;
                    case ConsoleKey.DownArrow: 
                        polegadasAtual--;
                        if (polegadasAtual < 0)
                        {
                            pesAtual--;
                            polegadasAtual = Constantes.POLEGADAS_POR_PE - 1;
                        }
                        if (pesAtual < 0) pesAtual = 0;
                        altura = UnitConverter.PesPolegadasParaMetros(pesAtual, polegadasAtual);
                        altura = Math.Max(altura, Constantes.ALTURA_MINIMA);
                        break;
                    case ConsoleKey.Enter: 
                        selecionando = false; 
                        break;
                }
            }
        }

        return altura;
    }

    // Editar o peso do utilizador
    private static float EditarPeso(Program.Pessoa pessoa)
    {
        var selecionando = true;
        var peso = pessoa.Peso;

        while (selecionando)
        {
            var conteudo = new List<IRenderable>();
            HelpersUI.CentrarVertical(conteudo);
            conteudo.Add(new FigletText(UnitConverter.FormatarPeso(peso, pessoa.UnidadeSistema))
                .Color(Tema.Atual.Peso).Centered());

            string textoControlos;
            float ajusteGrande;
            float ajustePequeno;
            float pesoMinimo;
            float pesoMaximo;
            
            if (pessoa.UnidadeSistema == Program.UnidadeSistema.Metrico)
            {
                ajusteGrande = Constantes.AJUSTE_PESO_GRANDE;
                ajustePequeno = Constantes.AJUSTE_PESO_PEQUENO;
                pesoMinimo = Constantes.PESO_MINIMO;
                pesoMaximo = Constantes.PESO_MAXIMO;
                textoControlos = $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                                "  [dim]↑[/] Aumentar 1kg\n" +
                                "  [dim]↓[/] Diminuir 1kg\n" +
                                " [dim]→[/] Aumentar 100g\n" +
                                " [dim]←[/] Diminuir 100g\n\n" +
                                $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar";
            }
            else
            {
                ajusteGrande = UnitConverter.LbsParaKg(Constantes.AJUSTE_PESO_GRANDE_LBS);
                ajustePequeno = UnitConverter.LbsParaKg(Constantes.AJUSTE_PESO_PEQUENO_LBS);
                pesoMinimo = Constantes.PESO_MINIMO;
                pesoMaximo = Constantes.PESO_MAXIMO;
                textoControlos = $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n\n" +
                                "  [dim]↑[/] Aumentar 1lb\n" +
                                "  [dim]↓[/] Diminuir 1lb\n" +
                                " [dim]→[/] Aumentar 0.5lb\n" +
                                " [dim]←[/] Diminuir 0.5lb\n\n" +
                                $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] para confirmar";
            }

            var controlos = new Panel(new Markup(textoControlos).Centered())
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda);

            conteudo.Add(Align.Center(controlos));

            HelpersUI.Render(conteudo, "Peso");

            var tecla = Console.ReadKey(true).Key;

            switch (tecla)
            {
                case ConsoleKey.UpArrow: 
                    peso = Math.Min(peso + ajusteGrande, pesoMaximo); 
                    break;
                case ConsoleKey.DownArrow: 
                    peso = Math.Max(peso - ajusteGrande, pesoMinimo); 
                    break;
                case ConsoleKey.RightArrow: 
                    peso = Math.Min(peso + ajustePequeno, pesoMaximo); 
                    break;
                case ConsoleKey.LeftArrow: 
                    peso = Math.Max(peso - ajustePequeno, pesoMinimo); 
                    break;
                case ConsoleKey.Enter: 
                    selecionando = false; 
                    break;
            }
        }

        return peso;
    }
}