namespace FileNamer
{
    using System;
    using System.IO;

    internal class Program
    {
        private static bool ShouldParseSubFolders { get; set; }
        private static string DirectoryToParse { get; set; }
        private static bool DeleteRenamed { get; set; }
        private static int FoldersChecked { get; set; }
        private static int FilesChecked { get; set; }
        private static int FilesRenamed { get; set; }
        private static bool SafeMode { get; set; }
        private static string ContentToReplaceWith { get; set; }
        private static string ContentToReplace { get; set; }

        private static void Main(string[] args)
        {
            if (!ParseArgs(args))
            {
                ShowHelp();
                return;
            }

            if (!Directory.Exists(DirectoryToParse))
            {
                Console.WriteLine($"The directory '{DirectoryToParse}' can't be found.");
                return;
            }

            var root = new DirectoryInfo(DirectoryToParse);
            ParseContents(root, ContentToReplace, ContentToReplaceWith);

            Console.WriteLine();
            Console.WriteLine($"Parsed {FoldersChecked} folders, and {FilesChecked} files - renaming {FilesRenamed} of them.");
        }

        private static void ParseContents(DirectoryInfo root, string contentToLookFor, string replacementContent)
        {
            FoldersChecked++;
            Console.WriteLine("Checking " + root.Name);
            foreach (var file in root.GetFiles())
            {
                FilesChecked++;
                if (file.Name.Contains(contentToLookFor))
                {
                    var newFilename = FindNewFileName(file.FullName.Replace(contentToLookFor, replacementContent));
                    try
                    {
                        if (!SafeMode)
                            File.Copy($@"\\?\{file.FullName}", $@"\\?\{newFilename}");
                        FilesRenamed++;
                        Console.WriteLine("'" + file.FullName + "'");
                        Console.WriteLine($"\tRenamed to: '{newFilename}'");
                        if (DeleteRenamed && !SafeMode)
                        {
                            try
                            {
                                file.Delete();
                            }
                            catch
                            {
                                Console.WriteLine($"\tCouldn't delete '{file.FullName}' - maybe open in another application?");
                            }
                        }
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Console.WriteLine($"**ERROR** -- Unable to move {file.FullName} - maybe too long? In use?");
                    }
                }
            }

            if (ShouldParseSubFolders)
                foreach (var folder in root.GetDirectories())
                    ParseContents(folder, contentToLookFor, replacementContent);
        }

        private static string FindNewFileName(string filename)
        {
            if (!File.Exists(filename))
                return filename;

            var fi = new FileInfo(filename);
            var nameWithoutExtension = fi.FullName.Remove(fi.FullName.Length - fi.Extension.Length, fi.Extension.Length);

            ;

            for (var i = 1; i < int.MaxValue; i++)
            {
                var newFilename = nameWithoutExtension + "." + i + fi.Extension;
                if (!File.Exists(newFilename))
                    return newFilename;
            }

            throw new IOException("Unable to write any file as all the file names have been used up!! (Which is a lot!)");
        }

        private static bool ParseArgs(string[] args)
        {
            var ok = true;
            for (var i = 0; i < args.Length; i++)
                switch (args[i].ToLowerInvariant())
                {
                    case "--s":
                    case "--sub":
                        ShouldParseSubFolders = true;
                        break;
                    case "--d":
                    case "--dir":
                        i++;
                        DirectoryToParse = args[i];
                        break;
                    case "--r":
                    case "--remove":
                        DeleteRenamed = true;
                        break;
                    case "--oc":
                    case "--oldchars":
                        i++;
                        ContentToReplace = args[i];
                        break;
                    case "--nc":
                    case "--newchars":
                        i++;
                        ContentToReplaceWith = args[i];
                        break;
                    case "--safe-mode":
                        SafeMode = true;
                        break;
                    default:
                        ok = false;
                        break;
                }

            return
                !string.IsNullOrWhiteSpace(DirectoryToParse)
                && !string.IsNullOrWhiteSpace(ContentToReplace)
                && !string.IsNullOrWhiteSpace(ContentToReplaceWith)
                && ok;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("FileNamer.exe --d <DIRECTORY> --oc <OLD_CHARACTERS> --nc <NEW_CHARACTERS> [--s] [--r] [--safe-mode]");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("\tFileNamer.exe --d c:\\temp --oc ┬ú --nc GBP --s --r");
            Console.WriteLine();
            Console.WriteLine("\t--d        : The root directory to rename files from.");
            Console.WriteLine("\t--oc       : The Old Characters to replace.");
            Console.WriteLine("\t--nc       : The New Characters to replace the Old Characters with.");
            Console.WriteLine("\t--s        : [OPTIONAL] If defined, FileNamer will parse the sub folders as well.");
            Console.WriteLine("\t--r        : [OPTIONAL] If defined, FileNamer will remove the original file.");
            Console.WriteLine("\t--safe-mode: [OPTIONAL] If defined, FileNamer won't actually do anything, just show you what it would do.");
        }
    }
}