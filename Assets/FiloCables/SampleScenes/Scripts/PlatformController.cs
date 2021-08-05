using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
	
    public float speed = 2;

	void Update () {
        if (Input.GetKey(KeyCode.DownArrow)){
            transform.Translate(0,-speed*Time.deltaTime,0,Space.World);
        }
        if (Input.GetKey(KeyCode.UpArrow)){
            transform.Translate(0,speed*Time.deltaTime,0,Space.World);
        }
	}
}
