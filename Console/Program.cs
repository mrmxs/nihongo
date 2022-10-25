namespace Nihongo.Console;

using Nihongo.App.Helpers;
using Nihongo.Core.Models;
using System;
using System.IO;
using System.Text;

public class Program
{
    private readonly static string _testFile =
        $@"{Environment.CurrentDirectory}\test\鬼劇获画 #kanji - Jisho.org.htm";
    private readonly static string _testUrl = "https://jisho.org/search/劇获画%23kanji";

    public static void Main(string[] args)
    {
        TestJishoHelper();
    }

    private static void TestJishoHelper() {
        // var htmlSrc = GetHelper.FromFile(_testFile);
        var htmlSrc = GetHelper.FromUrl(_testUrl);

        var jisho = new JishoHelper(htmlSrc);

        foreach (var kanji in jisho.Kanjis)
            Console.WriteLine(kanji);
    }
}

