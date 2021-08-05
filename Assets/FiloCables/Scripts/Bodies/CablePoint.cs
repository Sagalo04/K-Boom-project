using UnityEngine;
using System;
using System.Collections.Generic;

namespace Filo{

    [AddComponentMenu("Filo Cables/Bodies/Point")]
    public class CablePoint : CableBody
    {

        public override void ApplyFreezing(){
        }

        public override void CalculateInertiaTensor(){
        }

        public override Vector3 RandomHullPoint(){
            return transform.position;
        }
    
        public override Vector2 GetLeftOrRightMostPointFromOrigin(Vector2 origin, bool orientation){
            return Vector2.zero;
        }
    
        public override float SurfaceDistance(Vector2 p1, Vector2 p2,bool orientation,bool shortest = true){
            return 0;
        }

        public override Vector3 SurfacePointAtDistance(Vector3 origin, float distance, bool orientation, out int index){
            index = 0;
            return transform.position;
        }

        public override void AppendSamples(Cable.SampledCable samples, Vector3 origin, float distance ,float spoolSeparation, bool reverse, bool orientation){
            return;
        }
    }
}


