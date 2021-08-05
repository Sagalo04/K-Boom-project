using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Filo{
    
    [CustomEditor(typeof(Cable)), CanEditMultipleObjects] 
    public class CableEditor : Editor
    {

        [MenuItem("GameObject/Filo Cables/Cable", false, 10)]
        static void CreateCable(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Cable");
            go.AddComponent<Cable>();
            go.AddComponent<CableRenderer>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Filo Cables/Cable Solver", false, 10)]
        static void CreateSolver(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Solver");
            go.AddComponent<CableSolver>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        private ReorderableList list;
        
        public void OnEnable(){

            list = new ReorderableList(serializedObject, 
                                       serializedObject.FindProperty("links"), 
                                       true, true, true, true);

            list.drawElementCallback = 
                (Rect rect, int index, bool isActive, bool isFocused) => {

                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                SerializedProperty type = element.FindPropertyRelative("type");

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, 80, EditorGUIUtility.singleLineHeight),
                    type, GUIContent.none);

                EditorGUI.PropertyField(
                    new Rect(rect.x + 82, rect.y, rect.width - 80, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("body"), GUIContent.none);

                if (type.enumValueIndex == 0 || type.enumValueIndex == 2){

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2, rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("inAnchor"), new GUIContent("In Anchor"));
    
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 2)*2, rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("outAnchor"), new GUIContent("Out Anchor"));
                }else{
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2, rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("orientation"), new GUIContent("Orientation"));

                    if (type.enumValueIndex == 3){
                        EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 2)*2, rect.width, EditorGUIUtility.singleLineHeight),
                                 element.FindPropertyRelative("storedCable"), new GUIContent("Stored Cable"));

                        EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 2)*3, rect.width, EditorGUIUtility.singleLineHeight),
                                 element.FindPropertyRelative("spoolSeparation"), new GUIContent("Spool Separation"));

                    }
                }

            };

            list.elementHeightCallback = (index) => 
            { 
                var element = list.serializedProperty.GetArrayElementAtIndex(index); 
                SerializedProperty type = element.FindPropertyRelative("type");
                if (type.enumValueIndex == 0 || type.enumValueIndex == 2){
                    return EditorGUIUtility.singleLineHeight*3+6;
                }else if (type.enumValueIndex == 3){
                    return EditorGUIUtility.singleLineHeight*4+8;
                }else{
                    return EditorGUIUtility.singleLineHeight*2+4;
                }
            };

            list.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Links");
            };

        }
        
        public override void OnInspectorGUI() {
            
            serializedObject.UpdateIfRequiredOrScript();
            
            Editor.DrawPropertiesExcluding(serializedObject,"m_Script", "links");

            list.DoLayoutList();
            
            // Apply changes to the serializedProperty
            if (GUI.changed){
                serializedObject.ApplyModifiedProperties();                
            }
            
        }
        
    }

}


