using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalculadoraIMC.Core;

// Gere o armazenamento e carregamento de dados dos utilizadores
public static class UserDataManager
{
    // Appdata/CalculadoraIMC
    private static readonly string DiretorioDados = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CalculadoraIMC"
    );

    // Ficheiro que contem todos os utilizadores
    private static readonly string CaminhoFicheiroUsers = Path.Combine(DiretorioDados, "users.json");
    
    // Ficheiro que contem o utilizador atual
    private static readonly string CaminhoUserAtual = Path.Combine(DiretorioDados, "current_user.txt");

    // Criar a directory CalculadoraIMC se esta não existe
    static UserDataManager()
    {
        if (!Directory.Exists(DiretorioDados))
            Directory.CreateDirectory(DiretorioDados);
    }

    // Guarda os dados de um utilizador
    public static bool SaveUser(Program.Pessoa pessoa)
    {
        try
        {
            // Verificar se utilizador não é vazio
            if (string.IsNullOrEmpty(pessoa.Nome))
                return false;

            // Carregar utilizadores
            var utilizadores = LoadAllUsers();
            
            // Encontrar se utilizador existe
            int indiceExistente = utilizadores.FindIndex(u =>
                !string.IsNullOrEmpty(u.Nome) && 
                u.Nome.Equals(pessoa.Nome, StringComparison.OrdinalIgnoreCase));
            
            if (indiceExistente >= 0) // Se o utilizador existe
                utilizadores[indiceExistente] = pessoa; // Substituir informação atual.
            else // Senao
                utilizadores.Add(pessoa); // Criar um utilizador novo

            // Converter em JSON
            string json = JsonSerializer.Serialize(utilizadores, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            });

            File.WriteAllText(CaminhoFicheiroUsers, json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Carrega todos os utilizadores guardados
    public static List<Program.Pessoa> LoadAllUsers()
    {
        try
        {
            // Se ficheiro com todos os utilizadores existir
            if (!File.Exists(CaminhoFicheiroUsers))
                return new List<Program.Pessoa>();

            // Ler e criar uma lista com todos os utilizadores
            string json = File.ReadAllText(CaminhoFicheiroUsers);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Program.Pessoa>();

            // Converter JSON em uma lista C#
            var utilizadores = JsonSerializer.Deserialize<List<Program.Pessoa>>(json, 
                new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                }) ?? new List<Program.Pessoa>();

            // Filtra utilizadores com nome válido
            return utilizadores.Where(u => !string.IsNullOrEmpty(u.Nome)).ToList();
        }
        catch
        {
            return new List<Program.Pessoa>();
        }
    }

    // Carrega um utilizador específico pelo nome
    public static Program.Pessoa? LoadUser(string nome)
    {
        // Se utilizador existe
        if (string.IsNullOrEmpty(nome))
            return null;
        
        // Carregar utilizador
        var utilizadores = LoadAllUsers();
        return utilizadores.FirstOrDefault(u =>
            !string.IsNullOrEmpty(u.Nome) && 
            u.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }

    // Apaga um utilizador
    public static bool DeleteUser(string nome)
    {
        try
        {
            // Se utilizador existe
            if (string.IsNullOrEmpty(nome))
                return false;

            // Tentar apagar utilizador
            var utilizadores = LoadAllUsers();
            int removidos = utilizadores.RemoveAll(u =>
                !string.IsNullOrEmpty(u.Nome) && 
                u.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

            // Se algum utilizador foi removido
            if (removidos > 0)
            {
                // Converter para JSON
                string json = JsonSerializer.Serialize(utilizadores, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                });

                File.WriteAllText(CaminhoFicheiroUsers, json);

                // Remove o ficheiro de utilizador atual se for este
                if (File.Exists(CaminhoUserAtual))
                {
                    string userAtual = File.ReadAllText(CaminhoUserAtual).Trim();
                    if (!string.IsNullOrEmpty(userAtual) && 
                        userAtual.Equals(nome, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(CaminhoUserAtual);
                    }
                }

                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    // Guarda o nome do utilizador atual
    public static void SaveCurrentUser(string nome)
    {
        try
        {
            if (!string.IsNullOrEmpty(nome))
                File.WriteAllText(CaminhoUserAtual, nome);
        }
        catch
        {
            // Falha silenciosa
        }
    }
}