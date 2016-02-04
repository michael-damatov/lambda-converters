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
                string assemblyPath;
                bool isReleaseBuild;
                string nugetPath;
                string nuspecPath;
                string nuspecAnnotationsPath;
                GetPaths(out assemblyPath, out isReleaseBuild, out nugetPath, out nuspecPath, out nuspecAnnotationsPath);

                if (isReleaseBuild)
                {
                    if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                    {
                        throw new ArgumentException("SNK file not specified.");
                    }

                    var snkPath = args[0];
                    ResignAssembly(assemblyPath, snkPath);
                }

                string packageFileName;
                UpdateNuspec(assemblyPath, nuspecPath, out packageFileName);

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
            [NotNull] out string assemblyPath,
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
            Debug.Assert(solutionDirectory != null);

            nugetPath = Path.Combine(solutionDirectory, "NuGet.exe");

            assemblyPath = Path.Combine(solutionDirectory, "LambdaConverters.Wpf", "bin", executionDirectory, "LambdaConverters.Wpf.dll");

            nuspecPath = Path.Combine(executionDirectoryPath, "LambdaConverters.nuspec");

            nuspecAnnotationsPath = Path.Combine(executionDirectoryPath, "LambdaConverters.Annotations.nuspec");
        }

        static void ResignAssembly([NotNull] string assemblyPath, [NotNull] string snkPath)
        {
            Console.WriteLine("Resigning assembly...");

            Debug.Assert(Settings.Default != null);

            Console.WriteLine($"Tool path: {Settings.Default.SnPath}");

            RunConsoleApplication($"\"{Settings.Default.SnPath}\"", $"-R \"{assemblyPath}\" \"{snkPath}\"");
        }

        static void UpdateNuspec([NotNull] string assemblyPath, [NotNull] string nuspecPath, [NotNull] out string packageFileName)
        {
            Console.Write("Updating nuspec...");

            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            Debug.Assert(assembly != null);

            var nuspec = XDocument.Load(nuspecPath);
            Debug.Assert(nuspec.Root != null);

            var metadataElement = nuspec.Root.Element("metadata");
            Debug.Assert(metadataElement != null);

            var versionElement = metadataElement.Element("version");
            var fileVersionAttributeData = assembly.GetCustomAttributesData()?.First(a => a?.AttributeType == typeof(AssemblyFileVersionAttribute));
            Debug.Assert(versionElement != null);
            Debug.Assert(fileVersionAttributeData != null);
            Debug.Assert(fileVersionAttributeData.ConstructorArguments[0].Value is string);
            versionElement.Value = (string)fileVersionAttributeData.ConstructorArguments[0].Value;

            const string target = @"lib\net45";
            nuspec.Root.Element("files")?
                .Add(
                    new XElement("file", new XAttribute("src", assemblyPath), new XAttribute("target", target)),
                    new XElement("file", new XAttribute("src", Path.ChangeExtension(assemblyPath, "pdb")), new XAttribute("target", target)),
                    new XElement("file", new XAttribute("src", Path.ChangeExtension(assemblyPath, "xml")), new XAttribute("target", target)));

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
            Debug.Assert(nuspecDirectoryPath != null);

            using (Process.Start("explorer", "/select, \"" + Path.Combine(nuspecDirectoryPath, packageFileName) + "\"")) { }
        }
    }
}
