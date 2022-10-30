namespace Nihongo.App.Helpers;
public static class IOHelper
{
    public static string FromFile(string path)
    {
        return File.ReadAllText(path);
    }

    public static string FromUrl(string path)
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = client.GetAsync(path).Result)
            {
                using (HttpContent content = response.Content)
                {
                    return content.ReadAsStringAsync().Result;
                }
            }
        }
    }

    public static async Task ToFileAsync(string path, string[] lines)
    {
        await File.WriteAllLinesAsync(path, lines);
    }
}