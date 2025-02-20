// Program.cs
//
// Copyright (C) 2023 Andrea Gelmini

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.


using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGWC.WPTools
{
    public class WPPluginBuilder
    {
        private Dictionary<string, object>? config;

        // Creates the output directory specified in the configuration.
        public void MakeOutputPath()
        {
            // Check if the configuration is loaded.
            if (config is null)
            {
                Console.WriteLine("the config is null :-C");
                Environment.Exit(4); // Exit with error code 4.
            }

            // Check if the output directory exists.
            if (!Directory.Exists((string)config["output"]))
            {
                Console.WriteLine("The build folder does not exist.");
                Directory.CreateDirectory((string)config["output"]); // Create the directory.
            }
            else
            {
                Console.WriteLine("The build folder is already present.");
            }
        }

        // Loads configuration from composer.json.
        public void GetConfigs(string[] args)
        {
            // Check if composer.json exists in the current directory.
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "composer.json")))
            {
                string error_composer_path = Path.Combine(Environment.CurrentDirectory, "composer.json");
                Console.WriteLine($"The file {error_composer_path} not present.");
                Environment.Exit(1); // Exit with error code 1.
            }

            try
            {
                string composerFile = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "composer.json"));

                // Deserialize the composer.json file using Newtonsoft.Json.
                dynamic? jsonConfig = JsonConvert.DeserializeObject(composerFile);

                // Check if deserialization was successful.
                if (jsonConfig is null)
                {
                    Console.WriteLine("Error while reading the file composer.json returned a null object.");
                    Environment.Exit(2); // Exit with error code 2.
                }

                // Check if the "wp-build-config" key exists in composer.json.
                if (!jsonConfig.ContainsKey("wp-build-config"))
                {
                    Console.WriteLine("Error while reading the file composer.json not have the 'wp-build-config' key.");
                    Environment.Exit(3); // Exit with error code 3.
                }

                // Extract the "wp-build-config" section.
                JObject wpBuildConfig = jsonConfig["wp-build-config"];

                // Get the entry point path.
                var entryPoint = wpBuildConfig.GetValue("entry");

                // Check if the "entry" key exists.
                if (entryPoint is null)
                {
                    Console.WriteLine("the key entry non presente.");
                    Environment.Exit(4); // Exit with error code 4.
                }

                // If entry point is "/", set it to the current directory.
                if ("/" == entryPoint.Value<string>())
                {
                    wpBuildConfig["entry"] = Environment.CurrentDirectory;
                }

                // If command line arguments are provided, use the first argument as the output path.
                if (args.Length != 0)
                {
                    wpBuildConfig["output"] = args[0];
                }

                // Convert the JObject to a Dictionary<string, object>.
                if (wpBuildConfig is not null)
                {
                    config = wpBuildConfig.ToObject<Dictionary<string, object>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Print any exceptions that occur.
            }

        }

        // Copies files specified in the configuration.
        public void CopyFiles()
        {
            // Check if the configuration and "files" property are loaded.
            if (config is null || config["files"] is null)
            {
                Console.WriteLine("The files property is not present o correctly read");
                Environment.Exit(5); // Exit with error code 5.
            }
            else
            {
                // Get the list of files to copy.
                JArray filesToCopy = (JArray)config["files"];

                // Iterate through each file to copy.
                foreach (JToken fileToken in filesToCopy)
                {
                    string file = fileToken.ToString();
                    string sourcePath = Path.Combine((string)config["entry"], file); // Construct the source path.
                    string destPath = Path.Combine((string)config["output"], file); // Construct the destination path.

                    // Copy the file if it exists.
                    if (File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, destPath, true); // Overwrite if the destination exists.
                        Console.WriteLine($" - {file} copied to {(string)config["output"]}");
                    }
                    // Copy the directory if it exists.
                    else if (Directory.Exists(sourcePath))
                    {
                        CopyDirectory(sourcePath, destPath);
                    }
                }
            }
        }

        // Recursively copies a directory and its contents.
        private void CopyDirectory(string source, string destination)
        {
            DirectoryInfo dir = new DirectoryInfo(source);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory if it doesn't exist.
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string sourcePath = file.FullName;
                string parentPath = sourcePath.Replace(Environment.CurrentDirectory, ""); // Get relative path for console output.
                string destFile = Path.Combine(destination, file.Name);

                // Copy the file if it's not excluded.
                if (!IsExcludedPath(sourcePath))
                {
                    File.Copy(sourcePath, destFile, true);
                    Console.WriteLine($" - {parentPath} copied to {destination}");
                }
            }

            // Recursively copy subdirectories.
            foreach (DirectoryInfo subdir in dirs)
            {
                string sourcePath = subdir.FullName;
                string destPath = Path.Combine(destination, subdir.Name);

                // Copy the subdirectory if it's not excluded.
                if (!IsExcludedPath(sourcePath))
                {
                    CopyDirectory(sourcePath, destPath);
                }
            }
        }

        // Checks if a path is excluded based on the configuration.
        private bool IsExcludedPath(string path)
        {
            // Check if the "exclude" property exists in the configuration.
            if (config is not null && config.ContainsKey("exclude") && config["exclude"] is not null)
            {
                JArray excludedPaths = (JArray)config["exclude"];

                // Iterate through the excluded paths.
                foreach (JToken excludedPath in excludedPaths)
                {
                    string toExclude = excludedPath.ToString();

                    // Check if the path ends with any of the excluded paths.
                    if (path.EndsWith(toExclude, StringComparison.OrdinalIgnoreCase))
                    {
                        return true; // Return true if the path is excluded.
                    }
                }
            }
            return false; // Return false if the path is not excluded.
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            WPPluginBuilder builder = new WPPluginBuilder();
            builder.GetConfigs(args); // Load configuration.
            builder.MakeOutputPath(); // Create output directory.
            builder.CopyFiles(); // Copy files.
        }
    }
}