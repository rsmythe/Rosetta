﻿/// <summary>
/// Program.ConvertFile.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.Runner
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;

    using Rosetta.AST;
    
    /// <summary>
    /// Part of program responsible for translating one single file.
    /// </summary>
    internal partial class Program
    {
        protected virtual void ConvertFile()
        {
            this.InitializeForFileConversion();
            this.PrepareFiles();
            this.EmitFiles();
        }
        
        protected virtual void InitializeForFileConversion()
        {
            // Setting output folder
            // Making sure this gets translated into absolute path
            this.outputFolder = this.GetOutputFolderForFile(this.outputFolder);
            
            // Making sure we translate it into absolute path
            // Attention: Path correctness is checked later
            this.filePath = this.GetFilePath(this.filePath);

            // Initializing the file manager
            this.fileManager = new FileManager(this.outputFolder);
            this.fileManager.FileConversionProvider = PerformConversion;
        }
        
        protected virtual void PrepareFiles()
        {
            // This will perform anothe file existing check, redundant as we 
            // do it in initialization routine, but fine!
            fileManager.AddFile(this.filePath, this.fileName);
        }
        
        protected virtual void EmitFiles()
        {
            var writtenFiles = fileManager.WriteAllFilesToDestination();

            foreach (var file in writtenFiles)
            {
                Console.WriteLine("Wrote file {0}", file);
            }
        }

        #region Helpers

        /// <summary>
        /// We proceed in this order:
        /// 1. If no path specified: take the path of the input file.
        /// 2. Otherwise, a path is specified: use that.
        /// </summary>
        /// <param name="userInput">The user input. Can be <code>null</code>.</param>
        /// <returns>The absolute path to the output folder basing on user input.</returns>
        private string GetOutputFolderForFile(string userInput)
        {
            if (userInput != null)
            {
                // User provided a path: check the path is all right
                if (FileManager.IsDirectoryPathCorrect(userInput))
                {
                    return FileManager.GetAbsolutePath(userInput);
                }

                // Wrong path
                throw new InvalidOperationException("Invalid path provided for output folder!");
            }

            // User did not provide a path, we get the path of the input file
            if (FileManager.IsFilePathCorrect(this.filePath))
            {
                return FileManager.ExtractDirectoryPathFromFilePath(this.filePath);
            }

            throw new InvalidOperationException("Invalid path provided for input file!");
        }

        /// <summary>
        /// Checks that the path is OK and also translates into absolute path.
        /// </summary>
        /// <param name="userInput">
        /// The user input. At this point this is supposed not to be <code>null</code>.
        /// </param>
        /// <returns></returns>
        private string GetFilePath(string userInput)
        {
            return FileManager.GetAbsolutePath(userInput);
        }

        #endregion
    }
}
