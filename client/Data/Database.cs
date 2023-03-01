using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
namespace Data
{
    public class Database
    {
        public string Filename;
        public Dictionary<dynamic, dynamic> Dict = new();
        public Encryption Enc;
        public Database(string filename, string key)
        {
            Filename = filename;
            Enc = new(key);
        }
        public void Add(dynamic key, dynamic value)
        {
            Dict.Add(key, value);
        }
        public dynamic this[dynamic key]
        {
            get
            {
                return Dict[key];
            }
            set
            {
                Dict[key] = value;
            }
        }
        public bool Exists(dynamic key)
        {
            return Dict.ContainsKey(key);
        }
        public void Save(bool encrypted = true)
        {
            string str = JsonConvert.SerializeObject(Dict, Formatting.Indented);
            byte[] text = Encoding.UTF8.GetBytes(str);
            if (encrypted)
            {
                text = Enc.Encrypt(str);
            }
            File.WriteAllBytes(Filename, text);
        }
        public void Load(bool encrypted = true)
        {
            if (!File.Exists(Filename))
            {
                return;
            }
            byte[] text = File.ReadAllBytes(Filename);
            string str = Encoding.UTF8.GetString(text);
            if (encrypted)
            {
                str = Enc.Decrypt(text);
            }
            Dict = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(str);
        }
    }
}
