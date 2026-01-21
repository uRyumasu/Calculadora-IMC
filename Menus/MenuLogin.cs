using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using CalculadoraIMC.UserManager;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

public class MenuLogin
{
    // Mostra o menu de login
    public static void Mostrar()
    {
        var conteudo = new List<IRenderable>();

        HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_GRANDE);
        conteudo.Add(new FigletText("Calculadora IMC")
            .Color(Tema.Atual.Titulo)
            .Centered());

        HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_MEDIO);
        
        conteudo.Add(Align.Center(new Markup(
            $"[{Tema.Atual.Texto.ToMarkup()}]" +
            "1) Criar Utilizador\n" +
            "2) Selecionar Utilizador\n" +
            "3) Sem utilizador\n" +
            "4) Sair\n" +
            "[/]"
        )));

        HelpersUI.Render(conteudo, "Calculadora IMC");
    }

    // Pede ao utilizador para escolher uma opção
    public static Program.Pessoa Pedir()
    {
        char opcao;
        bool emEscolha = true;
        Program.Pessoa? pessoa = new Program.Pessoa();
        
        do
        {
            Mostrar();
            opcao = Console.ReadKey(true).KeyChar;
            
            switch (opcao)
            {
                case '1':
                    pessoa = UserCreationWizard.CriarPessoa();
                    if (pessoa == null) continue;
                    Tema.Atual = Tema.ObterPorNome(pessoa.NomeTema);
                    emEscolha = false;
                    break;

                case '2':
                    pessoa = UserSelector.SelecionarUtilizador();
                    if (pessoa == null) continue;
                    Tema.Atual = Tema.ObterPorNome(pessoa.NomeTema);
                    emEscolha = false;
                    break;

                case '3':
                    pessoa = new Program.Pessoa();
                    pessoa.Peso = 60;
                    emEscolha = false;
                    break;

                case '4':
                    emEscolha = false;
                    Splash.Mostrar();
                    Environment.Exit(0);
                    break;
            }
        } while (emEscolha);

        return pessoa;
    }
}