
using UnityEditor;
using UnityEngine;
using Object = System.Object;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    private Planet _planet;
    private Editor shapeEditor;
    private Editor colorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                _planet.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            _planet.GeneratePlanet();
        }

        DrawSettingsEditor(_planet.shapeSettings,_planet.OnShapeSettingsUpdated,ref _planet.shapeSettingsFoldOut,ref shapeEditor);
        DrawSettingsEditor(_planet.colorSettings,_planet.OnColorSettingsUpdated,ref _planet.colorSettingsFoldOut,ref colorEditor);
    }

    void DrawSettingsEditor(UnityEngine.Object settings, System.Action onSettingsUpdated,ref bool foldOut,ref Editor editor)
    {
        if (settings != null)
        {


            foldOut = EditorGUILayout.InspectorTitlebar(foldOut, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldOut)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();
                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }
    private void OnEnable()
    {
        _planet = (Planet)target;
    }
}
