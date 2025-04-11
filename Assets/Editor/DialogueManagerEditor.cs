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

        // Custom inspector logic for dialogue lines
        EditorGUILayout.LabelField("Dialogue Lines", EditorStyles.boldLabel);

        // Loop through each DialogueLine and display fields for speaker name and line text
        for (int i = 0; i < dialogueManager.dialogueLines.Length; i++)
        {
            // Display speaker name and line text fields for each dialogue line
            DialogueLine line = dialogueManager.dialogueLines[i];
            line.speakerName = EditorGUILayout.TextField($"Speaker {i + 1} Name", line.speakerName);
            line.lineText = EditorGUILayout.TextArea(line.lineText, GUILayout.Height(50));

            // Assign the modified DialogueLine back to the array
            dialogueManager.dialogueLines[i] = line;
        }

        // Apply any changes made in the inspector
        if (GUI.changed)
        {
            EditorUtility.SetDirty(dialogueManager);  // Mark the object as dirty (modified)
        }
    }
}



