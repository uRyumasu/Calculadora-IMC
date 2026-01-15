namespace CalculadoraIMC.Core;

// Constantes para evitar números mágicos
public static class Constantes
{
    // Offsets verticais para centrar conteúdo
    public const int OFFSET_VERTICAL_GRANDE = 12;
    public const int OFFSET_VERTICAL_MEDIO = 10;
    public const int OFFSET_VERTICAL_PEQUENO = 8;
    public const int OFFSET_VERTICAL_PADRAO = 15;
    public const int OFFSET_VERTICAL_MINIMO = 13;

    // Limites de altura (em metros)
    public const float ALTURA_MINIMA = 0.5f;
    public const float ALTURA_MAXIMA = 2.5f;

    // Limites de peso (em kg)
    public const float PESO_MINIMO = 30f;
    public const float PESO_MAXIMO = 300f;

    // Ajustes de peso (métrico)
    public const float AJUSTE_PESO_GRANDE = 1f;      // 1kg
    public const float AJUSTE_PESO_PEQUENO = 0.1f;   // 100g

    // Ajustes de altura (métrico)
    public const float AJUSTE_ALTURA_GRANDE = 0.1f;  // 10cm
    public const float AJUSTE_ALTURA_PEQUENO = 0.01f; // 1cm

    // Limites imperial
    public const float PESO_MINIMO_LBS = 66f;
    public const float PESO_MAXIMO_LBS = 661f;
    public const float AJUSTE_PESO_GRANDE_LBS = 1f;
    public const float AJUSTE_PESO_PEQUENO_LBS = 0.5f;

    public const int ALTURA_MINIMA_FEET = 3;
    public const int ALTURA_MAXIMA_FEET = 8;

    // Categorias de IMC
    public const float IMC_MAGREZA = 18.5f;
    public const float IMC_NORMAL = 25f;
    public const float IMC_SOBREPESO = 30f;
    public const float IMC_OBESIDADE_I = 35f;
    public const float IMC_OBESIDADE_II = 40f;

    // Percentis
    public const double PERCENTIL_MAGREZA = 3.0;
    public const double PERCENTIL_ABAIXO = 15.0;
    public const double PERCENTIL_NORMAL = 85.0;
    public const double PERCENTIL_SOBREPESO = 97.0;

    // Idades para percentil
    public const int IDADE_MINIMA_PERCENTIL = 2;
    public const int IDADE_MAXIMA_PERCENTIL = 18;

    // Ajustes calóricos
    public const int AJUSTE_DEFICIT_GRANDE = -500;
    public const int AJUSTE_DEFICIT_MEDIO = -300;
    public const int AJUSTE_DEFICIT_PEQUENO = -100;
    public const int AJUSTE_SUPERAVIT_HOMEM = 300;
    public const int AJUSTE_SUPERAVIT_MULHER = 200;

    // Multiplicadores de atividade
    public const float MULTIPLICADOR_SEDENTARIO = 1.2f;
    public const float MULTIPLICADOR_LEVE = 1.375f;
    public const float MULTIPLICADOR_MODERADO = 1.55f;
    public const float MULTIPLICADOR_ATIVO = 1.725f;
    public const float MULTIPLICADOR_MUITO_ATIVO = 1.9f;

    // Macros (g por kg de peso)
    public const float PROTEINA_PERDA_PESO = 2.2f;
    public const float PROTEINA_GANHO_MASSA = 2.0f;
    public const float PROTEINA_RECOMPOSICAO = 2.4f;
    public const float PROTEINA_MANUTENCAO = 1.8f;

    public const float GORDURA_PERDA_PESO = 0.8f;
    public const float GORDURA_GANHO_MASSA = 1.0f;
    public const float GORDURA_RECOMPOSICAO = 0.9f;
    public const float GORDURA_MANUTENCAO = 1.0f;

    // Calorias por grama de macro
    public const int CALORIAS_POR_GRAMA_PROTEINA = 4;
    public const int CALORIAS_POR_GRAMA_CARBOIDRATO = 4;
    public const int CALORIAS_POR_GRAMA_GORDURA = 9;

    // Tempos de espera
    public const int TEMPO_MENSAGEM = 1500; // ms
    public const int TEMPO_SPLASH = 5000;   // ms

    // Outros
    public const int TAMANHO_NOME_MAXIMO = 20;
    public const int POLEGADAS_POR_PE = 12;
}