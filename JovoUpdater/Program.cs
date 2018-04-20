using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace JovoUpdater
{
    class Program
    {
        static Jovo Config;

        static void Main(string[] args)
        {

            Config = JsonConvert.DeserializeObject<Jovo>(File.ReadAllText("manifest.json"));
            if (!Config.UpdateFromPath.EndsWith("\\"))
                Config.UpdateFromPath += "\\";
            if (!Config.InstalledPath.EndsWith("\\"))
                Config.InstalledPath += "\\";


            if (File.Exists(Config.InstalledPath + "\\manifest.json"))
            {
                Jovo Installed = JsonConvert.DeserializeObject<Jovo>(File.ReadAllText(Config.InstalledPath + "manifest.json"));

                if (File.Exists(Config.UpdateFromPath + "\\manifest.json"))
                {
                    Jovo Server = JsonConvert.DeserializeObject<Jovo>(File.ReadAllText(Config.UpdateFromPath + "\\manifest.json"));

                    if (new Version(Installed.Version) < new Version(Server.Version))
                    {
                        File.Copy(Config.UpdateFromPath + "Jovo.exe", Config.InstalledPath + "Jovo.exe", true);
                        File.Copy(Config.UpdateFromPath + "manifest.json", Config.InstalledPath + "manifest.json", true);
                    }
                }
            }
            else
            {
                if (File.Exists(Config.UpdateFromPath + "Jovo.exe") && File.Exists(Config.UpdateFromPath + "manifest.json"))
                {
                    string[] allFiles = Directory.GetFiles(Config.UpdateFromPath);
                    foreach (string file in allFiles)
                    {
                        File.Copy(Config.UpdateFromPath + file.Split('\\').Last(), Config.InstalledPath + file.Split('\\').Last());
                    }
                }
            }

            try
            {
                Process.Start(Config.InstalledPath + "Jovo.exe");
            }
            catch (Exception) { }

        }
    }

    public class Jovo
    {
        public string Version { get; set; }
        public string InstalledPath { get; set; }
        public string UpdateFromPath { get; set; }
    }
}
