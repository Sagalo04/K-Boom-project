using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Filo{
    
    [CustomEditor(typeof(CableDisc)), CanEditMultipleObjects] 
    public class CableDiscEditor : Editor
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
        private static void DrawGizmos(CableDisc disc, GizmoType gizmoType)
        {
            Handles.color = Color.cyan;
            Vector3 normal = Vector3.up;
            switch(disc.plane){
                case CableBody.CablePlane.XY: normal = disc.transform.forward; break;
                case CableBody.CablePlane.XZ: normal = disc.transform.up; break;
                case CableBody.CablePlane.YZ: normal = disc.transform.right; break;
            }
            Handles.DrawWireDisc(disc.transform.position,normal,disc.ScaledRadius);
        }
        
    }

}


