namespace Nihongo.Console;

using Nihongo.App.Helpers;
using Nihongo.Core.Models;
using System;
using System.IO;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        TestJishoHelper(GetFromUrl());
    }

    private static void TestJishoHelper(string htmlSrc) {
        var jisho = new JishoHelper(htmlSrc);

        foreach (var kanji in jisho.Kanjis)
            Console.WriteLine(kanji);
    }

    private static string GetFromUrl() {
        string url = "https://jisho.org/search/劇获画%23kanji";

        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = client.GetAsync(url).Result)
            {
                using (HttpContent content = response.Content)
                {
                    return content.ReadAsStringAsync().Result;
                }
            }
        }
    }

    private static string GetFromFile() {
        var path = Environment.CurrentDirectory + @"\test\https __jisho.org_search__E5_8A_87_E8_8E_B7_E7_94_BB_20_23kanji.htm";
        
        return File.ReadAllText(path);
    }
}

