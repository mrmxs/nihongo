using System.Text.RegularExpressions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace Nihongo.App.Helpers;

public class KanjiVGHelper
{
	public KanjiVGHelper(string kanjivgUrl)
	{
		Url = kanjivgUrl;
		SVG = GetSVG(Url);
		WritingDiagram = GetStrokesDiagram(SVG);
	}

	public string Url { get; internal set; }
	public string SVG { get; internal set; }
	public HtmlNode WritingDiagram { get; internal set; }

	public static string GetSVG(string url)
	{
		return GetHelper.FromUrl(url);
	}

	public static HtmlNode GetStrokesDiagram(string kanjiVG)
	{
		try
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(kanjiVG);

			var kvgNode = doc.DocumentNode;

			var id = Regex.Match( // id="kvg:StrokeNumbers_058eb"
					kvgNode.QuerySelector("[id*='StrokeNumbers']").Id,
					"[^_]+_(.+)",
					RegexOptions.None
				).Groups[1].Value;
			var strokes = int.Parse(
				kvgNode.QuerySelector("[id*='StrokeNumbers'] :last-child").InnerText);

			var diagram = HtmlNode.CreateNode(
$@"<svg
	class='stroke_order_diagram--svg_container_for_{id}'
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
						var path = kvgNode.QuerySelector($"path[id$='{ii}']").Clone();
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
		catch (Exception err)
		{
			throw err;
		}
	}
	public static HtmlNode GetStrokesDiagramByUrl(string url)
	{
		return GetStrokesDiagram(GetSVG(url));
	}
}