using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Filo{
    
    [CustomEditor(typeof(CableShape)), CanEditMultipleObjects] 
    public class CableShapeEditor : Editor
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
        private static void DrawGizmos(CableShape shape, GizmoType gizmoType)
        {

            if (shape.convexHull == null) 
                return;
    
            Handles.color = Color.cyan;
            Handles.matrix = shape.transform.localToWorldMatrix;
    
            for (int i = 0; i < shape.convexHull.hull.Count; ++i){

                int next = i+1;
                if (next == shape.convexHull.hull.Count) next = 0;

                Vector3 p1 = shape.convexHull.hull[i];
                Vector3 p2 = shape.convexHull.hull[next];

                switch(shape.plane){
                    case CableBody.CablePlane.XZ: p1 = new Vector3(p1.x,0,p1.y); p2 = new Vector3(p2.x,0,p2.y); break;
                    case CableBody.CablePlane.YZ: p1 = new Vector3(0,p1.x,p1.y); p2 = new Vector3(0,p2.x,p2.y); break;
                }   

                Handles.DrawLine(p1,p2);
            }

        }
        
    }

}


