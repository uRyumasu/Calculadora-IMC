using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

public static class MenuObterIMC
{
    public static void Mostrar(Program.Pessoa pessoa)
    {
        var star = @"
       .
      /O\
'oooooOOOooooo'
   `OOOOOOO`
    OOO'OOO
   O'     'O";

        var content = new List<IRenderable>();

        var usarPercentil = PercentilIMC.DeveUsarPercentil(pessoa);
        
        var imc = Helpers.CalcularIMC(pessoa.peso, pessoa.altura);
        var displayValue = imc;
        
        if (usarPercentil)
        {
            var (percentil, _) = PercentilIMC.CalcularPercentil(pessoa, imc);
            displayValue = (float)percentil;
        }

        static int starsCount(float value, bool usarPercentil)
        {
            if (usarPercentil)
            {
                return value switch
                {
                    < 3 => 1,      
                    < 15 => 2,     
                    < 85 => 5,     
                    < 97 => 4,     
                    _ => 2         
                };
            }
            else
            {
                return value switch
                {
                    < 18.5f => 2,
                    < 25.0f => 5,
                    < 30.0f => 4,
                    < 35.0f => 3,
                    < 40.0f => 2,
                    _ => 1
                };
            }
        }

        var gridInfo = new Grid();
        for (var i = 0; i < 2; i++) gridInfo.AddColumn();
        gridInfo.AddRow(
            new FigletText(UnitConverter.FormatWeight(pessoa.peso, pessoa.unidadeSistema)).Centered().Color(Tema.Atual.Peso),
            new FigletText(UnitConverter.FormatHeight(pessoa.altura, pessoa.unidadeSistema)).Centered().Color(Tema.Atual.Altura)
        );
        content.Add(new Panel(gridInfo).RoundedBorder().BorderColor(Tema.Atual.Borda));

        Helpers.CentrarVert(content, 13);

        content.Add(new FigletText($"{displayValue:f2}").Centered().Color(Helpers.IMCtoColor(imc)));

        Helpers.CentrarVert(content, 13);

        var grid = new Grid();
        for (var i = 0; i < 5; i++) grid.AddColumn();

        var starRow = new string[5];
        var starColor = Helpers.IMCtoColor(imc);
        for (var i = 0; i < 5; i++)
        {
            var color = i < starsCount(displayValue, usarPercentil) ? starColor.ToMarkup() : "grey";
            starRow[i] = $"[{color}]{star}[/]";
        }

        grid.AddRow(starRow[0], starRow[1], starRow[2], starRow[3], starRow[4]);

        content.Add(Align.Center(grid));
        
        if (usarPercentil) Helpers.Render(content, "Obter IMC (Percentil)");
        else Helpers.Render(content, "Obter IMC");
        
        Console.ReadKey(true);
    }
}