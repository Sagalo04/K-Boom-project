using System;
using UnityEngine;

namespace Filo{
    public static class Utils
    {
        
        public static bool Catenary(Vector2 p1, Vector2 p2, float l, ref Vector2[] points){
    
            float r = p1.x;
            float s = p1.y;
            float u = p2.x;
            float v = p2.y;
    
            // swap values if p1 is to the right of p2:
            if (r > u){
                float temp = r;
                r = u;
                u = temp;
                temp = s;
                s = v;
                v = temp;
            }
    
            // find z:
            float z = 0.005f;
            float target = Mathf.Sqrt(l*l-(v-s)*(v-s))/(u-r);
            while((float)System.Math.Sinh(z)/z < target){
                z += 0.005f;
            }
            
            if (z > 0.005f){
    
                float a = (u-r)/2.0f/z;
                float p = (r+u-a*Mathf.Log((l+v-s)/(l-v+s)))/2.0f;
                float q = (v+s-l*(float)System.Math.Cosh(z)/(float)System.Math.Sinh(z))/2.0f;

                int segments = points.Length;
                float inc = (u-r)*(1.0f/(segments-1));
        
                for (int i = 0; i < segments; ++i)
                {
                    float x = r+inc*i;
                    points[i] = new Vector2(x,a*(float)System.Math.Cosh((x-p)/a)+q);
                }
                return true;
            }else{
                return false;
            }
        }

        public static bool Sinusoid(Vector3 origin, Vector3 direction, float l, uint frequency, ref Vector3[] points){
    
            float magnitude = direction.magnitude;

            if (magnitude > 1E-4){

                direction /= magnitude;
                Vector3 ortho = Vector3.Cross(direction,Vector3.forward);

                int segments = points.Length;
                float inc = magnitude / (segments - 1);

                float d = frequency * 4;
                float d2 = d*d;

                // analytic approx to amplitude from wave arc length.
                float amplitude = Mathf.Sqrt(l*l/d2 - magnitude*magnitude/d2);

                if (float.IsNaN(amplitude))
                    return false;
    
                for (int i = 0; i < segments; ++i)
                {
                    float pctg = i/(float)(segments-1);   
                    points[i] = origin + direction * inc * i + ortho * Mathf.Sin(pctg * Mathf.PI*2 * frequency) * amplitude;
                }
                
                return true;
            }else{
                return false;
            }           
        }
    
        /**
         * Modulo operator that also follows intuition for negative arguments. That is , -1 mod 3 = 2, not -1.
         */
        public static float Mod(float a,float b)
        {
            return a - b * Mathf.Floor(a / b);
        }

        public static Vector3 Rotate2D(this Vector3 v, float angle){
            return new Vector3(
                    v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle),
                    v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle),
                    v.z
                );
        }
    }
}

