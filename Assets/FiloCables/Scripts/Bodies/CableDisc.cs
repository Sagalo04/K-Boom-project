using System;
using System.Collections.Generic;
using UnityEngine;

namespace Filo{

    [AddComponentMenu("Filo Cables/Bodies/Disc")] 
    public class CableDisc : CableBody
    {
    
        public float radius = 1;

        public float ScaledRadius{
            get{
                // avoid variable argument Max() function as it generates garbage.
                return radius * Mathf.Max(transform.lossyScale.x,
                                  Mathf.Max(transform.lossyScale.y,
                                            transform.lossyScale.z));
            }
        }
    
        public static Vector3 TangentPointCircle(Vector2 p1, Vector2 p2, float r, bool orientation){
    
            Vector2 d = p2 - p1;
            float dMag = d.magnitude;
    
            if (dMag > r){

                float alpha;
                if (d.x >= 0){
                    alpha = Mathf.Asin(d.y/dMag);
                }else{
                    alpha = Mathf.PI - Mathf.Asin(d.y/dMag);
                }
            
                float theta = Mathf.Asin(r/dMag);
    
                if (orientation)
                    alpha = alpha - Mathf.PI * 0.5f - theta;
                else
                    alpha = alpha + Mathf.PI * 0.5f + theta;
    
                return p2 + r * new Vector2(Mathf.Cos(alpha),Mathf.Sin(alpha));
            }
            return p1;
        }
    
        public override Vector3 RandomHullPoint(){
            return transform.position + transform.right * radius;
        }
    
        public override Vector2 GetLeftOrRightMostPointFromOrigin(Vector2 origin, bool orientation){
            return TangentPointCircle(origin,Vector2.zero,radius,orientation);
        }
    
        public override float SurfaceDistance(Vector2 p1, Vector2 p2, bool orientation, bool shortest = true){

            if (orientation){
                Vector2 aux = p1;
                p1 = p2;
                p2 = aux;
            }
 
            float theta = Mathf.Atan2(p1.x*p2.y-p1.y*p2.x,p1.x*p2.x+p1.y*p2.y);

            if (!shortest && theta < 0){
                theta = theta + Mathf.PI*2;
            }

            return ScaledRadius * theta;
    
        }

        public override Vector3 SurfacePointAtDistance(Vector3 origin, float distance, bool orientation, out int index){
            index = 0;
            
            // Calculate angle using arc length:
            float angle = distance / radius * (orientation?-1:1);
            return CableToWorld(origin.Rotate2D(angle));
        }

        public override void AppendSamples(Cable.SampledCable samples, Vector3 origin, float distance, float spoolSeparation, bool reverse, bool orientation){

            // If the distance or radius are roughly zero, add the first sample and bail out.
            if (ScaledRadius < 1e-4 || distance < 1e-4){
                samples.AppendSample(CableToWorld(origin));   
                return;
            }

            // Calculate angle using arc length:
            float angle = distance / ScaledRadius;
            int sampleCount = Mathf.CeilToInt(angle / (Mathf.PI*0.05f));

            Vector3 axisOffset = GetCablePlaneNormal() * distance * spoolSeparation / sampleCount;

            angle *= (orientation?-1:1) * (reverse?-1:1);
            float theta = - angle / sampleCount;
            
            // Decide whether to start at the origin or the end:
            Vector3 result = origin;

            // Sample cable:
            samples.AppendSample(CableToWorld(result),!reverse);
            for (int i = 0; i < sampleCount; ++i){
                result = result.Rotate2D(theta);
                samples.AppendSample(CableToWorld(result) + axisOffset * (i+1),!reverse);
            }

            if (reverse){
                samples.ReverseLastSamples(sampleCount+1);
            }
        }
    
    }
}

