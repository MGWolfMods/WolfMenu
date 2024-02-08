using FluffyUnderware.DevTools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Wolf_Menu.Tabs;

namespace Wolf_Menu.Misc
{
    internal class KeybindHandler
    {

        internal static KeyCode toggleMenu = KeyCode.F5;
        internal static KeyCode noClip = KeyCode.F6;

        private static List<KeybindJson> jsonKeys = new List<KeybindJson>();

        private static int[] values;
        private static bool[] keys;
        private static bool saved = false;  

        private static string currentAction = null;
        private static bool waiting = false;
        private static UnityAction currentCallback = null;

        internal static void Initialize()
        {
            jsonKeys = JsonParser.ReadFromJson<KeybindJson>(JsonParser.Json.Keybinds);
            if(jsonKeys.Count > 0)
            {
                foreach(KeybindJson jsonKey in jsonKeys)
                {
                    if(jsonKey.action == "toggleMenu")
                    {
                        toggleMenu = jsonKey.keybind;
                    }

                    if (jsonKey.action == "noClip")
                    {
                        noClip = jsonKey.keybind;
                    }
                }
            }
            else
            {
                SaveToJson(new string[] { "toggleMenu", "noClip" }, new KeyCode[] { toggleMenu, noClip});
            }

            if (!saved)
            {
                values = (int[])System.Enum.GetValues(typeof(KeyCode));
                keys = new bool[values.Length];
                saved = true;
            }
            
        }


        internal static void changeKeybind(string action, UnityAction callback)
        {
            if (!waiting)
            {
                waiting = true;
                currentAction = action;
                currentCallback = callback;
                T_Warning.ShowWarning("Change Keybind", "Waiting for Keyboard Input", false);
            }
        }

        internal static void WaitingForInput(string action)
        {
            KeyCode keybind = KeyCode.F6;

            SaveToJson(new string[] { action }, new KeyCode[] { keybind });
        }

        internal static void SavingKeystroke()
        {
            if (waiting)
            {
                KeyCode keycode = KeyCode.None;

                for (int i = 0; i < values.Length; i++)
                {
                    keys[i] = Input.GetKeyDown((KeyCode)values[i]);
                }

                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i])
                    {

                        KeyCode tempCode = (KeyCode)values[i];

                        if (!tempCode.ToString().Contains("Mouse"))
                        {
                            keycode = tempCode;
                        }
                        
                    }
                }

                if(keycode != KeyCode.None)
                {
                    SaveToJson(new string[] { currentAction }, new KeyCode[] { keycode });
                    waiting = false;
                    currentAction = null;
                    currentCallback.Invoke();
                    currentCallback = null;
                    T_Warning.CloseWarning();
                }
            }
        }


        private static void SaveToJson(string[] actions, KeyCode[] keybinds)
        {
            foreach(string action in actions)
            {
                int index = actions.IndexOf(action);
                KeybindJson tempKeybind = new KeybindJson(action, keybinds[index]);
                bool replaced = false;

                for (int i = 0; i < jsonKeys.Count; i++)
                {
                    if (action == jsonKeys[i].action)
                    {
                        jsonKeys[i] = tempKeybind;
                        replaced = true;
                    }
                }

                foreach (KeybindJson jsonKeybind in jsonKeys)
                {

                    

                    int index2 = jsonKeys.IndexOf(jsonKeybind);
                    
                }
                if (!replaced) { jsonKeys.Add(tempKeybind); }
            }

            JsonParser.SaveToJson(jsonKeys, JsonParser.Json.Keybinds);
            Initialize();
        }

    }
}
