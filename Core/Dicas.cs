using CalculadoraIMC.UI;

namespace CalculadoraIMC.Core;

/// <summary>
/// Fornece dicas aleatórias sobre saúde e IMC
/// </summary>
public class Dicas
{
    // Array de dicas sobre saúde, nutrição e IMC com formatação Spectre.Console
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
        $"[{Tema.Atual.Texto} bold]Dica:[/] Celebre cada pequena vitória! Saúde é maratona, não sprint. Você está no caminho certo! 💪",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Dormir bem (7–9h) é um dos maiores aliados para controlar o peso e melhorar o IMC a longo prazo!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Subir escadas, estacionar mais longe, brincar com as crianças/pets... todo movimento conta e soma no seu dia!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Trocar refrigerante e sucos industrializados por água com limão, chá sem açúcar ou água com gás já faz diferença grande!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] O IMC não diferencia músculo de gordura. Quem treina força pode ter IMC “alto” e estar muito saudável!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Coma devagar e preste atenção na fome e saciedade — isso reduz bastante a chance de comer além do necessário.",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Um prato colorido (muitos vegetais diferentes) geralmente é um prato mais nutritivo e mais amigo do seu IMC!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Estresse crônico aumenta o cortisol e facilita acúmulo de gordura abdominal. Respire fundo, medite, descanse!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Não precisa cortar tudo que gosta. 80–90% saudável + 10–20% prazer costuma funcionar muito melhor que 100% perfeito.",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Consistência > Intensidade. Melhor 3 caminhadas tranquilas por semana do que 1 mês insano e depois desistir.",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Meça além da balança: cintura, fotos, roupas, energia, sono, humor… são indicadores tão importantes quanto o IMC!",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Proteína em todas as refeições (ovos, frango, peixe, iogurte, leguminosas…) ajuda a manter massa magra e saciedade.",
        $"[{Tema.Atual.Texto} bold]Dica:[/] O maior erro não é ter um dia ruim. É deixar 1 dia ruim virar 1 semana, 1 mês… Volte pro trilho no próximo café da manhã! ❤️",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Saúde mental importa tanto quanto saúde física. Se cuidar emocionalmente também melhora escolhas alimentares e disposição para mexer o corpo.",
        $"[{Tema.Atual.Texto} bold]Dica:[/] Experimente caminhar após as refeições (mesmo que só 10–15 min). Ajuda no controle glicêmico e na saúde metabólica geral!"
    };

    /// <summary>
    /// Retorna uma dica aleatória sobre saúde e bem-estar
    /// </summary>
    /// <returns>String formatada com uma dica aleatória</returns>
    public static string ObterDica()
    {
        Random rnd = new Random();
        int indice = rnd.Next(DicasIMC.Length);
        return DicasIMC[indice];
    }
}