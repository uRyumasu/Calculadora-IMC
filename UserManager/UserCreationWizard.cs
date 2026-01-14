using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.UserManager;

public static class UserCreationWizard
{
    private const int TOTAL_STEPS = 10;
    private static readonly List<IRenderable> content = new();

    // ========================================================================
    // PUBLIC API
    // ========================================================================
    
    public static Program.Pessoa? CriarPessoa()
    {
        var pessoa = new Program.Pessoa();
        var stepStack = new Stack<int>();
        var currentStep = 1;

        while (currentStep <= TOTAL_STEPS)
        {
            var result = ExecuteStep(pessoa, currentStep);

            switch (result.Action)
            {
                case StepAction.Next:
                case StepAction.Skip:
                    if (result.Data != null)
                        ApplyStepData(pessoa, currentStep, result.Data);
                    stepStack.Push(currentStep);
                    currentStep++;
                    break;

                case StepAction.Back when stepStack.Count > 0:
                    currentStep = stepStack.Pop();
                    break;

                case StepAction.Cancel:
                    return null;
            }
        }

        return SaveAndReturnUser(pessoa);
    }

    // ========================================================================
    // STEP ROUTING
    // ========================================================================

    private static StepResult ExecuteStep(Program.Pessoa pessoa, int step)
    {
        return step switch
        {
            1 => StepName(pessoa, step),
            2 => StepBirthDate(pessoa, step),
            3 => StepGender(pessoa, step),
            4 => StepUnitSystem(pessoa, step),
            5 => StepHeight(pessoa, step),
            6 => StepWeight(pessoa, step),
            7 => StepActivityLevel(pessoa, step),
            8 => StepGoal(pessoa, step),
            9 => StepTargetWeight(pessoa, step),
            10 => StepConfirmation(pessoa),
            _ => StepResult.Cancel()
        };
    }

    private static void ApplyStepData(Program.Pessoa pessoa, int step, object data)
    {
        switch (step)
        {
            case 1: pessoa.nome = (string)data; break;
            case 2: pessoa.dataNascimento = (DateTime)data; break;
            case 3: pessoa.sexo = (Program.Sexo)data; break;
            case 4: pessoa.unidadeSistema = (Program.UnidadeSistema)data; break;
            case 5: pessoa.altura = (float)data; break;
            case 6: pessoa.peso = (float)data; break;
            case 7: pessoa.nivelAtividade = (Program.NivelAtividade)data; break;
            case 8: pessoa.objetivo = (Program.Objetivo)data; break;
            case 9: pessoa.pesoDesejado = (float)data; break;
        }
    }

    // ========================================================================
    // STEP 1: NAME
    // ========================================================================

    private static StepResult StepName(Program.Pessoa pessoa, int step)
    {
        var nome = pessoa.nome ?? "";
        const int MAX_LENGTH = 20;

        while (true)
        {
            var (left, right) = BuildStepPanels(pessoa, step,
                new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()}]Nome:[/]\n" +
                           $"[{Tema.Atual.Texto}]{(string.IsNullOrEmpty(nome) ? "_" : nome + "_")}" +
                           $"{new string(' ', Math.Max(0, MAX_LENGTH - nome.Length))}[/]")
            );

            ShowStep(step, left, right, "Digite o nome | ENTER=confirmar | ESC=cancelar");

            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter && !string.IsNullOrWhiteSpace(nome))
                return StepResult.Next(nome);
            if (key.Key == ConsoleKey.Escape)
                return StepResult.Cancel();
            if (key.Key == ConsoleKey.Backspace && nome.Length > 0)
                nome = nome[..^1];
            else if (!char.IsControl(key.KeyChar) && nome.Length < MAX_LENGTH)
                nome += key.KeyChar;
        }
    }

    // ========================================================================
    // STEP 2: BIRTH DATE
    // ========================================================================

    private static StepResult StepBirthDate(Program.Pessoa pessoa, int step)
    {
        var date = new DateInput();

        while (true)
        {
            var lista = BuildPersonInfo(pessoa);
            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()}]Data de Nascimento:[/]\n" +
                                 $"[{Tema.Atual.Texto}]{date.GetFormattedDisplay()}_[/]"));

            var (left, right) = BuildStepPanels(pessoa, step, new Rows(lista));
            ShowStep(step, left, right, "Digite a data | ENTER=confirmar | BACKSPACE=apagar | ESC=voltar");

            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter && date.IsComplete())
                return StepResult.Next(date.ToDateTime());
            if (key.Key == ConsoleKey.Escape)
                return StepResult.Back();
            if (key.Key == ConsoleKey.Backspace)
                date.Backspace();
            else if (char.IsDigit(key.KeyChar))
                date.AddDigit(key.KeyChar);
        }
    }

    // ========================================================================
    // STEP 3: GENDER
    // ========================================================================

    private static StepResult StepGender(Program.Pessoa pessoa, int step)
    {
        var options = new Dictionary<Program.Sexo, (string display, Color color)>
        {
            { Program.Sexo.Masculino, ("Homem", Color.Blue) },
            { Program.Sexo.Feminino, ("Mulher", Color.Pink1) }
        };

        return ShowEnumSelector(pessoa, step, options, pessoa.sexo, "Sexo:");
    }

    // ========================================================================
    // STEP 4: UNIT SYSTEM
    // ========================================================================

    private static StepResult StepUnitSystem(Program.Pessoa pessoa, int step)
    {
        var options = new Dictionary<Program.UnidadeSistema, (string display, Color color)>
        {
            { Program.UnidadeSistema.Metrico, ("Quilogramas (kg) e Metros (m)", Tema.Atual.Cabecalho) },
            { Program.UnidadeSistema.Imperial, ("Libras (lbs) e Pés/Polegadas (ft/in)", Tema.Atual.Cabecalho) }
        };

        return ShowEnumSelector(pessoa, step, options, pessoa.unidadeSistema, "Sistema de Unidades:");
    }

    // ========================================================================
    // STEP 5: HEIGHT
    // ========================================================================

    private static StepResult StepHeight(Program.Pessoa pessoa, int step)
    {
        return pessoa.unidadeSistema == Program.UnidadeSistema.Metrico
            ? StepHeightMetric(pessoa, step)
            : StepHeightImperial(pessoa, step);
    }

    private static StepResult StepHeightMetric(Program.Pessoa pessoa, int step)
    {
        var altura = pessoa.altura > 0 ? pessoa.altura : 1.70f;
        return ShowNumericAdjuster(
            pessoa, step, ref altura,
            0.5f, 2.5f,
            0.1f, 0.01f,
            altura => new FigletText($"{altura:F2}m").Color(Tema.Atual.Altura),
            "↑ +10cm  ↓ -10cm  → +1cm  ← -1cm",
            true
        );
    }

    private static StepResult StepHeightImperial(Program.Pessoa pessoa, int step)
    {
        var (feet, inches) = pessoa.altura > 0
            ? UnitConverter.MetersToFeetInches(pessoa.altura)
            : (5, 7);

        while (true)
        {
            var lista = BuildPersonInfo(pessoa);
            var alturaMetros = UnitConverter.FeetInchesToMeters(feet, inches);

            lista.Add(new FigletText($"{feet}'{inches}\"").Color(Tema.Atual.Altura).Centered());
            lista.Add(new Markup($"[dim]({alturaMetros:F2}m)[/]").Centered());

            if (pessoa.peso > 0)
                lista.Add(BuildBMIDisplay(pessoa.peso, alturaMetros));

            lista.Add(BuildControlsPanel("↑ +1ft  ↓ -1ft  → +1in  ← -1in"));

            var (left, right) = BuildStepPanels(pessoa, step, new Rows(lista));
            ShowStep(step, left, right, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: feet = Math.Min(feet + 1, 8); break;
                case ConsoleKey.DownArrow: feet = Math.Max(feet - 1, 3); break;
                case ConsoleKey.RightArrow:
                    inches++;
                    if (inches >= 12)
                    {
                        inches = 0;
                        feet = Math.Min(feet + 1, 8);
                    }

                    break;
                case ConsoleKey.LeftArrow:
                    inches--;
                    if (inches < 0)
                    {
                        inches = 11;
                        feet = Math.Max(feet - 1, 3);
                    }

                    break;
                case ConsoleKey.Enter:
                    return StepResult.Next(UnitConverter.FeetInchesToMeters(feet, inches));
                case ConsoleKey.Escape:
                    return StepResult.Back();
            }
        }
    }

    // ========================================================================
    // STEP 6: WEIGHT
    // ========================================================================

    private static StepResult StepWeight(Program.Pessoa pessoa, int step)
    {
        return pessoa.unidadeSistema == Program.UnidadeSistema.Metrico
            ? StepWeightMetric(pessoa, step)
            : StepWeightImperial(pessoa, step);
    }

    private static StepResult StepWeightMetric(Program.Pessoa pessoa, int step)
    {
        var peso = pessoa.peso > 0 ? pessoa.peso : 70f;
        return ShowNumericAdjuster(
            pessoa, step, ref peso,
            30f, 300f,
            1f, 0.1f,
            peso => new FigletText($"{peso:F1}kg").Color(Tema.Atual.Peso),
            "↑ +1kg  ↓ -1kg  → +100g  ← -100g",
            true,
            true
        );
    }

    private static StepResult StepWeightImperial(Program.Pessoa pessoa, int step)
    {
        var pesoLbs = pessoa.peso > 0 ? UnitConverter.KgToLbs(pessoa.peso) : 154f;

        while (true)
        {
            var lista = BuildPersonInfo(pessoa, true);
            lista.RemoveAt(lista.Count - 1); // Remove last rule
            lista.Add(new Rule());

            var pesoKg = UnitConverter.LbsToKg(pesoLbs);

            lista.Add(new FigletText($"{pesoLbs:F1}lb").Color(Tema.Atual.Peso).Centered());
            lista.Add(new Markup($"[dim]({pesoKg:F1}kg)[/]").Centered());

            if (pessoa.altura > 0)
                lista.Add(BuildBMIDisplay(pesoKg, pessoa.altura));

            lista.Add(BuildControlsPanel("↑ +1lbs  ↓ -1lbs  → +0.5lbs  ← -0.5lbs"));

            var (left, right) = BuildStepPanels(pessoa, step, new Rows(lista));
            ShowStep(step, left, right, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: pesoLbs = Math.Min(pesoLbs + 1f, 661f); break;
                case ConsoleKey.DownArrow: pesoLbs = Math.Max(pesoLbs - 1f, 66f); break;
                case ConsoleKey.RightArrow: pesoLbs = Math.Min(pesoLbs + 0.5f, 661f); break;
                case ConsoleKey.LeftArrow: pesoLbs = Math.Max(pesoLbs - 0.5f, 66f); break;
                case ConsoleKey.Enter: return StepResult.Next(UnitConverter.LbsToKg(pesoLbs));
                case ConsoleKey.Escape: return StepResult.Back();
            }
        }
    }

    // ========================================================================
    // STEP 7: ACTIVITY LEVEL (OPTIONAL)
    // ========================================================================

    private static StepResult StepActivityLevel(Program.Pessoa pessoa, int step)
    {
        var options = new Dictionary<Program.NivelAtividade, (string display, Color color)>
        {
            { Program.NivelAtividade.Sedentario, ("Pouco ou nenhum exercício", Tema.Atual.Texto) },
            { Program.NivelAtividade.Leve, ("Exercício 1-3 dias/semana", Tema.Atual.Texto) },
            { Program.NivelAtividade.Moderado, ("Exercício 3-5 dias/semana", Tema.Atual.Texto) },
            { Program.NivelAtividade.Ativo, ("Exercício 5-6 dias/semana", Tema.Atual.Texto) },
            { Program.NivelAtividade.MuitoAtivo, ("Exercício diário intenso", Tema.Atual.Texto) }
        };

        return ShowEnumSelector(pessoa, step, options,
            pessoa.nivelAtividade != default ? pessoa.nivelAtividade : Program.NivelAtividade.Moderado,
            "Nível de Atividade:", true, true);
    }

    // ========================================================================
    // STEP 8: GOAL
    // ========================================================================

    private static StepResult StepGoal(Program.Pessoa pessoa, int step)
    {
        var options = new Dictionary<Program.Objetivo, (string display, Color color)>
        {
            { Program.Objetivo.PerderPeso, ("Déficit calórico", Tema.Atual.Texto) },
            { Program.Objetivo.ManterPeso, ("Manutenção", Tema.Atual.Texto) },
            { Program.Objetivo.GanharMassa, ("Superávit calórico", Tema.Atual.Texto) },
            { Program.Objetivo.Definicao, ("Perder gordura mantendo músculo", Tema.Atual.Texto) },
            { Program.Objetivo.Recomposicao, ("Ganhar músculo e perder gordura", Tema.Atual.Texto) }
        };

        return ShowEnumSelector(pessoa, step, options,
            pessoa.objetivo != default ? pessoa.objetivo : Program.Objetivo.ManterPeso,
            "Objetivo:", includeFullInfo: true);
    }

    // ========================================================================
    // STEP 9: TARGET WEIGHT
    // ========================================================================

    private static StepResult StepTargetWeight(Program.Pessoa pessoa, int step)
    {
        return pessoa.unidadeSistema == Program.UnidadeSistema.Metrico
            ? StepTargetWeightMetric(pessoa, step)
            : StepTargetWeightImperial(pessoa, step);
    }

    private static StepResult StepTargetWeightMetric(Program.Pessoa pessoa, int step)
    {
        var pesoDesejado = pessoa.pesoDesejado > 0 ? pessoa.pesoDesejado : pessoa.peso;

        while (true)
        {
            var lista = BuildPersonInfo(pessoa, true);
            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()} bold]Peso Desejado:[/]"));
            lista.Add(new FigletText($"{pesoDesejado:F1}kg").Color(Tema.Atual.Peso).Centered());
            lista.Add(BuildGoalValidation(pessoa.objetivo, pessoa.peso, pesoDesejado, pessoa.unidadeSistema));
            lista.Add(BuildControlsPanel("↑ +1kg  ↓ -1kg  → +100g  ← -100g"));

            var (left, right) = BuildStepPanels(pessoa, step, new Rows(lista));
            ShowStep(step, left, right, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: pesoDesejado = Math.Min(pesoDesejado + 1f, 300f); break;
                case ConsoleKey.DownArrow: pesoDesejado = Math.Max(pesoDesejado - 1f, 30f); break;
                case ConsoleKey.RightArrow: pesoDesejado = Math.Min(pesoDesejado + 0.1f, 300f); break;
                case ConsoleKey.LeftArrow: pesoDesejado = Math.Max(pesoDesejado - 0.1f, 30f); break;
                case ConsoleKey.Enter:
                    if (ValidateGoalWeight(pessoa.objetivo, pessoa.peso, pesoDesejado))
                        return StepResult.Next(pesoDesejado);
                    break;
                case ConsoleKey.Escape: return StepResult.Back();
            }
        }
    }

    private static StepResult StepTargetWeightImperial(Program.Pessoa pessoa, int step)
    {
        var pesoDesejadoKg = pessoa.pesoDesejado > 0 ? pessoa.pesoDesejado : pessoa.peso;
        var pesoLbs = UnitConverter.KgToLbs(pesoDesejadoKg);

        while (true)
        {
            var lista = BuildPersonInfo(pessoa, true);
            pesoDesejadoKg = UnitConverter.LbsToKg(pesoLbs);

            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()} bold]Peso Desejado:[/]"));
            lista.Add(new FigletText($"{pesoLbs:F1}lb").Color(Tema.Atual.Peso).Centered());
            lista.Add(new Markup($"[dim]({pesoDesejadoKg:F1}kg)[/]").Centered());
            lista.Add(BuildGoalValidation(pessoa.objetivo, pessoa.peso, pesoDesejadoKg, pessoa.unidadeSistema));
            lista.Add(BuildControlsPanel("↑ +1lbs  ↓ -1lbs  → +0.5lbs  ← -0.5lbs"));

            var (left, right) = BuildStepPanels(pessoa, step, new Rows(lista));
            ShowStep(step, left, right, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: pesoLbs = Math.Min(pesoLbs + 1f, 661f); break;
                case ConsoleKey.DownArrow: pesoLbs = Math.Max(pesoLbs - 1f, 66f); break;
                case ConsoleKey.RightArrow: pesoLbs = Math.Min(pesoLbs + 0.5f, 661f); break;
                case ConsoleKey.LeftArrow: pesoLbs = Math.Max(pesoLbs - 0.5f, 66f); break;
                case ConsoleKey.Enter:
                    if (ValidateGoalWeight(pessoa.objetivo, pessoa.peso, pesoDesejadoKg))
                        return StepResult.Next(pesoDesejadoKg);
                    break;
                case ConsoleKey.Escape: return StepResult.Back();
            }
        }
    }

    // ========================================================================
    // STEP 10: CONFIRMATION
    // ========================================================================

    private static StepResult StepConfirmation(Program.Pessoa pessoa)
    {
        while (true)
        {
            content.Clear();
            Helpers.CentrarVert(content, 15);

            var table = BuildConfirmationTable(pessoa);
            content.Add(new Panel(table)
                .Header($"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Confirmação de Dados[/]")
                .RoundedBorder()
                .BorderColor(Tema.Atual.Borda));

            content.Add(new Markup($"\n{BuildProgressBar(TOTAL_STEPS)}").Centered());
            content.Add(
                new Markup(
                        $"\n[{Tema.Atual.Normal.ToMarkup()}]ENTER[/] = Confirmar e guardar | [dim]ESC = Voltar e editar[/]")
                    .Centered());

            Helpers.Render(content, "Criar Pessoa - Confirmação");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter: return StepResult.Next();
                case ConsoleKey.Escape: return StepResult.Back();
            }
        }
    }

    // ========================================================================
    // HELPER METHODS - UI BUILDING
    // ========================================================================

    private static List<IRenderable> BuildPersonInfo(Program.Pessoa pessoa, bool includeFullInfo = false)
    {
        var lista = new List<IRenderable>();

        if (!string.IsNullOrEmpty(pessoa.nome))
            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()}]Nome:[/] [{Tema.Atual.Texto}]{pessoa.nome}[/]"));

        if (pessoa.dataNascimento != default)
            lista.Add(new Markup(
                $"[{Tema.Atual.Cabecalho.ToMarkup()}]Data:[/] [{Tema.Atual.Texto}]{pessoa.dataNascimento:dd/MM/yyyy}[/]"));

        if (pessoa.sexo != default)
        {
            var corSexo = pessoa.sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
            lista.Add(new Markup(
                $"[{Tema.Atual.Cabecalho.ToMarkup()}]Sexo:[/] [{corSexo.ToMarkup()}]{pessoa.sexo}[/]"));
        }

        if (includeFullInfo)
        {
            if (pessoa.altura > 0)
                lista.Add(new Markup(
                    $"[{Tema.Atual.Cabecalho.ToMarkup()}]Altura:[/] [{Tema.Atual.Altura}]{UnitConverter.FormatHeight(pessoa.altura, pessoa.unidadeSistema)}[/]"));

            if (pessoa.peso > 0)
            {
                lista.Add(new Markup(
                    $"[{Tema.Atual.Cabecalho.ToMarkup()}]Peso:[/] [{Tema.Atual.Peso}]{UnitConverter.FormatWeight(pessoa.peso, pessoa.unidadeSistema)}[/]"));

                if (pessoa.altura > 0)
                {
                    var bmi = UnitConverter.CalculateBMI(pessoa.peso, pessoa.altura);
                    var bmiColor = Helpers.IMCtoColor(bmi);
                    lista.Add(new Markup(
                        $"[{Tema.Atual.Cabecalho.ToMarkup()}]IMC:[/] [{bmiColor.ToMarkup()}]{bmi:F1}[/] [dim]({UnitConverter.GetBMICategory(bmi)})[/]"));
                }
            }
        }

        if (lista.Count > 0 && !includeFullInfo)
            lista.Add(new Rule());

        return lista;
    }

    private static (Panel left, Panel right) BuildStepPanels(Program.Pessoa pessoa, int step, IRenderable inputContent)
    {
        var leftPanel = new Panel(inputContent)
            .RoundedBorder()
            .BorderColor(Tema.Atual.Borda);

        var rightContent = new List<IRenderable>
        {
            new Markup(
                $"\n[{Tema.Atual.Texto} bold]User:[/] {(string.IsNullOrEmpty(pessoa.nome) ? "?" : pessoa.nome)}"),
            new Markup(
                $"[{Tema.Atual.Texto} bold]Data:[/] {(pessoa.dataNascimento != default ? pessoa.dataNascimento.ToString("dd/MM/yyyy") : "??")}"),
            new Markup(
                $"[{Tema.Atual.Texto} bold]Idade:[/] {(pessoa.dataNascimento != default ? pessoa.Idade.ToString() : "??")}")
        };

        if (pessoa.sexo != default)
        {
            var corSexo = pessoa.sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
            rightContent.Add(new Markup($"[{Tema.Atual.Texto} bold]Sexo:[/] [{corSexo.ToMarkup()}]{pessoa.sexo}[/]\n"));
        }
        else
        {
            rightContent.Add(new Markup($"[{Tema.Atual.Texto} bold]Sexo:[/] ??\n"));
        }

        rightContent.Add(new Rule());
        rightContent.Add(new Markup(
            $"\n[{Tema.Atual.Altura} bold]Altura:[/] {(pessoa.altura > 0 ? UnitConverter.FormatHeight(pessoa.altura, pessoa.unidadeSistema) : "??")}"));
        rightContent.Add(new Markup(
            $"[{Tema.Atual.Peso} bold]Peso:[/] {(pessoa.peso > 0 ? UnitConverter.FormatWeight(pessoa.peso, pessoa.unidadeSistema) : "??")}"));

        if (pessoa.peso > 0 && pessoa.altura > 0)
        {
            var bmi = UnitConverter.CalculateBMI(pessoa.peso, pessoa.altura);
            var bmiColor = Helpers.IMCtoColor(bmi);
            rightContent.Add(new Markup(
                $"[{Tema.Atual.Peso} bold]IMC:[/] [{bmiColor.ToMarkup()}]{bmi:F1}[/] [dim]{UnitConverter.GetBMICategory(bmi)}[/]"));
        }

        rightContent.Add(new Markup(
            $"[{Tema.Atual.Peso} bold]Peso Desejado:[/] {(pessoa.pesoDesejado > 0 ? UnitConverter.FormatWeight(pessoa.pesoDesejado, pessoa.unidadeSistema) : "??\n")}"));
        rightContent.Add(new Rule());
        rightContent.Add(new Markup(
            $"\n[{Tema.Atual.Texto} bold]Atividade:[/] {(pessoa.nivelAtividade != default ? pessoa.nivelAtividade.ToString() : "??")}"));
        rightContent.Add(new Markup(
            $"[{Tema.Atual.Texto} bold]Objetivo:[/] {(pessoa.objetivo != default ? pessoa.objetivo.ToString() : "??\n")}"));

        var rightPanel = new Panel(new Rows(rightContent))
            .RoundedBorder()
            .BorderColor(Tema.Atual.Borda)
            .Header("[bold]Preview[/]");

        return (leftPanel, rightPanel);
    }

    private static Markup BuildBMIDisplay(float weight, float height)
    {
        var bmi = UnitConverter.CalculateBMI(weight, height);
        var bmiColor = Helpers.IMCtoColor(bmi);
        return new Markup($"[{bmiColor.ToMarkup()}]IMC: {bmi:F1} - {UnitConverter.GetBMICategory(bmi)}[/]").Centered();
    }

    private static Markup BuildControlsPanel(string controls)
    {
        return new Markup(
            $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n" +
            $"  {controls}\n" +
            $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] confirmar"
        ).Centered();
    }

    private static Markup BuildGoalValidation(Program.Objetivo objetivo, float pesoAtual, float pesoDesejado,
        Program.UnidadeSistema sistema)
    {
        var isValid = ValidateGoalWeight(objetivo, pesoAtual, pesoDesejado);

        if (isValid)
            return new Markup($"\n[{Tema.Atual.Normal.ToMarkup()}]✓ Objetivo compatível[/]").Centered();

        var currentWeight = UnitConverter.FormatWeight(pesoAtual, sistema);
        var comparison = objetivo == Program.Objetivo.PerderPeso ? "<" : ">";

        return new Markup($"\n[{Color.Red.ToMarkup()}]⚠ Peso desejado deve ser {comparison} {currentWeight}[/]")
            .Centered();
    }

    private static bool ValidateGoalWeight(Program.Objetivo objetivo, float pesoAtual, float pesoDesejado)
    {
        return objetivo switch
        {
            Program.Objetivo.PerderPeso => pesoDesejado < pesoAtual,
            Program.Objetivo.GanharMassa => pesoDesejado > pesoAtual,
            _ => true
        };
    }

    private static string BuildProgressBar(int currentStep)
    {
        var bar = "";
        for (var i = 1; i <= TOTAL_STEPS; i++)
            bar += i <= currentStep
                ? $"[{Tema.Atual.Normal.ToMarkup()}]●[/]"
                : "[dim]○[/]";
        return bar;
    }

    private static Table BuildConfirmationTable(Program.Pessoa pessoa)
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Tema.Atual.Borda)
            .AddColumn(new TableColumn("[bold]Campo[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Valor[/]").RightAligned());

        table.AddRow("Nome", $"[{Tema.Atual.Texto}]{pessoa.nome}[/]");
        table.AddRow("Data de Nascimento", $"[{Tema.Atual.Texto}]{pessoa.dataNascimento:dd/MM/yyyy}[/]");
        table.AddRow("Idade", $"[{Tema.Atual.Texto}]{pessoa.Idade} anos[/]");

        var corSexo = pessoa.sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
        table.AddRow("Sexo", $"[{corSexo.ToMarkup()}]{pessoa.sexo}[/]");
        table.AddRow("Sistema", $"[{Tema.Atual.Texto}]{pessoa.unidadeSistema}[/]");
        table.AddRow("Altura",
            $"[{Tema.Atual.Altura}]{UnitConverter.FormatHeight(pessoa.altura, pessoa.unidadeSistema)}[/]");
        table.AddRow("Peso", $"[{Tema.Atual.Peso}]{UnitConverter.FormatWeight(pessoa.peso, pessoa.unidadeSistema)}[/]");

        var bmi = UnitConverter.CalculateBMI(pessoa.peso, pessoa.altura);
        var bmiColor = Helpers.IMCtoColor(bmi);
        table.AddRow("IMC", $"[{bmiColor.ToMarkup()}]{bmi:F1} ({UnitConverter.GetBMICategory(bmi)})[/]");

        table.AddRow("Nível de Atividade",
            $"[{Tema.Atual.Texto}]{(pessoa.nivelAtividade != default ? pessoa.nivelAtividade.ToString() : "Não especificado")}[/]");
        table.AddRow("Objetivo", $"[{Tema.Atual.Texto}]{pessoa.objetivo}[/]");
        table.AddRow("Peso Desejado",
            $"[{Tema.Atual.Peso}]{UnitConverter.FormatWeight(pessoa.pesoDesejado, pessoa.unidadeSistema)}[/]");

        return table;
    }

    private static void ShowStep(int step, Panel left, Panel right, string instructions)
    {
        var mainTable = new Table()
            .AddColumn(new TableColumn("").Width(50))
            .AddColumn(new TableColumn("").Width(50))
            .HideHeaders()
            .NoBorder()
            .Collapse();

        mainTable.AddRow(left, right);

        content.Clear();
        Helpers.CentrarVert(content, 10);
        content.Add(Align.Center(mainTable));

        if (!string.IsNullOrEmpty(instructions))
            content.Add(new Markup($"\n[dim]{BuildProgressBar(step)}\n{instructions}[/]").Centered());

        Helpers.Render(content, $"Criar Pessoa - Passo {step}/{TOTAL_STEPS}");
    }

    // ========================================================================
    // GENERIC SELECTOR - Eliminates duplication across enum steps
    // ========================================================================

    private static StepResult ShowEnumSelector<T>(
        Program.Pessoa pessoa,
        int step,
        Dictionary<T, (string display, Color color)> options,
        T defaultValue,
        string fieldName,
        bool optional = false,
        bool includeFullInfo = false) where T : Enum
    {
        var opcoes = options.Keys.ToList();
        var selectedIndex = opcoes.IndexOf(defaultValue);
        if (selectedIndex == -1) selectedIndex = 0;

        while (true)
        {
            var lista = BuildPersonInfo(pessoa, includeFullInfo);
            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()} bold]{fieldName}[/]" +
                                 (optional ? " [dim](opcional)[/]" : "") + "\n"));

            for (var i = 0; i < opcoes.Count; i++)
            {
                var prefix = i == selectedIndex ? $"[{Tema.Atual.Cabecalho.ToMarkup()}]►[/] " : "  ";
                var (display, baseColor) = options[opcoes[i]];
                var color = i == selectedIndex ? Tema.Atual.Cabecalho : baseColor;

                lista.Add(new Markup($"{prefix}[{color.ToMarkup()}]{opcoes[i]}[/] [dim]- {display}[/]"));
            }

            var (left, right) = BuildStepPanels(pessoa, step, new Rows(lista));
            var instructions = optional
                ? "↑↓=navegar | ENTER=confirmar | S=pular | ESC=voltar"
                : "↑↓=navegar | ENTER=confirmar | ESC=voltar";

            ShowStep(step, left, right, instructions);

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + opcoes.Count) % opcoes.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % opcoes.Count;
                    break;
                case ConsoleKey.Enter:
                    return StepResult.Next(opcoes[selectedIndex]);
                case ConsoleKey.S when optional:
                    return StepResult.Skip();
                case ConsoleKey.Escape:
                    return StepResult.Back();
            }
        }
    }

    // ========================================================================
    // GENERIC NUMERIC ADJUSTER - Eliminates duplication in height/weight
    // ========================================================================

    private static StepResult ShowNumericAdjuster(
        Program.Pessoa pessoa,
        int step,
        ref float value,
        float min,
        float max,
        float largeStep,
        float smallStep,
        Func<float, IRenderable> format,
        string controls,
        bool showBMI = false,
        bool includeFullInfo = false)
    {
        var currentValue = value;

        while (true)
        {
            var lista = BuildPersonInfo(pessoa, includeFullInfo);
            if (includeFullInfo && lista.Count > 0)
            {
                lista.RemoveAt(lista.Count - 1);
                lista.Add(new Rule());
            }

            lista.Add(format(currentValue));

            if (showBMI && pessoa.peso > 0 && pessoa.altura > 0)
            {
                var weight = includeFullInfo ? currentValue : pessoa.peso;
                var height = includeFullInfo ? pessoa.altura : currentValue;
                lista.Add(BuildBMIDisplay(weight, height));
            }

            lista.Add(BuildControlsPanel(controls));

            var (left, right) = BuildStepPanels(pessoa, step, new Rows(lista));
            ShowStep(step, left, right, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    currentValue = Math.Min(currentValue + largeStep, max);
                    break;
                case ConsoleKey.DownArrow:
                    currentValue = Math.Max(currentValue - largeStep, min);
                    break;
                case ConsoleKey.RightArrow:
                    currentValue = Math.Min(currentValue + smallStep, max);
                    break;
                case ConsoleKey.LeftArrow:
                    currentValue = Math.Max(currentValue - smallStep, min);
                    break;
                case ConsoleKey.Enter:
                    value = currentValue;
                    return StepResult.Next(currentValue);
                case ConsoleKey.Escape:
                    return StepResult.Back();
            }
        }
    }

    // ========================================================================
    // DATA PERSISTENCE
    // ========================================================================

    private static Program.Pessoa? SaveAndReturnUser(Program.Pessoa pessoa)
    {
        if (UserDataManager.SaveUser(pessoa))
        {
            UserDataManager.SaveCurrentUser(pessoa.nome);
            Helpers.MostrarMensagem($"✓ Utilizador '{pessoa.nome}' criado e guardado!", Tema.Atual.Normal);
        }
        else
        {
            Helpers.MostrarMensagem($"✓ Utilizador '{pessoa.nome}' criado! (Aviso: Erro ao guardar)", Color.Yellow);
        }

        return pessoa;
    }

    // ========================================================================
    // PUBLIC UTILITY - Show User Details (used by other menus)
    // ========================================================================

    public static void MostrarDetalhesUtilizador(Program.Pessoa pessoa)
    {
        content.Clear();
        Helpers.CentrarVert(content, 10);

        var table = BuildConfirmationTable(pessoa);
        content.Add(new Panel(table)
            .Header($"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Detalhes do Utilizador[/]")
            .RoundedBorder()
            .BorderColor(Tema.Atual.Borda));

        content.Add(new Markup("\n[dim]Pressione qualquer tecla para voltar[/]").Centered());

        Helpers.Render(content, "Detalhes do Utilizador");
        Console.ReadKey(true);
    }

    // ========================================================================
    // DATE INPUT HELPER CLASS
    // ========================================================================

    private class DateInput
    {
        private int currentField; // 0=day, 1=month, 2=year
        private string day = "";
        private string month = "";
        private string year = "";

        public void AddDigit(char digit)
        {
            switch (currentField)
            {
                case 0 when day.Length < 2:
                    day += digit;
                    if (day.Length == 2 && int.Parse(day) is >= 1 and <= 31)
                        currentField = 1;
                    else if (day.Length == 2)
                        day = "";
                    break;

                case 1 when month.Length < 2:
                    month += digit;
                    if (month.Length == 2)
                    {
                        var monthNum = int.Parse(month);
                        if (monthNum is >= 1 and <= 12 &&
                            int.Parse(day) <= DateTime.DaysInMonth(DateTime.Now.Year, monthNum))
                        {
                            currentField = 2;
                        }
                        else
                        {
                            month = "";
                            day = "";
                            currentField = 0;
                        }
                    }

                    break;

                case 2 when year.Length < 4:
                    year += digit;
                    if (year.Length == 4)
                    {
                        var yearNum = int.Parse(year);
                        if (yearNum < 1900 || yearNum > DateTime.Now.Year ||
                            int.Parse(day) > DateTime.DaysInMonth(yearNum, int.Parse(month)))
                        {
                            year = "";
                            month = "";
                            day = "";
                            currentField = 0;
                        }
                    }

                    break;
            }
        }

        public void Backspace()
        {
            switch (currentField)
            {
                case 0 when day.Length > 0:
                    day = day[..^1];
                    break;
                case 1 when month.Length > 0:
                    month = month[..^1];
                    break;
                case 2 when year.Length > 0:
                    year = year[..^1];
                    break;
                case > 0:
                    currentField--;
                    if (currentField == 1) month = "";
                    else if (currentField == 0) day = "";
                    break;
            }
        }

        public bool IsComplete()
        {
            return day.Length == 2 && month.Length == 2 && year.Length == 4;
        }

        public DateTime ToDateTime()
        {
            return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
        }

        public string GetFormattedDisplay()
        {
            var dayDisplay = string.IsNullOrEmpty(day) ? "__" : day.PadLeft(2, '0');
            var monthDisplay = string.IsNullOrEmpty(month) ? "__" : month.PadLeft(2, '0');
            var yearDisplay = string.IsNullOrEmpty(year) ? "____" : year.PadLeft(4, '0');

            return currentField switch
            {
                0 => $"[{Tema.Atual.Cabecalho.ToMarkup()}]{dayDisplay}[/]/{monthDisplay}/{yearDisplay}",
                1 => $"{dayDisplay}/[{Tema.Atual.Cabecalho.ToMarkup()}]{monthDisplay}[/]/{yearDisplay}",
                _ => $"{dayDisplay}/{monthDisplay}/[{Tema.Atual.Cabecalho.ToMarkup()}]{yearDisplay}[/]"
            };
        }
    }
}