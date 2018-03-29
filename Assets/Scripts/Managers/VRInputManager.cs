using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using VRTK;

public class VRInputManager : Photon.MonoBehaviour {

    [SerializeField]
    VRTK_ControllerEvents left;
    [SerializeField]
    VRTK_ControllerEvents right;

    GrenadeGunManager gun;

    // Use this for initialization
    void Start () {
        gun = GetComponentInChildren<GrenadeGunManager>();
        right.TriggerClicked += new ControllerInteractionEventHandler(OnTriggerClicked);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerClicked(object sender, ControllerInteractionEventArgs e) {
        gun.Fire();
    }
}
