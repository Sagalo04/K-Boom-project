using UnityEngine;
using System;
using System.Collections.Generic;

namespace Filo{

    [AddComponentMenu("Filo Cables/Bodies/Shape")]
    public class CableShape : CableBody
    {
    
        public ConvexHull2D convexHull;

        public override Vector3 RandomHullPoint(){
            if (convexHull == null || convexHull.hull.Count == 0) 
                return transform.position;
            return CablePlaneToWorldSpace(convexHull.hull[0]);
        }
    
        public override Vector2 GetLeftOrRightMostPointFromOrigin(Vector2 origin, bool orientation){
    
            if (convexHull == null || convexHull.hull.Count == 0) 
                return Vector2.zero;
    
            Vector2 axis = -origin.normalized;
    
            int result = 0;
            float bestProj = float.MaxValue;
    
            // Maximize angle with axis (minimize dot product)
            for (int i = 0; i < convexHull.hull.Count; ++i){
    
                int side = ConvexHull2D.Orientation(origin,origin+axis,convexHull.hull[i]);
    
                if ((orientation && side >= 0) || (!orientation && side < 0))
                {
                    float proj = Vector2.Dot((convexHull.hull[i] - origin).normalized,axis);
                    if (proj < bestProj){
                        bestProj = proj;
                        result = i;
                    }
                }
            }
    
            return convexHull.hull[result];
    
        }

        public Vector3 ProjectToSurface(Vector3 p, out int vertex, bool draw = false){

            vertex = 0;
            Vector2 bestProjection = p;
            float bestProjectionDistance = float.MaxValue;      

            for (int i = 0; i < convexHull.hull.Count; ++i){

                int next = i+1;
                if (next == convexHull.hull.Count) next = 0;

                // Get edge vector and vertex-to-point vector:
                Vector2 edge = convexHull.hull[next] - convexHull.hull[i];
                Vector2 v2p = (Vector2)p - convexHull.hull[i];

                // Calculate dot product for vector projection:
                float dot = Vector2.Dot(v2p,edge)/edge.sqrMagnitude;

                // Clamp dot product to lie between 0 and a lil less that 1:
                dot = Mathf.Clamp(dot,0,1-1E-2f);

                // Project p onto hull edge:
                Vector2 proj = convexHull.hull[i] + dot * edge;

                // Calculate projection distance:
                float pd = Vector2.SqrMagnitude((Vector2)p - proj);

                if (pd < bestProjectionDistance){
                    bestProjectionDistance = pd;
                    bestProjection = proj;
                    vertex = i;
                }
          
            }

            return new Vector3(bestProjection.x,bestProjection.y,p.z);
        }
    
        public override float SurfaceDistance(Vector2 p1, Vector2 p2,bool orientation,bool shortest = true){
    
            if (convexHull == null || convexHull.hull.Count == 0 || Vector2.SqrMagnitude(p1-p2) < 1E-6)
                return 0; 
    
            // Project both points on hull and find hull indices:
            int a,b;
            Vector3 w1 = CablePlaneToWorldSpace(ProjectToSurface(p1,out a));
            Vector3 w2 = CablePlaneToWorldSpace(ProjectToSurface(p2,out b));

            // Initialize world space sample:
            Vector3 sample = CablePlaneToWorldSpace(convexHull.hull[a]);

            // Initialize both distances: clock and counterclockwise around the hull:
            float distance1 = - Vector3.Distance(w1,sample) 
                              + Vector3.Distance(w2,CablePlaneToWorldSpace(convexHull.hull[b]));

            float distance2 =   Vector3.Distance(w1,sample)
                              - Vector3.Distance(w2,CablePlaneToWorldSpace(convexHull.hull[b]));

            int c1 = a;
            int c2 = a;
    
            // Go one directin around the hull, accumulating distance in world space:
            while (c1 != b){
                int next = c1 = (int)Utils.Mod(c1+1,convexHull.hull.Count);
                distance1 += Vector3.Distance(sample,CablePlaneToWorldSpace(convexHull.hull[next]));
                sample = CablePlaneToWorldSpace(convexHull.hull[c1]);
            }

            // Reinitialize world space sample:
            sample = CablePlaneToWorldSpace(convexHull.hull[a]);

            // Go the other direction around the hull, accumulating distance in world space:
            while (c2 != b){
                int next = c2 = (int)Utils.Mod(c2-1,convexHull.hull.Count);
                distance2 += Vector3.Distance(sample,CablePlaneToWorldSpace(convexHull.hull[next]));
                sample = CablePlaneToWorldSpace(convexHull.hull[c2]);
            }

            // Grab one of the two distances, depending on orientation:
            if (!shortest){
                return orientation ? distance2 : distance1;
            }else{
                if (distance1 < Mathf.Abs(distance2)){
                    return distance1 * (orientation?-1:1);
                }else{
                    return -distance2 * (orientation?-1:1);
                }
            }
    
        }

        public override Vector3 SurfacePointAtDistance(Vector3 origin, float distance, bool orientation, out int index){

            // project input point to surface:
            origin = ProjectToSurface(origin,out index, true) ;
            Vector3 nextSample = CableToWorld(origin);
            Vector3 sample = nextSample;
            int nextIndex = index;

            int direction = (orientation?-1:1);
            float accumDistance = - Mathf.Min(distance,Vector2.Distance(origin,convexHull.hull[index]));
            float segmentDistance = 0;

            do{

                sample = nextSample;
                index = nextIndex;

                // Get next hull sample:
                nextIndex = (int)Utils.Mod(index + direction,convexHull.hull.Count);
                nextSample = CableToWorld(convexHull.hull[nextIndex]);

                // Calculate hull segment distance:
                segmentDistance = Vector3.Distance(sample,nextSample);

                // Accumulate distance:
                accumDistance += segmentDistance;

            } while (accumDistance + 1E-4 < distance);
    
            // Interpolate between current and next sample based on remaining distance:
            return Vector3.Lerp(nextSample,sample,(accumDistance - distance)/segmentDistance);
        }

        public override void AppendSamples(Cable.SampledCable samples, Vector3 origin, float distance , float spoolSeparation,bool reverse, bool orientation){

            if (convexHull == null || convexHull.hull.Count == 0)
                return; 

            int current;
            Vector3 originSample;

            origin = ProjectToSurface(origin,out current); 
            originSample = CableToWorld(origin);
            samples.AppendSample(originSample);

            if (distance < 1E-4)
                return;

            int direction = (orientation?-1:1)*(reverse?-1:1);
            Vector3 axisOffset = GetCablePlaneNormal() * spoolSeparation;

            int count = 1;
            float accumDistance = 0;
            while (accumDistance < Mathf.Abs(distance)){

                // Get next hull sample:
                int next = current = (int)Utils.Mod(current - direction,convexHull.hull.Count);
                Vector3 sample = CableToWorld(new Vector3(convexHull.hull[next].x,convexHull.hull[next].y,origin.z));

                // Calculate hull segment distance:
                float segmentDistance = Vector3.Distance(originSample,sample);

                // Accumulate segment distance to total distance:
                if (accumDistance + segmentDistance <= distance){
                    samples.AppendSample(sample + axisOffset * accumDistance,!reverse);
                }else{
                    Vector3 interpolatedSample = Vector3.Lerp(originSample,sample,(Mathf.Abs(distance)-accumDistance)/segmentDistance);
                    samples.AppendSample(interpolatedSample + axisOffset * accumDistance,!reverse);
                }
               
                originSample = sample;
                accumDistance += segmentDistance;
                count++;
            }

            if (reverse){
                samples.ReverseLastSamples(count);
            }

        }
    
    }
}

