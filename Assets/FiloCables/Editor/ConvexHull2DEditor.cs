using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Filo{
    
    [CustomEditor(typeof(ConvexHull2D))] 
    public class ConvexHull2DEditor : Editor
    {
        ConvexHull2D convexHull;

        public void OnEnable(){
            convexHull = (ConvexHull2D)target;
        }
        
        public override void OnInspectorGUI() {
            
            serializedObject.UpdateIfRequiredOrScript();
            
            Editor.DrawPropertiesExcluding(serializedObject,"m_Script");
            
            // Apply changes to the serializedProperty
            if (GUI.changed){
                serializedObject.ApplyModifiedProperties();                
            }
            
        }

        public override bool HasPreviewGUI(){
            return true;
        }

        private void DrawSectionOutline(Rect region, Color color){

            // Draw segment lines:
            Handles.BeginGUI( );
            Color oldColor = Handles.color;
            Handles.color = color;
            Vector3[] points = new Vector3[convexHull.hull.Count+1];

            if (convexHull.hull.Count > 0){

                float size = Mathf.Min(region.width,region.height) / 
                             (Mathf.Max(convexHull.bounds.width,convexHull.bounds.height) + 0.001f) * 0.5f;

                for (int i = 0; i < convexHull.hull.Count; i++){
                    points[i] = new Vector3(region.center.x + convexHull.hull[i].x * size,
                                            region.center.y + convexHull.hull[i].y * size,0);      
                }
                points[convexHull.hull.Count] =  new Vector3(region.center.x + convexHull.hull[0].x * size,
                                                             region.center.y + convexHull.hull[0].y * size,0); 
            }

            Handles.DrawAAPolyLine(points);
            Handles.EndGUI();
            Handles.color = oldColor;
        }

        public override void OnPreviewGUI(Rect region, GUIStyle background)
        {
            DrawSectionOutline(region, Color.cyan);
        }

        public override void OnInteractivePreviewGUI(Rect region, GUIStyle background)
        {
            DrawSectionOutline(region, Color.cyan);
            
        }
        
    }

}


