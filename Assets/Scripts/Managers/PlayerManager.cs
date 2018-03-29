using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

// Class to sync player to network
public class PlayerManager : Photon.MonoBehaviour, IPunObservable {
    [Header("PUN Parameters")]
    // PUN Sync variables
    Vector3 targetPos;
    Quaternion targetRot;
    float targetHealth;

    // To change how fast we interpolate between syncs
    [Range(1, 10)]
    public float interpolationFactor = 5f;

    [Header("Materials")]
    [SerializeField]
    Material visible;
    [SerializeField]
    Material invisible;

    [Header("Gameplay Settings")]
    [SerializeField]
    float health = 100f;

    public float Health {
        get { return health; }
    }

    PhotonView view;

    // Use this for initialization
    void Start() {
        view = PhotonView.Get(transform.parent.parent);

        List<MeshRenderer> meshes = new List<MeshRenderer>();

        GetComponentsInChildren<MeshRenderer>(meshes);

        foreach (MeshRenderer mr in meshes) {
            mr.material = view.isMine ? visible : invisible;
        }

        targetPos = transform.position;
        targetRot = transform.rotation;

    }

    // Update is called once per frame
    void Update() {
        // update gameobject of we don't own it
        if (!view.isMine) {
            float factor = Time.deltaTime * interpolationFactor;
            transform.position = Vector3.Lerp(transform.position, targetPos, factor);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, factor);
        }
    }

    // public method to damage players - called by stuff like grenades, bullets, etc.
    [PunRPC]
    public void Damage(float amt) {
        if (photonView.isMine) {
            health -= amt;

            if (health < 0) {
                Die();
            }
        } else {
            photonView.RPC("Damage", photonView.owner, amt);
        }
    }

    // method to determine what we do once we are ded (becoming a spectator, for example)
    void Die() {
        // do stuff
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) { // write to network
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(health);
        }
        else { // read from network
            targetPos = (Vector3)stream.ReceiveNext();
            //targetRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
