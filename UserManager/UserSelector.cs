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
                // No users found
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

            // Title at the top with 2-line padding
            content.Add(new Text(""));
            content.Add(new Text(""));
            var title = new FigletText("Utilizadores")
                .Color(Tema.Atual.Titulo)
                .Centered();
            content.Add(title);

            // Center the user list panel vertically
            var panelContent = new List<IRenderable>();

            // Create user list
            foreach (var (user, i) in usuarios.Select((u, idx) => (u, idx)))
            {
                var isSelected = i == selectedIndex;
    
                // Color code by gender
                var nomeColor = user.sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
    
                // Build user info line
                var userInfo = new List<string>();
    
                // Name (bold if selected)
                var nome = isSelected
                    ? $"[bold {nomeColor.ToMarkup()}]{user.nome}[/]"
                    : $"[{nomeColor.ToMarkup()}]{user.nome}[/]";
                userInfo.Add(nome);
    
                // Age
                if (user.dataNascimento != default)
                {
                    userInfo.Add($"[dim]{user.Idade}a[/]");
                }
    
                // IMC
                if (user.peso > 0 && user.altura > 0)
                {
                    var bmival = UnitConverter.CalculateBMI(user.peso, user.altura);
                    var bmiColor = Helpers.IMCtoColor(bmival);
                    userInfo.Add($"[{bmiColor.ToMarkup()}]{bmival:F1}[/]");
                }

                var userLine = string.Join(" [dim]•[/] ", userInfo);
    
                // Selection indicator
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

            // Wrap user list in a panel
            var mainPanel = new Panel(new Rows(panelContent))
                .Padding(4, 2)
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda);

            content.Add(Align.Center(mainPanel));
            
            // Controls 2 lines above the bottom
            content.Add(new Text("\n\n"));
            content.Add(new Markup(
                $"[{Tema.Atual.Cabecalho.ToMarkup()}]↑↓[/] Navegar  " +
                $"[{Tema.Atual.Normal.ToMarkup()}]ENTER[/] Selecionar  " +
                $"[{Tema.Atual.Normal.ToMarkup()}]N[/] Novo  " +
                $"[{Color.Red.ToMarkup()}]DEL[/] Apagar  " +
                $"[dim]ESC Voltar[/]"
            ).Centered());

            Helpers.Render(content, "Selecionar Utilizador");

            // Handle input
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
                    // Load selected user
                    result = usuarios[selectedIndex];
                    UserDataManager.SaveCurrentUser(result.nome);
                    Helpers.MostrarMensagem($"Utilizador '{result.nome}' carregado!", Tema.Atual.Normal);
                    running = false;
                    break;

                case ConsoleKey.N:
                    // Create new user
                    var novoUsuario = UserCreationWizard.CriarPessoa();
                    if (novoUsuario != null)
                    {
                        usuarios = UserDataManager.LoadAllUsers(); // Reload list
                        selectedIndex = usuarios.FindIndex(u => u.nome == novoUsuario.nome);
                        if (selectedIndex < 0) selectedIndex = 0;
                    }

                    break;

                case ConsoleKey.Delete:
                    // Delete selected user
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

    /// <summary>
    ///     Show user details
    /// </summary>
    public static void MostrarDetalhesUtilizador(Program.Pessoa pessoa)
    {
        var content = new List<IRenderable>();
        Helpers.CentrarVert(content, 10);

        var detailsTable = new Table()
            .RoundedBorder()
            .BorderColor(Tema.Atual.Borda)
            .AddColumn(new TableColumn("[bold]Campo[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Valor[/]").RightAligned());

        var corSexo = pessoa.sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;

        detailsTable.AddRow("Nome", $"[{Tema.Atual.Texto}]{pessoa.nome}[/]");
        detailsTable.AddRow("Data de Nascimento", $"[{Tema.Atual.Texto}]{pessoa.dataNascimento:dd/MM/yyyy}[/]");
        detailsTable.AddRow("Idade", $"[{Tema.Atual.Texto}]{pessoa.Idade} anos[/]");
        detailsTable.AddRow("Sexo", $"[{corSexo.ToMarkup()}]{pessoa.sexo}[/]");
        detailsTable.AddRow("Sistema", $"[{Tema.Atual.Texto}]{pessoa.unidadeSistema}[/]");
        detailsTable.AddRow("Altura",
            $"[{Tema.Atual.Altura}]{UnitConverter.FormatHeight(pessoa.altura, pessoa.unidadeSistema)}[/]");
        detailsTable.AddRow("Peso",
            $"[{Tema.Atual.Peso}]{UnitConverter.FormatWeight(pessoa.peso, pessoa.unidadeSistema)}[/]");

        if (pessoa.peso > 0 && pessoa.altura > 0)
        {
            var bmi = UnitConverter.CalculateBMI(pessoa.peso, pessoa.altura);
            var bmiColor = Helpers.IMCtoColor(bmi);
            detailsTable.AddRow("IMC", $"[{bmiColor.ToMarkup()}]{bmi:F1} ({UnitConverter.GetBMICategory(bmi)})[/]");
        }

        detailsTable.AddRow("Nível de Atividade",
            $"[{Tema.Atual.Texto}]{(pessoa.nivelAtividade != default ? pessoa.nivelAtividade.ToString() : "N/A")}[/]");
        detailsTable.AddRow("Objetivo", $"[{Tema.Atual.Texto}]{pessoa.objetivo}[/]");
        detailsTable.AddRow("Peso Desejado",
            $"[{Tema.Atual.Peso}]{UnitConverter.FormatWeight(pessoa.pesoDesejado, pessoa.unidadeSistema)}[/]");

        content.Add(new Panel(detailsTable)
            .Header($"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Detalhes do Utilizador[/]")
            .RoundedBorder()
            .BorderColor(Tema.Atual.Borda));

        content.Add(new Markup("\n[dim]Pressione qualquer tecla para voltar[/]").Centered());

        Helpers.Render(content, "Detalhes do Utilizador");
        Console.ReadKey(true);
    }
}