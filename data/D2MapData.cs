using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class D2MapData : GameData.XMLFile
{
    public static string _path = "data/xml/d2map/map_";

    public static int NEWER_MAP = 1;
    public int id;
    public string textures_info_path;
}