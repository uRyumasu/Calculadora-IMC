using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

public static class MenuTemas
{
    // Mostra o menu de seleção de temas
    public static void Mostrar(Program.Pessoa pessoa)
    {
        bool emExecucao = true;
        Console.CursorVisible = false;

        while (emExecucao)
        {
            var conteudo = new List<IRenderable>();

            HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_MEDIO);
            conteudo.Add(new FigletText("TEMAS")
                .Color(Tema.Atual.Titulo)
                .Centered());

            HelpersUI.CentrarVertical(conteudo);

            // Linha com setas e nome do tema atual
            var linha = new Columns(
                new Markup($"[{Tema.Atual.Peso.ToMarkup()}]◄[/]").LeftJustified(),
                new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()} bold]{Tema.Atual.Nome}[/]").Centered(),
                new Markup($"[{Tema.Atual.Altura.ToMarkup()}]►[/]").RightJustified()
            ).Expand();

            conteudo.Add(linha);
            HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_MEDIO);

            conteudo.Add(new Markup(
                "[dim]Use ← → para mudar de tema[/]\n" +
                "[dim]Pressione ENTER para voltar[/]"
            ).Centered());

            HelpersUI.Render(conteudo, "Configurações de Tema");

            var tecla = Console.ReadKey(true).Key;

            switch (tecla)
            {
                case ConsoleKey.LeftArrow:
                    CiclarTema(false);
                    pessoa.NomeTema = Tema.Atual.Nome;
                    break;

                case ConsoleKey.RightArrow:
                    CiclarTema(true);
                    pessoa.NomeTema = Tema.Atual.Nome;
                    break;

                case ConsoleKey.Enter:
                    emExecucao = false;
                    UserDataManager.SaveUser(pessoa);
                    break;
            }
        }
    }

    // Cicla entre os temas disponíveis
    private static void CiclarTema(bool avancar)
    {
        int indiceAtual = Tema.Todos.FindIndex(t => t.Nome == Tema.Atual.Nome);

        if (avancar)
        {
            int proximoIndice = (indiceAtual + 1) % Tema.Todos.Count;
            Tema.Atual = Tema.Todos[proximoIndice];
        }
        else
        {
            int indiceAnterior = (indiceAtual - 1 + Tema.Todos.Count) % Tema.Todos.Count;
            Tema.Atual = Tema.Todos[indiceAnterior];
        }
    }
}