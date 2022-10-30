namespace Nihongo.Core.Models;

public class AnkiKanjiCard
{
	public string Kanji { get; set; }
	public string Readings { get; set; }
	public string Meaning { get; set; }
	public string StrokeOrder { get; set; }
	public string Grade { get; set; }
	public string JLPT { get; set; }
	public string Frequency { get; set; }

	public override string ToString()
    {
        //  Example: 
        // 風	Kun: かぜ、 かざ-  <br>On: フウ、 フ	wind, air, style, manner	"<div class=""stroke_order_diagram--outer_container""></div>"	2		558
        return $"{Kanji}\t{Readings}\t{Meaning}\t\"{StrokeOrder}\"\t{(Grade ?? "")}\t{(JLPT ?? "")}\t{(Frequency ?? "")}";
	}
}