using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using JetBrains.Annotations;
using LambdaConverters.Deployment.Properties;

namespace LambdaConverters.Deployment
{
    internal static class Program
    {
        static int Main([NotNull] string[] args)
        {
            try
            {
                string assemblyPath45;
                string assemblyPath46;
                bool isReleaseBuild;
                string nugetPath;
                string nuspecPath;
                string nuspecAnnotationsPath;
                GetPaths(out assemblyPath45, out assemblyPath46, out isReleaseBuild, out nugetPath, out nuspecPath, out nuspecAnnotationsPath);

                if (isReleaseBuild)
                {
                    if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                    {
                        throw new ArgumentException("SNK file not specified.");
                    }

                    var snkPath = args[0];
                    ResignAssembly(assemblyPath45, assemblyPath46, snkPath);
                }

                string packageFileName;
                UpdateNuspec(assemblyPath45, assemblyPath46, nuspecPath, out packageFileName);

                BuildPackages(nugetPath, nuspecPath, nuspecAnnotationsPath);

                OpenInWindowsExplorer(nuspecPath, packageFileName);

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e);
                return -1;
            }
            finally
            {
                Console.WriteLine("\nPress ENTER to exit.");
                Console.ReadLine();
            }
        }

        [Pure]
        static void GetPaths(
            [NotNull] out string assemblyPath45,
            [NotNull] out string assemblyPath46,
            out bool isReleaseBuild,
            [NotNull] out string nugetPath,
            [NotNull] out string nuspecPath,
            [NotNull] out string nuspecAnnotationsPath)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            Debug.Assert(entryAssembly != null);

            var directoryPath = Path.GetDirectoryName(entryAssembly.Location);
            Debug.Assert(directoryPath != null);
            var executionDirectoryPath = directoryPath;

            var executionDirectory = Path.GetFileName(executionDirectoryPath);
            isReleaseBuild = string.Equals(executionDirectory, "release", StringComparison.OrdinalIgnoreCase);

            var solutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(executionDirectoryPath)));

            nugetPath = Path.Combine(solutionDirectory, "NuGet.exe");

            assemblyPath45 = Path.Combine(solutionDirectory, "LambdaConverters.Wpf", "bin", executionDirectory, "net45", "LambdaConverters.Wpf.dll");

            assemblyPath46 = Path.Combine(solutionDirectory, "LambdaConverters.Wpf", "bin", executionDirectory, "net46", "LambdaConverters.Wpf.dll");

            nuspecPath = Path.Combine(executionDirectoryPath, "LambdaConverters.nuspec");

            nuspecAnnotationsPath = Path.Combine(executionDirectoryPath, "LambdaConverters.Annotations.nuspec");
        }

        static void ResignAssembly([NotNull] string assemblyPath45, string assemblyPath46, [NotNull] string snkPath)
        {
            Console.WriteLine("Resigning assembly...");

            Debug.Assert(Settings.Default != null);

            Console.WriteLine($"Tool path: {Settings.Default.SnPath}");

            RunConsoleApplication($"\"{Settings.Default.SnPath}\"", $"-R \"{assemblyPath45}\" \"{snkPath}\"");

            RunConsoleApplication($"\"{Settings.Default.SnPath}\"", $"-R \"{assemblyPath46}\" \"{snkPath}\"");
        }

        static void UpdateNuspec([NotNull] string assemblyPath45, string assemblyPath46, [NotNull] string nuspecPath, [NotNull] out string packageFileName)
        {
            Console.Write("Updating nuspec...");

            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath45);
            Debug.Assert(assembly != null);

            var nuspec = XDocument.Load(nuspecPath);
            Debug.Assert(nuspec.Root != null);

            var metadataElement = nuspec.Root.Element("metadata");
            Debug.Assert(metadataElement != null);

            var versionElement = metadataElement.Element("version");
            var fileVersionAttributeData = assembly.GetCustomAttributesData()?.First(a => a.AttributeType == typeof(AssemblyFileVersionAttribute));
            Debug.Assert(versionElement != null);
            Debug.Assert(fileVersionAttributeData != null);
            Debug.Assert(fileVersionAttributeData.ConstructorArguments[0].Value is string);
            //versionElement.Value = (string)fileVersionAttributeData.ConstructorArguments[0].Value;

            const string targetNet45 = @"lib\net45";
            const string targetNet46 = @"lib\net46";
            nuspec.Root.Element("files")?
                .Add(
                    new XElement("file", new XAttribute("src", assemblyPath45), new XAttribute("target", targetNet45)),
                    new XElement("file", new XAttribute("src", Path.ChangeExtension(assemblyPath45, "pdb")), new XAttribute("target", targetNet45)),
                    new XElement("file", new XAttribute("src", Path.ChangeExtension(assemblyPath45, "xml")), new XAttribute("target", targetNet45)),
                    new XElement("file", new XAttribute("src", assemblyPath46), new XAttribute("target", targetNet46)),
                    new XElement("file", new XAttribute("src", Path.ChangeExtension(assemblyPath46, "pdb")), new XAttribute("target", targetNet46)),
                    new XElement("file", new XAttribute("src", Path.ChangeExtension(assemblyPath46, "xml")), new XAttribute("target", targetNet46)));

            packageFileName = $"{(string)metadataElement.Element("id")}.{(string)metadataElement.Element("version")}.nupkg";

            nuspec.Save(nuspecPath);

            Console.WriteLine("done");
        }

        static void BuildPackages([NotNull] string nugetPath, [NotNull] [ItemNotNull] params string[] nuspecPaths)
        {
            Console.WriteLine("Building package...");

            foreach (var nuspecPath in nuspecPaths)
            {
                RunConsoleApplication(
                    $"\"{nugetPath}\"",
                    $"pack \"{nuspecPath}\" -OutputDirectory \"{Path.GetDirectoryName(nuspecPath)}\" -NoPackageAnalysis -Verbosity detailed");
            }
        }

        static void RunConsoleApplication([NotNull] string executablePath, [NotNull] string arguments)
        {
            using (
                var process = new Process
                {
                    StartInfo =
                        new ProcessStartInfo
                        {
                            FileName = executablePath,
                            Arguments = arguments,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                        },
                    EnableRaisingEvents = true,
                })
            {
                process.OutputDataReceived += Process_DataReceived;
                process.ErrorDataReceived += Process_DataReceived;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Console application exited with the code {process.ExitCode}.");
                }
            }
        }

        static void Process_DataReceived(object sender, [NotNull] DataReceivedEventArgs e) => Console.WriteLine("    " + e.Data);

        static void OpenInWindowsExplorer([NotNull] string nuspecPath, [NotNull] string packageFileName)
        {
            var nuspecDirectoryPath = Path.GetDirectoryName(nuspecPath);

            using (Process.Start("explorer", "/select, \"" + Path.Combine(nuspecDirectoryPath, packageFileName) + "\"")) { }
        }
    }
}
