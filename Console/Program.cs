namespace Nihongo.Console;

using Nihongo.App.Helpers;
using System;

public class Program
{
    public static void Main(string[] args)
    {
        GenerateAnkiImportFile();
    }

    private static void GenerateAnkiImportFile()
    {
        var joyo = IOHelper.FromFile($@"{Environment.CurrentDirectory}\test\Jōyō_2020.txt")
            .Split("\n");

        foreach (var grade in joyo)
        {
            var gradeNumber = Array.IndexOf(joyo, grade) + 1;
            var path = $@"{Environment.CurrentDirectory}\test\anki_import_jōyō_grade{gradeNumber}.txt";

            using (StreamWriter file = new(path))
            {
                var header = new List<string> {
                    "#deck:\"Kanji Writing\"",
                    "#notetype:\"Kanji Writing\"",
                    "#html:true"
                };
                header.ForEach(line => file.WriteLine(line));

                // jisho search page contains max 10 kanjis
                foreach (var ten in grade.Chunk(10))
                {
                    var jisho = new JishoHelper(new string(ten), JishoHelper.SearchTag.Kanji);
                    var ankiNotes = jisho.Kanjis?
                        .Select(k => new JishoAnkiKanjiConverter().Convert(k));

                    // var url = $"https://jisho.org/search/{new string(ten)}%23kanji";
                    // var htmlSrc = IOHelper.FromUrl(url);
                    // var ankiNotes = new JishoHelper(htmlSrc).Kanjis
                    //     .Select(k => new JishoAnkiKanjiConverter().Convert(k));

                    foreach (var note in ankiNotes)
                    {
                        file.WriteLine(note.ToString());
                        Console.WriteLine(note.ToString());
                    }
                }

            }
        }
    }

    private static void TestJishoHelper()
    {
        var jisho = new JishoHelper("劇获画", JishoHelper.SearchTag.Kanji);

        // var testFile = $@"{Environment.CurrentDirectory}\test\鬼劇获画 #kanji - Jisho.org.htm";
        // var htmlSrc = IOHelper.FromFile(testFile);
        // var jisho = new JishoHelper().SetHtmlSource(htmlSrc);

        foreach (var kanji in jisho.Kanjis)
            Console.WriteLine(kanji);
    }
}