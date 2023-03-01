using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Data
{
    public class AccountManager
    {
        private readonly Encryption Enc;
        private readonly string key;
        public AccountManager(string k)
        {
            Enc = new(k);
            key = k;
        }
        public bool CreateAccount(string name, string password, string email)
        {
            if (!Directory.Exists("players"))
            {
                Directory.CreateDirectory("players");
            }
            if (File.Exists($"players/{name}"))
            {
                return false;
            }
            Directory.CreateDirectory($"players/{name}");
            Database db = new($"players/{name}/info.user", key)
            {
                ["name"] = name,
                ["password"] = password,
                ["mail"] = email,
            };
            db.Save();
            return true;
        }
        public bool AccessAccount(string name, string password)
        {
            if (!Directory.Exists($"players/{name}"))
            {
                return false;
            }
            Database db = new($"players/{name}/info.user", key);
            db.Load();
            string pw = db["password"];
            if (password != pw)
            {
                return false;
            }
            return true;
        }
    }
}
