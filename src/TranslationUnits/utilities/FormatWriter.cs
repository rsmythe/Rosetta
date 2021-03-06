﻿/// <summary>
/// FormatWriter.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.Translation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper for generating output.
    /// </summary>
    public class FormatWriter
    {
        private FormatOptions options;
        private IFormatter formatter;
        private StringWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatWriter"/> class.
        /// </summary>
        /// <param name="options"></param>
        public FormatWriter(FormatOptions options = null)
        {
            this.formatter = null;
            this.writer = new StringWriter();
            this.options = options ?? new FormatOptions();
        }

        /// <summary>
        /// Gets or sets the formatter being used.
        /// </summary>
        /// <remarks>
        /// This will cause the internal writer to be reset.
        /// </remarks>
        public IFormatter Formatter
        {
            get
            {
                if (this.formatter == null)
                {
                    this.formatter = new WhiteSpaceFormatter();
                    this.writer = new StringWriter();
                }

                return this.formatter;
            }

            set { this.formatter = value; }
        }

        /// <summary>
        /// Writes a line by replacing the syntax for a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="format">The format pattern.</param>
        /// <param name="arg">The replacing arguments.</param>
        public void WriteLine(string format, params string[] arg)
        {
            this.writer.WriteLine(this.GetStringToWrite(format, arg));
        }

        /// <summary>
        /// Writes by replacing the syntax for a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="format">The format pattern.</param>
        /// <param name="preprocessor">The action to perform before writing and after applying placeholders.</param>
        /// <param name="arg">The replacing arguments.</param>
        /// <remarks>
        /// We must ensure that <see cref="preprocessor"/> is executed before <see cref="formatter"/> is called. In
        /// fact, formatter will add spaces, possibly, and this formatting would be removed if the post processor
        /// removes spaces.
        /// </remarks>
        public void WriteLine(string format, Func<string, string> preprocessor, params string[] arg)
        {
            this.writer.WriteLine(this.GetStringToWrite(format, preprocessor, arg));
        }

        /// <summary>
        /// Writes a line by replacing the syntax for a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="format">The format pattern.</param>
        /// <param name="arg">The replacing arguments.</param>
        public void Write(string format, params string[] arg)
        {
            this.writer.Write(this.GetStringToWrite(format, arg));
        }

        /// <summary>
        /// Writes by replacing the syntax for a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="format">The format pattern.</param>
        /// <param name="preprocessor">The action to perform before writing and after applying placeholders.</param>
        /// <param name="arg">The replacing arguments.</param>
        /// <remarks>
        /// We must ensure that <see cref="preprocessor"/> is executed before <see cref="formatter"/> is called. In
        /// fact, formatter will add spaces, possibly, and this formatting would be removed if the post processor
        /// removes spaces.
        /// </remarks>
        public void Write(string format, Func<string, string> preprocessor, params string[] arg)
        {
            this.writer.Write(this.GetStringToWrite(format, preprocessor, arg));
        }

        private string GetStringToWrite(string format, params string[] arg)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            string escapedProcessedString = "";

            // Argument `format` will contain a format-pattern
            // thus, it is possible that the formatting fails because of curly brackets
            // like in `public void MyMethod() { /* something */ }`
            //                                 +- This bracket   +
            //                                                   |- And this!
            //
            // Spaces must be propagated across multiple lines
            if (arg.Length > 0)
            {
                //original unchanged path
                escapedProcessedString = string.Format(EscapeInputFormat(format), arg);
            }
            else
            {
                //created this path when the file being converted contains a string.format, which was causing the original path to error out.
                escapedProcessedString = format;
            }
            
            var lines = GetAllLinesInString(escapedProcessedString);
            var formattedLines = lines.Select(item => this.formatter.FormatLine(item)).ToArray();
            var singleLine = AllLinesIntoOneString(formattedLines);

            return singleLine;
        }

        private string GetStringToWrite(string format, Func<string, string> preprocessor, params string[] arg)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }
            if (preprocessor == null)
            {
                throw new ArgumentNullException(nameof(preprocessor));
            }

            StringWriter writer = new StringWriter();

            // Argument `format` will contain a format-pattern
            // thus, it is possible that the formatting fails because of curly brackets
            // like in `public void MyMethod() { /* something */ }`
            //                                 +- This bracket   +
            //                                                   |- And this!
            //
            // Spaces must be propagated across multiple lines
            var escapedProcessedString = string.Format(EscapeInputFormat(format), arg);
            var preprocessedString = preprocessor(escapedProcessedString);
            var lines = GetAllLinesInString(preprocessedString);
            var formattedLines = lines.Select(item => this.formatter.FormatLine(item)).ToArray();
            var singleLine = AllLinesIntoOneString(formattedLines);

            writer.Write(singleLine);
            return writer.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var output = this.writer.ToString();

            if (!options.ShouldWriteLastBlankLine)
            {
                output = RemoveLastBlank(output);
            }

            return output;
        }
        
        private static string RemoveLastBlank(string source)
        {
            var lines = GetAllLinesInString(source);

            if (lines.Length <= 1)
            {
                return source;
            }

            if (lines[lines.Length - 1].Length > 0)
            {
                return source;
            }

            return AllLinesIntoOneString(lines, lines.Length - 1);
        }

        private static string[] GetAllLinesInString(string source)
        {
            return source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }

        private static string AllLinesIntoOneString(string[] lines)
        {
            return AllLinesIntoOneString(lines, lines.Length);
        }

        private static string AllLinesIntoOneString(string[] lines, int limit)
        {
            string output = string.Empty;
            for (int i = 0; i < limit; i++)
            {
                output += lines[i] + (i == limit - 1 ? string.Empty : Environment.NewLine);
            }

            return output;
        }

        /// <summary>
        /// Escapes the input string.
        /// 
        /// TODO: At the moment repetitive placeholders are not supported. 
        /// Thus a stirng like `{1}{2}{1}` will not be correctly escaped. Fix this!
        /// </summary>
        /// <param name="inputFormat"></param>
        /// <returns></returns>
        private static string EscapeInputFormat(string inputFormat)
        {
            Regex regex = new Regex(@"\{\d+\}"); // A `{` followed by a number and a `}`
            string result = inputFormat;

            MatchCollection matches = regex.Matches(inputFormat);
            List<string> parts = new List<string>();
            parts.Add(inputFormat);

            foreach (Match match in matches)
            {
                parts.AddRange(parts.Last().Split(new string[] { match.Value }, StringSplitOptions.None));
            }

            // We might have no placeholders, thus parts contains one element only 
            // which we must escape before returning
            if (parts.Count == 1)
            {
                return EscapeCleanString(parts[0]);
            }

            // Remove first and last
            parts.RemoveAt(0);
            parts.RemoveAt(parts.Count - 1);
            // Remove all even positions (in even positions we have correct 
            // placeholders that should not be escaped)
            parts = parts.Where((c, i) => i % 2 == 0).ToList();

            // Escape each element in the sequence
            List<string> escapedParts = new List<string>();

            foreach (string part in parts)
            {
                escapedParts.Add(EscapeCleanString(part));
            }

            // Aggregate back
            string aggregate = escapedParts[0];

            for (int i = 0; i < matches.Count - 1; i++)
            {
                aggregate += matches[i].Value + escapedParts[i + 1];
            }
            aggregate += matches[matches.Count - 1].Value;

            return aggregate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cleanString"></param>
        /// <returns></returns>
        public static string EscapeCleanString(string cleanString)
        {
            return cleanString.Replace("{", "{{").Replace("}", "}}");
        }
    }
}
