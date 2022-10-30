using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Nihongo.Core.Models;

namespace Nihongo.App.Helpers;

/// <summary>
/// Jisho project https://jisho.org/
/// </summary>
public class JishoHelper
{
    public const string DOMEN = "https://jisho.org/";

    private string? _html;
    private HtmlNode? _dom;
    private Dictionary<string, string>? _kanjiVGLinksDic;


    public JishoHelper() { }

    public JishoHelper(string search)
    {
        Search(search);
    }

    public JishoHelper(string search, SearchTag tag)
    {
        Search(search, tag);
    }


    public enum SearchTag { Words, Kanji }

    public IEnumerable<JishoKanji>? Kanjis { get; internal set; }


    /// <summary>
    /// Search at Jisho https://jisho.org/search/大丈夫です
    /// </summary>
    /// <param name="search">search request</param>
    /// <returns>html response</returns>
    public string Search(string search)
    {
        var url = $"{DOMEN}search/{search}";
        var response = IOHelper.FromUrl(url);

        SetHtmlSource(response);

        return response;
    }

    /// <summary>
    /// Kanji search at Jisho https://jisho.org/search/風获劇士#kanji
    /// 获 is chinese character which is not used in Japan so no info on it
    /// </summary>
    /// <param name="search">search request</param>
    /// <param name="tag">categories for search</param>
    /// <returns>html response</returns>
    public string Search(string search, SearchTag tag)
    {
        return Search(search + tag switch
        {
            SearchTag.Words => "%23words",
            SearchTag.Kanji => "%23kanji",
            _ => "",
        });
    }

    public JishoHelper SetHtmlSource(string html)
    {
        _html = html;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        _dom = doc.DocumentNode;

        _kanjiVGLinksDic = SvgSrcParser();

        Kanjis = _dom.QuerySelectorAll(".kanji.details")
            .Select(kd => KanjiParser(kd)).ToList();

        return this;
    }

    private Dictionary<string, string> SvgSrcParser()
    {
        // var url = '//d1w6u4xc3l95km.cloudfront.net/kanji-2015-03/09b3c.svg';
        // var el = $('#kanji_strokes_51866279d5dda796580001ea');

        var pattern = @"'(//.+cloudfront[^']+)'[^']+'(#[^']+)'";
        var matches = Regex.Matches(_dom.InnerHtml, pattern, RegexOptions.None);

        return
            matches?.ToDictionary(
                keySelector: el =>
                    el.Groups[2].Value.Replace("#kanji_strokes_", ""),
                elementSelector: el =>
                    $"https:{el.Groups[1].Value}")

            ?? new Dictionary<string, string>();
    }

    private JishoKanji KanjiParser(HtmlNode kd)
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