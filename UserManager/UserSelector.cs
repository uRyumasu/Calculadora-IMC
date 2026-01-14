using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.UserManager;


public static class UserSelector
{
    public static Program.Pessoa? SelecionarUtilizador()
    {
        var usuarios = UserDataManager.LoadAllUsers();
        var selectedIndex = 0;
        var running = true;
        Program.Pessoa? result = null;

        while (running)
        {
            var content = new List<IRenderable>();
            
            if (usuarios.Count == 0)
            {
                Tema.Atual = Tema.Default;
                
                content.Add(new Align(new Panel(
                    new Markup(
                        $"[{Tema.Atual.Cabecalho.ToMarkup()} bold]Nenhum utilizador encontrado[/]\n\n" +
                        $"[{Tema.Atual.Texto}]Pressione ENTER para criar um novo utilizador[/]\n" +
                        $"[dim]ou ESC para voltar ao menu[/]"
                    ).Centered()
                ).RoundedBorder().BorderColor(Tema.Atual.Borda), HorizontalAlignment.Center, VerticalAlignment.Middle));

                Helpers.Render(content, "Selecionar Utilizador");

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter)
                {
                    var novoUser = UserCreationWizard.CriarPessoa();
                    if (novoUser != null)
                    {
                        result = novoUser;
                        running = false;
                    }
                }
                else if (key == ConsoleKey.Escape)
                {
                    running = false;
                }

                continue;
            }

            
            content.Add(new Text(""));
            content.Add(new Text(""));
            var title = new FigletText("Utilizadores")
                .Color(Tema.Atual.Titulo)
                .Centered();
            content.Add(title);

            
            var panelContent = new List<IRenderable>();

            
            foreach (var (user, i) in usuarios.Select((u, idx) => (u, idx)))
            {
                var isSelected = i == selectedIndex;
    
                
                var nomeColor = user.sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
    
                
                var userInfo = new List<string>();
    
                
                var nome = isSelected
                    ? $"[bold {nomeColor.ToMarkup()}]{user.nome}[/]"
                    : $"[{nomeColor.ToMarkup()}]{user.nome}[/]";
                userInfo.Add(nome);
    
                
                if (user.dataNascimento != default)
                {
                    userInfo.Add($"[dim]{user.Idade}a[/]");
                }
    
                
                if (user.peso > 0 && user.altura > 0)
                {
                    var bmival = UnitConverter.CalculateBMI(user.peso, user.altura);
                    var bmiColor = Helpers.IMCtoColor(bmival);
                    userInfo.Add($"[{bmiColor.ToMarkup()}]{bmival:F1}[/]");
                }

                var userLine = string.Join(" [dim]•[/] ", userInfo);
    
                
                if (isSelected)
                {
                    userLine = $"[{Tema.Atual.Cabecalho.ToMarkup()}]►[/] " + userLine;
                }
                else
                {
                    userLine = "  " + userLine;
                }

                panelContent.Add(new Markup(userLine).Centered());
            }

            
            var mainPanel = new Panel(new Rows(panelContent))
                .Padding(4, 2)
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda);

            content.Add(Align.Center(mainPanel));
            
            
            content.Add(new Text("\n\n"));
            content.Add(new Markup(
                $"[{Tema.Atual.Cabecalho.ToMarkup()}]↑↓[/] Navegar  " +
                $"[{Tema.Atual.Normal.ToMarkup()}]ENTER[/] Selecionar  " +
                $"[{Tema.Atual.Normal.ToMarkup()}]N[/] Novo  " +
                $"[{Color.Red.ToMarkup()}]DEL[/] Apagar  " +
                $"[dim]ESC Voltar[/]"
            ).Centered());

            Helpers.Render(content, "Selecionar Utilizador");

            
            var keyPressed = Console.ReadKey(true);

            switch (keyPressed.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + usuarios.Count) % usuarios.Count;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % usuarios.Count;
                    break;

                case ConsoleKey.Enter:
                    
                    result = usuarios[selectedIndex];
                    UserDataManager.SaveCurrentUser(result.nome);
                    Helpers.MostrarMensagem($"Utilizador '{result.nome}' carregado!", Tema.Atual.Normal);
                    running = false;
                    break;

                case ConsoleKey.N:
                    
                    var novoUsuario = UserCreationWizard.CriarPessoa();
                    if (novoUsuario != null)
                    {
                        usuarios = UserDataManager.LoadAllUsers(); 
                        selectedIndex = usuarios.FindIndex(u => u.nome == novoUsuario.nome);
                        if (selectedIndex < 0) selectedIndex = 0;
                    }

                    break;

                case ConsoleKey.Delete:
                    
                    if (usuarios.Count > 0)
                    {
                        var userToDelete = usuarios[selectedIndex];
                        if (Helpers.ConfirmarAcao($"Tem certeza que deseja apagar '{userToDelete.nome}'?"))
                        {
                            if (UserDataManager.DeleteUser(userToDelete.nome))
                            {
                                
                                Helpers.MostrarMensagem($"Utilizador '{userToDelete.nome}' apagado!", Color.Red);
                                usuarios = UserDataManager.LoadAllUsers();
                                if (selectedIndex >= usuarios.Count)
                                    selectedIndex = Math.Max(0, usuarios.Count - 1);
                            }
                            else
                            {
                                Helpers.MostrarMensagem("Erro ao apagar utilizador!", Color.Red);
                            }
                        }
                    }

                    break;

                case ConsoleKey.Escape:
                    running = false;
                    break;
            }
        }

        return result;
    }
}