using UnityEditor;  // Make sure to import the UnityEditor namespace
using UnityEngine;
using System.Collections;
using Unity.AI.Navigation.Samples;

[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DialogueManager dialogueManager = (DialogueManager)target;

        // Ensure the base inspector is drawn
        DrawDefaultInspector();

        // Custom inspector logic for colorized dialogue lines
        EditorGUILayout.LabelField("Dialogue Lines", EditorStyles.boldLabel);
        for (int i = 0; i < dialogueManager.dialogueLines.Length; i++)
        {
            // Use a text field for each line
            dialogueManager.dialogueLines[i] = EditorGUILayout.TextField($"Line {i + 1}", dialogueManager.dialogueLines[i]);
        }

        // Apply any changes made in the inspector
        if (GUI.changed)
        {
            EditorUtility.SetDirty(dialogueManager);  // Mark the object as dirty (modified)
        }
    }
}


