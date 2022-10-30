using Nihongo.Core.Models;

namespace Nihongo.App.Helpers;

public interface IConverter<TSource, TDestination>
{
	TDestination Convert(TSource source_object);
	TSource Convert(TDestination source_object);
}

public class JishoAnkiKanjiConverter : IConverter<JishoKanji, AnkiKanjiCard>
{
	public AnkiKanjiCard Convert(JishoKanji jishoKanji)
	{
		return new AnkiKanjiCard
		{
			Kanji = jishoKanji.Kanji,
			Readings = $@"{(
				null == jishoKanji.KunReading ? ""
					: $"Kun: {string.Join("、", jishoKanji.KunReading)}"
			)}<br/>{(
				null == jishoKanji.OnReading ? ""
					: $"On: {string.Join("、", jishoKanji.OnReading)}"
			)}",
			Meaning = jishoKanji.Meaning,
			StrokeOrder = $"{jishoKanji.StrokeOrder}",
			Grade = jishoKanji.Grade ?? "",
			JLPT = jishoKanji.JLPT ?? "",
			Frequency = jishoKanji.Frequency ?? "",
		};
	}

	public JishoKanji Convert(AnkiKanjiCard source)
	{
		throw new NotImplementedException();
	}
}