using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

public static class MenuPrincipal
{
    // Mostra o menu principal da aplicação
    public static void Mostrar(Program.Pessoa pessoa)
    {
        var conteudo = new List<IRenderable>();

        HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_GRANDE);
        conteudo.Add(new FigletText("Calculadora IMC")
            .Color(Tema.Atual.Titulo)
            .Centered());

        HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_MEDIO);

        conteudo.Add(Align.Center(new Markup(
            $"[{Tema.Atual.Texto.ToMarkup()}]" +
            "1) Definir dados\n" +
            "2) Obter IMC\n" +
            "3) Status IMC\n" +
            "\n7) Definições\n" +
            "8) Selecionar Utilizador\n" +
            "9) Sair[/]"
        )));

        conteudo.Add(new Markup(new string('\n', 4)));
        conteudo.Add(new Markup($"[dim]{(string.IsNullOrEmpty(pessoa.Nome) ? "Sem utilizador" : pessoa.Nome)}[/]"));

        HelpersUI.Render(conteudo, "Calculadora IMC");
    }
}