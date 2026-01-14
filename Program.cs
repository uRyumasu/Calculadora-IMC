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
        NaoDefinido,
        Masculino,
        Feminino
    }

    public enum UnidadeSistema
    {
        Metrico,
        Imperial
    }

    private static Pessoa pessoa;

    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        // Optional: Show splash screen
        // SplashScreen.Show("path/to/image.png");

        pessoa = MenuLogin.Pedir();
        
        char opcao;
        do
        {
            MenuPrincipal.Mostrar(pessoa);
            opcao = Console.ReadKey(true).KeyChar;

            switch (opcao)
            {
                case '1': MenuDefinirDados.Mostrar(pessoa); break;
                case '2': MenuObterIMC.Mostrar(pessoa); break;
                case '3': MenuStatusIMC.Mostrar(pessoa); break;
                case '7': MenuTemas.Mostrar(pessoa); break;
                case '8': Tema.Atual = Tema.Default ;pessoa = MenuLogin.Pedir(); break;
                
            }
        } while (opcao != '9');

        // Optional: Show splash screen on exit
        // SplashScreen.Show("path/to/image.png");
        Console.Clear();
    }

    public class Pessoa
    {
        public string nome { get; set; }
        public float altura { get; set; } = 1.5f;
        public DateTime dataNascimento { get; set; }
        public Sexo sexo { get; set; } = Sexo.NaoDefinido;
        public NivelAtividade nivelAtividade { get; set; }
        public UnidadeSistema unidadeSistema { get; set; }
        public Objetivo objetivo { get; set; }

        public int Idade => DateTime.Now.Year - dataNascimento.Year;
        public float pesoDesejado { get; set; }
        public string NomeTema { get; set; } = "Default";
        
        
        private float _pesoInicial;
        private float _peso;
        private bool _pesoFoiAlterado = false;
        public float pesoInicial 
        { 
            get => _pesoInicial;
            set 
            {
                _pesoInicial = value;
                // Only sync to current peso if it hasn't been modified by the user/app yet
                if (!_pesoFoiAlterado)
                {
                    _peso = value;
                }
            }
        }

        public float peso 
        { 
            get => _peso;
            set 
            {
                _peso = value;
                _pesoFoiAlterado = true; // Once this is true, pesoInicial stops syncing
            }
        }
    }
}