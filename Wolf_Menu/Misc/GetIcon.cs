using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Wolf_Menu.Tabs;
using static UnityEngine.GridBrushBase;

namespace Wolf_Menu.Misc
{
    internal class GetIcon
    {

        private static Dictionary<string, Sprite> itemdata = new Dictionary<string, Sprite>();
        private static Dictionary<string, Sprite> cardata = new Dictionary<string, Sprite>();

        private static bool _done=false;


        internal static void LoadIcons()
        {
            if(_done) return;

            RuntimePreviewGenerator.BackgroundColor = new Color(0f, 0f, 0f, 0f);

            foreach (GameObject item in T_ItemSpawner.partList)
            { 
                if(itemdata.ContainsKey(item.name)) { continue; }
                itemdata.Add(item.name, Sprite.Create(RuntimePreviewGenerator.GenerateModelPreview(item.transform, 200, 200, false), new Rect(0f, 0f, 200f, 200f), new Vector2(0.5f, 0.5f), 100f));
            }

            GameObject[] cars = GameObject.Find("CarsParent").GetComponent<CarList>().Cars;
            foreach (GameObject carObjct in cars)
            {
                if (cardata.ContainsKey(carObjct.name)) { continue; }
                cardata.Add(carObjct.name, Sprite.Create(RuntimePreviewGenerator.GenerateModelPreview(carObjct.transform, 200, 200, false), new Rect(0f, 0f, 200f, 200f), new Vector2(0.5f, 0.5f), 100f));
            }


            _done = true;
        }

        internal static Sprite allFromString(string itemName)
        {
            Sprite itemSprite;
            if (cardata.TryGetValue(itemName, out itemSprite))
            {
                return itemSprite;
            }
            if (itemdata.TryGetValue(itemName, out itemSprite))
            {
                return itemSprite;
            }

            return null;
        }

        internal static Sprite itemFromString(string itemName) 
        {
            Sprite itemSprite;
            if (itemdata.TryGetValue(itemName, out itemSprite))
            {
                return itemSprite;
            }

            return null;
        }

        internal static Sprite carFromString(string itemName)
        {
            Sprite itemSprite;
            if (cardata.TryGetValue(itemName, out itemSprite))
            {
                return itemSprite;
            }

            return null;
        }

        internal static List<Texture2D> customTextures()
        {
            List<Texture2D> textures = new List<Texture2D>();
            foreach (string fileName in Directory.GetFiles(JsonParser.imageFoler, "*.*", SearchOption.AllDirectories))
            {

                if(Path.GetExtension(fileName) != ".png") { continue; }
                byte[] filedata = File.ReadAllBytes(fileName);
                Texture2D temp_text = new Texture2D(200, 200);
                temp_text.LoadImage(filedata);
                temp_text.name = Path.GetFileName(fileName);
                textures.Add(temp_text);
            }

            return textures;
        }

        internal static Texture2D customFromString(string itemName)
        {
            Texture2D tex = null;
            byte[] fileData;
            string filePath = Path.Combine(JsonParser.imageFoler, itemName);

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(200, 200);
                tex.LoadImage(fileData);
            }
            return tex;
        }

    }
}
