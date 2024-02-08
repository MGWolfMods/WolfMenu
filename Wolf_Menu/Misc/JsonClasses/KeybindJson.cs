using System;
using UnityEngine;

[Serializable]
public class KeybindJson
{
    public string action;
    public KeyCode keybind;

    public KeybindJson(string action, KeyCode keybind)
    {
        this.action = action;
        this.keybind = keybind;
    }
}
