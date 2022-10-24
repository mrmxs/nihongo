using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Nihongo.Core.Models;

namespace Nihongo.App.Helpers;

public class JishoHelper
{
    // Basic Kanji Information
    // https://jisho.org/search/劇获画%23kanji
    // todo Strokes order
    // http://kanjivg.tagaini.net
    // https://d1w6u4xc3l95km.cloudfront.net/kanji-2015-03/09b3c.svg

    private HtmlNode _htmlNode;

    public JishoHelper(string htmlSrc)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlSrc);
        _htmlNode = htmlDocument.DocumentNode;

        var kanjiDetails = _htmlNode.QuerySelectorAll(".kanji.details");

        Kanjis = kanjiDetails
            .Select(p =>
            {
                var k = p.QuerySelector(".character").InnerHtml;
                var m = p.QuerySelector(".kanji-details__main-meanings").InnerText.Trim();
                var kun = p.QuerySelectorAll(".kun_yomi .kanji-details__main-readings-list a")?
                    .Select(o => o.InnerHtml).ToArray();
                var on = p.QuerySelectorAll(".on_yomi .kanji-details__main-readings-list a")?
                    .Select(o => o.InnerHtml).ToArray();
                var g = p.QuerySelector(".grade strong")?.InnerHtml
                    .Replace("grade ", "")
                    .Replace("junior high", "H");
                var j = p.QuerySelector(".jlpt strong")?.InnerHtml;
                var f = p.QuerySelector(".frequency strong")?.InnerHtml;
                var s = p.QuerySelector(".stroke_order_diagram--outer_container").OuterHtml;

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
}