using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UnusedScriptsChecker : EditorWindow
{
    private List<string> unusedScripts = new List<string>();

    [MenuItem("Tools/Find Unused Scripts")]
    public static void ShowWindow()
    {
        GetWindow<UnusedScriptsChecker>("Unused Scripts Checker");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Unused Scripts"))
        {
            FindUnusedScripts();
        }

        if (unusedScripts.Count > 0)
        {
            GUILayout.Label("Unused Scripts:");
            foreach (var script in unusedScripts)
            {
                GUILayout.Label(script);
            }
        }
    }

    private void FindUnusedScripts()
    {
        unusedScripts.Clear();
        string[] allScripts = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories);
        HashSet<string> usedScripts = new HashSet<string>();

        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            var components = go.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component is MonoBehaviour monoBehaviour)
                {
                    string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(monoBehaviour));
                    if (!string.IsNullOrEmpty(scriptPath))
                    {
                        usedScripts.Add(scriptPath);
                    }
                }
            }
        }

        foreach (var script in allScripts)
        {
            if (!usedScripts.Contains(script))
            {
                unusedScripts.Add(script);
            }
        }
    }
}