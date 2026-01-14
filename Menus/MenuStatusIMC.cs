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
        var leftContent = new List<IRenderable>
        {
            new Panel(new Rows(
                    new Markup(
                        $"\n[{Tema.Atual.Texto} bold]User:[/] {(string.IsNullOrEmpty(pessoa.nome) ? "?" : pessoa.nome)}"),
                    new Markup(
                        $"[{Tema.Atual.Texto} bold]Data de Nascimento:[/] {(pessoa.dataNascimento != default ? pessoa.dataNascimento.ToString("dd/MM/yyyy") : "??")}"),
                    new Markup(
                        $"[{Tema.Atual.Texto} bold]Idade:[/] {(pessoa.dataNascimento != default ? pessoa.Idade.ToString() : "??")} anos"),
                    new Markup($"[{Tema.Atual.Texto} bold]Género:[/] {pessoa.sexo}\n"),
                    new Rule(),
                    new Markup(
                        $"\n[{Tema.Atual.Altura} bold]Altura:[/] {UnitConverter.FormatHeight(pessoa.altura, pessoa.unidadeSistema)}"),
                    new Markup(
                        $"[{Tema.Atual.Peso} bold]Peso:[/] {UnitConverter.FormatWeight(pessoa.peso, pessoa.unidadeSistema)}\n"),
                    new Rule(),
                    new Markup(
                        $"\n[{Tema.Atual.Texto} bold]Nível de Atividade:[/] {(pessoa.nivelAtividade != default ? pessoa.nivelAtividade.ToString() : "N/A")}"),
                    new Markup(
                        $"[{Tema.Atual.Texto} bold]Objetivo:[/] {(pessoa.objetivo != default ? pessoa.objetivo.ToString() : "N/A")}\n")
                ))
                .BorderColor(Tema.Atual.Borda)
                .RoundedBorder()
        };

        return new Panel(new Rows(leftContent)) { Width = 40 }.NoBorder();
    }

    private static Rows CriarPainelDireito(Program.Pessoa pessoa, float imc, string fixedIMC)
    {
        var barraIMC = CriarBarraIMC(imc, fixedIMC);
        var classificacoes = CriarTabelaClassificacoes(imc);

        var sideBySection = new Table()
            .NoBorder()
            .HideHeaders()
            .Collapse()
            .AddColumn(new TableColumn("").LeftAligned())
            .AddColumn(new TableColumn("").LeftAligned());

        sideBySection.AddRow(
            classificacoes.BorderColor(Tema.Atual.Borda),
            new Panel(new FigletText($"{imc:F1}").Color(Helpers.IMCtoColor(imc)).Centered())
                .BorderColor(Tema.Atual.Borda)
                .RoundedBorder()
        );

        var rightContent = new List<IRenderable>
        {
            new Panel(new FigletText(Helpers.IMCtoString(imc)).Color(Helpers.IMCtoColor(imc)).Centered())
                .BorderColor(Tema.Atual.Borda)
                .RoundedBorder(),
            new Panel(new Rows(barraIMC)) { Width = 100 }
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda),
            sideBySection,
            new Panel(new Markup(
                    $"[{Tema.Atual.Texto} bold]Dica:[/] Um IMC entre 18,5 e 24,9 contribui para a saúde cardiovascular"))
                .BorderColor(Tema.Atual.Borda)
                .RoundedBorder()
        };

        return new Rows(rightContent);
    }

    private static List<IRenderable> CriarBarraIMC(float imc, string fixedIMC)
    {
        var barraIMC = new List<IRenderable>();

        var bar = $"[{Tema.Atual.Magreza}]██████████[/]" +
                  $"[{Tema.Atual.Normal}]██████████[/]" +
                  $"[{Tema.Atual.Sobrepeso}]██████████[/]" +
                  $"[{Tema.Atual.ObesidadeI}]██████████[/]" +
                  $"[{Tema.Atual.ObesidadeII}]██████████[/]" +
                  $"[{Tema.Atual.ObesidadeIII}]██████████[/]";

        barraIMC.Add(new Markup(bar));

        var padding = CalculateCursorPosition(imc);

        var cursor = new string(' ', padding) + "▲";
        barraIMC.Add(new Markup(cursor));

        barraIMC.Add(new Markup(new string(' ', Math.Max(0, padding - 2)) + fixedIMC));

        return barraIMC;
    }

    private static Table CriarTabelaClassificacoes(float imc)
    {
        var classificacoes = new Table()
            .RoundedBorder()
            .HideHeaders()
            .Collapse()
            .AddColumn(new TableColumn("").LeftAligned())
            .AddColumn(new TableColumn("").RightAligned());

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

        return classificacoes;
    }

    private static string GetArrow(float imcValue, float min, float? max = null)
    {
        if (max.HasValue)
            return imcValue >= min && imcValue < max ? "►" : "";
        return imcValue >= min ? "►" : "";
    }

    private static int CalculateCursorPosition(float imc)
    {
        return imc switch
        {
            <= 0f => 0,
            < 18.5f => (int)(imc / 18.5f * 10),
            < 25f => 10 + (int)((imc - 18.5f) / (25f - 18.5f) * 10),
            < 29.9f => 20 + (int)((imc - 25f) / (29.9f - 25f) * 10),
            < 34.9f => 30 + (int)((imc - 29.9f) / (34.9f - 29.9f) * 10),
            < 39.9f => 40 + (int)((imc - 34.9f) / (39.9f - 34.9f) * 10),
            >= 39.9f => 50 + Math.Min((int)((imc - 39.9f) / 10f * 10), 9),
            _ => 0
        };
    }
}