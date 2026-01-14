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
            var users = LoadAllUsers();
            var existingIndex = users.FindIndex(u =>
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
            return JsonSerializer.Deserialize<List<Program.Pessoa>>(json, new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            }) ?? new List<Program.Pessoa>();
        }
        catch
        {
            return new List<Program.Pessoa>();
        }
    }

    public static Program.Pessoa? LoadUser(string nome)
    {
        var users = LoadAllUsers();
        return users.FirstOrDefault(u =>
            u.nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }

    public static bool DeleteUser(string nome)
    {
        try
        {
            var users = LoadAllUsers();
            var removed = users.RemoveAll(u =>
                u.nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

            if (removed > 0)
            {
                var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                });

                File.WriteAllText(UsersFilePath, json);

                // If deleted user was current user, clear it
                if (File.Exists(CurrentUserFilePath))
                {
                    var currentUser = File.ReadAllText(CurrentUserFilePath).Trim();
                    if (currentUser.Equals(nome, StringComparison.OrdinalIgnoreCase)) File.Delete(CurrentUserFilePath);
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
            File.WriteAllText(CurrentUserFilePath, nome);
        }
        catch
        {
        }
    }

    public static Program.Pessoa? LoadCurrentUser()
    {
        try
        {
            if (File.Exists(CurrentUserFilePath))
            {
                var nome = File.ReadAllText(CurrentUserFilePath).Trim();
                return LoadUser(nome);
            }
        }
        catch
        {
        }

        return null;
    }

    public static string GetDataDirectory()
    {
        return DataDirectory;
    }
}