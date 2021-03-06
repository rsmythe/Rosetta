﻿/// <summary>
/// SyntaxUtility.cs
/// Andrea Tino - 2016
/// </summary>

namespace Rosetta.ScriptSharp.AST.Helpers
{
    using System;
    using System.Linq;

    /// <summary>
    /// Utility class for tokens.
    /// </summary>
    public static class SyntaxUtility
    {
        /// <summary>
        /// Outputs the name in the correct ScriptSharp convention:
        /// - First letter must be lowercase.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToScriptSharpName(this string name)
        {
            return string.IsNullOrEmpty(name) 
                ? name 
                : name.First().ToString().ToLower() + name.Substring(1);
        }
    }
}
