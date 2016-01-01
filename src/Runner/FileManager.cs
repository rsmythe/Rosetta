﻿/// <summary>
/// FileManager.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.Runner
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Collections.Generic;
    using System.Collections;

    /// <summary>
    /// Handles files. Reading, writing and general handling.
    /// </summary>
    /// <remarks>
    /// This class is not responsible for discvering files to convert. 
    /// It simply allows us to specify which files to convert, how to convert 
    /// them and where to write output files.
    /// </remarks>
    internal class FileManager : IEnumerable<FileManager.FileEntry>
    {
        // The paths of files to convert and conversions
        private IEnumerable<FileEntry> fileEntries;

        // The path to output directory
        private readonly string directory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath"></param>
        public FileManager(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (!IsDirectoryPathCorrect(directoryPath))
            {
                throw new ArgumentException("Invalid path!", nameof(directoryPath));
            }

            this.directory = directoryPath;
            this.FileConversionProvider = input => string.Empty;

            this.fileEntries = new List<FileEntry>();
        }

        /// <summary>
        /// Gets or sets the conversion provider.
        /// </summary>
        public ConversionProvider FileConversionProvider { get; set; }

        /// <summary>
        /// Gets the directory path.
        /// </summary>
        public string DirectoryPath
        {
            get { return this.directory; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<FileEntry> GetEnumerator()
        {
            return this.fileEntries.GetEnumerator();
        }

        /// <summary>
        /// Gets the list of files.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Files()
        {
            return this.fileEntries.Select(entry => entry.FilePath);
        }

        /// <summary>
        /// Gets the list of file conversions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> FileConversions()
        {
            return this.fileEntries.Select(entry => entry.FileConversion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.fileEntries.GetEnumerator();
        }

        /// <summary>
        /// Adds a file.
        /// </summary>
        /// <param name="filePath"></param>
        public void AddFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!IsFilePathCorrect(filePath))
            {
                throw new ArgumentException("Invalid path!", nameof(filePath));
            }

            ((List<FileEntry>)this.fileEntries).Add(new FileEntry()
                {
                    FilePath = filePath,
                    FileConversion = string.Empty
                });
        }

        /// <summary>
        /// Applies <see cref="FileConversionProvider"/> to all files and stores result in the couple.
        /// </summary>
        public void ApplyConversion()
        {
            foreach (var entry in this.fileEntries)
            {
                entry.FileConversion = this.FileConversionProvider(File.ReadAllText(entry.FilePath));
            }
        }

        /// <summary>
        /// Causes <see cref="ApplyConversion"/> to be called and all files to be written to <see cref="DirectoryPath"/>.
        /// </summary>
        public IEnumerable<string> WriteAllFilesToDestination()
        {
            var writtenFiles = new List<string>();

            this.ApplyConversion();

            foreach (var entry in this.fileEntries)
            {
                var destinationFilePath = GetDestinationFilePath(entry.FilePath);
                WriteToFile(entry.FileConversion, destinationFilePath);
                writtenFiles.Add(destinationFilePath);
            }

            return writtenFiles;
        }

        #region Utilities
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="path"></param>
        public static void WriteToFile(string output, string path)
        {
            File.WriteAllText(path, output);
        }

        /// <summary>
        /// 
        /// </summary>
        public static string ApplicationExecutingPath
        {
            get
            {
                // To get the location the assembly is executing from
                string executingPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                return Path.GetDirectoryName(executingPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string ApplicationAssemblyPath
        {
            get
            {
                // To get the location the assembly normally resides on disk or the install directory
                string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                return Path.GetDirectoryName(assemblyPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectoryPathCorrect(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return Directory.Exists(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFilePathCorrect(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return File.Exists(path);
        }

        private static IEnumerable<string> GetAllFiles(string path)
        {
            if (path == null || path == string.Empty)
            {
                return new string[0];
            }

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(path));

            foreach (string directory in Directory.GetDirectories(path))
            {
                files.AddRange(GetAllFiles(directory));
            }

            return files;
        }

        #endregion

        #region Types

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public delegate string ConversionProvider(string source);

        /// <summary>
        /// 
        /// </summary>
        public sealed class FileEntry
        {
            /// <summary>
            /// 
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string FileConversion { get; set; }
        }

        #endregion

        /// <summary>
        /// Strips the file name from the path and concatenates it to <see cref="DirectoryPath"/>.
        /// Also, different extension is applied.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private string GetDestinationFilePath(string filepath)
        {
            return Path.Combine(directory, Path.GetFileName(Path.ChangeExtension(filepath, "ts")));
        }
    }
}
