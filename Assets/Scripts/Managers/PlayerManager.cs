using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

// Class to sync player to network
public class PlayerManager : Photon.MonoBehaviour, IPunObservable {
    [Header("PUN Parameters")]
    // PUN Sync variables
    Vector3[] targetPositions = new Vector3[4];
    Quaternion[] targetRotations = new Quaternion[3];
    float targetHealth;

    // To change how fast we interpolate between syncs
    [Range(1, 10)]
    public float interpolationFactor = 5f;

    [Header("Puppet References")]
    public Transform pHead;
    public Transform pRight;
    Transform head;
    Transform body;
    Transform right;

    [Header("Materials")]
    public bool debug = false;
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
		view = photonView; //PhotonView.Get(transform.parent.parent);

        List<MeshRenderer> meshes = new List<MeshRenderer>();

        GetComponentsInChildren<MeshRenderer>(meshes);

        foreach (MeshRenderer mr in meshes) {
            mr.material = view.isMine || debug ? visible : invisible;
        }

        //targetPos = transform.position;
        //targetRot = transform.rotation;

        head = transform.Find("Avatar_Head");
        body = transform.Find("Avatar_Body");
        right = transform.Find("GrenadeLauncher");
    }

    // Update is called once per frame
    void Update() {
        // update gameobject of we don't own it
        if (!view.isMine) {
            float factor = Time.deltaTime * interpolationFactor;
            transform.position = Vector3.Lerp(transform.position, targetPositions[0], factor);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotations[0], factor);

            head.position = Vector3.Lerp(head.position, targetPositions[1], factor);
            head.rotation = Quaternion.Lerp(head.rotation, targetRotations[1], factor);

            right.position = Vector3.Lerp(right.position, targetPositions[2], factor);
            right.rotation = Quaternion.Lerp(right.rotation, targetRotations[2], factor);

            body.position = Vector3.Lerp(body.position, targetPositions[3], factor);
        } else {
            

            head.position = pHead.position;
            head.rotation = pHead.rotation;

            right.position = pRight.position;
            right.rotation = pRight.rotation;

            body.position = head.position + Vector3.down;
        }
    }

    // public method to damage players - called by stuff like grenades, bullets, etc.
    [PunRPC]
    public void Damage(float amt) {
        if (photonView.isMine) {
            health -= amt;
            Debug.Log("I just took " + amt + " damage!");
            if (health < 0) {
                Die();
            }
        }
        else {
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
            stream.SendNext(head.position);
            stream.SendNext(right.position);
            stream.SendNext(body.position);

            stream.SendNext(transform.rotation);
            stream.SendNext(head.rotation);
            stream.SendNext(right.rotation);

            //stream.SendNext(health);
        }
        else { // read from network
            for (int i = 0; i < targetPositions.Length; ++i) {
                targetPositions[i] = (Vector3)stream.ReceiveNext();
            }

            for (int i = 0; i < targetRotations.Length; ++i) {
                targetRotations[i] = (Quaternion)stream.ReceiveNext();
            }

            //targetRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
