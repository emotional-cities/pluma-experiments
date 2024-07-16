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
        //int Width = camera.activeTexture.width;
        //int Height = camera.activeTexture.height;

        //Texture2D target = new Texture2D(Width, Height, TextureFormat.RGBAFloat, false);

        //camera.Render();

        //RenderTexture.active = camera.targetTexture;
        //target.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        //target.Apply();

        //return target;

        camera.aspect = 1;

        var currentRenderTexture = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        camera.Render();

        Texture2D snapShot = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        snapShot.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        snapShot.Apply();

        RenderTexture.active = currentRenderTexture;

        return snapShot;
    }
}
