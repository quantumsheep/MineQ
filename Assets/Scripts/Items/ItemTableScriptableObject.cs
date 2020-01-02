using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Table", order = 1)]
public class ItemTableScriptableObject : ScriptableObject
{
    public List<Item> items = new List<Item>();

    public Item GetItem(string id)
    {
        return this.items.Find(item => item.id == id);
    }

    public BlockScriptableObject GetBlock(string id)
    {
        var block = this.GetItem(id);
        return block.item as BlockScriptableObject;
    }

    private Texture2D blocksAtlas = null;
    private Texture2D itemsAtlas = null;

    public Texture2D GetBlockAtlasTexture()
    {
        return this.blocksAtlas;
    }

    public Texture2D GetItemAtlasTexture()
    {
        return this.itemsAtlas;
    }

    public void GenerateTextureAtlas()
    {
        this.blocksAtlas = new Texture2D(8192, 8192)
        {
            filterMode = FilterMode.Point,
        };

        this.itemsAtlas = new Texture2D(8192, 8192)
        {
            filterMode = FilterMode.Point,
        };

        List<Texture2D> blocksTextures = new List<Texture2D>();
        List<Texture2D> itemsTextures = new List<Texture2D>();

        foreach (var item in items)
        {
            if (item.type == ItemType.Block)
            {
                var block = item.item as BlockScriptableObject;

                var blockTextures = block.GetTextures();
                blocksTextures.AddRange(blockTextures);
            }
            else if (item.type == ItemType.Item)
            {
                var blockTextures = item.item.sprite;
                itemsTextures.Add(item.item.sprite);
            }
        }

        List<Rect> blocksRects = this.blocksAtlas.PackTextures(blocksTextures.ToArray(), 0, 8192).ToList();
        List<Rect> itemsRects = this.itemsAtlas.PackTextures(itemsTextures.ToArray(), 0, 8192).ToList();

        foreach (var item in items)
        {
            if (item.type == ItemType.Block)
            {
                var block = item.item as BlockScriptableObject;
                block.rects = blocksRects.GetRange(0, 6).ToArray();
                blocksRects.RemoveRange(0, 6);
            }
            else if (item.type == ItemType.Item)
            {
                var blockTextures = item.item.sprite;
                item.item.rects = itemsRects.GetRange(0, 1).ToArray();
                itemsRects.RemoveRange(0, 1);
            }
        }
    }
}
