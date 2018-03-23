using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


// Class to manage player input
public class InputManager : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // allow input only if we own the gameobject
		if (photonView.isMine) {
            
        }
	}
}
