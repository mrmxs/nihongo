namespace Nihongo.Console;

using Nihongo.App.Helpers;
using Nihongo.Core.Models;
using System;
using System.IO;
using System.Text;

public class Program
{
    private static string _testFile = @"test\鬼劇获画 #kanji - Jisho.org.htm";
    private static string _testUrl = "https://jisho.org/search/劇获画%23kanji";

    public static void Main(string[] args)
    {
        TestJishoHelper(GetFromFile());
    }

    private static void TestJishoHelper(string htmlSrc) {
        var jisho = new JishoHelper(htmlSrc);

        foreach (var kanji in jisho.Kanjis)
            Console.WriteLine(kanji);
    }

    private static string GetFromUrl() {
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = client.GetAsync(_testUrl).Result)
            {
                using (HttpContent content = response.Content)
                {
                    return content.ReadAsStringAsync().Result;
                }
            }
        }
    }

    private static string GetFromFile() {
        var path = Environment.CurrentDirectory + $@"\{_testFile}";
        
        return File.ReadAllText(path);
    }
}

