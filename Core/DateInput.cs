using CalculadoraIMC.UI;

namespace CalculadoraIMC.Core;

// Classe para entrada de data com validação
public class DateInput
{
    private int campoAtual; // 0=dia, 1=mês, 2=ano
    private string dia = "";
    private string mes = "";
    private string ano = "";

    // Adiciona um dígito ao campo atual
    public void AdicionarDigito(char digito)
    {
        switch (campoAtual)
        {
            case 0 when dia.Length < 2:
                dia += digito;
                if (dia.Length == 2 && int.Parse(dia) is >= 1 and <= 31)
                    campoAtual = 1;
                else if (dia.Length == 2)
                    dia = "";
                break;

            case 1 when mes.Length < 2:
                mes += digito;
                if (mes.Length == 2)
                {
                    int numeroMes = int.Parse(mes);
                    if (numeroMes is >= 1 and <= 12 &&
                        int.Parse(dia) <= DateTime.DaysInMonth(DateTime.Now.Year, numeroMes))
                    {
                        campoAtual = 2;
                    }
                    else
                    {
                        mes = "";
                        dia = "";
                        campoAtual = 0;
                    }
                }
                break;

            case 2 when ano.Length < 4:
                ano += digito;
                if (ano.Length == 4)
                {
                    int numeroAno = int.Parse(ano);
                    if (numeroAno < 1900 || numeroAno > DateTime.Now.Year ||
                        int.Parse(dia) > DateTime.DaysInMonth(numeroAno, int.Parse(mes)))
                    {
                        ano = "";
                        mes = "";
                        dia = "";
                        campoAtual = 0;
                    }
                }
                break;
        }
    }

    // Remove o último dígito (backspace)
    public void ApagarDigito()
    {
        switch (campoAtual)
        {
            case 0 when dia.Length > 0:
                dia = dia[..^1];
                break;
            case 1 when mes.Length > 0:
                mes = mes[..^1];
                break;
            case 2 when ano.Length > 0:
                ano = ano[..^1];
                break;
            case > 0:
                campoAtual--;
                if (campoAtual == 1) mes = "";
                else if (campoAtual == 0) dia = "";
                break;
        }
    }

    // Verifica se a data está completa
    public bool EstaCompleta()
    {
        return dia.Length == 2 && mes.Length == 2 && ano.Length == 4;
    }

    // Converte para DateTime
    public DateTime ParaDateTime()
    {
        return new DateTime(int.Parse(ano), int.Parse(mes), int.Parse(dia));
    }

    // Retorna a data formatada para exibição (com highlight no campo ativo)
    public string ObterExibicaoFormatada()
    {
        string exibicaoDia = string.IsNullOrEmpty(dia) ? "__" : dia.PadLeft(2, '0');
        string exibicaoMes = string.IsNullOrEmpty(mes) ? "__" : mes.PadLeft(2, '0');
        string exibicaoAno = string.IsNullOrEmpty(ano) ? "____" : ano.PadLeft(4, '0');

        return campoAtual switch
        {
            0 => $"[{Tema.Atual.Cabecalho.ToMarkup()}]{exibicaoDia}[/]/{exibicaoMes}/{exibicaoAno}",
            1 => $"{exibicaoDia}/[{Tema.Atual.Cabecalho.ToMarkup()}]{exibicaoMes}[/]/{exibicaoAno}",
            _ => $"{exibicaoDia}/{exibicaoMes}/[{Tema.Atual.Cabecalho.ToMarkup()}]{exibicaoAno}[/]"
        };
    }
}