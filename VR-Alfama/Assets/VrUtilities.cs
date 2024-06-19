using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public static class VrUtilities
{
    public static Texture2D TextureFromCamera(Camera camera)
    {
        int Width = camera.activeTexture.width;
        int Height = camera.activeTexture.height;

        Texture2D target = new Texture2D(Width, Height, TextureFormat.RGBAFloat, false);

        camera.Render();

        RenderTexture.active = camera.targetTexture;
        target.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        target.Apply();

        return target;
    }
}
