using ArctiumStudios.SplineTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wolf_Menu.Misc
{
    public static class JsonParser
    {

        public enum Json { Teleport, Keybinds };

        private static string settingsFolder = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "Settings\\wolf.menu");
        internal static string imageFoler = Path.Combine(settingsFolder, "custom images");
        private static string teleportJson = Path.Combine(settingsFolder, "teleports.json");
        private static string keybindJson = Path.Combine(settingsFolder, "keybinds.json");

        internal static void Initialize()
        {
            if (!Directory.Exists(imageFoler))
            {
                Directory.CreateDirectory(imageFoler);
            }
        }


        internal static List<T> ReadFromJson<T>(Json json)
        {
            string path = "";

            switch (json)
            {
                case Json.Teleport:
                    path = teleportJson;
                    break;
                case Json.Keybinds:
                    path = keybindJson;
                    break;
            }

            string content = ReadFile(path);

            if (string.IsNullOrEmpty(content) || content == "{}") { return new List<T>(); }




            List<T> res = JsonConvert.DeserializeObject<ArrayWrapper<T>>(content).items.ToList();

            return res;
        }

        public static void SaveToJson<T>(List<T> toSave, Json json) 
        {
            string content = JsonConvert.SerializeObject(new ArrayWrapper<T>
            {
                items = toSave.ToArray()
            });
            switch(json) 
            {
                case Json.Teleport:
                    WriteFile(teleportJson, content);
                    break;
                case Json.Keybinds:
                    WriteFile(keybindJson, content);
                    break;
            }
        }

        private static void WriteFile(string path, string content)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);

            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(content);
            }
        }

        private static string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string content = reader.ReadToEnd();
                    return content;
                }
            }
            return "";
        }

        [Serializable]
        private class ArrayWrapper<T>
        {
            public T[] items;
        }

    }

}
