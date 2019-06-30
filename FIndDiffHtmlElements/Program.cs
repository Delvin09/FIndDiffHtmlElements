using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace FIndDiffHtmlElements
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var consoleArgs = new ConsoleArgs(args);
                if (!consoleArgs.IsValid)
                {
                    Console.WriteLine($"Invalid parameters: {nameof(consoleArgs.OriginalFilePath)} - {consoleArgs.OriginalFilePath};" +
                                      $" {nameof(consoleArgs.DiffFilePath)} - {consoleArgs.DiffFilePath};" +
                                      $" {nameof(consoleArgs.TargetElementId)} - {consoleArgs.TargetElementId}");
                    return;
                }

                var element = GetOriginElement(consoleArgs);
                if (element == null)
                {
                    Console.WriteLine("Can't find origin element");
                }
                else
                {
                    Console.WriteLine("Origin Element: ");
                    ShowElement(element);
                }

                var diffDocument = new HtmlDocument();
                diffDocument.Load(consoleArgs.DiffFilePath);
                var diffElement = diffDocument.GetElementbyId(consoleArgs.TargetElementId) ?? GetDiffElement(diffDocument, element);

                if (diffElement == null)
                    Console.WriteLine("Can't find diff element.");
                else
                {
                    Console.WriteLine("Diff element: ");
                    ShowElement(diffElement);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error trying to find element by id, Message: {0}", ex.Message);
            }
        }

        private static HtmlNode GetDiffElement(HtmlDocument diffDocument, HtmlNode element)
        {
            var allElements = diffDocument.DocumentNode.SelectNodes($"//{element.Name}");
            return allElements?.Select(i => new { Element = i, Rate = CalcRate(element, i) }).OrderByDescending(i => i.Rate).FirstOrDefault()?.Element;
        }

        private static int CalcRate(HtmlNode originElement, HtmlNode diffElement)
        {
            return originElement.Attributes
                .Select(attr => diffElement.Attributes
                    .FirstOrDefault(a => string.Equals(a.Name, attr.Name, StringComparison.OrdinalIgnoreCase)
                                         && string.Equals(a.Value, attr.Value, StringComparison.OrdinalIgnoreCase))).Count(diffAttr => diffAttr != null);
        }

        private static void ShowElement(HtmlNode element)
        {
            Console.WriteLine($"Successfully found. InnerText: {element.InnerText}");
            var attributesData = string.Join(", ",
                element.Attributes.Select(attribute => $"{attribute.Name} = {attribute.Value}"));
            Console.WriteLine(attributesData);
            Console.WriteLine($"Path: {GetElementPath(element)}");
        }

        private static string GetElementPath(HtmlNode element)
        {
            return element.ParentNode == null ? string.Empty : $"{GetElementPath(element.ParentNode)}>{element.Name}";
        }

        private static HtmlNode GetOriginElement(ConsoleArgs consoleArgs)
        {
            var document = new HtmlDocument();
            document.Load(consoleArgs.OriginalFilePath);

            var element = document.GetElementbyId(consoleArgs.TargetElementId);
            if (element == null)
                throw new Exception("Element not found");
            return element;
        }
    }
}
