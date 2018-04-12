﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class GrenadeGunManager : Photon.MonoBehaviour {

	/*
	 * Script for handling the grenade gun logic for the player.
	 * Grenade Gun Logic:
	 * When the player shoots (currently a shot is a click, eventually a Vive trigger pull), spawn and launch a grenade from the tip of the grenade gun
		 * In this PC-prototype, this might feel "wrong" because in non-VR FPS games the bullets actually spawn from the center of your vision, NOT your gun barrel.
		 * I'm spawning them from the tip of the gun so that it's easier to reuse code in the final game without need for much rewriting.
	 * Once a grenade has been launched, there's a "cooldown" until you're allowed to shoot again.
	 * You can only have N grenades active at a time. If you have N already active, nothing will happen when you click again.
	 * So, logic:
	 * OnClick:
	 * If less than N of my pellets in the game right now:
	 * 	Instantiate a grenade. Give it some impulse which is in whatever is that object's forward direction
	 */
    // There's a little empty Game Object just a tiny bit ahead of the grenade launcher's tip which is the position from which the grenades will be spawned.
    // We store its position so we know where to create our grenade.
	Vector3 grenadeSpawnLocation;
    
    // Maximum amount of grenades allowed per player
    // static because it should be the same for everyone
    static int grenadeLimit = 3;

    // Wrap it under a Property to allow only get
    public static int GrenadeLimit {
        get { return grenadeLimit; }
    }

    // Internal counter to track grenades shot
    int grenadeCounter = 0;

    Transform spawnTip;

    float dummy;

    // Reference to the grenade prefab
    // string now because Photon loads it from the resources folder
    // photon somehow screws up if it's a variable so I hard coded it
    //public string grenade = "Grenade";

    // How strong we push grenade
    public float forceMult;

	void Start () {
        spawnTip = transform.Find("Puppet (clone)").Find("Controller (right)").Find("GrenadeLauncher").Find("GrenadeSpawnPoint");

        grenadeSpawnLocation = spawnTip.position;
	}


	void Update () {
        if (spawnTip == null) {
            spawnTip = transform.Find("Puppet (clone)").Find("Controller (right)").Find("GrenadeLauncher").Find("GrenadeSpawnPoint");
        }
    }

	// Are there already three of my children grenades in the game?
    // i.e, of all the grenades in the game, how many of them have photon's isMine as true
	bool IsGrenadeSpawnValid() {
        // Check our grenade counter against the maximum allowed
        return grenadeCounter < grenadeLimit;
	}

	void SpawnGrenade(){
        // Instantiate a grenade at the location of the tip
        GameObject launchedGrenade = PhotonNetwork.Instantiate("Grenade", grenadeSpawnLocation, Quaternion.identity, 0);
        launchedGrenade.GetComponent<Rigidbody>().AddForce(spawnTip.forward * forceMult, ForceMode.Impulse);
        launchedGrenade.GetComponent<GrenadeManager>().Gun = this;

        // Increase our grenade counter on grenade spawn
        ++grenadeCounter;
	}

    // Public method to allow player to fire
    public void Fire() {
        if (IsGrenadeSpawnValid()) {
            grenadeSpawnLocation = spawnTip.position;
            SpawnGrenade();
        }
    }

    // Public method to allow grenades to report their de-spawn
    public void DespawnGrenade() {
        --grenadeCounter; 
    }
}
