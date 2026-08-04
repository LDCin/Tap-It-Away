using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RemoveMissingScriptsWindow : EditorWindow
{
    private List<GameObject> targets = new List<GameObject>();

    [MenuItem("Tools/Remove Missing Scripts Window")]
    public static void ShowWindow()
    {
        GetWindow<RemoveMissingScriptsWindow>("Remove Missing Scripts");
    }

    private void OnGUI()
    {
        GUILayout.Label("Drag GameObjects Here", EditorStyles.boldLabel);

        int removeIndex = -1;

        for (int i = 0; i < targets.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            targets[i] = (GameObject)EditorGUILayout.ObjectField(
                targets[i],
                typeof(GameObject),
                true
            );

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                removeIndex = i;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0)
        {
            targets.RemoveAt(removeIndex);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add Empty Slot"))
        {
            targets.Add(null);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Remove Missing Scripts"))
        {
            RemoveMissingScripts();
        }
    }

    private void RemoveMissingScripts()
    {
        int totalRemoved = 0;
        HashSet<GameObject> processedObjects = new HashSet<GameObject>();

        foreach (GameObject go in targets)
        {
            if (go == null)
                continue;

            Transform[] transforms = go.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in transforms)
            {
                if (child == null || !processedObjects.Add(child.gameObject))
                    continue;

                totalRemoved += GameObjectUtility
                    .RemoveMonoBehavioursWithMissingScript(child.gameObject);
            }
        }

        Debug.Log($"Removed {totalRemoved} missing scripts.");
    }
}
