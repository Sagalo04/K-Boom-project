using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Filo;

[RequireComponent(typeof(Cable))]
public class RuntimeCableGenerator : MonoBehaviour {

    Cable cable;
    public GameObject pulleyPrefab;
    public GameObject attachmentPrefab;
    public int numPulleys = 25;

	// Use this for initialization
	void Awake () {
        cable = GetComponent<Cable>();
        Generate();
	}
	
	// Update is called once per frame
	void Generate () {

        if (pulleyPrefab != null && attachmentPrefab != null)

        cable.links = new Cable.Link[numPulleys];
        Vector3 position = Vector3.zero;

        // first and last links are attachments:
        GameObject body = GameObject.Instantiate(attachmentPrefab);
        body.GetComponent<Rigidbody>().mass = 2;
        body.transform.position = position;
        Cable.Link link = new Cable.Link();
        link.type = Cable.Link.LinkType.Attachment;
        link.body = body.GetComponent<CableBody>();
        link.outAnchor = Vector3.up*0.5f;
        cable.links[0] = link;

        position += new Vector3(0.5f,3,0);

        for (int i = 1; i < cable.links.Length-1; ++i){

            body = GameObject.Instantiate(pulleyPrefab);
            body.transform.position = position;

            link = new Cable.Link();
            link.type = Cable.Link.LinkType.Rolling;
            link.body = body.GetComponent<CableBody>();
            link.orientation = i%2 == 0;

            cable.links[i] = link;
        
            position += new Vector3(1,(i%2 == 0)?3:-3,0);
        }

        body = GameObject.Instantiate(attachmentPrefab);
        body.transform.position = position + new Vector3(0,-10,0);
        link = new Cable.Link();
        link.type = Cable.Link.LinkType.Attachment;
        link.body = body.GetComponent<CableBody>();
        link.inAnchor = Vector3.up*0.5f;
        cable.links[cable.links.Length-1] = link;

	}
}
