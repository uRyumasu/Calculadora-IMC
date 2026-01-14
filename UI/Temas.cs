using Spectre.Console;

namespace CalculadoraIMC.UI;

public class Cores
{
    public string Nome { get; set; }
    public Color Peso { get; set; }
    public Color Altura { get; set; }
    public Color Magreza { get; set; }
    public Color Normal { get; set; }
    public Color Sobrepeso { get; set; }
    public Color ObesidadeI { get; set; }
    public Color ObesidadeII { get; set; }
    public Color ObesidadeIII { get; set; }

    public Color Titulo { get; set; }
    public Color Cabecalho { get; set; }
    public Color Borda { get; set; }
    public Color Fundo { get; set; }
    public Color Texto { get; set; }
}

public static class Tema
{
    public static readonly Cores Default = new()
    {
        Nome = "Default",
        Peso = Color.Orange1,
        Altura = Color.Cyan,
        Magreza = Color.DeepSkyBlue1,
        Normal = Color.SpringGreen3,
        Sobrepeso = Color.Yellow,
        ObesidadeI = Color.Orange1,
        ObesidadeII = Color.OrangeRed1,
        ObesidadeIII = Color.Red,
        Titulo = Color.Blue,
        Cabecalho = Color.Yellow,
        Borda = Color.Yellow,
        Fundo = Color.Grey19,
        Texto = Color.Grey93
    };

    public static readonly Cores Oceano = new()
    {
        Nome = "Oceano",
        Peso = Color.Blue,
        Altura = Color.Aqua,
        Magreza = Color.SkyBlue1,
        Normal = Color.Turquoise2,
        Sobrepeso = Color.DeepSkyBlue2,
        ObesidadeI = Color.Blue1,
        ObesidadeII = Color.Blue3,
        ObesidadeIII = Color.NavyBlue,
        Titulo = Color.DeepSkyBlue1,
        Cabecalho = Color.Aqua,
        Borda = Color.Aqua,
        Fundo = Color.Grey11,
        Texto = Color.Aqua
    };

    public static readonly Cores Floresta = new()
    {
        Nome = "Floresta",
        Peso = Color.Green,
        Altura = Color.GreenYellow,
        Magreza = Color.PaleGreen1,
        Normal = Color.Green3,
        Sobrepeso = Color.Yellow3,
        ObesidadeI = Color.Orange3,
        ObesidadeII = Color.DarkOrange,
        ObesidadeIII = Color.OrangeRed1,
        Titulo = Color.Green3,
        Cabecalho = Color.SpringGreen3,
        Borda = Color.Green,
        Fundo = Color.Grey15,
        Texto = Color.PaleGreen1
    };

    public static readonly Cores Noturno = new()
    {
        Nome = "Noturno",
        Peso = Color.Purple,
        Altura = Color.Magenta1,
        Magreza = Color.Violet,
        Normal = Color.MediumPurple,
        Sobrepeso = Color.Plum1,
        ObesidadeI = Color.Pink1,
        ObesidadeII = Color.HotPink,
        ObesidadeIII = Color.DeepPink1,
        Titulo = Color.MediumPurple,
        Cabecalho = Color.Magenta1,
        Borda = Color.Purple,
        Fundo = Color.Grey7,
        Texto = Color.Plum1
    };

    public static readonly Cores PorDoSol = new()
    {
        Nome = "Pôr do Sol",
        Peso = Color.Orange3,
        Altura = Color.Red1,
        Magreza = Color.Yellow,
        Normal = Color.Orange1,
        Sobrepeso = Color.DarkOrange,
        ObesidadeI = Color.OrangeRed1,
        ObesidadeII = Color.Red,
        ObesidadeIII = Color.DarkRed,
        Titulo = Color.OrangeRed1,
        Cabecalho = Color.Orange1,
        Borda = Color.DarkOrange,
        Fundo = Color.Grey11,
        Texto = Color.LightGoldenrod1
    };

    public static readonly Cores Neon = new()
    {
        Nome = "Neon",
        Peso = Color.Fuchsia,
        Altura = Color.Lime,
        Magreza = Color.Cyan1,
        Normal = Color.Green1,
        Sobrepeso = Color.Yellow1,
        ObesidadeI = Color.Orange1,
        ObesidadeII = Color.Magenta1,
        ObesidadeIII = Color.Red1,
        Titulo = Color.Magenta1,
        Cabecalho = Color.Cyan1,
        Borda = Color.Fuchsia,
        Fundo = Color.Black,
        Texto = Color.Cyan1
    };

    public static readonly Cores Matrix = new()
    {
        Nome = "Matrix",
        Peso = Color.Lime,
        Altura = Color.Green1,
        Magreza = Color.Green3,
        Normal = Color.Lime,
        Sobrepeso = Color.Green1,
        ObesidadeI = Color.Green,
        ObesidadeII = Color.DarkGreen,
        ObesidadeIII = Color.DarkOliveGreen3,
        Titulo = Color.Lime,
        Cabecalho = Color.Green1,
        Borda = Color.Green,
        Fundo = Color.Black,
        Texto = Color.Lime
    };

    public static readonly Cores Dracula = new()
    {
        Nome = "Drácula",
        Peso = Color.Purple,
        Altura = Color.Pink1,
        Magreza = Color.Cyan1,
        Normal = Color.Green1,
        Sobrepeso = Color.Yellow1,
        ObesidadeI = Color.Orange1,
        ObesidadeII = Color.Red1,
        ObesidadeIII = Color.Red3,
        Titulo = Color.Purple,
        Cabecalho = Color.Pink1,
        Borda = Color.Purple,
        Fundo = Color.Grey7,
        Texto = Color.Pink1
    };

    public static readonly Cores Fogo = new()
    {
        Nome = "Fogo",
        Peso = Color.Red,
        Altura = Color.Orange1,
        Magreza = Color.Yellow,
        Normal = Color.Orange1,
        Sobrepeso = Color.OrangeRed1,
        ObesidadeI = Color.Red,
        ObesidadeII = Color.Red3,
        ObesidadeIII = Color.Maroon,
        Titulo = Color.Red,
        Cabecalho = Color.Orange1,
        Borda = Color.OrangeRed1,
        Fundo = Color.Grey11,
        Texto = Color.Orange1
    };

    public static readonly Cores Gelo = new()
    {
        Nome = "Gelo",
        Peso = Color.Cyan1,
        Altura = Color.Blue,
        Magreza = Color.White,
        Normal = Color.Cyan1,
        Sobrepeso = Color.DeepSkyBlue1,
        ObesidadeI = Color.Blue,
        ObesidadeII = Color.Blue3,
        ObesidadeIII = Color.NavyBlue,
        Titulo = Color.Cyan1,
        Cabecalho = Color.DeepSkyBlue1,
        Borda = Color.Blue,
        Fundo = Color.Grey11,
        Texto = Color.PaleTurquoise1
    };

    public static readonly Cores Outono = new()
    {
        Nome = "Outono",
        Peso = Color.DarkOrange,
        Altura = Color.Gold1,
        Magreza = Color.Yellow,
        Normal = Color.Gold1,
        Sobrepeso = Color.Orange1,
        ObesidadeI = Color.DarkOrange,
        ObesidadeII = Color.OrangeRed1,
        ObesidadeIII = Color.Maroon,
        Titulo = Color.DarkOrange,
        Cabecalho = Color.Gold1,
        Borda = Color.Orange3,
        Fundo = Color.Grey15,
        Texto = Color.Gold1
    };

    public static readonly Cores Cereja = new()
    {
        Nome = "Cereja",
        Peso = Color.DeepPink1,
        Altura = Color.HotPink,
        Magreza = Color.Pink1,
        Normal = Color.LightPink1,
        Sobrepeso = Color.HotPink,
        ObesidadeI = Color.DeepPink1,
        ObesidadeII = Color.Red1,
        ObesidadeIII = Color.Maroon,
        Titulo = Color.DeepPink1,
        Cabecalho = Color.HotPink,
        Borda = Color.Pink1,
        Fundo = Color.Grey11,
        Texto = Color.Pink1
    };

    public static readonly Cores Cibernetico = new()
    {
        Nome = "Cibernético",
        Peso = Color.Cyan1,
        Altura = Color.Blue1,
        Magreza = Color.SkyBlue1,
        Normal = Color.Cyan1,
        Sobrepeso = Color.Blue1,
        ObesidadeI = Color.Blue3,
        ObesidadeII = Color.Purple,
        ObesidadeIII = Color.Magenta3,
        Titulo = Color.Cyan1,
        Cabecalho = Color.Blue1,
        Borda = Color.Purple,
        Fundo = Color.Black,
        Texto = Color.Cyan1
    };

    public static readonly Cores Arcoiris = new()
    {
        Nome = "Arco-íris",
        Peso = Color.Red,
        Altura = Color.Blue,
        Magreza = Color.Violet,
        Normal = Color.Green,
        Sobrepeso = Color.Yellow,
        ObesidadeI = Color.Orange1,
        ObesidadeII = Color.Red,
        ObesidadeIII = Color.Magenta3,
        Titulo = Color.Magenta1,
        Cabecalho = Color.Yellow,
        Borda = Color.Green,
        Fundo = Color.Grey15,
        Texto = Color.Magenta1
    };

    public static readonly Cores Vintage = new()
    {
        Nome = "Vintage",
        Peso = Color.Tan,
        Altura = Color.SandyBrown,
        Magreza = Color.Wheat1,
        Normal = Color.Tan,
        Sobrepeso = Color.DarkOrange3,
        ObesidadeI = Color.DarkOrange3,
        ObesidadeII = Color.DarkOrange,
        ObesidadeIII = Color.Maroon,
        Titulo = Color.SandyBrown,
        Cabecalho = Color.Tan,
        Borda = Color.DarkOrange3,
        Fundo = Color.Grey19,
        Texto = Color.Wheat1
    };

    public static readonly Cores Lavanda = new()
    {
        Nome = "Lavanda",
        Peso = Color.MediumPurple,
        Altura = Color.Plum1,
        Magreza = Color.Thistle1,
        Normal = Color.Plum1,
        Sobrepeso = Color.MediumPurple,
        ObesidadeI = Color.Purple,
        ObesidadeII = Color.Purple4,
        ObesidadeIII = Color.Purple3,
        Titulo = Color.MediumPurple,
        Cabecalho = Color.Plum1,
        Borda = Color.Purple,
        Fundo = Color.Grey15,
        Texto = Color.Thistle1
    };

    public static readonly Cores Deserto = new()
    {
        Nome = "Deserto",
        Peso = Color.Gold3,
        Altura = Color.Yellow3,
        Magreza = Color.Khaki1,
        Normal = Color.Gold1,
        Sobrepeso = Color.Orange3,
        ObesidadeI = Color.DarkOrange3,
        ObesidadeII = Color.DarkOrange,
        ObesidadeIII = Color.Maroon,
        Titulo = Color.Gold1,
        Cabecalho = Color.Yellow3,
        Borda = Color.Orange3,
        Fundo = Color.Grey11,
        Texto = Color.Khaki1
    };

    public static readonly Cores Menta = new()
    {
        Nome = "Menta",
        Peso = Color.Aquamarine1,
        Altura = Color.Turquoise2,
        Magreza = Color.PaleTurquoise1,
        Normal = Color.Aquamarine1,
        Sobrepeso = Color.Turquoise2,
        ObesidadeI = Color.DarkCyan,
        ObesidadeII = Color.Teal,
        ObesidadeIII = Color.DarkCyan,
        Titulo = Color.Aquamarine1,
        Cabecalho = Color.Turquoise2,
        Borda = Color.DarkCyan,
        Fundo = Color.Grey15,
        Texto = Color.Aquamarine1
    };

    public static readonly Cores Vampiro = new()
    {
        Nome = "Vampiro",
        Peso = Color.Red,
        Altura = Color.Grey50,
        Magreza = Color.Grey70,
        Normal = Color.Silver,
        Sobrepeso = Color.Red3,
        ObesidadeI = Color.Red,
        ObesidadeII = Color.DarkRed,
        ObesidadeIII = Color.Maroon,
        Titulo = Color.Red,
        Cabecalho = Color.Grey50,
        Borda = Color.DarkRed,
        Fundo = Color.Black,
        Texto = Color.Silver
    };

    public static readonly Cores Tropical = new()
    {
        Nome = "Tropical",
        Peso = Color.Orange1,
        Altura = Color.Turquoise2,
        Magreza = Color.Yellow,
        Normal = Color.SpringGreen2,
        Sobrepeso = Color.Orange1,
        ObesidadeI = Color.Orange1,
        ObesidadeII = Color.Red1,
        ObesidadeIII = Color.DeepPink3,
        Titulo = Color.Turquoise2,
        Cabecalho = Color.Orange1,
        Borda = Color.SpringGreen2,
        Fundo = Color.Grey11,
        Texto = Color.SpringGreen2
    };

    public static Cores Atual { get; set; } = Default;

    public static List<Cores> Todos => new()
    {
        Default, Oceano, Floresta, Noturno, PorDoSol, Neon, Matrix, Dracula,
        Fogo, Gelo, Outono, Cereja, Cibernetico, Arcoiris, Vintage, Lavanda,
        Deserto, Menta, Vampiro, Tropical
    };
}