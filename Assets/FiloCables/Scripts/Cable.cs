using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Filo{

    [ExecuteInEditMode]
    [AddComponentMenu("Filo Cables/Cable")]
    public class Cable : MonoBehaviour {
    
        [Serializable]
        public struct Link{
    
            public enum LinkType{
                Attachment,
                Rolling,
                Pinhole,
                Hybrid
            }
    
            public CableBody body;
            public LinkType type;
            public bool orientation;

            public bool hybridRolling;
            public float storedCable;
            public float spoolSeparation;

            public Vector3 inAnchor;
            public Vector3 outAnchor;
        }

        public class SampledCable{

            private List<List<Vector3>> segments = new List<List<Vector3>>();
            private int segmentCount = 0;
            private float length = 0;
            private Vector3 lastSample = Vector3.zero;

            public IList<List<Vector3>> Segments{
                get{return segments;}
            }

            public float Length{
                get{return length;}
            }

            public void AppendSample(Vector3 sample, bool accumulateLength = true){

                // If this is not the first sample, update the sampled cable length:
                if (accumulateLength && segmentCount > 0 && segments[0].Count > 0)
                    length += Vector3.Distance(sample,lastSample);

                // Add the first segment, if the list is empty.
                if (segmentCount == 0)
                    segmentCount = 1;

                if(segments.Count == 0)
                    segments.Add(new List<Vector3>());

                // Append sample to last segment:
                segments[segmentCount-1].Add(sample);
                lastSample = sample;
            }

            public void ReverseLastSamples(int count){
                if (segmentCount > 0){

                    List<Vector3> segment = segments[segmentCount-1];
                    if (count <= segment.Count){

                        segment.Reverse(segment.Count-count,count);

                        for (int i = segment.Count - count; i < segment.Count-1; ++i){
                            length += Vector3.Distance(segment[i],segment[i+1]);
                        }   

                        lastSample = segment[segment.Count-1];

                    }
                }
            }

            public void NewSegment(){

                // Increase our segment count. Only if it is larger than the length of the segment list,
                // add a new segment. This allows to reuse empty segments from previous frames and 
                // gets rid of garbage generation.

                segmentCount++;
                if (segments.Count < segmentCount)
                    segments.Add(new List<Vector3>());
            }

            public void Clear(){

                // Do not clear the segment list. Instead, clear each segment and leave it empty.
                // This gets rid of garbage generation.
                for (int i = 0; i < segments.Count; ++i)    
                    segments[i].Clear();

                // Reset segment count and cable length:
                segmentCount = 0;
                length = 0;
            }

            public void Close(){

                // Re-add the first sample at the end of the cable:
                if (segmentCount > 0 && segments[0].Count > 0){
                    segments[segmentCount-1].Add(segments[0][0]);
                }
            }
            
        }
    
        [Range(0,1)] 
        [Tooltip("Percentage of loose cable visually represented. Set it to zero to deactivate cable looseness. The amount of loose cable is first clamped to maxLooseCable, then multiplied by loosenessScale.")]
        public float loosenessScale = 1;
        [Tooltip("Maximum amount of loose cable per cable segment, in meters. Set it to zero to deactivate cable looseness. The amount of loose cable is first clamped to maxLooseCable, then multiplied by loosenessScale.")]
        public float maxLooseCable = 0.25f;
        
        [Tooltip("Maximum distance between two cable links (projected in the XZ plane) to consider the cable segment vertical.")]
        public float verticalThreshold = 0.25f;
        [Tooltip("Frequency of the sine curve used to represent vertical cables.")]
        public uint verticalCurlyness = 1;

        public Link[] links = null;
        [HideInInspector][SerializeField] private CableJoint[] joints = null;
        [HideInInspector][SerializeField] private float restLength = 0;

        private Vector2[] catenaryBuffer = new Vector2[16];
        private Vector3[] sinusoidBuffer = new Vector3[24];

        [HideInInspector] public SampledCable sampledCable = new SampledCable();
    
        public float RestLength{
            get{return restLength;}
        }

        public int JointCount{
            get{return joints.Length;}
        }

        void Start(){ 
           Setup();
        }

        void OnValidate(){
            verticalThreshold = Mathf.Max(1E-4f, verticalThreshold);
            Setup();
        }
        
        public void OnDrawGizmosSelected(){

            if (joints == null) return;

            Gizmos.color = Color.cyan;

            for (int i = 0; i < joints.Length; ++i){

                if (joints[i] != null){
                    Vector3 pos1 = joints[i].WorldSpaceAttachment1;
                    Vector3 pos2 = joints[i].WorldSpaceAttachment2;
    
                    Gizmos.DrawLine(pos1,pos2);
                    Gizmos.DrawWireSphere(pos1,0.02f);
                    Gizmos.DrawWireSphere(pos2,0.02f);
                }
            }
   
        }

        public void Setup(){

            joints = null;
            if (links != null && links.Length > 0){

                joints = new CableJoint[links.Length-1];
    
                for (int i = 0; i < links.Length-1; ++i){
        
                    if (links[i].body != null && links[i+1].body != null){
        
                        Vector3 t1,t2;
                        FindCommonTangents(links[i],links[i+1],out t1, out t2);

                        t1 -= links[i].body.GetCablePlaneNormal() * links[i].storedCable * links[i].spoolSeparation;
                        t2 -= links[i+1].body.GetCablePlaneNormal() * links[i+1].storedCable * links[i+1].spoolSeparation;
                        
                        joints[i] = new CableJoint(links[i].body,links[i+1].body, 
                                                   links[i].body.transform.InverseTransformPoint(t1),
                                                   links[i+1].body.transform.InverseTransformPoint(t2),
                                                   (t2-t1).magnitude);
                    }
                    
                }
            }

            CalculateRestLength();
        }

        private void CalculateRestLength(){

            restLength = 0;
            if (joints == null) return;

            bool closed = (links[0].body == links[links.Length-1].body);

            for (int i = 0; i < links.Length; ++i){

                if (links[i].body != null){

                    CableJoint prevJoint = GetPreviousJoint(i,closed);
                    CableJoint nextJoint = GetNextJoint(i,closed);
    
                    if (nextJoint != null && prevJoint != null && (!(i == 0 && closed))){

                        links[i].storedCable = Mathf.Abs(links[i].body.SurfaceDistance(links[i].body.WorldSpaceToCablePlane(prevJoint.WorldSpaceAttachment2),
                                                                                       links[i].body.WorldSpaceToCablePlane(nextJoint.WorldSpaceAttachment1),
                                                                                       links[i].orientation));
                        restLength += links[i].storedCable;

                    }else if (links[i].type == Link.LinkType.Hybrid){

                        restLength += links[i].storedCable;

                        if (nextJoint != null){
                            Vector2 tangent = links[i].body.WorldSpaceToCablePlane(nextJoint.WorldSpaceAttachment1);
                            int j = 0; 
   
                            links[i].outAnchor = links[i].body.transform.InverseTransformPoint(links[i].body.SurfacePointAtDistance(tangent,links[i].storedCable,links[i].orientation,out j));
                        }else if (prevJoint != null){
                            Vector2 tangent = links[i].body.WorldSpaceToCablePlane(prevJoint.WorldSpaceAttachment2);
                            int j = 0; 
                            links[i].inAnchor = links[i].body.transform.InverseTransformPoint(links[i].body.SurfacePointAtDistance(tangent,links[i].storedCable,!links[i].orientation,out j));
                        }
                    }
    
                    if (i < links.Length-1 && joints[i] != null){
                        restLength += joints[i].restLength;
                    }
                }

            }
        }

        private void SampleLink(CableJoint prevJoint, Link link, CableJoint nextJoint){

            Vector3? t1 = null, t2 = null;

            if (prevJoint != null)
               t1 = prevJoint.body2.WorldToCable(prevJoint.WorldSpaceAttachment2); 
            if (nextJoint != null)
               t2 = nextJoint.body1.WorldToCable(nextJoint.WorldSpaceAttachment1);

            // Rolling links:
            if (link.type == Link.LinkType.Rolling || link.type == Link.LinkType.Hybrid){
    
                if (t1.HasValue && t2.HasValue){

                    float distance = link.body.SurfaceDistance(t1.Value,t2.Value,!link.orientation,false);
                    link.body.AppendSamples(sampledCable,t1.Value,distance,link.spoolSeparation,false,link.orientation);

                }else if (t1.HasValue){
                    link.body.AppendSamples(sampledCable,t1.Value,link.storedCable,link.spoolSeparation,false,link.orientation);
                }else if (t2.HasValue){
                    link.body.AppendSamples(sampledCable,t2.Value,link.storedCable,link.spoolSeparation,true,link.orientation);
                }

            }
            // Attachment and pinhole links:        
            else{ 

                if (t1.HasValue)
                    sampledCable.AppendSample(prevJoint.body2.transform.TransformPoint(link.inAnchor));

                if (t1.HasValue && t2.HasValue && t1.Value != t2.Value)
                    sampledCable.NewSegment();

                if (t2.HasValue)
                    sampledCable.AppendSample(nextJoint.body1.transform.TransformPoint(link.outAnchor));
            }
        }

        private void SampleJoint(CableJoint joint){

            Vector3 p1 = joint.WorldSpaceAttachment1;
            Vector3 p2 = joint.WorldSpaceAttachment2;
            
            if (joint.length < joint.restLength){

                Vector3 point = p2-p1;
                Vector3 dir = Vector3.Scale(point, new Vector3(1,0,1));
            
                if (loosenessScale > 0){

                    float sampledLength = Mathf.Lerp(joint.length,Mathf.Min(joint.restLength, joint.length + maxLooseCable),loosenessScale);

                    if (dir.sqrMagnitude > verticalThreshold){

                        Quaternion rot = Quaternion.LookRotation(dir);
                        Quaternion irot = Quaternion.Inverse(rot);
                        Vector3 n = irot*point;
                        
                        if (Utils.Catenary(Vector2.zero,new Vector2(n.z,n.y),sampledLength,ref catenaryBuffer)){
                            for (int j = 1; j < catenaryBuffer.Length-1; ++j){
                                sampledCable.AppendSample(p1 + rot * new Vector3(0,catenaryBuffer[j].y,  catenaryBuffer[j].x));
                            }
                        }
    
                    }else{
                        
                        if (Utils.Sinusoid(p1,point,sampledLength,verticalCurlyness,ref sinusoidBuffer)){
                            for (int j = 1; j < sinusoidBuffer.Length-1; ++j){
                                sampledCable.AppendSample(sinusoidBuffer[j]);
                            }
                        }
                    }
                }

            }

        }
    
        private void FindCommonTangents(Link link1, Link link2, out Vector3 t1, out Vector3 t2){

            // Pick a random point in each shape:
            t1 = link1.body.RandomHullPoint();
            t2 = link2.body.RandomHullPoint();

            Vector3 prevT1,prevT2;

            // iterate: find a tangent point on the other body until both tangent points remain unchanged.
            do{

                prevT1 = t1;
                prevT2 = t2;

                if (link2.type == Link.LinkType.Attachment || link2.type == Link.LinkType.Pinhole || (link2.type == Link.LinkType.Hybrid && !link2.hybridRolling))
                    t2 = link2.body.transform.TransformPoint(link2.inAnchor);
                else
                    t2 = link2.body.GetWorldSpaceTangent(t1,link2.orientation);

                if (link1.type == Link.LinkType.Attachment || link1.type == Link.LinkType.Pinhole || (link1.type == Link.LinkType.Hybrid && !link1.hybridRolling))
                    t1 = link1.body.transform.TransformPoint(link1.outAnchor);
                else
                    t1 = link1.body.GetWorldSpaceTangent(t2,!link1.orientation);

            }while ((prevT1 - t1).sqrMagnitude > 1E-6 || 
                    (prevT2 - t2).sqrMagnitude > 1E-6 );
            
        }
    
    
        private void UpdatePinhole(CableJoint joint1, CableJoint joint2){
    
            if (joint1 != null && joint2 != null){

                float restLenght1 = joint1.restLength;
                float restLength2 = joint2.restLength;
    
                if (joint1.length > restLenght1){
                    float delta = joint1.length - restLenght1;
                    joint1.restLength += delta;
                    joint2.restLength -= delta;
                }
                if (joint2.length > restLength2){
                    float delta = joint2.length - restLength2;
                    joint1.restLength -= delta;
                    joint2.restLength += delta;
                }
            }
        }

        private void UpdatePinholes(){
            for (int i = 1; i < links.Length-1; ++i){
                if (links[i].body != null && joints[i-1] != null && joints[i] != null){
                    if (links[i].type == Link.LinkType.Pinhole)
                        UpdatePinhole(joints[i-1],joints[i]);
                }
            }
        }

        private void UpdateHybridLink(ref Link link, bool cableGoesIn, Vector3 attachment){

            if (link.storedCable < 0 && link.hybridRolling){

                // Switch to attachment mode:
                link.hybridRolling = false;

                // Update joints again, since joint attachment points have now changed.
                UpdateJoints();

            }else if (!link.hybridRolling){

                // Find positive and negative attachment points:
                Vector3 tplus =  link.body.GetWorldSpaceTangent(attachment,false);
                Vector3 tminus = link.body.GetWorldSpaceTangent(attachment,true);

                // Current cable space attachment point:
                Vector2 t = link.body.WorldSpaceToCablePlane(link.body.transform.TransformPoint(cableGoesIn ? link.inAnchor : link.outAnchor));
            
                // Calculate distance to positive and negative attachment points:
                float d1 = link.body.SurfaceDistance(link.body.WorldSpaceToCablePlane(tplus), t,false);
                float d2 = link.body.SurfaceDistance(link.body.WorldSpaceToCablePlane(tminus),t,false);

                // In case the attachment point exceeds either tangent, go back to rolling mode and adapt orientation accordingly:
                if (d1 < 0 || d2 > 0){

                    // Pick the closest tangent. This avoids issues when both distances change sign at the same time.
                    if (Mathf.Abs(d1) < Mathf.Abs(d2)){
                        link.hybridRolling = true;
                        link.orientation = !cableGoesIn;
                    }else{
                        link.hybridRolling = true;
                        link.orientation = cableGoesIn;
                    }
                }

            }
        }

        private void UpdateHybridLinks(){

            // Only first and last link can be hybrid, update them:
            if (links[0].body != null && links[0].type == Link.LinkType.Hybrid) 
                UpdateHybridLink(ref links[0], false,joints[0].WorldSpaceAttachment2);

            if (links.Length > 1){
                int lastLink = links.Length-1;
                if (links[lastLink].body != null && links[lastLink].type == Link.LinkType.Hybrid) 
                    UpdateHybridLink(ref links[lastLink], true,joints[joints.Length-1].WorldSpaceAttachment1);
            } 
        }

        private void UpdateJoints(){

            for (int i = 0; i < joints.Length; ++i){

                if (joints[i] != null && links[i].body != null && links[i+1].body != null){
    
                    CableJoint joint = joints[i];

                    Vector3 t1,t2;
                    FindCommonTangents(links[i],links[i+1], out t1, out t2);
        
                    Vector2 currentT1 = joint.body1.WorldSpaceToCablePlane(joint.WorldSpaceAttachment1); 
                    Vector2 currentT2 = joint.body2.WorldSpaceToCablePlane(joint.WorldSpaceAttachment2); 
            
                    // Get surface distances between old and new tangents:
                    float d1 = joint.body1.SurfaceDistance(currentT1,joint.body1.WorldSpaceToCablePlane(t1),links[i].orientation);
                    float d2 = joint.body2.SurfaceDistance(currentT2,joint.body2.WorldSpaceToCablePlane(t2),links[i+1].orientation);

                    // Update stored lengths:
                    links[i].storedCable   -= d1;
                    links[i+1].storedCable += d2;

                    // Update rest lengths:
                    joint.restLength += d1; 
                    joint.restLength -= d2; 

                    // Update tangent points:
                    joint.offset1 = joint.body1.transform.InverseTransformPoint(t1 - joint.body1.GetCablePlaneNormal() * links[i].storedCable * links[i].spoolSeparation);
                    joint.offset2 = joint.body2.transform.InverseTransformPoint(t2 - joint.body2.GetCablePlaneNormal() * links[i+1].storedCable * links[i+1].spoolSeparation);
                    
                }
    
            }    
        }

        private void InitializeJoints(){
            for (int i = 0; i < joints.Length; ++i){
                if (joints[i] != null)
                    joints[i].Initialize(); 
            }
        }
        
        public void UpdateCable () { 

            if (joints == null) return;

            UpdateJoints();

            UpdateHybridLinks();   
    
            InitializeJoints();
    
            UpdatePinholes();
            
        }

        public void Solve(float deltaTime, float bias){

            if (joints == null) return;

            for (int i = 0; i < joints.Length; ++i){
                if (joints[i] != null)
                    joints[i].Solve(deltaTime, bias);
            }
            for (int i = 0; i < links.Length; ++i){
                if (links[i].body != null)
                    links[i].body.ApplyFreezing();
            }
        }

        private CableJoint GetPreviousJoint(int linkIndex, bool closed){
            if (linkIndex > 0){
                return joints[linkIndex-1]; 
            }else if (closed && joints.Length > 0){
                return joints[joints.Length-1];
            }
            return null;
        } 

        private CableJoint GetNextJoint(int linkIndex, bool closed){
            if (linkIndex < joints.Length){
                return joints[linkIndex];
            }else if (closed && joints.Length > 0){
                return joints[0];
            }
            return null;
        }
        
        private void Update(){
        
            sampledCable.Clear();
            
            if (joints == null || links.Length == 0)
                return;

            bool closed = (links[0].body == links[links.Length-1].body);
        
            for (int i = 0; i < links.Length; ++i){

                if (links[i].body != null){

                    CableJoint prevJoint = GetPreviousJoint(i,closed);
                    CableJoint nextJoint = GetNextJoint(i,closed);
    
                    // Sample the link, except if the cable is closed and this is the first link.
                    if (!(i == 0 && closed) || links[i].type == Link.LinkType.Attachment || links[i].type == Link.LinkType.Pinhole)
                        SampleLink(prevJoint,links[i],nextJoint);
    
                    // Sample the joint (only adds sample points if cable is not tense):
                    if (i < joints.Length && joints[i] != null)
                        SampleJoint(joints[i]);
                }

            }

            if (closed){
                sampledCable.Close();
            }

        }

    }
}
