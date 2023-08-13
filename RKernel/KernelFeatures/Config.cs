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
        private Dictionary<string, string> configuration;
        private string _filepath;
        public Config(string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);
            configuration = new Dictionary<string, string>();
            foreach (string line in lines)
                configuration.Add(line.Split('=')[0], line.Split('=')[1]);
            _filepath = filepath;
        }
        public string this[string ConfLine]
        {
            get => configuration[ConfLine];
            set => configuration[ConfLine] = value;
        }
        public string[] GetConfigEntries() => configuration.Keys.ToArray();
        public string GetValue(string key) => configuration[key];
        public void SetValue(string key, string value) => configuration[key] = value;
        public void AddPair(string key, string value) => configuration.Add(key, value);
        public void RemovePair(string key) => configuration.Remove(key);
        public void Save()
        {
            List<string> conf = new List<string>();
            foreach (KeyValuePair<string, string> pair in configuration)
                conf.Add($"{pair.Key}={pair.Value}");
            File.WriteAllLines(_filepath, conf.ToArray());
        }
    }
}
