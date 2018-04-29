using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using VRTK;

public class PuppetSpawner : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (PhotonNetwork.isMasterClient) {
            PhotonNetwork.Instantiate("GM", Vector3.zero, Quaternion.identity, 0);
        }

        GameObject puppet = PhotonNetwork.Instantiate ("Puppet", transform.position, Quaternion.identity, 0);
		puppet.transform.SetParent (transform);

		PlayerManager pm = puppet.GetComponent<PlayerManager> ();
		Transform head = transform.Find ("Camera (eye)");
		Transform right = transform.Find ("Controller (right)");
        Transform left = transform.Find("Controller (left)");

		pm.pHead = head;
		pm.pRight = right;
        pm.pLeft = left;

        GameObject rController = right.Find("test_Right").gameObject;

        VRTK_ControllerReference rRef = new VRTK_ControllerReference(rController);
        VRTK_ControllerReference lRef = new VRTK_ControllerReference(left.gameObject);
        pm.rightRef = rRef;
        pm.leftRef = lRef;

        pm.rCont = right.GetComponent<SteamVR_TrackedObject>();
        pm.lCont = left.GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
