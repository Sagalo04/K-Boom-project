using System;
using System.Collections.Generic;
using UnityEngine;

namespace Filo
{

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(Cable))]
    [ExecuteInEditMode]
    [AddComponentMenu("Filo Cables/Cable Renderer")]
    public class CableRenderer : MonoBehaviour
    {

        public class CurveFrame{

            public Vector3 position = Vector3.zero;
            public Vector3 tangent = Vector3.forward;
            public Vector3 normal = Vector3.up;
            public Vector3 binormal = Vector3.left;

            public void Reset(){
                position = Vector3.zero;
                tangent = Vector3.forward;
                normal = Vector3.up;
                binormal = Vector3.left;
            }

            public void SetTwist(float twist){
                Quaternion twistQ = Quaternion.AngleAxis(twist,tangent);
                normal = twistQ*normal;
                binormal = twistQ*binormal;
            }

            public void SetTwistAndTangent(float twist, Vector3 tangent){
                this.tangent = tangent;
                normal = new Vector3(-tangent.y,tangent.x,0).normalized;
                binormal = Vector3.Cross(tangent,normal);

                Quaternion twistQ = Quaternion.AngleAxis(twist,tangent);
                normal = twistQ*normal;
                binormal = twistQ*binormal;
            }

            public void Transport(Vector3 newPosition, Vector3 newTangent, float twist){

                // Calculate delta rotation:
                Quaternion rotQ = Quaternion.FromToRotation(tangent,newTangent);
                Quaternion twistQ = Quaternion.AngleAxis(twist,newTangent);
                Quaternion finalQ = twistQ*rotQ;
                
                // Rotate previous frame axes to obtain the new ones:
                normal = finalQ*normal;
                binormal = finalQ*binormal;
                tangent = newTangent;
                position = newPosition;

            }

            public void DrawDebug(float length){
                Debug.DrawRay(position,normal*length,Color.blue);
                Debug.DrawRay(position,tangent*length,Color.red);
                Debug.DrawRay(position,binormal*length,Color.green);
            }
        }

        private Cable cable;
        private MeshFilter filter;
        private Mesh mesh;
        
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector4> tangents = new List<Vector4>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<int> tris = new List<int>();

        private CurveFrame frame;

        public CableSection section;
        public Vector2 uvScale = Vector2.one;
        public float thickness = 0.025f;

        public void OnEnable(){

            cable = GetComponent<Cable>();
            filter = GetComponent<MeshFilter>();

            mesh = new Mesh();
            mesh.name = "cable";
            mesh.MarkDynamic(); 
            filter.mesh = mesh; 
        }

        public void OnDisable(){
            DestroyImmediate(mesh);
        }

        public void LateUpdate()
        {
            ClearMeshData();

            if (section == null)
                return;

            IList<List<Vector3>> segments = cable.sampledCable.Segments;

            int sectionSegments = section.Segments;
            int verticesPerSection = sectionSegments + 1;  // the last vertex in each section must be duplicated, due to uv wraparound.

            float vCoord = -uvScale.y * cable.RestLength;  // v texture coordinate.
            int sectionIndex = 0;

            float strain = cable.sampledCable.Length / cable.RestLength;

            // we will define and transport a reference frame along the curve using parallel transport method:
            if (frame == null)          
                frame = new CurveFrame();

            Vector4 texTangent = Vector4.zero;
            Vector2 uv = Vector2.zero;

            Matrix4x4 w2l = transform.worldToLocalMatrix;

            for (int k = 0; k < segments.Count; ++k){

                List<Vector3> samples = segments[k];

                // Reinitialize frame for each segment.
                frame.Reset();

                for (int i = 0; i < samples.Count; ++i){
    
                    // Calculate previous and next curve indices:
                    int nextIndex = Mathf.Min(i+1,samples.Count-1);
                    int prevIndex = Mathf.Max(i-1,0);
    
                    Vector3 point = w2l.MultiplyPoint3x4(samples[i]);
                    Vector3 prevPoint = w2l.MultiplyPoint3x4(samples[prevIndex]);
                    Vector3 nextPoint = w2l.MultiplyPoint3x4(samples[nextIndex]);
    
                    Vector3 nextV = (nextPoint - point).normalized;
                    Vector3 prevV = (point - prevPoint).normalized;
                    Vector3 tangent = nextV + prevV;
    
                    // update frame:
                    frame.Transport(point,tangent,0);
        
                    // advance v texcoord:
                    vCoord += uvScale.y * (Vector3.Distance(point,prevPoint) /  strain);
    
                    // Loop around each segment:
                    for (int j = 0; j <= sectionSegments; ++j){
    
                        vertices.Add(frame.position + (section.vertices[j].x*frame.normal + section.vertices[j].y*frame.binormal) * thickness);
                        normals.Add(vertices[vertices.Count-1] - frame.position);
                        texTangent = -Vector3.Cross(normals[normals.Count-1],frame.tangent);
                        texTangent.w = 1;
                        tangents.Add(texTangent);
    
                        uv.Set((j/(float)sectionSegments)*uvScale.x,vCoord);
                        uvs.Add(uv);
    
                        if (j < sectionSegments && i < samples.Count-1){
    
                            tris.Add(sectionIndex*verticesPerSection + j);          
                            tris.Add(sectionIndex*verticesPerSection + (j+1));      
                            tris.Add((sectionIndex+1)*verticesPerSection + j);      
    
                            tris.Add(sectionIndex*verticesPerSection + (j+1));      
                            tris.Add((sectionIndex+1)*verticesPerSection + (j+1));  
                            tris.Add((sectionIndex+1)*verticesPerSection + j);      
    
                        }
                    }
    
                    sectionIndex++;
                }
            }

            CommitMeshData();
        }

        private void ClearMeshData(){
            mesh.Clear();
            vertices.Clear();
            normals.Clear();
            tangents.Clear();
            uvs.Clear();
            tris.Clear();
        }

        private void CommitMeshData(){
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTangents(tangents);
            mesh.SetUVs(0,uvs);
            mesh.SetTriangles(tris,0,true);
        }
    }
}


