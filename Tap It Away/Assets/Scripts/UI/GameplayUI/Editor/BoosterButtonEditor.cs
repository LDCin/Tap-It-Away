using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(BoosterButton))]
public class BoosterButtonEditor : ButtonEditor
{
    private SerializedProperty backgroundImage;
    private SerializedProperty icon;
    private SerializedProperty lockIcon;
    private SerializedProperty unlockLevelText;
    private SerializedProperty countFrame;
    private SerializedProperty countText;
    private SerializedProperty moreBoosterIcon;

    protected override void OnEnable()
    {
        base.OnEnable();

        backgroundImage = serializedObject.FindProperty("backgroundImage");
        icon = serializedObject.FindProperty("icon");
        lockIcon = serializedObject.FindProperty("lockIcon");
        unlockLevelText = serializedObject.FindProperty("unlockLevelText");
        countFrame = serializedObject.FindProperty("countFrame");
        countText = serializedObject.FindProperty("countText");
        moreBoosterIcon = serializedObject.FindProperty("moreBoosterIcon");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Booster Button", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(backgroundImage);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(lockIcon);
        EditorGUILayout.PropertyField(unlockLevelText);
        EditorGUILayout.PropertyField(countFrame);
        EditorGUILayout.PropertyField(countText);
        EditorGUILayout.PropertyField(moreBoosterIcon);
        serializedObject.ApplyModifiedProperties();
    }
}
