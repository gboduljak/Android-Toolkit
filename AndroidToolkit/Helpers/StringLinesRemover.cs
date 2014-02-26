using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidToolkit.Helpers
{
    internal static class StringLinesRemover
    {
        public static string RemoveLine(string input, int numberOfLines)
        {
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return string.Join(Environment.NewLine, lines.Skip(numberOfLines).ToArray()); 
        }
        public static string ForgetLastLine(string input)
        {
            string newText = null;
            List<String> lines = input.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            lines.RemoveAll(str => str.Contains("Android"));
            lines.ForEach(str => newText += str + Environment.NewLine);
            return newText;
        }
    }
}
