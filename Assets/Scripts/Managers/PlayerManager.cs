using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

// Class to sync player to network
public class PlayerManager : Photon.MonoBehaviour, IPunObservable {
    [Header("PUN Parameters")]
    // PUN Sync variables
    Vector3[] targetPositions = new Vector3[5];
    Quaternion[] targetRotations = new Quaternion[4];
    float targetHealth;

    // To change how fast we interpolate between syncs
    [Range(1, 10)]
    public float interpolationFactor = 5f;

    [Header("Puppet References")]
    public Transform pHead;
    public Transform pRight;
    public Transform pLeft;
    Transform head;
    Transform body;
    Transform right;
    Transform left;

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

    public Gradient healthGradient;

    PhotonView view;

    // Use this for initialization
    void Start() {
		view = photonView;

        List<MeshRenderer> meshes = new List<MeshRenderer>();

        GetComponentsInChildren<MeshRenderer>(meshes);

        foreach (MeshRenderer mr in meshes) {
            if (!view.isMine) {
                mr.material = invisible;
            }

            //mr.material = view.isMine || debug ? visible : invisible;
        }

        //targetPos = transform.position;
        //targetRot = transform.rotation;

        head = transform.Find("Avatar_Head");
        body = transform.Find("Avatar_Body");
        right = transform.Find("GrenadeLauncher");
        left = transform.Find("Bracelet");
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

            left.position = Vector3.Lerp(left.position, targetPositions[3], factor);
            left.rotation = Quaternion.Lerp(left.rotation, targetRotations[3], factor);

            body.position = Vector3.Lerp(body.position, targetPositions[4], factor);
        } else {       
            head.position = pHead.position;
            head.rotation = pHead.rotation;

            right.position = pRight.position;
            right.rotation = pRight.rotation;

            left.position = pLeft.position;
            left.rotation = pLeft.rotation;

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

    void UpdateBracelet() {
        if (photonView.isMine) {
            Color currentHealth = healthGradient.Evaluate(health / 100f);

            UpdateColor(currentHealth);
        }
    }

    void UpdateColor(Color col) {
        Material mat = left.GetComponent<MeshRenderer>().materials[2];
        mat.SetColor("_Color", col);
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
            stream.SendNext(left.position);
            stream.SendNext(body.position);

            stream.SendNext(transform.rotation);
            stream.SendNext(head.rotation);
            stream.SendNext(right.rotation);
            stream.SendNext(left.rotation);

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
