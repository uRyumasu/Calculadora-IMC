using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;


public static class MenuObterIMC
{
    public static void Mostrar(Program.Pessoa pessoa, string version)
    {
        var star = @"
       .
      /O\
'oooooOOOooooo'
   `OOOOOOO`
    OOO'OOO
   O'     'O";

        var content = new List<IRenderable>();

        var imc = Helpers.CalcularIMC(pessoa.peso, pessoa.altura);

        var starsCount = imc switch
        {
            < 18.5f => 2,
            < 25.0f => 5,
            < 30.0f => 4,
            < 35.0f => 3,
            < 40.0f => 2,
            _ => 1
        };

        var gridInfo = new Grid();
        for (var i = 0; i < 2; i++) gridInfo.AddColumn();
        gridInfo.AddRow(
            new FigletText($"{pessoa.peso:F1}kg").Centered().Color(Tema.Atual.Peso),
            new FigletText($"{pessoa.altura:F2}m").Centered().Color(Tema.Atual.Altura)
        );
        content.Add(new Panel(gridInfo).RoundedBorder().BorderColor(Tema.Atual.Borda));

        Helpers.CentrarVert(content, 13);

        content.Add(new FigletText($"{imc:f2}").Centered().Color(Helpers.IMCtoColor(imc)));

        Helpers.CentrarVert(content, 13);

        var grid = new Grid();
        for (var i = 0; i < 5; i++) grid.AddColumn();

        var starRow = new string[5];
        var starColor = Helpers.IMCtoColor(imc);
        for (var i = 0; i < 5; i++)
        {
            var color = i < starsCount ? starColor.ToMarkup() : "grey";
            starRow[i] = $"[{color}]{star}[/]";
        }

        grid.AddRow(starRow[0], starRow[1], starRow[2], starRow[3], starRow[4]);

        content.Add(Align.Center(grid));

        content.Add(new Markup($"\n[dim]{version}[/]"));

        Helpers.Render(content, "Obter IMC");
        Console.ReadKey(true);
    }
}