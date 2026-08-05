using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        DataManager dataManager = (DataManager)target;

        if (GUILayout.Button("Load Json To Inspector"))
        {
            dataManager.LoadJsonToInspector();
        }

        if (GUILayout.Button("Save Inspector To Json"))
        {
            dataManager.SaveInspectorToJson();
        }

        if (GUILayout.Button("Delete Runtime Save Json"))
        {
            dataManager.DeleteRuntimeSaveJson();
        }
    }
}
