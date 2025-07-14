
using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageUtil
{
    public void RenderFromBase64(string base64, Image targetImage)
    {
        byte[] imageData = Convert.FromBase64String(base64);
        Texture2D tex = new(2, 2, TextureFormat.ARGB32, false);
        if (!tex.LoadImage(imageData)) return;

        tex.filterMode = FilterMode.Point;
        tex.Apply(false, false);

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        targetImage.sprite = sprite;
    }

    public void RenderFromBase64(string base64, SpriteRenderer targetImage)
    {
        byte[] imageData = Convert.FromBase64String(base64);
        Texture2D tex = new(2, 2, TextureFormat.ARGB32, false);
        if (!tex.LoadImage(imageData)) return;

        tex.filterMode = FilterMode.Point;
        tex.Apply(false, false);

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        targetImage.sprite = sprite;
    }
}