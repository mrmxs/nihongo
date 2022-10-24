namespace Nihongo.Core.Models;

public class JishoKanji
{
    public string Kanji { get; set; }
    public string Meaning { get; set; }
    public string[]? KunReading { get; set; }
    public string[]? OnReading { get; set; }
    public string? Grade { get; set; }
    public string? JLPT { get; set; }
    public string? Frequency { get; set; }
    public string StrokeOrder { get; set; }


    public override string ToString()
    {
        return $@"Kanji:		{Kanji}
Kun:		{(null == KunReading ? "" : string.Join("、", KunReading))}
On:		{(null == OnReading ? "" : string.Join("、", OnReading))}
Meaning:	{Meaning}
Grade:		{(Grade ?? "")}
JLPT:		{(JLPT ?? "")}
Frequency:	{(Frequency ?? "")}
Stroke order:
{StrokeOrder}
";
    }
}