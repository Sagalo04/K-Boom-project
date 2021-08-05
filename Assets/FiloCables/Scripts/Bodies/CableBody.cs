using UnityEngine;
using System;
using System.Collections.Generic;

namespace Filo{

    public abstract class CableBody : MonoBehaviour
    {
        public enum CablePlane{
            XY,
            XZ,
            YZ
        }
    
        public CablePlane plane;
        public bool freezeRotation = true;
        private Rigidbody rbody = null;
    
        private void Awake()
        {
            FindRigidbody();
        }

        private void FindRigidbody(){
            rbody = GetComponentInParent<Rigidbody>();
            CalculateInertiaTensor();
        }

        public Rigidbody GetRigidbody(){
            return rbody;
        }

        /**
         * Transforms a world space point to cable space.
         */
        public Vector3 WorldToCable(Vector3 wsPoint){
    
            Vector3 lsPoint = transform.InverseTransformPoint(wsPoint);
    
            switch(plane){
                case CablePlane.XY: return lsPoint;
                case CablePlane.XZ: return new Vector3(lsPoint.x,lsPoint.z,lsPoint.y); 
                case CablePlane.YZ: return new Vector3(lsPoint.y,lsPoint.z,lsPoint.x); 
                default: return Vector3.zero; 
            }
        }

        /**
         * Transforms a cable space point to world space.
         */
        public Vector3 CableToWorld(Vector3 cablePoint){
    
            Vector3 lsPoint;
            switch(plane){
                case CablePlane.XY: lsPoint = cablePoint; break;
                case CablePlane.XZ: lsPoint = new Vector3(cablePoint.x,cablePoint.z,cablePoint.y); break;
                case CablePlane.YZ: lsPoint = new Vector3(cablePoint.z,cablePoint.x,cablePoint.y); break;
                default: lsPoint = Vector3.zero; break;
            }
    
            return transform.TransformPoint(lsPoint);
            
        }

        /**
         * Projects a 3D world-space point to the local-space 2D cable plane.
         */
        public Vector2 WorldSpaceToCablePlane(Vector3 wsPoint){
    
            Vector3 lsPoint = transform.InverseTransformPoint(wsPoint);
    
            switch(plane){
                case CablePlane.XY: return new Vector2(lsPoint.x,lsPoint.y); 
                case CablePlane.XZ: return new Vector2(lsPoint.x,lsPoint.z); 
                case CablePlane.YZ: return new Vector2(lsPoint.y,lsPoint.z); 
                default: return Vector2.zero; 
            }
        }
    
        /**
         * Transforms a local-space 2D cable plane to world space.
         */
        public Vector3 CablePlaneToWorldSpace(Vector2 cablePoint){
    
            Vector3 lsPoint;
            switch(plane){
                case CablePlane.XY: lsPoint = new Vector3(cablePoint.x,cablePoint.y,0); break;
                case CablePlane.XZ: lsPoint = new Vector3(cablePoint.x,0,cablePoint.y); break;
                case CablePlane.YZ: lsPoint = new Vector3(0,cablePoint.x,cablePoint.y); break;
                default: lsPoint = Vector3.zero; break;
            }
    
            return transform.TransformPoint(lsPoint);
            
        }

        public Vector3 GetWorldSpaceTangent(Vector3 origin, bool orientation){
            return CablePlaneToWorldSpace(GetLeftOrRightMostPointFromOrigin(WorldSpaceToCablePlane(origin),orientation));
        }

        public Vector3 GetCablePlaneNormal(){

            switch(plane){
                case CablePlane.XY: return transform.forward; 
                case CablePlane.XZ: return transform.up; 
                case CablePlane.YZ: return transform.right; 
                default: return Vector3.zero; 
            }
   
        }

        public virtual void ApplyFreezing(){

            if (rbody != null){
    
                // project angular velocity onto world space inertia axis (which is orthogonal to cable plane):
                rbody.angularVelocity = Vector3.Project(rbody.angularVelocity,GetCablePlaneNormal());
            }
   
        }


        public virtual void CalculateInertiaTensor(){

            if (rbody != null){

                rbody.ResetInertiaTensor();

                if (freezeRotation)
                switch(plane){
                    case CablePlane.XY: rbody.inertiaTensor = new Vector3(Mathf.Infinity,Mathf.Infinity,rbody.inertiaTensor.z); break;
                    case CablePlane.XZ: rbody.inertiaTensor = new Vector3(Mathf.Infinity,rbody.inertiaTensor.y,Mathf.Infinity); break;
                    case CablePlane.YZ: rbody.inertiaTensor = new Vector3(rbody.inertiaTensor.x,Mathf.Infinity,Mathf.Infinity); break;
                }
            }
        }
    
        /**
         * Returns a worldspace point that lies in the 2D convex hull of the body. 
         */
        public abstract Vector3 RandomHullPoint();
    
        /**
         * Returns the left or rightmost visible point in the cable plane convex hull from a given origin.
         */
        public abstract Vector2 GetLeftOrRightMostPointFromOrigin(Vector2 origin, bool orientation);
    
        /**
         * Returns the distance along the convex hull surface between two points in the cable plane.
         * The sign of the distance depends on body orientation.
         */
        public abstract float SurfaceDistance(Vector2 p1, Vector2 p2,bool orientation,bool shortest = true);

        public abstract Vector3 SurfacePointAtDistance(Vector3 origin, float distance, bool orientation, out int index);

        public abstract void AppendSamples(Cable.SampledCable samples, Vector3 origin, float distance , float spoolSeparation,bool reverse, bool orientation);
    }
}

