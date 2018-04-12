using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PuppetSpawner : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject puppet = PhotonNetwork.Instantiate("Puppet", transform.position, Quaternion.identity, 0);
        puppet.transform.SetParent(transform);

        PlayerManager pm = puppet.GetComponent<PlayerManager>();
        Transform head = transform.Find("Camera (head)");
        Transform right = transform.Find("Controller (right)");

        pm.pHead = head;
        pm.pRight = right;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
