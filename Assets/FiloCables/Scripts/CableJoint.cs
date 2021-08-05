using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Filo{

    public class CableJoint {
    
        public CableBody body1;
        public Vector3 offset1;    
        
        public CableBody body2;
        public Vector3 offset2;
       
        [HideInInspector] public float length = 0;
        public float restLength = 1;
    
        private Rigidbody rb1;
        private Rigidbody rb2;

        private float totalLambda = 0;

        private float invMass1;
        private float invMass2;
        private Matrix4x4 invInertiaTensor1;
        private Matrix4x4 invInertiaTensor2;

        private Vector3 worldOffset1;
        private Vector3 worldOffset2;
        private Vector3 r1;
        private Vector3 r2;
        private Vector3 jacobian;
        private float k;    

        public Vector3 WorldSpaceAttachment1{
            get{return body1 != null ? body1.transform.TransformPoint(offset1) : Vector3.zero;}
        }
        
        public Vector3 WorldSpaceAttachment2{
            get{return body2 != null ? body2.transform.TransformPoint(offset2) : Vector3.zero;}
        }
    
        public CableJoint(CableBody body1, CableBody body2, Vector3 offset1, Vector3 offset2, float restLength){
            this.body1 = body1;
            this.body2 = body2;
            this.rb1 = body1.GetRigidbody();
            this.rb2 = body2.GetRigidbody();
            this.offset1 = offset1;
            this.offset2 = offset2;
            this.restLength = restLength;
        }

        public void Initialize(){

            totalLambda = 0;

            if (body1 == null || body2 == null) return;

            worldOffset1 = body1.transform.TransformPoint(offset1);
            worldOffset2 = body2.transform.TransformPoint(offset2);

            Vector3 vector = worldOffset2 - worldOffset1;
            length = vector.magnitude;
            jacobian = vector/(length + 0.00001f); 

            invInertiaTensor1 = Matrix4x4.zero;
            invInertiaTensor2 = Matrix4x4.zero;

            if (rb1 != null){
                Vector3 invInertia1 = rb1.inertiaTensorRotation * new Vector3(rb1.inertiaTensor.x > 0?1.0f/rb1.inertiaTensor.x:0,
                                                                              rb1.inertiaTensor.y > 0?1.0f/rb1.inertiaTensor.y:0,
                                                                              rb1.inertiaTensor.z > 0?1.0f/rb1.inertiaTensor.z:0);
    
                Matrix4x4 m = Matrix4x4.Rotate(rb1.rotation);
                invInertiaTensor1[0,0] = invInertia1.x;
                invInertiaTensor1[1,1] = invInertia1.y;
                invInertiaTensor1[2,2] = invInertia1.z;
                invInertiaTensor1[3,3] = 1;
                invInertiaTensor1 = m * invInertiaTensor1 * m.transpose;
            }
    
            if (rb2 != null){
                Vector3 invInertia2 = rb2.inertiaTensorRotation * new Vector3(rb2.inertiaTensor.x > 0?1.0f/rb2.inertiaTensor.x:0,
                                                                              rb2.inertiaTensor.y > 0?1.0f/rb2.inertiaTensor.y:0,
                                                                              rb2.inertiaTensor.z > 0?1.0f/rb2.inertiaTensor.z:0);
        
                Matrix4x4 m2 = Matrix4x4.Rotate(rb2.rotation);
                invInertiaTensor2[0,0] = invInertia2.x;
                invInertiaTensor2[1,1] = invInertia2.y;
                invInertiaTensor2[2,2] = invInertia2.z;
                invInertiaTensor2[3,3] = 1;
                invInertiaTensor2 = m2 * invInertiaTensor2 * m2.transpose;
            }

            invMass1 = 0;
            invMass2 = 0;
            float w1 = 0,w2 = 0;

            if (rb1 != null && !rb1.isKinematic)
            {
                invMass1 = 1.0f/rb1.mass;
                r1 = worldOffset1 - rb1.worldCenterOfMass;
                w1 = Vector3.Dot(Vector3.Cross(invInertiaTensor1.MultiplyVector(Vector3.Cross(r1,jacobian)),r1),jacobian);
            }

            if (rb2 != null && !rb2.isKinematic)
            {
                invMass2 = 1.0f/rb2.mass;
                r2 = worldOffset2 - rb2.worldCenterOfMass;
                w2 = Vector3.Dot(Vector3.Cross(invInertiaTensor2.MultiplyVector(Vector3.Cross(r2,jacobian)),r2),jacobian);
            }
           
            k = invMass1 + invMass2 + w1 + w2;
        }
        
        public void Solve (float deltaTime, float bias) { 
    
            // position constraint: distance between attachment points minus rest distance must be zero.   
            float c = length - restLength;

            if (body1 != null && body2 != null && c > 0 && k > 0)
            {
                // calculate the relative velocity of both attachment points:
                Vector3 relVel = (rb2 != null ? rb2.GetPointVelocity(worldOffset2):Vector3.zero) - 
                                 (rb1 != null ? rb1.GetPointVelocity(worldOffset1):Vector3.zero);
    
                // velocity constraint: velocity along jacobian must be zero.
                float cDot = Vector3.Dot(relVel,jacobian);  
    
                // calculate constraint force intensity:  
                float lambda = (- cDot - c * bias/deltaTime) / k;

                // accumulate and clamp impulse:
                float tempLambda = totalLambda;
                totalLambda = Mathf.Min(0,totalLambda + lambda);
                lambda = totalLambda - tempLambda;
        
                // apply impulse to both rigidbodies:
                Vector3 impulse = jacobian * lambda;

                if (rb1 != null && !rb1.isKinematic){
                    rb1.velocity -= impulse * invMass1;
                    rb1.angularVelocity -= invInertiaTensor1.MultiplyVector(Vector3.Cross(r1,impulse));   
                } 
        
                if (rb2 != null && !rb2.isKinematic){
                    rb2.velocity += impulse * invMass2;
                    rb2.angularVelocity += invInertiaTensor2.MultiplyVector(Vector3.Cross(r2,impulse)); 
                } 
            }
    
    	}

    }
}
