using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using CalculadoraIMC.UserManager;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.Menus;

public class MenuLogin
{
    public static void Mostrar()
    {
        var content = new List<IRenderable>();

        Helpers.CentrarVert(content, 12);
        content.Add(new FigletText("Calculadora IMC")
            .Color(Tema.Atual.Titulo)
            .Centered());

        Helpers.CentrarVert(content, 10);
        
        content.Add(Align.Center(new Markup(
            $"[{Tema.Atual.Texto.ToMarkup()}]" +
            "1) Criar Utilizador\n" +
            "2) Selecionar Utilizador\n" +
            "3) Sem utilizador\n" +
            "4) Sair\n" +
            "[/]"
        )));

        Helpers.Render(content, "Calculadora IMC");
    }

    public static Program.Pessoa Pedir()
    {
        char opcaoLogin;
        var escolhaLogin = true;
        
        Program.Pessoa? pessoa = new Program.Pessoa();
        
        do
        {
            Mostrar();
            opcaoLogin = Console.ReadKey(true).KeyChar;
            
            switch (opcaoLogin)
            {
                case '1':
                    pessoa = UserCreationWizard.CriarPessoa();
                    if (pessoa == null) continue;
                    Tema.Atual = Tema.ObterPorNome(pessoa.NomeTema);
                    escolhaLogin = false;
                    break;
                case '2':
                    pessoa = UserSelector.SelecionarUtilizador();
                    if (pessoa == null) continue;
                    Tema.Atual = Tema.ObterPorNome(pessoa.NomeTema);
                    escolhaLogin = false;
                    break;
                case '3':
                    pessoa = new Program.Pessoa();
                    pessoa.peso = 60;
                    escolhaLogin = false;
                    break;
                case '4':
                    escolhaLogin = false;
                    Environment.Exit(0);
                    break;
            }
        } while (escolhaLogin);

        return pessoa;
    }
}