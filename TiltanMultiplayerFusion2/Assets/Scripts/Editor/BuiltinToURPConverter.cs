using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public static class BuiltinToURPConverter 
{
    [MenuItem("Tools/Convert Built-in Materials to URP Lit" )]
    public static void ConvertMaterialsToURP()
    {
        // Get all selected materials
        Object[] selectedObjects = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
        
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No materials selected.");
            return;
        }

        // Find the standard URP Lit shader
        Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null)
        {
            Debug.LogError("Could not find 'Universal Render Pipeline/Lit' shader. Is URP installed?");
            return;
        }

        int convertedCount = 0;

        foreach (Object obj in selectedObjects)
        {
            Material mat = obj as Material;
            if (mat != null && mat.shader.name != urpLitShader.name)
            {
                // Store old properties
                Texture mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
                Color color = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;
                
                // Assign the new shader
                mat.shader = urpLitShader;
                
                // Re-apply properties to URP equivalents
                if (mainTex != null)
                {
                    mat.SetTexture("_BaseMap", mainTex);
                }
                mat.SetColor("_BaseColor", color);

                EditorUtility.SetDirty(mat);
                convertedCount++;
            }
        }

        // Save changes to disk
        AssetDatabase.SaveAssets();
        Debug.Log($"Successfully converted {convertedCount} materials to URP Lit while preserving textures.");
    }
}
