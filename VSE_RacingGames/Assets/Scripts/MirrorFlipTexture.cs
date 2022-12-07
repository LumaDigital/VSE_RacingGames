using System;

using UnityEngine;

[ExecuteInEditMode]
public class MirrorFlipTexture : MonoBehaviour
{
    public bool FlipHorizontal
    {
        get => flipHorizontal;
        set
        {
            flipHorizontal = value;
            //FlipTexture(this.gameObject.);
        }
    }
    [SerializeField]
    private bool flipHorizontal;

    public static void FlipTexture(ref Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        Array.Reverse(pixels);
        texture.SetPixels(pixels);
    }
}
