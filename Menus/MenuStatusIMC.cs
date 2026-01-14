using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

public static class MenuStatusIMC
{
    public static void Mostrar(Program.Pessoa pessoa)
    {
        var content = new List<IRenderable>();

        var imc = Helpers.CalcularIMC(pessoa.peso, pessoa.altura);
        var fixedIMC = $"{imc:F2}";

        var janelaIMC = new Table()
            .AddColumn("")
            .AddColumn("")
            .HideHeaders()
            .NoBorder()
            .Collapse();

        var leftPanel = CriarPainelEsquerdo(pessoa);
        var rightPanel = CriarPainelDireito(pessoa, imc, fixedIMC);

        janelaIMC.AddRow(leftPanel, rightPanel);
        content.Add(janelaIMC);

        Helpers.Render(content, "Status IMC");
        Console.ReadKey(true);
    }

    private static Panel CriarPainelEsquerdo(Program.Pessoa pessoa)
    {
        string genero;
        if (pessoa.sexo != default)
        {
            var corSexo = pessoa.sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
            genero = $"[{corSexo.ToMarkup()}]{pessoa.sexo}[/]";
        }
        else
        {
            genero = "??";
        }

        string textoProgresso(Program.Pessoa pessoa)
        {
            if (pessoa.nome == null)
                return "Progresso indisponivel";

            double diferenca = pessoa.pesoInicial - pessoa.peso;
            string seta = diferenca > 0 ? "▼" : diferenca < 0 ? "▲" : "→";
            string cor = diferenca > 0 ? "green" : diferenca < 0 ? "red" : "dim";
            string diferencaTexto = diferenca != 0 ? $"[{cor}]{seta} {UnitConverter.FormatWeightDifference((float)Math.Abs(diferenca), pessoa.unidadeSistema)}[/]" : "";

            
            string corPesoAtual = diferenca > 0 ? "green" : diferenca < 0 ? "red" : "yellow";

            return $"[{Tema.Atual.Texto} bold]Progresso:\n[/]" +
                   $"[yellow]{UnitConverter.FormatWeight(pessoa.pesoInicial, pessoa.unidadeSistema)}[/] " +
                   $"[dim]→[/] " +
                   $"[{corPesoAtual} bold]{UnitConverter.FormatWeight(pessoa.peso, pessoa.unidadeSistema)}[/] " +
                   $"[dim]→[/] " +
                   $"[cyan]{UnitConverter.FormatWeight(pessoa.pesoDesejado, pessoa.unidadeSistema)}[/] " +
                   $"{diferencaTexto}\n";
        }
        
        var leftContent = new List<IRenderable>
        {
            new Panel(new Rows(
                    new Markup(
                        $"\n[{Tema.Atual.Texto} bold]User:[/] {(string.IsNullOrEmpty(pessoa.nome) ? "Sem utilizador" : pessoa.nome)}"),
                    new Markup(
                        $"[{Tema.Atual.Texto} bold]Data de Nascimento:[/] {(pessoa.dataNascimento != default ? pessoa.dataNascimento.ToString("dd/MM/yyyy") : "??")}"),
                    new Markup(
                        $"[{Tema.Atual.Texto} bold]Idade:[/] {(pessoa.dataNascimento != default ? pessoa.Idade.ToString() : "??")} anos"),
                    new Markup($"[{Tema.Atual.Texto} bold]Género:[/] {genero}\n"),
                    new Rule(),
                    new Markup(
                        $"\n[{Tema.Atual.Altura} bold]Altura:[/] {UnitConverter.FormatHeight(pessoa.altura, pessoa.unidadeSistema)}"),
                    new Markup(
                        $"[{Tema.Atual.Peso} bold]Peso:[/] {UnitConverter.FormatWeight(pessoa.peso, pessoa.unidadeSistema)}\n"),
                    new Rule(),
                    new Markup("\n" + textoProgresso(pessoa)),
                    new Markup(ConselhosNutri.ObterConselhosCalorias(pessoa) + "\n")
                ))
                .BorderColor(Tema.Atual.Borda)
                .RoundedBorder()
        };

        return new Panel(new Rows(leftContent)) { Width = 40 }.NoBorder();
    }

    private static Rows CriarPainelDireito(Program.Pessoa pessoa, float imc, string fixedIMC)
    {
        var usarPercentil = PercentilIMC.DeveUsarPercentil(pessoa);
        
        IRenderable painelPrincipal;
        IRenderable painelNumero;
        
        if (usarPercentil)
        {
            var (percentil, classificacao) = PercentilIMC.CalcularPercentil(pessoa, imc);
            
            painelPrincipal = new Panel(
                new FigletText(classificacao)
                    .Color(Helpers.IMCtoColor(imc))
                    .Centered()
            )
            .BorderColor(Tema.Atual.Borda)
            .RoundedBorder();
            
            painelNumero = new Panel(
                new FigletText($"{percentil:F1}")
                    .Color(Helpers.IMCtoColor(imc))
                    .Centered()
            )
            .Header("Percentil IMC")
            .BorderColor(Tema.Atual.Borda)
            .RoundedBorder();
        }
        else
        {
            painelPrincipal = new Panel(
                new FigletText(Helpers.IMCtoString(imc))
                    .Color(Helpers.IMCtoColor(imc))
                    .Centered()
            )
            .BorderColor(Tema.Atual.Borda)
            .RoundedBorder();
            
            painelNumero = new Panel(
                new FigletText($"{imc:F1}")
                    .Color(Helpers.IMCtoColor(imc))
                    .Centered()
            )
            .Header("IMC")
            .BorderColor(Tema.Atual.Borda)
            .RoundedBorder();
        }

        var barraIMC = CriarBarraIMC(imc, fixedIMC, usarPercentil, pessoa);
        var classificacoes = CriarTabelaClassificacoes(imc, usarPercentil, pessoa);

        var sideBySection = new Table()
            .NoBorder()
            .HideHeaders()
            .Collapse()
            .AddColumn(new TableColumn("").LeftAligned())
            .AddColumn(new TableColumn("").LeftAligned());

        sideBySection.AddRow(
            classificacoes.BorderColor(Tema.Atual.Borda),
            painelNumero
        );

        var rightContent = new List<IRenderable>
        {
            painelPrincipal,
            new Panel(new Rows(barraIMC)) { Width = 100 }
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda),
            sideBySection,
            new Panel(new Markup(Dicas.ObterDica()))
                .BorderColor(Tema.Atual.Borda)
                .RoundedBorder()
        };

        return new Rows(rightContent);
    }

    private static List<IRenderable> CriarBarraIMC(float imc, string fixedIMC, bool usarPercentil, Program.Pessoa pessoa)
    {
        var barraIMC = new List<IRenderable>();

        if (usarPercentil)
        {
            var (percentil, _) = PercentilIMC.CalcularPercentil(pessoa, imc);
            
            var bar = $"\n[dim]P3[/] [{Tema.Atual.Magreza}]██████████[/]" +
                      $"[{Tema.Atual.Magreza}]████[/]" +
                      $"[{Tema.Atual.Normal}]██████████████████████████████[/]" +
                      $"[{Tema.Atual.Sobrepeso}]████████[/]" +
                      $"[{Tema.Atual.ObesidadeI}]██[/] [dim]P97[/]";

            barraIMC.Add(new Markup(bar).Centered());

            var padding = CalculateCursorPositionPercentil(percentil);
            var cursor = new string(' ', padding) + "▲";
            barraIMC.Add(new Markup(cursor));

            barraIMC.Add(new Markup(new string(' ', Math.Max(0, padding - 2)) + $"P{percentil:F1}"));
        }
        else
        {
            var bar = $"\n[dim]18.5[/] [{Tema.Atual.Magreza}]██████████[/]" +
                      $"[{Tema.Atual.Normal}]██████████[/]" +
                      $"[{Tema.Atual.Sobrepeso}]██████████[/]" +
                      $"[{Tema.Atual.ObesidadeI}]██████████[/]" +
                      $"[{Tema.Atual.ObesidadeII}]██████████[/]" +
                      $"[{Tema.Atual.ObesidadeIII}]██████████[/] [dim]40[/]";

            barraIMC.Add(new Markup(bar).Centered());

            var padding = CalculateCursorPosition(imc);
            var cursor = new string(' ', padding) + "▲";
            barraIMC.Add(new Markup(cursor));

            barraIMC.Add(new Markup(new string(' ', Math.Max(0, padding - 2)) + fixedIMC));
        }

        return barraIMC;
    }

    private static int CalculateCursorPositionPercentil(double percentil)
    {
        return percentil switch
        {
            < 3 => 5 + (int)(percentil / 3.0 * 3),
            < 15 => 8 + (int)((percentil - 3) / 12.0 * 9),
            < 85 => 17 + (int)((percentil - 15) / 70.0 * 30),
            < 97 => 47 + (int)((percentil - 85) / 12.0 * 10),
            _ => 57 + Math.Min((int)((percentil - 97) / 3.0 * 3), 3)
        };
    }

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
                new Markup($"{GetArrowPercentil(percentil, 0, 3)}[{Tema.Atual.Magreza}]Magreza[/]"),
                new Markup("< P3")
            );
            classificacoes.AddRow(
                new Markup($"{GetArrowPercentil(percentil, 3, 15)}[{Tema.Atual.Magreza}]Abaixo[/]"),
                new Markup("P3 – P15")
            );
            classificacoes.AddRow(
                new Markup($"{GetArrowPercentil(percentil, 15, 85)}[{Tema.Atual.Normal}]Normal[/]"),
                new Markup("P15 – P85")
            );
            classificacoes.AddRow(
                new Markup($"{GetArrowPercentil(percentil, 85, 97)}[{Tema.Atual.Sobrepeso}]Sobrepeso[/]"),
                new Markup("P85 – P97")
            );
            classificacoes.AddRow(
                new Markup($"{GetArrowPercentil(percentil, 97, 100)}[{Tema.Atual.ObesidadeI}]Obesidade[/]"),
                new Markup("≥ P97")
            );
        }
        else
        {
            classificacoes.AddRow(
                new Markup($"{GetArrow(imc, 0, 18.5f)}[{Tema.Atual.Magreza}]Magreza[/]"),
                new Markup("≤ 18,5")
            );
            classificacoes.AddRow(
                new Markup($"{GetArrow(imc, 18.5f, 25f)}[{Tema.Atual.Normal}]Normal[/]"),
                new Markup("18,5 – 24,9")
            );
            classificacoes.AddRow(
                new Markup($"{GetArrow(imc, 25f, 30f)}[{Tema.Atual.Sobrepeso}]Sobrepeso[/]"),
                new Markup("25,0 – 29,9")
            );
            classificacoes.AddRow(
                new Markup($"{GetArrow(imc, 30f, 35f)}[{Tema.Atual.ObesidadeI}]Obesidade I[/]"),
                new Markup("30,0 – 34,9")
            );
            classificacoes.AddRow(
                new Markup($"{GetArrow(imc, 35f, 40f)}[{Tema.Atual.ObesidadeII}]Obesidade II[/]"),
                new Markup("35,0 – 39,9")
            );
            classificacoes.AddRow(
                new Markup($"{GetArrow(imc, 40f)}[{Tema.Atual.ObesidadeIII}]Obesidade III[/]"),
                new Markup("≥ 40,0")
            );
        }

        return classificacoes;
    }

    private static string GetArrow(float imcValue, float min, float? max = null)
    {
        if (max.HasValue)
            return imcValue >= min && imcValue < max ? "►" : "";
        return imcValue >= min ? "►" : "";
    }
    
    private static string GetArrowPercentil(double percentil, double min, double max)
    {
        return percentil >= min && percentil < max ? "►" : "";
    }

    private static int CalculateCursorPosition(float imc)
    {
        return imc switch
        {
            <= 0f => 0,
            < 18.5f => 5,
            < 25f => 15 + (int)((imc - 18.5f) / (25f - 18.5f) * 10),
            < 29.9f => 25 + (int)((imc - 25f) / (29.9f - 25f) * 10),
            < 34.9f => 35 + (int)((imc - 29.9f) / (34.9f - 29.9f) * 10),
            < 39.9f => 45 + (int)((imc - 34.9f) / (39.9f - 34.9f) * 10),
            >= 39.9f => 55 + Math.Min((int)((imc - 39.9f) / 10f * 10), 9),
            _ => 0
        };
    }
}