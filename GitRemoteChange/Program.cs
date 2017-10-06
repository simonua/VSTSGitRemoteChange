using System;
using System.Diagnostics;
using System.IO;

namespace GitRemoteChange
{
    enum Protocol
    {
        Unknown,
        Https,
        Ssh
    }
    
    class Program
    {
        private const string ACCOUNT = "contoso";
        private const string COLLECTION = "defaultcollection";
        private const string PROJECT = "myteamproject";

        static void Main(string[] args)
        {
            string path = null;
            var protocol = Protocol.Unknown;

            #region Harvest Arguments

            #region Argument 0

            if (args.Length >= 1)
            {
                if (args[0].Contains("basepath="))
                {
                    path = args[0].Split('=')[1]; //extract the development directory path containing the git repos

                    if (!string.IsNullOrEmpty(path))
                    {
                        Console.WriteLine("Using base path {0}", path);
                    }
                }
            }

            #endregion

            #region Argument 1

            if (args.Length >= 2)
            {
                switch (args[1])
                {
                    case "https":
                        Console.WriteLine("Using https");
                        protocol = Protocol.Https;
                        break;
                    case "ssh":
                        Console.WriteLine("Using ssh");
                        protocol = Protocol.Ssh;
                        break;
                }
            }

            #endregion
            
            #endregion

            #region Perform Migration

            if (!string.IsNullOrEmpty(path) && protocol != Protocol.Unknown)
            {
                MigrateOrigins.DoIt(path, protocol);    
            }
            else
            {
                PrintHelp();
            }

            #endregion
        }

        static void PrintHelp()
        {
            Console.Write(
                "\nGit Remote Change V1.1.0" +
                "\n\nChanges the remote \"origin\" url setting in .git/config." +
                "\n\nUsage:\n" +
                "\n   gitremotechange basepath=<path> https|ssh" +
                "\n\nArguments:\n" +
                "\n   basepath=<path>\tUses this base development path (e.g. 'c:\\dev', 'c:\\vso') to convert all git configs" +
                "\n   https|ssh\t\tUse https or ssh"
            );
        }

        public class MigrateOrigins
        {
            public static void DoIt(string directoryWithRepoFolders, Protocol protocol)
            {
                var repoDirectories = new DirectoryInfo(directoryWithRepoFolders).EnumerateDirectories();

                foreach (var repoDirectory in repoDirectories)
                {
                    Console.WriteLine(ExecuteGitMigrateOrigin(directoryWithRepoFolders, repoDirectory.Name, protocol));
                }
            }

            private static string ExecuteGitMigrateOrigin(string parentPath, string repoName, Protocol protocol)
            {
                var insideRepoFolder = Path.Combine(parentPath, repoName);
                var cmd = new Process
                {
                    StartInfo =
                    {
                        FileName = "cmd.exe",
                        WorkingDirectory = insideRepoFolder,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    }
                };
                cmd.Start();

                #region Determine URL
                string url = null;

                switch (protocol)
                {
                    case Protocol.Https:
                        url = $"https://{ACCOUNT}.visualstudio.com/{COLLECTION}/{PROJECT}/_git/{repoName}";
                        break;
                    case Protocol.Ssh:
                        //10/06/17 - SJK: As of November 2017, the ssh URLs will all need to follow this new format as per Microsoft:
                        url = $"ssh://{ACCOUNT}@vs-ssh.visualstudio.com:22/{COLLECTION}/{PROJECT}/_ssh/{repoName}";
                        
                        break;
                }
                #endregion

                var command = $"git remote set-url origin {url}";
                cmd.StandardInput.WriteLine(command);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();

                return cmd.StandardOutput.ReadToEnd();
            }
        }
    }
}