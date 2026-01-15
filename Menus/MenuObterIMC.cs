using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

// Mostra o menu para obter o imc
public static class MenuObterIMC
{
    // Desenho ascii de uma estrela
    private const string ESTRELA = @"
       .
      /O\
'oooooOOOooooo'
   `OOOOOOO`
    OOO'OOO
   O'     'O";

    // Mostra o IMC calculado com visualização de estrelas
    public static void Mostrar(Program.Pessoa pessoa)
    {
        var conteudo = new List<IRenderable>();
        bool usarPercentil = PercentilIMC.DeveUsarPercentil(pessoa);
        
        float imc = CalcIMC.Calcular(pessoa.Peso, pessoa.Altura);
        float valorExibir = imc;
        
        // Se for criança/adolescente, usa percentil em vez de IMC
        if (usarPercentil)
        {
            var (percentil, _) = PercentilIMC.CalcularPercentil(pessoa, imc);
            valorExibir = (float)percentil;
        }

        // Grid com peso e altura
        var gridInfo = new Grid();
        for (int i = 0; i < 2; i++) gridInfo.AddColumn();
        gridInfo.AddRow(
            new FigletText(UnitConverter.FormatarPeso(pessoa.Peso, pessoa.UnidadeSistema))
                .Centered().Color(Tema.Atual.Peso),
            new FigletText(UnitConverter.FormatarAltura(pessoa.Altura, pessoa.UnidadeSistema))
                .Centered().Color(Tema.Atual.Altura)
        );
        conteudo.Add(new Panel(gridInfo).RoundedBorder().BorderColor(Tema.Atual.Borda));

        HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_MINIMO);

        // Valor principal (IMC ou percentil)
        conteudo.Add(new FigletText($"{valorExibir:f2}")
            .Centered()
            .Color(CalcIMC.ObterCor(imc)));

        HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_MINIMO);

        // Estrelas de avaliação
        var grid = new Grid();
        for (int i = 0; i < 5; i++) grid.AddColumn();

        int numEstrelas = CalcularNumeroEstrelas(valorExibir, usarPercentil);
        var corEstrela = CalcIMC.ObterCor(imc);
        
        var linhaEstrelas = new string[5];
        for (int i = 0; i < 5; i++)
        {
            string cor = i < numEstrelas ? corEstrela.ToMarkup() : "grey";
            linhaEstrelas[i] = $"[{cor}]{ESTRELA}[/]";
        }

        grid.AddRow(linhaEstrelas[0], linhaEstrelas[1], linhaEstrelas[2], 
                    linhaEstrelas[3], linhaEstrelas[4]);

        conteudo.Add(Align.Center(grid));
        
        string titulo = usarPercentil ? "Obter IMC (Percentil)" : "Obter IMC";
        HelpersUI.Render(conteudo, titulo);
        
        Console.ReadKey(true);
    }

    // Calcula quantas estrelas mostrar baseado no IMC/percentil
    private static int CalcularNumeroEstrelas(float valor, bool usarPercentil)
    {
        if (usarPercentil)
        {
            return valor switch
            {
                < (float)Constantes.PERCENTIL_MAGREZA => 1, // 1 estrela
                < (float)Constantes.PERCENTIL_ABAIXO => 2, // 2 estrela
                < (float)Constantes.PERCENTIL_NORMAL => 5, // 5 estrela
                < (float)Constantes.PERCENTIL_SOBREPESO => 4, // 4 estrela
                _ => 2
            };
        }
        else
        {
            return valor switch
            {
                < Constantes.IMC_MAGREZA => 2, // 2 estrela
                < Constantes.IMC_NORMAL => 5, // 5 estrela
                < Constantes.IMC_SOBREPESO => 4, // 4 estrela
                < Constantes.IMC_OBESIDADE_I => 3, // 3 estrela
                < Constantes.IMC_OBESIDADE_II => 2, // 2 estrela
                _ => 1
            };
        }
    }
}