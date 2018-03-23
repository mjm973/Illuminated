using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

// Class to sync player to network
public class PlayerManager : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // update gameobject of we don't own it
		if (!photonView.isMine) {

        }
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) { // write to network

        } else { // read from network

        }
    }
}
