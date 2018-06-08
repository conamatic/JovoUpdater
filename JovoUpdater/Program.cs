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
            try
            {
                Config = JsonConvert.DeserializeObject<Jovo>(File.ReadAllText("manifest.json"));
                if (!Config.UpdateFromPath.EndsWith("\\"))
                    Config.UpdateFromPath += "\\";
                if (!Config.InstalledPath.EndsWith("\\"))
                    Config.InstalledPath += "\\";
                //Console.WriteLine("Got configuration manifest");
            } catch (Exception ex_a)
            {
                //Console.WriteLine(ex_a.ToString());
            }

            try
            {
                if (!Directory.Exists(Config.InstalledPath))
                    Directory.CreateDirectory(Config.InstalledPath);
            } catch (Exception ex_b)
            {
                //Console.WriteLine(ex_b);
            }

            try
            {
                if (File.Exists(Config.InstalledPath + "manifest.json") && File.Exists(Config.UpdateFromPath + "manifest.json"))
                {
                    Jovo Installed = JsonConvert.DeserializeObject<Jovo>(File.ReadAllText(Config.InstalledPath + "manifest.json"));
                    Jovo Server = JsonConvert.DeserializeObject<Jovo>(File.ReadAllText(Config.UpdateFromPath + "manifest.json"));
                    //Console.WriteLine("Got installed & server manifests");

                    if (new Version(Installed.Version) < new Version(Server.Version))
                    {
                        //Console.WriteLine("Trying to update...");
                        File.Copy(Config.UpdateFromPath + "Jovo.exe", Config.InstalledPath + "Jovo.exe", true);
                        File.Copy(Config.UpdateFromPath + "manifest.json", Config.InstalledPath + "manifest.json", true);
                        //Console.Write("Updated");
                    } else
                    {
                        //Console.WriteLine("Didn't update (up to date)");
                    }
                }
                else
                {
                    if (File.Exists(Config.UpdateFromPath + "Jovo.exe") && File.Exists(Config.UpdateFromPath + "manifest.json"))
                    {
                        //Console.WriteLine("Installing for the first time");
                        string[] allFiles = Directory.GetFiles(Config.UpdateFromPath);
                        foreach (string file in allFiles)
                        {
                            File.Copy(Config.UpdateFromPath + file.Split('\\').Last(), Config.InstalledPath + file.Split('\\').Last());
                            //Console.WriteLine("Copied " + file);
                        }
                    }
                }
            } catch (Exception ex_c)
            {
                //Console.WriteLine(ex_c.ToString());
            }
            //Console.WriteLine("Ready to start");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = Config.InstalledPath,
                FileName = "Jovo.exe",
                Arguments = System.Reflection.Assembly.GetExecutingAssembly().Location
            };

            Directory.SetCurrentDirectory(Config.InstalledPath);
            Process Jovo = Process.Start(startInfo);
        }
    }

    public class Jovo
    {
        public string Version { get; set; }
        public string InstalledPath { get; set; }
        public string UpdateFromPath { get; set; }
    }
}
