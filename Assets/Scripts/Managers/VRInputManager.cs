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
    public bool allowInput = true;

    static VRInputManager instance;
    public static VRInputManager Instance {
        get {
            return instance;
        }
    }

    // Use this for initialization
    void Start () {
        gun = GetComponentInChildren<GrenadeGunManager>();
        right.TriggerClicked += new ControllerInteractionEventHandler(OnTriggerClicked);

        left.GripPressed += new ControllerInteractionEventHandler(OnGripPressed);
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerClicked(object sender, ControllerInteractionEventArgs e) {
        if (allowInput) {
            gun.Fire();
        }
    }

    void OnGripPressed(object sender, ControllerInteractionEventArgs e) {
        GameManager.GM.StartGame();
    }
}
