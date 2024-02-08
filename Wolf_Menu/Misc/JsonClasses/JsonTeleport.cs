using System;
using UnityEngine;

[Serializable]
public class TeleportJson
{
    public string name;
    public Tuple<float, float, float> position;
    public Tuple<float, float, float> rotation;
    public int imageType;
    public string imageName;

    public TeleportJson(string name, Tuple<float, float, float> position, Tuple<float, float, float> rotation, int imageType, string imageName)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
        this.imageType = imageType;
        this.imageName = imageName;
    }
}
