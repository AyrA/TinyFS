using System.Diagnostics.CodeAnalysis;
using TinyFSLib;

namespace TinyFS
{
    internal class Arguments
    {
        public const string TinyFsPassEnv = "TINYFS_PASS";

        public ArgumentMode Mode { get; }
        public string? FsFileName { get; }
        public string? FileName { get; }
        public string? ContentName { get; }

        public Arguments(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            if (args.Contains("/?"))
            {
                Mode = ArgumentMode.Help;
                return;
            }
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i].ToUpper();
                var next = args.Length > i + 1 ? args[i + 1] : null;

                switch (arg.ToUpper())
                {
                    case "/T":
                        CheckNext(next);
                        if (FsFileName != null)
                        {
                            throw new ArgumentException("TinyFS File has already been specified");
                        }
                        FsFileName = next;
                        ++i;
                        break;
                    case "/F":
                        CheckNext(next);
                        if (FileName != null)
                        {
                            throw new ArgumentException("File name has already been specified");
                        }
                        FileName = next;
                        ++i;
                        break;
                    case "/C":
                        CheckNext(next);
                        if (ContentName != null)
                        {
                            throw new ArgumentException("Content file name has already been specified");
                        }
                        ContentName = next;
                        ++i;
                        break;
                    case "/A":
                        CheckMode(arg);
                        Mode = ArgumentMode.Add;
                        break;
                    case "/R":
                        CheckMode(arg);
                        Mode = ArgumentMode.Remove;
                        break;
                    case "/S":
                        CheckMode(arg);
                        Mode = ArgumentMode.Save;
                        break;
                    case "/E":
                        CheckMode(arg);
                        RequireEnv(TinyFsPassEnv);
                        Mode = ArgumentMode.Encrypt;
                        break;
                    case "/D":
                        CheckMode(arg);
                        RequireEnv(TinyFsPassEnv);
                        Mode = ArgumentMode.Decrypt;
                        break;
                    default:
                        throw new ArgumentException($"unknown argument: '{arg}'");
                }
            }
            if (Mode == ArgumentMode.None)
            {
                Mode = ArgumentMode.Help;
                FsFileName = ContentName = FileName = null;
            }
            switch (Mode)
            {
                case ArgumentMode.None:
                case ArgumentMode.Help:
                    break;
                case ArgumentMode.Add:
                case ArgumentMode.Save:
                    Require(FsFileName, "/T");
                    Require(FileName, "/F");
                    Require(ContentName, "/C");
                    break;
                case ArgumentMode.Remove:
                    Require(FsFileName, "/T");
                    Require(FileName, "/F");
                    Forbidden(ContentName, "/C");
                    break;
                case ArgumentMode.Encrypt:
                case ArgumentMode.Decrypt:
                    Require(FsFileName, "/T");
                    Forbidden(FileName, "/F");
                    Forbidden(ContentName, "/C");
                    break;
            }
        }

        private static void RequireEnv(string envName)
        {
            if (string.IsNullOrEmpty(envName))
            {
                throw new ArgumentException($"'{nameof(envName)}' cannot be null or empty.", nameof(envName));
            }
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(envName)))
            {
                throw new Exception($"The environment variable '{envName}' is not defined or empty");
            }
        }

        private static string GetPassword()
        {
            RequireEnv(TinyFsPassEnv);
            return Environment.GetEnvironmentVariable(TinyFsPassEnv)!;
        }

        private static void Require([NotNull] string? name, string arg)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception($"{arg} must be specified with a file name in this mode");
            }
        }

        private static void Forbidden(string? name, string arg)
        {
            if (name != null)
            {
                throw new ArgumentException($"{arg} must not be specified in this mode");
            }
        }

        private static void CheckNext(string? next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
        }

        private void CheckMode(string arg)
        {
            if (Mode != ArgumentMode.None)
            {
                throw new ArgumentException($"Mode has already been set when parsing '{arg}'");
            }
        }

        public void ExecuteMode()
        {
            FS fileSystem;
            switch (Mode)
            {
                case ArgumentMode.None:
                case ArgumentMode.Help:
                    ShowHelp();
                    break;

                case ArgumentMode.Add:
                    Require(FsFileName, "");
                    Require(FileName, "");
                    Require(ContentName, "");
                    fileSystem = new FS(FsFileName);
                    fileSystem.SetFile(FileName, File.ReadAllBytes(ContentName));
                    fileSystem.Write(FsFileName);
                    break;

                case ArgumentMode.Save:
                    Require(FsFileName, "");
                    Require(FileName, "");
                    Require(ContentName, "");
                    fileSystem = new FS(FsFileName);
                    File.WriteAllBytes(ContentName, fileSystem.GetFile(FileName).Data);
                    break;
                case ArgumentMode.Remove:
                    Require(FsFileName, "");
                    Require(FileName, "");
                    fileSystem = new FS(FsFileName);
                    fileSystem.DeleteFile(FileName);
                    fileSystem.Write(FsFileName);
                    break;

                case ArgumentMode.Encrypt:
                    Require(FsFileName, "");
                    RequireEnv(TinyFsPassEnv);
                    fileSystem = new FS(FsFileName)
                    {
                        IsEncrypted = true
                    };
                    fileSystem.Write(FsFileName, GetPassword());
                    break;
                case ArgumentMode.Decrypt:
                    Require(FsFileName, "");
                    RequireEnv(TinyFsPassEnv);
                    fileSystem = new FS(FsFileName, GetPassword())
                    {
                        IsEncrypted = false
                    };
                    fileSystem.Write(FsFileName);
                    break;
                default:
                    throw new NotImplementedException($"'{Mode}' is not implemented");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine(@"TinyFS /{{A|R|S|E|D}} /T <tinyFile> /F <fileName> [/C <contentFile>]
Creates and updates TinyFS files

Modes:
/A   Add a file or update existing file
/R   Remove a file
/S   Save data to file
/E   Encrypt TinyFS file in-place
/D   Decrypt TinyFS file in-place

/T   TinyFS file. Will be created in mode /A if not exists
/F   File entry in TinyFS file
/C   File containing file content

Mode /A:
Contents of file specified with /C will be added to the file specified with /T
and stored internally under the name /F

Mode /R:
The file specified with /F will be removed from TinyFS file specified with /T.
Attempting to remove the last file will return an error.

Mode /S:
Saves data from the TinyFS file /T stored under the name /F into the file
specified with /C. The file will be overwritten

Mode /E:
Encrypt TinyFS file in-place.
The password must be specified via environment variable {0}

Mode /D:
Decrypt TinyFS file in-place.
The password must be specified via environment variable {0}
", TinyFsPassEnv);
        }
    }
}
