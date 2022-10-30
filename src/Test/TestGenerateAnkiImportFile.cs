using Nihongo.App.Helpers;

namespace Nihongo.Test;

public class TestGenerateAnkiImportFile
{
	public static void TestCheckAllJuniorHighKanjiExists()
	{
		var list = IOHelper.FromFile(@"");
		var notes = IOHelper.FromFile(@"");

		foreach (var kanji in list)
		{
			if (!notes.Contains(kanji))
				Console.WriteLine($"{kanji}\tNOT FOUND");
		}
	}

	public static void TestJishoHelper()
	{
		var jisho = new JishoHelper("劇获画", JishoHelper.SearchTag.Kanji);

		// var testFile = $@"{Environment.CurrentDirectory}\test\鬼劇获画 #kanji - Jisho.org.htm";
		// var htmlSrc = IOHelper.FromFile(testFile);
		// var jisho = new JishoHelper().SetHtmlSource(htmlSrc);

		foreach (var kanji in jisho.Kanjis)
			Console.WriteLine(kanji);
	}
}
