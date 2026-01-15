using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

// Menu que mostra o status completo do IMC do utilizador
public static class MenuStatusIMC
{
    // Mostra todas as informações de IMC e progresso
    public static void Mostrar(Program.Pessoa pessoa)
    {
        var conteudo = new List<IRenderable>();

        var imc = CalcIMC.Calcular(pessoa.Peso, pessoa.Altura);
        var imcFormatado = $"{imc:F2}";

        var tabelaPrincipal = new Table()
            .AddColumn("")
            .AddColumn("")
            .HideHeaders()
            .NoBorder()
            .Collapse();

        var painelEsquerdo = CriarPainelEsquerdo(pessoa);
        var painelDireito = CriarPainelDireito(pessoa, imc, imcFormatado);

        tabelaPrincipal.AddRow(painelEsquerdo, painelDireito);
        conteudo.Add(tabelaPrincipal);

        HelpersUI.Render(conteudo, "Status IMC");
        Console.ReadKey(true);
    }

    // Cria o painel esquerdo com informações pessoais
    private static Panel CriarPainelEsquerdo(Program.Pessoa pessoa)
    {
        string genero;
        if (pessoa.Sexo != default)
        {
            var corSexo = pessoa.Sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
            genero = $"[{corSexo.ToMarkup()}]{pessoa.Sexo}[/]";
        }
        else
        {
            genero = "??";
        }

        var conteudoEsquerdo = new List<IRenderable>
        {
            new Panel(new Rows(
                    new Markup(
                        $"[{Tema.Atual.Texto} bold]User:[/] {(string.IsNullOrEmpty(pessoa.Nome) ? "Sem utilizador" : pessoa.Nome)}"),
                    new Markup(
                        $"[{Tema.Atual.Texto} bold]Data de Nascimento:[/] {(pessoa.DataNascimento != default ? pessoa.DataNascimento.ToString("dd/MM/yyyy") : "??")}"),
                    new Markup(
                        $"[{Tema.Atual.Texto} bold]Idade:[/] {(pessoa.DataNascimento != default ? pessoa.Idade.ToString() : "??")} anos"),
                    new Markup($"[{Tema.Atual.Texto} bold]Género:[/] {genero}\n"),
                    new Rule(),
                    new Markup(
                        $"\n[{Tema.Atual.Altura} bold]Altura:[/] {UnitConverter.FormatarAltura(pessoa.Altura, pessoa.UnidadeSistema)}"),
                    new Markup(
                        $"[{Tema.Atual.Peso} bold]Peso:[/] {UnitConverter.FormatarPeso(pessoa.Peso, pessoa.UnidadeSistema)}\n"),
                    new Rule(),
                    new Markup("\n" + ObterTextoProgresso(pessoa)),
                    new Markup(ConselhosNutri.ObterConselhosCalorias(pessoa) + "")
                ))
                .BorderColor(Tema.Atual.Borda)
                .RoundedBorder()
        };

        return new Panel(new Rows(conteudoEsquerdo)) { Width = 40 }.NoBorder();
    }

    // Gera o texto de progresso do utilizador
    private static string ObterTextoProgresso(Program.Pessoa pessoa)
    {
        if (pessoa.Nome == null)
            return "Progresso indisponível";

        double diferenca = pessoa.PesoInicial - pessoa.Peso;
        string seta = diferenca > 0 ? "▼" : diferenca < 0 ? "▲" : "→";
        string cor = diferenca > 0 ? "green" : diferenca < 0 ? "red" : "dim";
        string diferencaTexto = diferenca != 0 
            ? $"[{cor}]{seta} {UnitConverter.FormatarDiferencaPeso((float)Math.Abs(diferenca), pessoa.UnidadeSistema)}[/]" 
            : "";

        string corPesoAtual = diferenca > 0 ? "green" : diferenca < 0 ? "red" : "yellow";

        return $"[{Tema.Atual.Texto} bold]Progresso:\n[/]" +
               $"[yellow]{UnitConverter.FormatarPeso(pessoa.PesoInicial, pessoa.UnidadeSistema)}[/] " +
               $"[dim]→[/] " +
               $"[{corPesoAtual} bold]{UnitConverter.FormatarPeso(pessoa.Peso, pessoa.UnidadeSistema)}[/] " +
               $"[dim]→[/] " +
               $"[cyan]{UnitConverter.FormatarPeso(pessoa.PesoDesejado, pessoa.UnidadeSistema)}[/] " +
               $"{diferencaTexto}\n";
    }

    // Cria o painel direito com visualizações do IMC/Percentil
    private static Rows CriarPainelDireito(Program.Pessoa pessoa, float imc, string imcFormatado)
    {
        var usarPercentil = PercentilIMC.DeveUsarPercentil(pessoa);
        
        IRenderable painelPrincipal;
        IRenderable painelNumero;
        
        if (usarPercentil)
        {
            var (percentil, classificacao) = PercentilIMC.CalcularPercentil(pessoa, imc);
            
            painelPrincipal = new Panel(
                new FigletText(classificacao)
                    .Color(CalcIMC.ObterCor(imc))
                    .Centered()
            )
            .BorderColor(Tema.Atual.Borda)
            .RoundedBorder();
            
            painelNumero = new Panel(
                new FigletText($"{percentil:F1}")
                    .Color(CalcIMC.ObterCor(imc))
                    .Centered()
            )
            .Header("Percentil IMC")
            .BorderColor(Tema.Atual.Borda)
            .RoundedBorder();
        }
        else
        {
            painelPrincipal = new Panel(
                new FigletText(CalcIMC.ObterClassificacao(imc))
                    .Color(CalcIMC.ObterCor(imc))
                    .Centered()
            )
            .BorderColor(Tema.Atual.Borda)
            .RoundedBorder();
            
            painelNumero = new Panel(
                new FigletText($"{imc:F1}")
                    .Color(CalcIMC.ObterCor(imc))
                    .Centered()
            )
            .Header("IMC")
            .BorderColor(Tema.Atual.Borda)
            .RoundedBorder();
        }

        var barraIMC = CriarBarraIMC(imc, imcFormatado, usarPercentil, pessoa);
        var classificacoes = CriarTabelaClassificacoes(imc, usarPercentil, pessoa);

        var tabelaLadoALado = new Table()
            .NoBorder()
            .HideHeaders()
            .Collapse()
            .AddColumn(new TableColumn("").LeftAligned())
            .AddColumn(new TableColumn("").LeftAligned());

        tabelaLadoALado.AddRow(
            classificacoes.BorderColor(Tema.Atual.Borda),
            painelNumero
        );

        var conteudoDireito = new List<IRenderable>
        {
            painelPrincipal,
            new Panel(new Rows(barraIMC)) { Width = 100 }
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda),
            tabelaLadoALado,
            new Panel(new Markup(Dicas.ObterDica()))
                .BorderColor(Tema.Atual.Borda)
                .RoundedBorder()
        };

        return new Rows(conteudoDireito);
    }

    // Cria a barra visual do IMC/Percentil
    private static List<IRenderable> CriarBarraIMC(float imc, string imcFormatado, bool usarPercentil, Program.Pessoa pessoa)
    {
        var barraIMC = new List<IRenderable>();

        if (usarPercentil)
        {
            var (percentil, _) = PercentilIMC.CalcularPercentil(pessoa, imc);
            
            var barra = $"\n[dim]P3[/] [{Tema.Atual.Magreza}]██████████[/]" +
                      $"[{Tema.Atual.Magreza}]████[/]" +
                      $"[{Tema.Atual.Normal}]██████████████████████████████[/]" +
                      $"[{Tema.Atual.Sobrepeso}]████████[/]" +
                      $"[{Tema.Atual.ObesidadeI}]██[/] [dim]P97[/]";

            barraIMC.Add(new Markup(barra).Centered());

            var espacamento = CalcularPosicaoCursorPercentil(percentil);
            var cursor = new string(' ', espacamento) + "▲";
            barraIMC.Add(new Markup(cursor));

            barraIMC.Add(new Markup(new string(' ', Math.Max(0, espacamento - 2)) + $"P{percentil:F1}"));
        }
        else
        {
            var barra = $"\n[dim]18.5[/] [{Tema.Atual.Magreza}]██████████[/]" +
                      $"[{Tema.Atual.Normal}]██████████[/]" +
                      $"[{Tema.Atual.Sobrepeso}]██████████[/]" +
                      $"[{Tema.Atual.ObesidadeI}]██████████[/]" +
                      $"[{Tema.Atual.ObesidadeII}]██████████[/]" +
                      $"[{Tema.Atual.ObesidadeIII}]██████████[/] [dim]40[/]";

            barraIMC.Add(new Markup(barra).Centered());

            var espacamento = CalcularPosicaoCursor(imc);
            var cursor = new string(' ', espacamento) + "▲";
            barraIMC.Add(new Markup(cursor));

            barraIMC.Add(new Markup(new string(' ', Math.Max(0, espacamento - 2)) + imcFormatado));
        }

        return barraIMC;
    }

    // Calcula a posição do cursor na barra de percentil
    private static int CalcularPosicaoCursorPercentil(double percentil)
    {
        return percentil switch
        {
            < Constantes.PERCENTIL_MAGREZA => 5 + (int)(percentil / Constantes.PERCENTIL_MAGREZA * 3),
            < Constantes.PERCENTIL_ABAIXO => 8 + (int)((percentil - Constantes.PERCENTIL_MAGREZA) / (Constantes.PERCENTIL_ABAIXO - Constantes.PERCENTIL_MAGREZA) * 9),
            < Constantes.PERCENTIL_NORMAL => 17 + (int)((percentil - Constantes.PERCENTIL_ABAIXO) / (Constantes.PERCENTIL_NORMAL - Constantes.PERCENTIL_ABAIXO) * 30),
            < Constantes.PERCENTIL_SOBREPESO => 47 + (int)((percentil - Constantes.PERCENTIL_NORMAL) / (Constantes.PERCENTIL_SOBREPESO - Constantes.PERCENTIL_NORMAL) * 10),
            _ => 57 + Math.Min((int)((percentil - Constantes.PERCENTIL_SOBREPESO) / 3.0 * 3), 3)
        };
    }

    // Calcula a posição do cursor na barra de IMC
    private static int CalcularPosicaoCursor(float imc)
    {
        return imc switch
        {
            <= 0f => 0,
            < Constantes.IMC_MAGREZA => 5,
            < Constantes.IMC_NORMAL => 15 + (int)((imc - Constantes.IMC_MAGREZA) / (Constantes.IMC_NORMAL - Constantes.IMC_MAGREZA) * 10),
            < Constantes.IMC_SOBREPESO => 25 + (int)((imc - Constantes.IMC_NORMAL) / (Constantes.IMC_SOBREPESO - Constantes.IMC_NORMAL) * 10),
            < Constantes.IMC_OBESIDADE_I => 35 + (int)((imc - Constantes.IMC_SOBREPESO) / (Constantes.IMC_OBESIDADE_I - Constantes.IMC_SOBREPESO) * 10),
            < Constantes.IMC_OBESIDADE_II => 45 + (int)((imc - Constantes.IMC_OBESIDADE_I) / (Constantes.IMC_OBESIDADE_II - Constantes.IMC_OBESIDADE_I) * 10),
            >= Constantes.IMC_OBESIDADE_II => 55 + Math.Min((int)((imc - Constantes.IMC_OBESIDADE_II) / 10f * 10), 9),
            _ => 0
        };
    }

    // Cria a tabela de classificações
    private static Table CriarTabelaClassificacoes(float imc, bool usarPercentil, Program.Pessoa pessoa)
    {
        var classificacoes = new Table()
            .RoundedBorder()
            .HideHeaders()
            .Expand()
            .AddColumn(new TableColumn("").LeftAligned())
            .AddColumn(new TableColumn("").RightAligned());

        if (usarPercentil)
        {
            var (percentil, _) = PercentilIMC.CalcularPercentil(pessoa, imc);
    
            classificacoes.AddRow(
                new Markup($"{ObterSeta(percentil, 0, Constantes.PERCENTIL_MAGREZA)}[{Tema.Atual.Magreza}]Magreza[/]"),
                new Markup("< P3")
            );
            classificacoes.AddRow(
                new Markup($"{ObterSeta(percentil, Constantes.PERCENTIL_MAGREZA, Constantes.PERCENTIL_ABAIXO)}[{Tema.Atual.Magreza}]Abaixo[/]"),
                new Markup("P3 – P15")
            );
            classificacoes.AddRow(
                new Markup($"{ObterSeta(percentil, Constantes.PERCENTIL_ABAIXO, Constantes.PERCENTIL_NORMAL)}[{Tema.Atual.Normal}]Normal[/]"),
                new Markup("P15 – P85")
            );
            classificacoes.AddRow(
                new Markup($"{ObterSeta(percentil, Constantes.PERCENTIL_NORMAL, Constantes.PERCENTIL_SOBREPESO)}[{Tema.Atual.Sobrepeso}]Sobrepeso[/]"),
                new Markup("P85 – P97")
            );
            classificacoes.AddRow(
                new Markup($"{ObterSeta(percentil, Constantes.PERCENTIL_SOBREPESO, 100)}[{Tema.Atual.ObesidadeI}]Obesidade[/]"),
                new Markup("≥ P97")
            );
        }
        else
        {
            classificacoes.AddRow(
                new Markup($"{ObterSetaIMC(imc, 0, Constantes.IMC_MAGREZA)}[{Tema.Atual.Magreza}]Magreza[/]"),
                new Markup("≤ 18,5")
            );
            classificacoes.AddRow(
                new Markup($"{ObterSetaIMC(imc, Constantes.IMC_MAGREZA, Constantes.IMC_NORMAL)}[{Tema.Atual.Normal}]Normal[/]"),
                new Markup("18,5 – 24,9")
            );
            classificacoes.AddRow(
                new Markup($"{ObterSetaIMC(imc, Constantes.IMC_NORMAL, Constantes.IMC_SOBREPESO)}[{Tema.Atual.Sobrepeso}]Sobrepeso[/]"),
                new Markup("25,0 – 29,9")
            );
            classificacoes.AddRow(
                new Markup($"{ObterSetaIMC(imc, Constantes.IMC_SOBREPESO, Constantes.IMC_OBESIDADE_I)}[{Tema.Atual.ObesidadeI}]Obesidade I[/]"),
                new Markup("30,0 – 34,9")
            );
            classificacoes.AddRow(
                new Markup($"{ObterSetaIMC(imc, Constantes.IMC_OBESIDADE_I, Constantes.IMC_OBESIDADE_II)}[{Tema.Atual.ObesidadeII}]Obesidade II[/]"),
                new Markup("35,0 – 39,9")
            );
            classificacoes.AddRow(
                new Markup($"{ObterSetaIMC(imc, Constantes.IMC_OBESIDADE_II, null)}[{Tema.Atual.ObesidadeIII}]Obesidade III[/]"),
                new Markup("≥ 40,0")
            );
        }

        return classificacoes;
    }

    // Retorna seta se o IMC estiver no intervalo
    private static string ObterSetaIMC(float valorIMC, float min, float? max = null)
    {
        if (max.HasValue)
            return valorIMC >= min && valorIMC < max ? "►" : "";
        return valorIMC >= min ? "►" : "";
    }
    
    // Retorna seta se o percentil estiver no intervalo
    private static string ObterSeta(double valor, double min, double max)
    {
        return valor >= min && valor < max ? "►" : "";
    }
}