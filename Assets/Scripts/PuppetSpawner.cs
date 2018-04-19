﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using VRTK;

public class PuppetSpawner : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject puppet = PhotonNetwork.Instantiate ("Puppet", transform.position, Quaternion.identity, 0);
		puppet.transform.SetParent (transform);

		PlayerManager pm = puppet.GetComponent<PlayerManager> ();
		Transform head = transform.Find ("Camera (eye)");
		Transform right = transform.Find ("Controller (right)");
        Transform left = transform.Find("Controller (left)");

		pm.pHead = head;
		pm.pRight = right;
        pm.pLeft = left;

        VRTK_ControllerReference rRef = new VRTK_ControllerReference(right.gameObject);
        VRTK_ControllerReference lRef = new VRTK_ControllerReference(left.gameObject);
        pm.rightRef = rRef;
        pm.leftRef = lRef;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
