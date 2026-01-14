using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;


public static class MenuPrincipal
{
    public static void Mostrar(Program.Pessoa pessoa)
    {
        var content = new List<IRenderable>();

        Helpers.CentrarVert(content, 12);
        content.Add(new FigletText("Calculadora IMC")
            .Color(Tema.Atual.Titulo)
            .Centered());

        Helpers.CentrarVert(content, 10);

        content.Add(Align.Center(new Markup(
            $"[{Tema.Atual.Texto.ToMarkup()}]" +
            "1) Definir dados\n" +
            "2) Obter IMC\n" +
            "3) Status IMC\n" +
            "\n7) Definições\n" +
            "8) Selecionar Utilizador\n" +
            "9) Sair[/]"
        )));

        content.Add(new Markup(new string('\n', 4)));
        content.Add(new Markup($"[dim]{(string.IsNullOrEmpty(pessoa.nome) ? "Sem utilizador" : pessoa.nome)}[/]"));

        Helpers.Render(content, "Calculadora IMC");
    }
}