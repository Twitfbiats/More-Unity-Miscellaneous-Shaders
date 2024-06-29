// using UnityEngine;
// using UnityEditor;

// [CustomEditor(typeof(Morph))]
// [CanEditMultipleObjects]
// public class ButtonExampleEditor : Editor
// {
//     SerializedProperty someVariable;

//     void OnEnable()
//     {
//         someVariable = serializedObject.FindProperty("triangles");
//     }

//     public override void OnInspectorGUI()
//     {
//         // Update the serializedObject's representation
//         serializedObject.Update();

//         // Display the default inspector GUI for the target script
//         EditorGUILayout.PropertyField(someVariable);

//         // Apply any changes to the serializedProperty
//         serializedObject.ApplyModifiedProperties();

//         // Add a button below your variables
//         if (GUILayout.Button("Log Something"))
//         {
//             foreach (var targetObject in serializedObject.targetObjects)
//             {
//                 Morph morphScript = (Morph)targetObject;
//                 Debug.Log("Button clicked for " + morphScript.gameObject.name);
//             }
//         }
//     }
// }
