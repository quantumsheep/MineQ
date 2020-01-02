using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public string id;
    public string name;

    public ItemType type = ItemType.Item;

    public ItemScriptableObject item;
}
