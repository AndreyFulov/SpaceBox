using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    [Range(2,256)]
    public int resolution = 10;

    public float planetMass = 1;

    public bool autoUpdate = true;

    public enum FaceRenderMask
    {
        All,
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back
    };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;
    public AtmosphereSettings atmosphereSettings;
    
    [HideInInspector]
    public bool shapeSettingsFoldOut;
    [HideInInspector]
    public bool colorSettingsFoldOut;

    private AtmosphereEffect _atmosphereEffect = new AtmosphereEffect();
    private ShapeGenerator _shapeGenerator = new ShapeGenerator();
    ColorGenerator _colorGenerator = new ColorGenerator();
    
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;
    

    void Initialize()
    {
        _shapeGenerator.UpdateSettings(shapeSettings);
        _colorGenerator.UpdateSettings(colorSettings);
        _atmosphereEffect.UpdateSettings(this);
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshObj.AddComponent<MeshCollider>();
                meshFilters[i].sharedMesh = new Mesh();
                meshObj.layer = LayerMask.NameToLayer("Ground");
                meshObj.GetComponent<MeshCollider>().sharedMesh = meshFilters[i].sharedMesh;
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(_shapeGenerator,meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }
    
    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }
    
    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }
    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }
        
        _colorGenerator.UpdateElevation(_shapeGenerator.elevationMinMax);
    }

    void GenerateColors()
    {
        _colorGenerator.UpdateColors();
    }
    
    public float BodyScale {
        get {
            // Body radius is determined by the celestial body class,
            // which sets the local scale of the generator object (this object)
            return transform.localScale.x;
        }
    }
}