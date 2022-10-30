namespace Nihongo.Console;

using Nihongo.App.Helpers;
using Nihongo.Test;
using Anki = Nihongo.App.Helpers.Anki;
using Microsoft.Extensions.Configuration;
using System;

public class Program
{
    private static IConfiguration _config;

    public static void Main(string[] args)
    {
        _config = new ConfigurationBuilder()
			.AddJsonFile("config.json", optional: false, reloadOnChange: true)
			.Build();
        TestGenerateAnkiImportFile.TestCheckAllJuniorHighKanjiExists();
        TestGenerateAnkiImportFile.TestJishoHelper();
        // GenerateAnkiImportFile();
    }

    private static void GenerateAnkiImportFile()
    {
        var _joyo2020Src = $@"{Environment.CurrentDirectory}\{_config["joyo"]}";
        var joyo = IOHelper.FromFile(_joyo2020Src).Split("\n");

        foreach (var grade in joyo)
        {
            var path = Anki.Importing.OutputPathJoyo(Array.IndexOf(joyo, grade) + 1);
            using (StreamWriter file = new(path))
            {
                Anki.Importing.FileHeader(Anki.Deck.KanjiWriting, Anki.NoteType.KanjiWriting)
                    .ForEach(line => file.WriteLine(line));

                // jisho search page contains max 10 kanjis
                foreach (var ten in grade.Chunk(10))
                {
                    var jisho = new JishoHelper(new string(ten), JishoHelper.SearchTag.Kanji);
                    jisho.Kanjis?
                        .Select(k => new JishoAnkiKanjiConverter().Convert(k))
                        .ToList().ForEach(note =>
                        {
                            file.WriteLine(note.ToString());
                            Console.WriteLine(note.ToString());
                        });
                }
            }
        }
    }
}