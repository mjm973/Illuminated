using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 * Grenade logic:
 * Bounce around for the specified seconds.
 * Freeze
 * Once frozen, stay there for another short time
 * Then, physics overlap sphere to see which items in Layer 8,
 * (which players will belong to)
 * are inside the Grenade's blast radius
 * Which is fun because you can hurt yourself too LMAO
 */


public class GrenadeManager : Photon.MonoBehaviour {

    public float grenadeFreezeDelay;
    public float grenadeExplosionDelay;

    // If you got hit by this grenade at point-blank range, how much damage would you take?
    public float maxHit;
    public float blastRadius;

    [Range(0f, 1f)]
    public float maxHitCutoff;

    // Position sync 
    Vector3 targetPos;

    // Reference to our grenade launcher
    GrenadeGunManager gun = null;
    // Public API allows it to be set only once
    public GrenadeGunManager Gun {
        get { return gun; }
        set {
            if (gun == null) {
                gun = value;
            }
        }
    }

    // Use this for initialization
    void Start() {
        // Set to kinematic in remote machines - we are lerping the position anyway
        if (!photonView.isMine) {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else {
            StartCoroutine(LifetimeCountdown());
        }
    }

    // Update is called once per frame
    void Update() {
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
        }
    }

    IEnumerator LifetimeCountdown() {
        // Once the bounce time is over, freeze in place
        yield return new WaitForSeconds(grenadeFreezeDelay);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Then once the explosion timer runs out, explode 
        yield return new WaitForSeconds(grenadeExplosionDelay);
        // Tell our gun that we are despawning
        if (gun) {
            gun.DespawnGrenade();
        }
        ExplosionDamage(transform.position, blastRadius);
        PhotonNetwork.Destroy(gameObject);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(transform.position);
        }
        else {
            targetPos = (Vector3)stream.ReceiveNext();
        }
    }

    void ExplosionDamage(Vector3 center, float blastRadius) {
        //Debug.Log("Boom!");
        int layer = 1 << 8; // bitshift to get to the correct layer
        Collider[] hitColliders = Physics.OverlapSphere(center, blastRadius, layer);
        Debug.Log("I hit " + hitColliders.Length + " players");
        foreach(Collider col in hitColliders) {
            PlayerManager hurtPlayer = col.GetComponent<PlayerManager>();

            // linear damage falloff
            float proximity = (center - hurtPlayer.transform.position).magnitude;
            Debug.Log("Proximity: " + proximity);
            float intensity = 1 - (proximity/blastRadius);
            Debug.Log("Intensity: " + intensity);

            // if the player is super close to the grenade, hit them for max damage
            if(intensity > maxHitCutoff) {
                Debug.Log("MAXIMUM DAMAGE!");
                hurtPlayer.Damage(maxHit);
            }
            else {
                Debug.Log("Hitting for " + (maxHit * intensity));
                hurtPlayer.Damage(maxHit * intensity);
            }
        }
    }
}
