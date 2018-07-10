using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace JovoUpdater
{
    class Program
    {
        static Jovo Config;
        static void Main(string[] args)
        {
            foreach(string arg in args)
            {
                Output(arg);
            }

            try
            {
                if (args.Length == 1)
                {
                    Process jovo = Process.GetProcessById(Convert.ToInt32(args[0].Trim('"')));
                    if (!jovo.HasExited)
                        jovo.WaitForExit();
                }
            } catch (Exception) { }

            try
            {
                Config = JsonConvert.DeserializeObject<Jovo>(File.ReadAllText("manifest.json"));
                if (!Config.UpdateFromPath.EndsWith("\\"))
                    Config.UpdateFromPath += "\\";
                if (!Config.InstalledPath.EndsWith("\\"))
                    Config.InstalledPath += "\\";
            } catch (Exception)
            {
            }

            try
            {
                if (!Directory.Exists(Config.InstalledPath))
                    Directory.CreateDirectory(Config.InstalledPath);
            } catch (Exception)
            {
            }

            try
            {
                if (File.Exists(Config.InstalledPath + "manifest.json") && File.Exists(Config.UpdateFromPath + "manifest.json"))
                {
                    //Runs the update
                    Jovo Installed = JsonConvert.DeserializeObject<Jovo>(File.ReadAllText(Config.InstalledPath + "manifest.json"));
                    Jovo Server = JsonConvert.DeserializeObject<Jovo>(File.ReadAllText(Config.UpdateFromPath + "manifest.json"));
                    //Console.WriteLine("Got installed & server manifests");

                    if (new Version(Installed.Version) < new Version(Server.Version))
                    {
                        foreach (string file in Directory.GetFiles(Config.UpdateFromPath))
                        {
                            File.Copy(Config.UpdateFromPath + file.Split('\\').Last(), Config.InstalledPath + file.Split('\\').Last(), true);
                        }
                        //Console.Write("Updated");
                    } 
                }
                else
                {
                    //Runs the install
                    if (File.Exists(Config.UpdateFromPath + "Jovo.exe") && File.Exists(Config.UpdateFromPath + "manifest.json"))
                    {
                        string[] allFiles = Directory.GetFiles(Config.UpdateFromPath);
                        foreach (string file in allFiles)
                        {
                            File.Copy(Config.UpdateFromPath + file.Split('\\').Last(), Config.InstalledPath + file.Split('\\').Last());
                        }
                    }
                }
            } catch (Exception)
            {
                //Console.WriteLine(ex_c.ToString());
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = Config.InstalledPath,
                FileName = "Jovo.exe",
                Arguments = "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\" \"" + Config.UpdateFromPath + "\""
            };

            Directory.SetCurrentDirectory(Config.InstalledPath);
            Process Jovo = Process.Start(startInfo);
        }

        static void Output(string message)
        {
            using(StreamWriter writer = new StreamWriter("log.txt", true))
            {
                writer.WriteLine(message);
            }
        }
    }

    public class Jovo
    {
        public string Version { get; set; }
        public string InstalledPath { get; set; }
        public string UpdateFromPath { get; set; }
    }
}
