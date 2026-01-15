using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.UserManager;

// Menu para selecionar um utilizador existente
public static class UserSelector
{
    // Mostra lista de utilizadores e permite selecionar um
    public static Program.Pessoa? SelecionarUtilizador()
    {
        var utilizadores = UserDataManager.LoadAllUsers();
        var indiceSelecionado = 0;
        var emExecucao = true;
        Program.Pessoa? resultado = null;

        while (emExecucao)
        {
            var conteudo = new List<IRenderable>();
            
            // Se não existem utilizadores, mostra mensagem
            if (utilizadores.Count == 0)
            {
                Tema.Atual = Tema.Default;
                
                conteudo.Add(new Align(new Panel(
                    new Markup(
                        $"[{Tema.Atual.Cabecalho.ToMarkup()} bold]Nenhum utilizador encontrado[/]\n\n" +
                        $"[{Tema.Atual.Texto}]Pressione ENTER para criar um novo utilizador[/]\n" +
                        $"[dim]ou ESC para voltar ao menu[/]"
                    ).Centered()
                ).RoundedBorder().BorderColor(Tema.Atual.Borda), HorizontalAlignment.Center, VerticalAlignment.Middle));

                HelpersUI.Render(conteudo, "Selecionar Utilizador");

                var tecla = Console.ReadKey(true).Key;
                if (tecla == ConsoleKey.Enter)
                {
                    var novoUtilizador = UserCreationWizard.CriarPessoa();
                    if (novoUtilizador != null)
                    {
                        resultado = novoUtilizador;
                        emExecucao = false;
                    }
                }
                else if (tecla == ConsoleKey.Escape)
                {
                    emExecucao = false;
                }

                continue;
            }

            // Mostra título
            conteudo.Add(new Text(""));
            conteudo.Add(new Text(""));
            var titulo = new FigletText("Utilizadores")
                .Color(Tema.Atual.Titulo)
                .Centered();
            conteudo.Add(titulo);

            // Cria painel com lista de utilizadores
            var conteudoPainel = new List<IRenderable>();

            foreach (var (utilizador, i) in utilizadores.Select((u, idx) => (u, idx)))
            {
                var selecionado = i == indiceSelecionado;
    
                // Cor baseada no sexo
                var corNome = utilizador.Sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
    
                var infoUtilizador = new List<string>();
    
                // Nome do utilizador
                var nome = selecionado
                    ? $"[bold {corNome.ToMarkup()}]{utilizador.Nome}[/]"
                    : $"[{corNome.ToMarkup()}]{utilizador.Nome}[/]";
                infoUtilizador.Add(nome);
    
                // Idade
                if (utilizador.DataNascimento != default)
                {
                    infoUtilizador.Add($"[dim]{utilizador.Idade}a[/]");
                }
    
                // IMC
                if (utilizador.Peso > 0 && utilizador.Altura > 0)
                {
                    var imc = CalcIMC.Calcular(utilizador.Peso, utilizador.Altura);
                    var corIMC = CalcIMC.ObterCor(imc);
                    infoUtilizador.Add($"[{corIMC.ToMarkup()}]{imc:F1}[/]");
                }

                var linhaUtilizador = string.Join(" [dim]•[/] ", infoUtilizador);
    
                // Adiciona seta se selecionado
                if (selecionado)
                {
                    linhaUtilizador = $"[{Tema.Atual.Cabecalho.ToMarkup()}]►[/] " + linhaUtilizador;
                }
                else
                {
                    linhaUtilizador = "  " + linhaUtilizador;
                }

                conteudoPainel.Add(new Markup(linhaUtilizador).Centered());
            }

            // Painel principal
            var painelPrincipal = new Panel(new Rows(conteudoPainel))
                .Padding(4, 2)
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda);

            conteudo.Add(Align.Center(painelPrincipal));
            
            // Instruções
            conteudo.Add(new Text("\n\n"));
            conteudo.Add(new Markup(
                $"[{Tema.Atual.Cabecalho.ToMarkup()}]↑↓[/] Navegar  " +
                $"[{Tema.Atual.Normal.ToMarkup()}]ENTER[/] Selecionar  " +
                $"[{Tema.Atual.Normal.ToMarkup()}]N[/] Novo  " +
                $"[{Color.Red.ToMarkup()}]DEL[/] Apagar  " +
                $"[dim]ESC Voltar[/]"
            ).Centered());

            HelpersUI.Render(conteudo, "Selecionar Utilizador");

            // Processa input
            var teclaPressionada = Console.ReadKey(true);

            switch (teclaPressionada.Key)
            {
                case ConsoleKey.UpArrow:
                    indiceSelecionado = (indiceSelecionado - 1 + utilizadores.Count) % utilizadores.Count;
                    break;

                case ConsoleKey.DownArrow:
                    indiceSelecionado = (indiceSelecionado + 1) % utilizadores.Count;
                    break;

                case ConsoleKey.Enter:
                    // Seleciona utilizador
                    resultado = utilizadores[indiceSelecionado];
                    UserDataManager.SaveCurrentUser(resultado.Nome);
                    HelpersUI.MostrarMensagem($"Utilizador '{resultado.Nome}' carregado!", Tema.Atual.Normal);
                    emExecucao = false;
                    break;

                case ConsoleKey.N:
                    // Cria novo utilizador
                    var novoUtilizador = UserCreationWizard.CriarPessoa();
                    if (novoUtilizador != null)
                    {
                        utilizadores = UserDataManager.LoadAllUsers(); 
                        indiceSelecionado = utilizadores.FindIndex(u => u.Nome == novoUtilizador.Nome);
                        if (indiceSelecionado < 0) indiceSelecionado = 0;
                    }
                    break;

                case ConsoleKey.Delete:
                    // Apaga utilizador
                    if (utilizadores.Count > 0)
                    {
                        var utilizadorApagar = utilizadores[indiceSelecionado];
                        if (HelpersUI.ConfirmarAcao($"Tem certeza que deseja apagar '{utilizadorApagar.Nome}'?"))
                        {
                            if (UserDataManager.DeleteUser(utilizadorApagar.Nome))
                            {
                                HelpersUI.MostrarMensagem($"Utilizador '{utilizadorApagar.Nome}' apagado!", Color.Red);
                                utilizadores = UserDataManager.LoadAllUsers();
                                if (indiceSelecionado >= utilizadores.Count)
                                    indiceSelecionado = Math.Max(0, utilizadores.Count - 1);
                            }
                            else
                            {
                                HelpersUI.MostrarMensagem("Erro ao apagar utilizador!", Color.Red);
                            }
                        }
                    }
                    break;

                case ConsoleKey.Escape:
                    emExecucao = false;
                    break;
            }
        }

        return resultado;
    }
}