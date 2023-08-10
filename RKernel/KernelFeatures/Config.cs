using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.KernelFeatures
{
    public class Config
    {
        Dictionary<string, string> configuration;
        public Config(string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);
            configuration = new Dictionary<string, string>();
            foreach (string line in lines)
                configuration.Add(line.Split('=')[0], line.Split('=')[1]);
        }
        public string[] GetConfigEntries() => configuration.Keys.ToArray();
        public string GetValue(string key) => configuration[key];
        public void SetValue(string key, string value) => configuration[key] = value;
        public void AddPair(string key, string value) => configuration.Add(key, value);
        public void RemovePair(string key) => configuration.Remove(key);
    }
}
