namespace CalculadoraIMC.Core;

public static class ImageDownloader
{
    private static readonly HttpClient _client = new();
    
    public static bool Download(string url, string savePath)
    {
        try
        {
            var bytes = _client.GetByteArrayAsync(url).Result;
            File.WriteAllBytes(savePath, bytes);
            return true;
        }
        catch
        {
            return false;
        }
    }
}