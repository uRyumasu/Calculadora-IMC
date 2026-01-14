using System.Text;
using CalculadoraIMC.Core;
using CalculadoraIMC.Menus;
using CalculadoraIMC.UI;
using CalculadoraIMC.UserManager;

namespace CalculadoraIMC;

public class Program
{
    public enum NivelAtividade
    {
        Sedentario,
        Leve,
        Moderado,
        Ativo,
        MuitoAtivo
    }

    public enum Objetivo
    {
        PerderPeso,
        ManterPeso,
        GanharMassa,
        Definicao,
        Recomposicao
    }

    public enum Sexo
    {
        Masculino,
        Feminino
    }

    public enum UnidadeSistema
    {
        Metrico,
        Imperial
    }

    private const string version = "1.2.5";

    private static Pessoa pessoa = new();

    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        // Optional: Show splash screen
        // SplashScreen.Show("path/to/image.png");

        // Load last user if exists
        var lastUser = UserDataManager.LoadCurrentUser();
        if (lastUser != null) pessoa = lastUser;

        Tema.Atual = Tema.Default;

        char opcao;
        do
        {
            MenuPrincipal.Mostrar(version);
            opcao = Console.ReadKey(true).KeyChar;

            switch (opcao)
            {
                case '1': MenuDefinirDados.Mostrar(pessoa, version); break;
                case '2': MenuObterIMC.Mostrar(pessoa, version); break;
                case '3': MenuTemas.Mostrar(version); break;
                case '4': MenuStatusIMC.Mostrar(pessoa); break;
                case '7':
                    var selectedUser = UserSelector.SelecionarUtilizador();
                    if (selectedUser != null) pessoa = selectedUser;
                    break;
                case '8':
                    var novaPessoa = UserCreationWizard.CriarPessoa();
                    if (novaPessoa != null) pessoa = novaPessoa;
                    break;
            }
        } while (opcao != '5');

        // Optional: Show splash screen on exit
        // SplashScreen.Show("path/to/image.png");
        Console.Clear();
    }

    public class Pessoa
    {
        public string nome { get; set; }
        public float peso { get; set; }
        public float altura { get; set; }
        public DateTime dataNascimento { get; set; }
        public Sexo sexo { get; set; } = Sexo.Masculino;
        public NivelAtividade nivelAtividade { get; set; }
        public UnidadeSistema unidadeSistema { get; set; }
        public Objetivo objetivo { get; set; }

        public int Idade => DateTime.Now.Year - dataNascimento.Year;
        public float pesoDesejado { get; set; }
    }
}