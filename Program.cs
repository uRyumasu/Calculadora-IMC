using System.Diagnostics;
using System.Text;
using CalculadoraIMC.Core;
using CalculadoraIMC.Menus;
using CalculadoraIMC.UI;
using CalculadoraIMC.UserManager;

namespace CalculadoraIMC;

public class Program
{
    // Níveis de atividade física do utilizador
    public enum NivelAtividade
    {
        Sedentario,
        Leve,
        Moderado,
        Ativo,
        MuitoAtivo
    }

    // Objetivos do utilizador (perda, ganho, manutenção)
    public enum Objetivo
    {
        PerderPeso,
        ManterPeso,
        GanharMassa,
        Definicao,
        Recomposicao
    }

    // Sexo do utilizador
    public enum Sexo
    {
        NaoDefinido,
        Masculino,
        Feminino
    }

    // Sistema de unidades (métrico ou imperial)
    public enum UnidadeSistema
    {
        Metrico,
        Imperial
    }

    private static Pessoa pessoa;

    private static void Main(string[] args)
    {
        try
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
        
            ImageDownloader.Download("https://i.ibb.co/CKV6qzT8/image.png", "./splash.png");

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
                    case '8': 
                        Tema.Atual = Tema.Default;
                        pessoa = MenuLogin.Pedir(); 
                        break;
                }
            } while (opcao != '9');
            Console.Clear();
        }
        finally
        {
            // Ao fechar, executa splash no cmd.exe
            Splash.ExecutarSplashNoCmd();
        }
    }

    // Representa os dados de uma pessoa/utilizador
    public class Pessoa
    {
        public string Nome { get; set; }
        public float Altura { get; set; } = 1.5f;
        public DateTime DataNascimento { get; set; }
        public Sexo Sexo { get; set; } = Sexo.NaoDefinido;
        public NivelAtividade NivelAtividade { get; set; }
        public UnidadeSistema UnidadeSistema { get; set; }
        public Objetivo Objetivo { get; set; }
        public float PesoDesejado { get; set; }
        public string NomeTema { get; set; } = "Default";

        // Calcula a idade a partir da data de nascimento
        public int Idade => DateTime.Now.Year - DataNascimento.Year;

        // Peso inicial (quando a pessoa foi criada/registada)
        public float PesoInicial { get; set; }

        // Peso atual da pessoa
        public float Peso { get; set; }

        // Construtor padrão - inicializa com valores base
        public Pessoa()
        {
            Peso = 60;
            PesoInicial = 60;
        }
    }
}