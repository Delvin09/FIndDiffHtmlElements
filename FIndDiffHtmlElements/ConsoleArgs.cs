using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FIndDiffHtmlElements
{
    internal class ConsoleArgs
    {
        private const string DefaultDirectory = "Samples";
        private const string DefaultTargetElementId = "make-everything-ok-button";

        public ConsoleArgs(string[] args)
        {
            var files = Directory.GetFiles(DefaultDirectory, "*.html");
            OriginalFilePath = files.FirstOrDefault(f => f.Contains("origin"));
            DiffFilePath = files.FirstOrDefault(f => f.Contains("diff"));
            TargetElementId = DefaultTargetElementId;

            if (args == null || args.Length == 0)
                return;

            var argsList = args.ToList();
            GetOption(argsList, "-id", value => TargetElementId = value);
            GetOption(argsList, "-o", value => OriginalFilePath = value);
            GetOption(argsList, "-d", value => DiffFilePath = value);
        }

        private void GetOption(List<string> argsList, string key, Action<string> processAction)
        {
            var idIndex = argsList.FindIndex(i => i.Equals(key, StringComparison.OrdinalIgnoreCase)) + 1;
            if (idIndex < argsList.Count)
            {
                processAction(argsList[idIndex]);
                argsList.RemoveAt(idIndex);
                argsList.RemoveAt(idIndex - 1);
            }
        }

        public bool IsValid =>
            !string.IsNullOrEmpty(OriginalFilePath)
            && !string.IsNullOrEmpty(DiffFilePath)
            && File.Exists(OriginalFilePath)
            && File.Exists(DiffFilePath);

        public string OriginalFilePath { get; private set; }

        public string DiffFilePath { get; private set; }

        public string TargetElementId { get; private set; }
    }
}