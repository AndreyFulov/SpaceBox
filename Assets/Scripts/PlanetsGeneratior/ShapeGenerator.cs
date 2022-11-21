using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    private ShapeSettings settings;
    private INoiseFilter[] _noiseFilters;

    public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings;
        _noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < _noiseFilters.Length; i++)
        {
            _noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (_noiseFilters.Length > 0)
        {
            firstLayerValue = _noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }
        for (int i = 0; i < _noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += _noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        return pointOnUnitSphere * settings.planetRadius * (1+elevation);
    } 
}
