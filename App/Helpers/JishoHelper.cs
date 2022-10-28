using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Nihongo.Core.Models;

namespace Nihongo.App.Helpers;

public class JishoHelper
{
	// Basic Kanji Information
	// https://jisho.org/search/劇获画%23kanji
	// http://kanjivg.tagaini.net
	// https://d1w6u4xc3l95km.cloudfront.net/kanji-2015-03/09b3c.svg

	private HtmlNode _htmlNode;
	private Dictionary<string, string> _kanjiVGLinksDic;

	public JishoHelper(string htmlSrc)
	{
		var doc = new HtmlDocument();
		doc.LoadHtml(htmlSrc);
		_htmlNode =  doc.DocumentNode;

		_kanjiVGLinksDic = SvgSrcParser();

		var kanjiDetails = _htmlNode.QuerySelectorAll(".kanji.details");

		Kanjis = kanjiDetails
			.Select(kd =>
			{
				var k = kd.QuerySelector(".character").InnerHtml;
				var m = kd.QuerySelector(".kanji-details__main-meanings").InnerText.Trim();
				var kun = kd.QuerySelectorAll(".kun_yomi .kanji-details__main-readings-list a")?
					.Select(r => r.InnerHtml).ToArray();
				var on = kd.QuerySelectorAll(".on_yomi .kanji-details__main-readings-list a")?
					.Select(r => r.InnerHtml).ToArray();
				var g = kd.QuerySelector(".grade strong")?.InnerHtml
					.Replace("grade ", "")
					.Replace("junior high", "H");
				var j = kd.QuerySelector(".jlpt strong")?.InnerHtml;
				var f = kd.QuerySelector(".frequency strong")?.InnerHtml;
				var s = StrokeOrderDiagramGenerator(
					container: kd.QuerySelector(".stroke_order_diagram--outer_container"));

				return new JishoKanji
				{
					Kanji = k,
					Meaning = m,
					KunReading = kun,
					OnReading = on,
					Grade = g,
					JLPT = j,
					Frequency = f,
					StrokeOrder = s
				};
			}).ToList();
	}

	public IEnumerable<JishoKanji> Kanjis { get; internal set; }

	private Dictionary<string, string> SvgSrcParser()
	{
		// var url = '//d1w6u4xc3l95km.cloudfront.net/kanji-2015-03/09b3c.svg';
		// var el = $('#kanji_strokes_51866279d5dda796580001ea');

		var pattern = @"'(//.+cloudfront[^']+)'[^']+'(#[^']+)'";
		var matches = Regex.Matches(_htmlNode.InnerHtml, pattern, RegexOptions.None);

		return
			matches?.ToDictionary(
				keySelector: el =>
					el.Groups[2].Value.Replace("#kanji_strokes_", ""),
				elementSelector: el =>
					$"https:{el.Groups[1].Value}")

			?? new Dictionary<string, string>();
	}

	private string StrokeOrderDiagramGenerator(HtmlNode container)
	{
		// <svg class="stroke_order_diagram--svg_container_for_51866279d5dda796580001ea"
		var diagramClass = container.Element("svg")
			.Attributes.First(a => a.Name == "class").Value;
		var kanjiVGHelper = new KanjiVGHelper(_kanjiVGLinksDic[
			diagramClass
				.Replace("stroke_order_diagram--svg_container_for_", "")
		]);

		var diagram = kanjiVGHelper.WritingDiagram.Clone();
		diagram.SetAttributeValue("class", diagramClass)
			.QuoteType = AttributeValueQuote.SingleQuote;

		return $"<div class='stroke_order_diagram--outer_container'>{diagram.OuterHtml}</div>";
	}
}