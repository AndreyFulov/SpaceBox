using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    private ColorSettings settings;
    private Texture2D texture;
    private const int textureRes = 50;

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if (texture == null)
        {
            texture = new Texture2D(textureRes, 1);
        }
    }

    public void UpdateElevation(MinMax elevationMinMax) 
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min,elevationMinMax.Max));
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[textureRes];
        for (int i = 0; i < textureRes; i++)
        {
            colors[i] = settings.gradient.Evaluate(i/(textureRes-1f));
        }
        texture.SetPixels(colors);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture",texture);
    }
}
