using CalculadoraIMC.Core;
using CalculadoraIMC.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CalculadoraIMC.UserManager;

// Formulario para criar um novo utilizador passo a passo
public static class UserCreationWizard
{
    private const int TOTAL_PASSOS = 10;
    private static readonly List<IRenderable> conteudo = new();
    
    // Cria uma nova pessoa através de um formulario interativo
    public static Program.Pessoa? CriarPessoa()
    {
        Tema.Atual = Tema.Default;
    
        var pessoa = new Program.Pessoa();
        var passoAtual = 1;

        while (passoAtual <= TOTAL_PASSOS)
        {
            var resultado = ExecutarPasso(pessoa, passoAtual);

            switch (resultado.Action)
            {
                case StepAction.Next:
                    // Aplica os dados do passo atual
                    if (resultado.Data != null)
                        AplicarDadosPasso(pessoa, passoAtual, resultado.Data);
                
                    // Avança para próximo passo
                    passoAtual++;
                    break;

                case StepAction.Back:
                    // Volta ao passo anterior (com validação!)
                    if (passoAtual > 1)
                    {
                        passoAtual--;
                    }
                    else
                    {
                        // Já está no primeiro passo, confirma cancelamento
                        if (HelpersUI.ConfirmarAcao("Cancelar criação de utilizador?"))
                            return null;
                    }
                    break;

                case StepAction.Cancel:
                    return null;
            }
        }

        return GuardarERetornarUtilizador(pessoa);
    }

    // Executa o passo correspondente do formulario
    private static StepResult ExecutarPasso(Program.Pessoa pessoa, int passo)
    {
        return passo switch
        {
            1 => PassoNome(pessoa, passo),
            2 => PassoDataNascimento(pessoa, passo),
            3 => PassoSexo(pessoa, passo),
            4 => PassoSistemaUnidades(pessoa, passo),
            5 => PassoAltura(pessoa, passo),
            6 => PassoPeso(pessoa, passo),
            7 => PassoNivelAtividade(pessoa, passo),
            8 => PassoObjetivo(pessoa, passo),
            9 => PassoPesoDesejado(pessoa, passo),
            10 => PassoConfirmacao(pessoa),
            _ => StepResult.Cancel()
        };
    }

    // Aplica os dados recolhidos em cada passo à pessoa
    private static void AplicarDadosPasso(Program.Pessoa pessoa, int passo, object dados)
    {
        switch (passo)
        {
            case 1: pessoa.Nome = (string)dados; break;
            case 2: pessoa.DataNascimento = (DateTime)dados; break;
            case 3: pessoa.Sexo = (Program.Sexo)dados; break;
            case 4: pessoa.UnidadeSistema = (Program.UnidadeSistema)dados; break;
            case 5: pessoa.Altura = (float)dados; break;
            case 6: pessoa.PesoInicial = pessoa.Peso = (float)dados; break;
            case 7: pessoa.NivelAtividade = (Program.NivelAtividade)dados; break;
            case 8: pessoa.Objetivo = (Program.Objetivo)dados; break;
            case 9: pessoa.PesoDesejado = (float)dados; break;
        }
    }

    // Passo 1: Pedir o nome do utilizador
    private static StepResult PassoNome(Program.Pessoa pessoa, int passo)
    {
        var nome = pessoa.Nome ?? "";

        while (true)
        {
            var utilizadorExiste = UserDataManager.LoadUser(nome) != null;

            var (esquerda, direita) = CriarPaineisPasso(pessoa, passo,
                new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()}]Nome:[/]\n" +
                           $"[{(utilizadorExiste ? "red" : Tema.Atual.Texto.ToMarkup())}]{(string.IsNullOrEmpty(nome) ? "_" : nome + "_")}" +
                           $"{new string(' ', Math.Max(0, Constantes.TAMANHO_NOME_MAXIMO - nome.Length))}[/]" +
                           (utilizadorExiste ? $"\n[red]⚠ Utilizador já existe![/]" : ""))
            );

            MostrarPasso(passo, esquerda, direita, "Digite o nome | ENTER=confirmar | ESC=cancelar");

            var tecla = Console.ReadKey(true);

            if (tecla.Key == ConsoleKey.Enter && !string.IsNullOrWhiteSpace(nome) && !utilizadorExiste) 
                return StepResult.Next(nome);
            if (tecla.Key == ConsoleKey.Escape)
                return StepResult.Cancel();
            if (tecla.Key == ConsoleKey.Backspace && nome.Length > 0)
                nome = nome[..^1];
            else if (!char.IsControl(tecla.KeyChar) && nome.Length < Constantes.TAMANHO_NOME_MAXIMO)
                nome += tecla.KeyChar;

            pessoa.Nome = nome; 
        }
    }

    // Passo 2: Pedir a data de nascimento
    private static StepResult PassoDataNascimento(Program.Pessoa pessoa, int passo)
    {
        var data = new DateInput();

        while (true)
        {
            var lista = ConstruirInfoPessoa(pessoa);
            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()}]Data de Nascimento:[/]\n" +
                                 $"[{Tema.Atual.Texto}]{data.ObterExibicaoFormatada()}_[/]"));

            var (esquerda, direita) = CriarPaineisPasso(pessoa, passo, new Rows(lista));
            MostrarPasso(passo, esquerda, direita, "Digite a data | ENTER=confirmar | BACKSPACE=apagar | ESC=voltar");

            var tecla = Console.ReadKey(true);

            if (tecla.Key == ConsoleKey.Enter && data.EstaCompleta())
                return StepResult.Next(data.ParaDateTime());
            if (tecla.Key == ConsoleKey.Escape)
                return StepResult.Back();
            if (tecla.Key == ConsoleKey.Backspace)
                data.ApagarDigito();
            else if (char.IsDigit(tecla.KeyChar))
                data.AdicionarDigito(tecla.KeyChar);
        }
    }

    // Passo 3: Pedir o sexo
    private static StepResult PassoSexo(Program.Pessoa pessoa, int passo)
    {
        var opcoes = new Dictionary<Program.Sexo, (string exibicao, Color cor)>
        {
            { Program.Sexo.Masculino, ("Homem", Color.Blue) },
            { Program.Sexo.Feminino, ("Mulher", Color.Pink1) }
        };

        return MostrarSeletorEnum(pessoa, passo, opcoes, pessoa.Sexo, "Sexo:");
    }

    // Passo 4: Pedir o sistema de unidades
    private static StepResult PassoSistemaUnidades(Program.Pessoa pessoa, int passo)
    {
        var opcoes = new Dictionary<Program.UnidadeSistema, (string exibicao, Color cor)>
        {
            { Program.UnidadeSistema.Metrico, ("Quilogramas (kg) e Metros (m)", Tema.Atual.Cabecalho) },
            { Program.UnidadeSistema.Imperial, ("Libras (lbs) e Pés/Polegadas (ft/in)", Tema.Atual.Cabecalho) }
        };

        return MostrarSeletorEnum(pessoa, passo, opcoes, pessoa.UnidadeSistema, "Sistema de Unidades:");
    }

    // Passo 5: Pedir a altura (métrico ou imperial)
    private static StepResult PassoAltura(Program.Pessoa pessoa, int passo)
    {
        return pessoa.UnidadeSistema == Program.UnidadeSistema.Metrico
            ? PassoAlturaMetrico(pessoa, passo)
            : PassoAlturaImperial(pessoa, passo);
    }

    // Passo 5 (métrico): Altura em metros
    private static StepResult PassoAlturaMetrico(Program.Pessoa pessoa, int passo)
    {
        var altura = pessoa.Altura > 0 ? pessoa.Altura : 1.70f;
        return MostrarAjustadorNumerico(
            pessoa, passo, ref altura,
            Constantes.ALTURA_MINIMA, Constantes.ALTURA_MAXIMA,
            Constantes.AJUSTE_ALTURA_GRANDE, Constantes.AJUSTE_ALTURA_PEQUENO,
            a => new Rows(new FigletText($"{a:F2}m").Color(Tema.Atual.Altura).Centered()).Expand(),
            "↑ +10cm  ↓ -10cm  → +1cm  ← -1cm",
            true
        );
    }

    // Passo 5 (imperial): Altura em pés e polegadas
    private static StepResult PassoAlturaImperial(Program.Pessoa pessoa, int passo)
    {
        var (pes, polegadas) = pessoa.Altura > 0
            ? UnitConverter.MetrosParaPesPolegadas(pessoa.Altura)
            : (5, 7);

        while (true)
        {
            var lista = ConstruirInfoPessoa(pessoa);
            var alturaMetros = UnitConverter.PesPolegadasParaMetros(pes, polegadas);

            lista.Add(new Rows(new FigletText($"{pes}'{polegadas}\"").Color(Tema.Atual.Altura).Centered()).Expand());
            lista.Add(new Markup($"[dim]({alturaMetros:F2}m)[/]").Centered());

            if (pessoa.PesoInicial > 0)
                lista.Add(CriarExibicaoIMC(pessoa.PesoInicial, alturaMetros));

            lista.Add(CriarPainelControlos("↑ +1ft  ↓ -1ft  → +1in  ← -1in"));

            var (esquerda, direita) = CriarPaineisPasso(pessoa, passo, new Rows(lista));
            MostrarPasso(passo, esquerda, direita, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: 
                    pes = Math.Min(pes + 1, Constantes.ALTURA_MAXIMA_FEET); 
                    break;
                case ConsoleKey.DownArrow: 
                    pes = Math.Max(pes - 1, Constantes.ALTURA_MINIMA_FEET); 
                    break;
                case ConsoleKey.RightArrow:
                    polegadas++;
                    if (polegadas >= Constantes.POLEGADAS_POR_PE)
                    {
                        polegadas = 0;
                        pes = Math.Min(pes + 1, Constantes.ALTURA_MAXIMA_FEET);
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    polegadas--;
                    if (polegadas < 0)
                    {
                        polegadas = Constantes.POLEGADAS_POR_PE - 1;
                        pes = Math.Max(pes - 1, Constantes.ALTURA_MINIMA_FEET);
                    }
                    break;
                case ConsoleKey.Enter:
                    return StepResult.Next(UnitConverter.PesPolegadasParaMetros(pes, polegadas));
                case ConsoleKey.Escape:
                    return StepResult.Back();
            }
        }
    }

    // Passo 6: Pedir o peso (métrico ou imperial)
    private static StepResult PassoPeso(Program.Pessoa pessoa, int passo)
    {
        return pessoa.UnidadeSistema == Program.UnidadeSistema.Metrico
            ? PassoPesoMetrico(pessoa, passo)
            : PassoPesoImperial(pessoa, passo);
    }

    // Passo 6 (métrico): Peso em kg
    private static StepResult PassoPesoMetrico(Program.Pessoa pessoa, int passo)
    {
        var peso = pessoa.PesoInicial > 0 ? pessoa.PesoInicial : 70f;
        return MostrarAjustadorNumerico(
            pessoa, passo, ref peso,
            Constantes.PESO_MINIMO, Constantes.PESO_MAXIMO,
            Constantes.AJUSTE_PESO_GRANDE, Constantes.AJUSTE_PESO_PEQUENO,
            p => new Rows(new FigletText($"{p:F1}kg").Color(Tema.Atual.Peso).Centered()).Expand(),
            "↑ +1kg  ↓ -1kg  → +100g  ← -100g",
            true,
            true
        );
    }

    // Passo 6 (imperial): Peso em libras
    private static StepResult PassoPesoImperial(Program.Pessoa pessoa, int passo)
    {
        var pesoLbs = pessoa.PesoInicial > 0 ? UnitConverter.KgParaLbs(pessoa.PesoInicial) : 154f;

        while (true)
        {
            var lista = ConstruirInfoPessoa(pessoa, true);
            lista.RemoveAt(lista.Count - 1);
            lista.Add(new Rule());

            var pesoKg = UnitConverter.LbsParaKg(pesoLbs);

            lista.Add(new Rows(new FigletText($"{pesoLbs:F1}lb").Color(Tema.Atual.Peso).Centered()).Expand());
            lista.Add(new Markup($"[dim]({pesoKg:F1}kg)[/]").Centered());

            if (pessoa.Altura > 0)
                lista.Add(CriarExibicaoIMC(pesoKg, pessoa.Altura));

            lista.Add(CriarPainelControlos("↑ +1lbs  ↓ -1lbs  → +0.5lbs  ← -0.5lbs"));

            var (esquerda, direita) = CriarPaineisPasso(pessoa, passo, new Rows(lista));
            MostrarPasso(passo, esquerda, direita, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: 
                    pesoLbs = Math.Min(pesoLbs + Constantes.AJUSTE_PESO_GRANDE_LBS, Constantes.PESO_MAXIMO_LBS); 
                    break;
                case ConsoleKey.DownArrow: 
                    pesoLbs = Math.Max(pesoLbs - Constantes.AJUSTE_PESO_GRANDE_LBS, Constantes.PESO_MINIMO_LBS); 
                    break;
                case ConsoleKey.RightArrow: 
                    pesoLbs = Math.Min(pesoLbs + Constantes.AJUSTE_PESO_PEQUENO_LBS, Constantes.PESO_MAXIMO_LBS); 
                    break;
                case ConsoleKey.LeftArrow: 
                    pesoLbs = Math.Max(pesoLbs - Constantes.AJUSTE_PESO_PEQUENO_LBS, Constantes.PESO_MINIMO_LBS); 
                    break;
                case ConsoleKey.Enter: 
                    return StepResult.Next(UnitConverter.LbsParaKg(pesoLbs));
                case ConsoleKey.Escape: 
                    return StepResult.Back();
            }
        }
    }

    // Passo 7: Pedir nível de atividade física
    private static StepResult PassoNivelAtividade(Program.Pessoa pessoa, int passo)
    {
        var opcoes = new Dictionary<Program.NivelAtividade, (string exibicao, Color cor)>
        {
            { Program.NivelAtividade.Sedentario, ("Pouco ou nenhum exercício", Tema.Atual.Texto) },
            { Program.NivelAtividade.Leve, ("Exercício 1-3 dias/semana", Tema.Atual.Texto) },
            { Program.NivelAtividade.Moderado, ("Exercício 3-5 dias/semana", Tema.Atual.Texto) },
            { Program.NivelAtividade.Ativo, ("Exercício 5-6 dias/semana", Tema.Atual.Texto) },
            { Program.NivelAtividade.MuitoAtivo, ("Exercício diário intenso", Tema.Atual.Texto) }
        };

        return MostrarSeletorEnum(pessoa, passo, opcoes,
            pessoa.NivelAtividade != default ? pessoa.NivelAtividade : Program.NivelAtividade.Moderado,
            "Nível de Atividade:", true);
    }

    // Passo 8: Pedir objetivo
    private static StepResult PassoObjetivo(Program.Pessoa pessoa, int passo)
    {
        var opcoes = new Dictionary<Program.Objetivo, (string exibicao, Color cor)>
        {
            { Program.Objetivo.PerderPeso, ("Déficit calórico", Tema.Atual.Texto) },
            { Program.Objetivo.ManterPeso, ("Manutenção", Tema.Atual.Texto) },
            { Program.Objetivo.GanharMassa, ("Superávit calórico", Tema.Atual.Texto) },
            { Program.Objetivo.Definicao, ("Perder gordura mantendo músculo", Tema.Atual.Texto) },
            { Program.Objetivo.Recomposicao, ("Ganhar músculo e perder gordura", Tema.Atual.Texto) }
        };

        return MostrarSeletorEnum(pessoa, passo, opcoes,
            pessoa.Objetivo != default ? pessoa.Objetivo : Program.Objetivo.ManterPeso,
            "Objetivo:", incluirInfoCompleta: true);
    }

    // Passo 9: Pedir peso desejado (métrico ou imperial)
    private static StepResult PassoPesoDesejado(Program.Pessoa pessoa, int passo)
    {
        return pessoa.UnidadeSistema == Program.UnidadeSistema.Metrico
            ? PassoPesoDesejadoMetrico(pessoa, passo)
            : PassoPesoDesejadoImperial(pessoa, passo);
    }

    // Passo 9 (métrico): Peso desejado em kg
    private static StepResult PassoPesoDesejadoMetrico(Program.Pessoa pessoa, int passo)
    {
        var pesoDesejado = pessoa.PesoDesejado > 0 ? pessoa.PesoDesejado : pessoa.PesoInicial;

        while (true)
        {
            var lista = ConstruirInfoPessoa(pessoa, true);
            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()} bold]Peso Desejado:[/]"));
            lista.Add(new FigletText($"{pesoDesejado:F1}kg").Color(Tema.Atual.Peso).Centered());
            lista.Add(CriarValidacaoObjetivo(pessoa.Objetivo, pessoa.PesoInicial, pesoDesejado, pessoa.UnidadeSistema));
            lista.Add(CriarPainelControlos("↑ +1kg  ↓ -1kg  → +100g  ← -100g"));

            var (esquerda, direita) = CriarPaineisPasso(pessoa, passo, new Rows(lista));
            MostrarPasso(passo, esquerda, direita, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: pesoDesejado = Math.Min(pesoDesejado + 1f, Constantes.PESO_MAXIMO); break;
                case ConsoleKey.DownArrow: pesoDesejado = Math.Max(pesoDesejado - 1f, Constantes.PESO_MINIMO); break;
                case ConsoleKey.RightArrow: pesoDesejado = Math.Min(pesoDesejado + 0.1f, Constantes.PESO_MAXIMO); break;
                case ConsoleKey.LeftArrow: pesoDesejado = Math.Max(pesoDesejado - 0.1f, Constantes.PESO_MINIMO); break;
                case ConsoleKey.Enter:
                    if (ValidarPesoObjetivo(pessoa.Objetivo, pessoa.PesoInicial, pesoDesejado))
                        return StepResult.Next(pesoDesejado);
                    break;
                case ConsoleKey.Escape: return StepResult.Back();
            }
        }
    }

    // Passo 9 (imperial): Peso desejado em libras
    private static StepResult PassoPesoDesejadoImperial(Program.Pessoa pessoa, int passo)
    {
        var pesoDesejadoKg = pessoa.PesoDesejado > 0 ? pessoa.PesoDesejado : pessoa.PesoInicial;
        var pesoLbs = UnitConverter.KgParaLbs(pesoDesejadoKg);

        while (true)
        {
            var lista = ConstruirInfoPessoa(pessoa, true);
            pesoDesejadoKg = UnitConverter.LbsParaKg(pesoLbs);

            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()} bold]Peso Desejado:[/]"));
            lista.Add(new FigletText($"{pesoLbs:F1}lb").Color(Tema.Atual.Peso).Centered());
            lista.Add(new Markup($"[dim]({pesoDesejadoKg:F1}kg)[/]").Centered());
            lista.Add(CriarValidacaoObjetivo(pessoa.Objetivo, pessoa.PesoInicial, pesoDesejadoKg, pessoa.UnidadeSistema));
            lista.Add(CriarPainelControlos("↑ +1lbs  ↓ -1lbs  → +0.5lbs  ← -0.5lbs"));

            var (esquerda, direita) = CriarPaineisPasso(pessoa, passo, new Rows(lista));
            MostrarPasso(passo, esquerda, direita, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow: pesoLbs = Math.Min(pesoLbs + 1f, Constantes.PESO_MAXIMO_LBS); break;
                case ConsoleKey.DownArrow: pesoLbs = Math.Max(pesoLbs - 1f, Constantes.PESO_MINIMO_LBS); break;
                case ConsoleKey.RightArrow: pesoLbs = Math.Min(pesoLbs + 0.5f, Constantes.PESO_MAXIMO_LBS); break;
                case ConsoleKey.LeftArrow: pesoLbs = Math.Max(pesoLbs - 0.5f, Constantes.PESO_MINIMO_LBS); break;
                case ConsoleKey.Enter:
                    if (ValidarPesoObjetivo(pessoa.Objetivo, pessoa.PesoInicial, pesoDesejadoKg))
                        return StepResult.Next(pesoDesejadoKg);
                    break;
                case ConsoleKey.Escape: return StepResult.Back();
            }
        }
    }

    // Passo 10: Confirmar todos os dados
    private static StepResult PassoConfirmacao(Program.Pessoa pessoa)
    {
        while (true)
        {
            conteudo.Clear();
            HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_PADRAO);

            var tabela = CriarTabelaConfirmacao(pessoa);
            List<IRenderable> conteudoConfirmar = new List<IRenderable>
            {
                Align.Center(new Panel(tabela)
                    .Header($"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Confirmação de Dados[/]")
                    .RoundedBorder()
                    .BorderColor(Tema.Atual.Borda)),
                new Markup($"\n{CriarBarraProgresso(TOTAL_PASSOS)}").Centered(),
                new Markup($"\n[{Tema.Atual.Normal.ToMarkup()}]ENTER[/] = Confirmar e guardar | [dim]ESC = Voltar e editar[/]").Centered(),
            };

            conteudo.Add(new Rows(conteudoConfirmar));

            HelpersUI.Render(conteudo, "Criar Pessoa - Confirmação");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter: return StepResult.Next();
                case ConsoleKey.Escape: return StepResult.Back();
            }
        }
    }

    // Constrói lista de informações da pessoa para exibir
    private static List<IRenderable> ConstruirInfoPessoa(Program.Pessoa pessoa, bool incluirInfoCompleta = false)
    {
        var lista = new List<IRenderable>();

        if (!string.IsNullOrEmpty(pessoa.Nome))
            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()}]Nome:[/] [{Tema.Atual.Texto}]{pessoa.Nome}[/]"));

        if (pessoa.DataNascimento != default)
            lista.Add(new Markup(
                $"[{Tema.Atual.Cabecalho.ToMarkup()}]Data:[/] [{Tema.Atual.Texto}]{pessoa.DataNascimento:dd/MM/yyyy}[/]"));

        if (pessoa.Sexo != default)
        {
            var corSexo = pessoa.Sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
            lista.Add(new Markup(
                $"[{Tema.Atual.Cabecalho.ToMarkup()}]Sexo:[/] [{corSexo.ToMarkup()}]{pessoa.Sexo}[/]"));
        }

        if (incluirInfoCompleta)
        {
            if (pessoa.Altura > 0)
                lista.Add(new Markup(
                    $"[{Tema.Atual.Cabecalho.ToMarkup()}]Altura:[/] [{Tema.Atual.Altura}]{UnitConverter.FormatarAltura(pessoa.Altura, pessoa.UnidadeSistema)}[/]"));

            if (pessoa.PesoInicial > 0)
            {
                lista.Add(new Markup(
                    $"[{Tema.Atual.Cabecalho.ToMarkup()}]Peso:[/] [{Tema.Atual.Peso}]{UnitConverter.FormatarPeso(pessoa.PesoInicial, pessoa.UnidadeSistema)}[/]"));

                if (pessoa.Altura > 0)
                {
                    var imc = CalcIMC.Calcular(pessoa.PesoInicial, pessoa.Altura);
                    var corIMC = CalcIMC.ObterCor(imc);
                    lista.Add(new Markup(
                        $"[{Tema.Atual.Cabecalho.ToMarkup()}]IMC:[/] [{corIMC.ToMarkup()}]{imc:F1}[/] [dim]({UnitConverter.ObterCategoriaIMC(imc)})[/]"));
                }
            }
        }

        if (lista.Count > 0 && !incluirInfoCompleta)
            lista.Add(new Rule());

        return lista;
    }

    // Cria painéis esquerdo e direito para cada passo
    private static (Panel esquerda, Panel direita) CriarPaineisPasso(Program.Pessoa pessoa, int passo, IRenderable conteudoInput)
    {
        var painelEsquerdo = new Panel(conteudoInput)
            .RoundedBorder()
            .BorderColor(Tema.Atual.Borda);

        var conteudoDireito = new List<IRenderable>
        {
            new Markup(
                $"\n[{Tema.Atual.Texto} bold]User:[/] {(string.IsNullOrEmpty(pessoa.Nome) ? "?" : pessoa.Nome)}"),
            new Markup(
                $"[{Tema.Atual.Texto} bold]Data:[/] {(pessoa.DataNascimento != default ? pessoa.DataNascimento.ToString("dd/MM/yyyy") : "??")}"),
            new Markup(
                $"[{Tema.Atual.Texto} bold]Idade:[/] {(pessoa.DataNascimento != default ? pessoa.Idade.ToString() : "??")}")
        };

        if (pessoa.Sexo != default)
        {
            var corSexo = pessoa.Sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
            conteudoDireito.Add(new Markup($"[{Tema.Atual.Texto} bold]Sexo:[/] [{corSexo.ToMarkup()}]{pessoa.Sexo}[/]\n"));
        }
        else
        {
            conteudoDireito.Add(new Markup($"[{Tema.Atual.Texto} bold]Sexo:[/] ??\n"));
        }

        conteudoDireito.Add(new Rule());
        conteudoDireito.Add(new Markup(
            $"\n[{Tema.Atual.Altura} bold]Altura:[/] {(pessoa.Altura > 0 ? UnitConverter.FormatarAltura(pessoa.Altura, pessoa.UnidadeSistema) : "??")}"));
        conteudoDireito.Add(new Markup(
            $"[{Tema.Atual.Peso} bold]Peso:[/] {(pessoa.PesoInicial > 0 ? UnitConverter.FormatarPeso(pessoa.PesoInicial, pessoa.UnidadeSistema) : "??")}"));

        if (pessoa.PesoInicial > 0 && pessoa.Altura > 0)
        {
            var imc = CalcIMC.Calcular(pessoa.PesoInicial, pessoa.Altura);
            var corIMC = CalcIMC.ObterCor(imc);
            conteudoDireito.Add(new Markup(
                $"[{Tema.Atual.Peso} bold]IMC:[/] [{corIMC.ToMarkup()}]{imc:F1}[/] [dim]{UnitConverter.ObterCategoriaIMC(imc)}[/]"));
        }

        conteudoDireito.Add(new Markup(
            $"[{Tema.Atual.Peso} bold]Peso Desejado:[/] {(pessoa.PesoDesejado > 0 ? UnitConverter.FormatarPeso(pessoa.PesoDesejado, pessoa.UnidadeSistema) : "??\n")}"));
        conteudoDireito.Add(new Rule());
        conteudoDireito.Add(new Markup(
            $"\n[{Tema.Atual.Texto} bold]Atividade:[/] {(pessoa.NivelAtividade != default ? pessoa.NivelAtividade.ToString() : "??")}"));
        conteudoDireito.Add(new Markup(
            $"[{Tema.Atual.Texto} bold]Objetivo:[/] {(pessoa.Objetivo != default ? pessoa.Objetivo.ToString() : "??\n")}"));

        var painelDireito = new Panel(new Rows(conteudoDireito))
            .RoundedBorder()
            .BorderColor(Tema.Atual.Borda)
            .Header("[bold]Preview[/]");

        return (painelEsquerdo, painelDireito);
    }

    // Cria exibição do IMC calculado
    private static Markup CriarExibicaoIMC(float peso, float altura)
    {
        var imc = CalcIMC.Calcular(peso, altura);
        var corIMC = CalcIMC.ObterCor(imc);
        return new Markup($"[{corIMC.ToMarkup()}]IMC: {imc:F1} - {UnitConverter.ObterCategoriaIMC(imc)}[/]").Centered();
    }

    // Cria painel de controlos
    private static Markup CriarPainelControlos(string controlos)
    {
        return new Markup(
            $"[bold {Tema.Atual.Cabecalho.ToMarkup()}]Controlos:[/]\n" +
            $"  {controlos}\n" +
            $"[{Tema.Atual.Normal.ToMarkup()}]⏎ Enter[/] confirmar"
        ).Centered();
    }

    // Cria validação visual do objetivo vs peso desejado
    private static Markup CriarValidacaoObjetivo(Program.Objetivo objetivo, float pesoAtual, float pesoDesejado,
        Program.UnidadeSistema sistema)
    {
        var valido = ValidarPesoObjetivo(objetivo, pesoAtual, pesoDesejado);

        if (valido)
            return new Markup($"\n[{Tema.Atual.Normal.ToMarkup()}]✓ Objetivo compatível[/]").Centered();

        var pesoAtualFormatado = UnitConverter.FormatarPeso(pesoAtual, sistema);
        var comparacao = objetivo == Program.Objetivo.PerderPeso ? "<" : ">";

        return new Markup($"\n[{Color.Red.ToMarkup()}]⚠ Peso desejado deve ser {comparacao} {pesoAtualFormatado}[/]")
            .Centered();
    }

    // Valida se o peso desejado é compatível com o objetivo
    private static bool ValidarPesoObjetivo(Program.Objetivo objetivo, float pesoAtual, float pesoDesejado)
    {
        return objetivo switch
        {
            Program.Objetivo.PerderPeso => pesoDesejado < pesoAtual,
            Program.Objetivo.GanharMassa => pesoDesejado > pesoAtual,
            _ => true
        };
    }

    // Cria barra de progresso dos passos
    private static string CriarBarraProgresso(int passoAtual)
    {
        var barra = "";
        for (var i = 1; i <= TOTAL_PASSOS; i++)
            barra += i <= passoAtual
                ? $"[{Tema.Atual.Normal.ToMarkup()}]●[/]"
                : "[dim]○[/]";
        return barra;
    }

    // Cria tabela de confirmação com todos os dados
    private static Table CriarTabelaConfirmacao(Program.Pessoa pessoa)
    {
        var tabela = new Table()
            .Centered()
            .RoundedBorder()
            .BorderColor(Tema.Atual.Borda)
            .AddColumn(new TableColumn("[bold]Campo[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Valor[/]").RightAligned());

        tabela.AddRow("Nome", $"[{Tema.Atual.Texto}]{pessoa.Nome}[/]");
        tabela.AddRow("Data de Nascimento", $"[{Tema.Atual.Texto}]{pessoa.DataNascimento:dd/MM/yyyy}[/]");
        tabela.AddRow("Idade", $"[{Tema.Atual.Texto}]{pessoa.Idade} anos[/]");

        var corSexo = pessoa.Sexo == Program.Sexo.Masculino ? Color.Blue : Color.Pink1;
        tabela.AddRow("Sexo", $"[{corSexo.ToMarkup()}]{pessoa.Sexo}[/]");
        tabela.AddRow("Sistema", $"[{Tema.Atual.Texto}]{pessoa.UnidadeSistema}[/]");
        tabela.AddRow("Altura",
            $"[{Tema.Atual.Altura}]{UnitConverter.FormatarAltura(pessoa.Altura, pessoa.UnidadeSistema)}[/]");
        tabela.AddRow("Peso", $"[{Tema.Atual.Peso}]{UnitConverter.FormatarPeso(pessoa.PesoInicial, pessoa.UnidadeSistema)}[/]");

        var imc = CalcIMC.Calcular(pessoa.PesoInicial, pessoa.Altura);
        var corIMC = CalcIMC.ObterCor(imc);
        tabela.AddRow("IMC", $"[{corIMC.ToMarkup()}]{imc:F1} ({UnitConverter.ObterCategoriaIMC(imc)})[/]");

        tabela.AddRow("Nível de Atividade",
            $"[{Tema.Atual.Texto}]{(pessoa.NivelAtividade != default ? pessoa.NivelAtividade.ToString() : "Não especificado")}[/]");
        tabela.AddRow("Objetivo", $"[{Tema.Atual.Texto}]{pessoa.Objetivo}[/]");
        tabela.AddRow("Peso Desejado",
            $"[{Tema.Atual.Peso}]{UnitConverter.FormatarPeso(pessoa.PesoDesejado, pessoa.UnidadeSistema)}[/]");

        return tabela;
    }

    // Mostra o passo atual do formulario
    private static void MostrarPasso(int passo, Panel esquerda, Panel direita, string instrucoes)
    {
        var tabelaPrincipal = new Table()
            .AddColumn(new TableColumn("").Width(50))
            .AddColumn(new TableColumn("").Width(50))
            .HideHeaders()
            .NoBorder()
            .Collapse();

        tabelaPrincipal.AddRow(esquerda, direita);

        conteudo.Clear();
        HelpersUI.CentrarVertical(conteudo, Constantes.OFFSET_VERTICAL_MEDIO);
        conteudo.Add(Align.Center(tabelaPrincipal));

        if (!string.IsNullOrEmpty(instrucoes))
            conteudo.Add(new Markup($"\n[dim]{CriarBarraProgresso(passo)}\n{instrucoes}[/]").Centered());

        HelpersUI.Render(conteudo, $"Criar Pessoa - Passo {passo}/{TOTAL_PASSOS}");
    }

    // Mostra um seletor de enum genérico
    private static StepResult MostrarSeletorEnum<T>(
        Program.Pessoa pessoa,
        int passo,
        Dictionary<T, (string exibicao, Color cor)> opcoes,
        T valorPadrao,
        string nomeCampo,
        bool incluirInfoCompleta = false) where T : Enum
    {
        var listaOpcoes = opcoes.Keys.ToList();
        var indiceSelecionado = listaOpcoes.IndexOf(valorPadrao);
        if (indiceSelecionado == -1) indiceSelecionado = 0;

        while (true)
        {
            var lista = ConstruirInfoPessoa(pessoa, incluirInfoCompleta);
            lista.Add(new Markup($"[{Tema.Atual.Cabecalho.ToMarkup()} bold]{nomeCampo}[/]"));

            for (var i = 0; i < listaOpcoes.Count; i++)
            {
                var prefixo = i == indiceSelecionado ? $"[{Tema.Atual.Cabecalho.ToMarkup()}]►[/] " : "  ";
                var (exibicao, corBase) = opcoes[listaOpcoes[i]];
                var cor = i == indiceSelecionado ? Tema.Atual.Cabecalho : corBase;

                lista.Add(new Markup($"{prefixo}[{cor.ToMarkup()}]{listaOpcoes[i]}[/] [dim]- {exibicao}[/]"));
            }

            var (esquerda, direita) = CriarPaineisPasso(pessoa, passo, new Rows(lista));
            var instrucoes = "↑↓=navegar | ENTER=confirmar | ESC=volta";

            MostrarPasso(passo, esquerda, direita, instrucoes);

            var tecla = Console.ReadKey(true).Key;

            switch (tecla)
            {
                case ConsoleKey.UpArrow:
                    indiceSelecionado = (indiceSelecionado - 1 + listaOpcoes.Count) % listaOpcoes.Count;
                    break;
                case ConsoleKey.DownArrow:
                    indiceSelecionado = (indiceSelecionado + 1) % listaOpcoes.Count;
                    break;
                case ConsoleKey.Enter:
                    return StepResult.Next(listaOpcoes[indiceSelecionado]);
                case ConsoleKey.Escape:
                    return StepResult.Back();
            }
        }
    }

    // Mostra um ajustador numérico
    private static StepResult MostrarAjustadorNumerico(
        Program.Pessoa pessoa,
        int passo,
        ref float valor,
        float min,
        float max,
        float passoGrande,
        float passoPequeno,
        Func<float, IRenderable> formatar,
        string controlos,
        bool mostrarIMC = false,
        bool incluirInfoCompleta = false)
    {
        var valorAtual = valor;

        while (true)
        {
            var lista = ConstruirInfoPessoa(pessoa, incluirInfoCompleta);
            if (incluirInfoCompleta && lista.Count > 0)
            {
                lista.RemoveAt(lista.Count - 1);
                lista.Add(new Rule());
            }

            lista.Add(formatar(valorAtual));

            if (mostrarIMC && pessoa.PesoInicial > 0 && pessoa.Altura > 0)
            {
                var peso = incluirInfoCompleta ? valorAtual : pessoa.PesoInicial;
                var altura = incluirInfoCompleta ? pessoa.Altura : valorAtual;
                lista.Add(CriarExibicaoIMC(peso, altura));
            }

            lista.Add(CriarPainelControlos(controlos));

            var (esquerda, direita) = CriarPaineisPasso(pessoa, passo, new Rows(lista));
            MostrarPasso(passo, esquerda, direita, "ESC=voltar");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    valorAtual = Math.Min(valorAtual + passoGrande, max);
                    break;
                case ConsoleKey.DownArrow:
                    valorAtual = Math.Max(valorAtual - passoGrande, min);
                    break;
                case ConsoleKey.RightArrow:
                    valorAtual = Math.Min(valorAtual + passoPequeno, max);
                    break;
                case ConsoleKey.LeftArrow:
                    valorAtual = Math.Max(valorAtual - passoPequeno, min);
                    break;
                case ConsoleKey.Enter:
                    valor = valorAtual;
                    return StepResult.Next(valorAtual);
                case ConsoleKey.Escape:
                    return StepResult.Back();
            }
        }
    }

    // Guarda o utilizador e retorna
    private static Program.Pessoa? GuardarERetornarUtilizador(Program.Pessoa pessoa)
    {
        if (UserDataManager.SaveUser(pessoa))
        {
            UserDataManager.SaveCurrentUser(pessoa.Nome);
            HelpersUI.MostrarMensagem($"✓ Utilizador '{pessoa.Nome}' criado e guardado!", Tema.Atual.Normal);
        }
        else
        {
            HelpersUI.MostrarMensagem($"✓ Utilizador '{pessoa.Nome}' criado! (Aviso: Erro ao guardar)", Color.Yellow);
        }

        return pessoa;
    }
}