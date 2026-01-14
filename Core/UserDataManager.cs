using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalculadoraIMC.Core;

public static class UserDataManager
{
    private static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "IMC_Calculator"
    );

    private static readonly string UsersFilePath = Path.Combine(DataDirectory, "users.json");
    private static readonly string CurrentUserFilePath = Path.Combine(DataDirectory, "current_user.txt");

    static UserDataManager()
    {
        if (!Directory.Exists(DataDirectory))
            Directory.CreateDirectory(DataDirectory);
    }

    public static bool SaveUser(Program.Pessoa pessoa)
    {
        try
        {
            if (string.IsNullOrEmpty(pessoa.nome))
                return false;

            var users = LoadAllUsers();
            var existingIndex = users.FindIndex(u =>
                !string.IsNullOrEmpty(u.nome) && 
                u.nome.Equals(pessoa.nome, StringComparison.OrdinalIgnoreCase));

            if (existingIndex >= 0)
                users[existingIndex] = pessoa;
            else
                users.Add(pessoa);

            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            });

            File.WriteAllText(UsersFilePath, json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static List<Program.Pessoa> LoadAllUsers()
    {
        try
        {
            if (!File.Exists(UsersFilePath))
                return new List<Program.Pessoa>();

            var json = File.ReadAllText(UsersFilePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Program.Pessoa>();

            var users = JsonSerializer.Deserialize<List<Program.Pessoa>>(json, new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            }) ?? new List<Program.Pessoa>();

            
            return users.Where(u => !string.IsNullOrEmpty(u.nome)).ToList();
        }
        catch
        {
            return new List<Program.Pessoa>();
        }
    }

    public static Program.Pessoa? LoadUser(string nome)
    {
        if (string.IsNullOrEmpty(nome))
            return null;
    
        var users = LoadAllUsers();
        return users.FirstOrDefault(u =>
            !string.IsNullOrEmpty(u.nome) && 
            u.nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }

    public static bool DeleteUser(string nome)
    {
        try
        {
            if (string.IsNullOrEmpty(nome))
                return false;

            var users = LoadAllUsers();
            var removed = users.RemoveAll(u =>
                !string.IsNullOrEmpty(u.nome) && 
                u.nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

            if (removed > 0)
            {
                var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                });

                File.WriteAllText(UsersFilePath, json);

                
                if (File.Exists(CurrentUserFilePath))
                {
                    var currentUser = File.ReadAllText(CurrentUserFilePath).Trim();
                    if (!string.IsNullOrEmpty(currentUser) && 
                        currentUser.Equals(nome, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(CurrentUserFilePath);
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

    public static void SaveCurrentUser(string nome)
    {
        try
        {
            if (!string.IsNullOrEmpty(nome))
                File.WriteAllText(CurrentUserFilePath, nome);
        }
        catch
        {
        }
    }
}