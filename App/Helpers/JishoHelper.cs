using System.Text;
using System.Reflection.Metadata;
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
    private Dictionary<string, string> _svgSrc;

    public JishoHelper(string htmlSrc)
    {
        _htmlNode = DocNode(htmlSrc);
        _svgSrc = SvgSrcParser();

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

    private HtmlNode DocNode(string src)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(src);

        return doc.DocumentNode;
    }

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

        var kanjiId = container.Element("svg")
            .Attributes.First(a => a.Name == "class").Value
            .Replace("stroke_order_diagram--svg_container_for_", "");

        var kanjivgPath = _svgSrc[kanjiId];
        var kanjivgSrc = GetHelper.FromUrl(kanjivgPath);
        var kanjivg = DocNode(kanjivgSrc);

        return SVGToDiagramConverter(kanjiId, kanjivg).OuterHtml;
    }

    private HtmlNode SVGToDiagramConverter(string kanjiId, HtmlNode kanjivg) //mb xml, mb svg
    {
        var id = Regex.Match( // id="kvg:StrokeNumbers_058eb"
                kanjivg.QuerySelector("[id*='StrokeNumbers']").Id,
                "[^_]+_(.+)",
                RegexOptions.None
            ).Groups[1].Value;
        var strokes = int.Parse(
            kanjivg.QuerySelector("[id*='StrokeNumbers'] :last-child").InnerText);

        var diagram = HtmlNode.CreateNode(
$@"<svg
    class='stroke_order_diagram--svg_container_for_{kanjiId}'
    style='height: 100px; width: {strokes * 100}px;'
    viewBox='0 0 {strokes * 100} 100'>
</svg>");

        for (int i = 0; i <= strokes; i++)
        {
            if (0 == i) // add borderlines
            {
                diagram.AppendChild(HtmlNode.CreateNode(
$@"<g id='{id}_borders'>
    <line x1='1' x2='{strokes * 100 - 1}' y1='1'  y2='1'  class='stroke_order_diagram--bounding_box'></line>
    <line x1='1' x2='1'                   y1='1'  y2='99' class='stroke_order_diagram--bounding_box'></line>
    <line x1='1' x2='{strokes * 100 - 1}' y1='99' y2='99' class='stroke_order_diagram--bounding_box'></line>
    <line x1='0' x2='{strokes * 100}'     y1='50' y2='50' class='stroke_order_diagram--guide_line'></line>
</g>"));
            }
            else
            {
                // add stroke group
                var strokeNode = HtmlNode.CreateNode(
$@"<g id='{id}_{i}'>
    <line x1='{i * 100 - 50}' x2='{i * 100 - 50}' y1='1' y2='99' class='stroke_order_diagram--guide_line'></line>
    <line x1='{i * 100 - 1}'  x2='{i * 100 - 1}'  y1='1' y2='99' class='stroke_order_diagram--bounding_box'></line>
</g>");

                for (var ii = 1; ii <= i; ii++)
                {
                    // add stroke path
                    var path = kanjivg.QuerySelector($"path[id$='{ii}']").Clone();
                    path.AddClass(i == ii
                        ? "stroke_order_diagram--current_path"
                        : "stroke_order_diagram--existing_path");
                    path.Attributes.ToList().ForEach(a =>
                        a.QuoteType = AttributeValueQuote.SingleQuote);
                    strokeNode.AppendChild(path);

                    // add path start marker
                    if (i == ii)
                    {
                        var pathStart = Regex.Match(path.OuterHtml, "M([^c]+)c", RegexOptions.None)
                            .Groups[1].Value.Split(',');
                        strokeNode.AppendChild(HtmlNode.CreateNode(
// <circle cx="52.25" cy="17.25" r="4" class="stroke_order_diagram--path_start" transform="matrix(1,0,0,1,96,-4)"></circle>
$"<circle cx='{pathStart[0]}' cy='{pathStart[1]}' r='4' class='stroke_order_diagram--path_start'></circle>"
                        ));
                    }
                }

                // add transformation
                foreach (var node in strokeNode.QuerySelectorAll("path, circle"))
                {
                    node
                        .SetAttributeValue("transform", $"matrix(1, 0, 0, 1, {i * 100 - 104}, -4)")
                        .QuoteType = AttributeValueQuote.SingleQuote;
                }

                diagram.AppendChild(strokeNode);
            }
        }

        diagram.InnerHtml = diagram.InnerHtml.Replace("\n", "");

        return diagram;
    }
}