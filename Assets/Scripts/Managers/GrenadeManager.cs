using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 * Grenade logic:
 * Bounce around for the specified seconds and then die
 */


public class GrenadeManager : Photon.MonoBehaviour {


    public float grenadeDespawnDelay;

	// Use this for initialization
	void Start () {
        StartCoroutine(DeathDelay());
        Debug.Log("I exist mmkay");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(grenadeDespawnDelay);
        Debug.Log("Boom!");
        Destroy(gameObject);
    }
}
