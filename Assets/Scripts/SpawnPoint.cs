using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class SpawnPoint : Photon.MonoBehaviour, IPunObservable {

    public Transform Point {
        get { return transform.Find("SpawnPoint"); }
    }

    new Light light;
    float lightAngle;

    [Range(0, 5)]
    public float fadeTime;
    float startTime;

	// Use this for initialization
	void Start () {
        light = GetComponentInChildren<Light>();
        lightAngle = light.spotAngle;
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
            light.spotAngle = Mathf.Lerp(lightAngle, 0, (Time.time - startTime) / fadeTime);
        } else {
            light.spotAngle = lightAngle;
        }
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(light.spotAngle);
        } else {
            lightAngle = (float)stream.ReceiveNext();
        }
    }
}
