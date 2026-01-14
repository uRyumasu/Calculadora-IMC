using CalculadoraIMC.UI;

namespace CalculadoraIMC.Core;

public class Dicas
{
    private static readonly string[] DicasIMC = new[]
    {
        $"[{Tema.Atual.Texto} bold]Dica:[/] Um IMC entre [bold]18,5 e 24,9[/] é considerado [bold]peso normal[/] — excelente para a saúde cardiovascular!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] IMC abaixo de 18,5? Que tal aumentar um pouco as calorias com alimentos nutritivos como abacate, castanhas e proteínas!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Se o seu IMC está entre [bold]25 e 29,9[/] (sobrepeso), pequenas mudanças como caminhar 30 min/dia já fazem grande diferença!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] IMC ≥ 30 indica obesidade. Comece devagar: menos ultraprocessados + mais movimento = caminho poderoso para a saúde!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Lembre-se: o IMC é só uma pista! Combine com medida da cintura, força muscular e bem-estar geral.",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Independente do número: cada passo saudável que dá hoje é um presente para o seu futuro!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Beba água, coma mais vegetais e mexa-se — o corpo agradece muito mais do que qualquer dieta milagrosa.",
        $"[{Tema.Atual.Texto} bold]Dica:[/] O segredo não é ficar magro... é ficar [bold]saudável[/] e com energia para aproveitar a vida!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Treino de força 2–3× por semana ajuda a melhorar a composição corporal (mesmo que o IMC mude devagar).",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Celebre cada pequena vitória! Saúde é maratona, não sprint. Você está no caminho certo! 💪"
    };

    public static string ObterDica()
    {
        Random rnd = new Random();
        
        int indice = rnd.Next(DicasIMC.Length);
        return DicasIMC[indice];
    }
}