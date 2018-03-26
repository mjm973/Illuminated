using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeGunManager : MonoBehaviour {

	/*
	 * Script for handling the grenade gun logic for the player.
	 * Grenade Gun Logic:
	 * When the player shoots (currently a shot is a click, eventually a Vive trigger pull), spawn and launch a grenade from the tip of the grenade gun
		 * In this PC-prototype, this might feel "wrong" because in non-VR FPS games the bullets actually spawn from the center of your vision, NOT your gun barrel.
		 * I'm spawning them from the tip of the gun so that it's easier to reuse code in the final game without need for much rewriting.
	 * Once a grenade has been launched, there's a "cooldown" until you're allowed to shoot again.
	 * You can only have three grenades active at a time. If you have three already active, nothing will happen when you click again.
	 */




	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
