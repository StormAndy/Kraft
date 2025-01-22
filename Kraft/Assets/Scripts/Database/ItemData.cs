using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{

    public string name;
    public string description;
    public bool isStackable;
    public string uniqueID;

    // Placeholder fields for tags and graphics
    public List<string> tags = new List<string>();
    public string graphicPath;

}
