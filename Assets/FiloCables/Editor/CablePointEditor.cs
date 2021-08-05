using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Filo{
    
    [CustomEditor(typeof(CablePoint)), CanEditMultipleObjects] 
    public class CablePointEditor : Editor
    {
        
        public override void OnInspectorGUI() {
            
            serializedObject.UpdateIfRequiredOrScript();
            
            Editor.DrawPropertiesExcluding(serializedObject,"m_Script");
            
            // Apply changes to the serializedProperty
            if (GUI.changed){
                serializedObject.ApplyModifiedProperties();                
            }
            
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmos(CablePoint point, GizmoType gizmoType)
        {
            Handles.color = Color.cyan;
            Handles.DrawWireCube(point.transform.position,HandleUtility.GetHandleSize(point.transform.position)*Vector3.one*0.1f);
        }
        
    }

}


