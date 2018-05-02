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

    #region Grenade Variables

    public LayerMask layer;

    public float grenadeFreezeDelay;
    public float grenadeExplosionDelay;

    [Tooltip("Damage inflicted at point-blank range")]
    public float maxHit;
    public float blastRadius;

    [Range(0f, 1f)]
    public float maxHitCutoff;

    #endregion

    #region Light Variables

    [Range(0f, 20f)]
    [Tooltip("Target radius for the light source")]
    public float lightRadius = 10f;

    [Range(0f, 1f)]
    [Tooltip("0 = Light never grows // 1 = Light reaches lightRadius instantly")]
    public float lerpFactor = 0.1f;

    new Light light;

    #endregion

    #region Photon Sync

    Vector3 targetPos;
    float targetRadius;

    #endregion

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

        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update() {
        if (!photonView.isMine) {
            SyncGrenade();
        } else {
            UpdateGrenade();
        }
    }

    // updates network copies of the grenade
    void SyncGrenade() {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
        float r = Mathf.Lerp(light.range, targetRadius, 0.5f);
        light.range = r;
        light.intensity = r;
    }

    // updates master copy of the grenade
    void UpdateGrenade() {
        float r = light.range;

        if (lightRadius - r > 0.01f) {
            // Lerp light range towards our target (exponential approach)
            r = Mathf.Lerp(r, lightRadius, lerpFactor);
            // Apply new value to range and intensity
            light.range = r;
            light.intensity = r;
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
            stream.SendNext(light.range);
        }
        else {
            targetPos = (Vector3)stream.ReceiveNext();
            targetRadius = (float)stream.ReceiveNext();
        }
    }

    void ExplosionDamage(Vector3 center, float blastRadius) {
        Collider[] hitColliders = Physics.OverlapSphere(center, blastRadius, layer);

        foreach (Collider col in hitColliders) {
            PlayerManager hurtPlayer = col.GetComponentInParent<PlayerManager>();

            // linear damage falloff
            float proximity = (center - hurtPlayer.transform.position).magnitude;
            float intensity = 1 - (proximity/blastRadius);

            // if the player is super close to the grenade, hit them for max damage
            if(intensity > maxHitCutoff) {
                hurtPlayer.Damage(maxHit);
            }
            else {
                hurtPlayer.Damage(maxHit * intensity);
            }
        }

        PhotonNetwork.Instantiate("Explosion", transform.position, transform.rotation, 0);
    }
}
