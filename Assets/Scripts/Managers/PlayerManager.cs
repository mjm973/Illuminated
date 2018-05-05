using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using VRTK;
using UnityEngine.UI;

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

    public VRTK_ControllerReference rightRef;
    public VRTK_ControllerReference leftRef;
    public SteamVR_TrackedObject rCont;
    public SteamVR_TrackedObject lCont;

    [Range(0, 1)]
    public float bodyHeight;

    [Header("Materials")]
    public bool debug = false;
    [SerializeField]
    Material visible;
    [SerializeField]
    Material invisible;

    [Header("Gameplay Settings")]
    [SerializeField]
    float maxHealth = 100f;
    float health;
    [SerializeField]
    float lowHealth = 20f;

    public float Health {
        get { return health; }
    }

    [Tooltip("Defines the color the bracelet will display from 0% to 100% health")]
    public Gradient healthGradient;
    [Tooltip("Defines a gradient for the blinking behavior from 0% to 100% health. white is no blink, black is full blink.")]
    public Gradient blinkGradient;
    [Tooltip("Defines the blinking period, from 0% to 100% health.")]
    public AnimationCurve blinkPeriod;
    [Range(0, 7)]
    public float glow = 1.0f;

    PhotonView view;

    static PlayerManager player;
    public static PlayerManager Player {
        get { return player; }
    }

    [Header("Audio")]
    new AudioSource audio;
    public AudioClip hurtSound;
    public AudioClip deadSound;

    [Header("UI")]
    public GameObject canvas;
    GameObject ui;
    [Range(1, 5)]
    public float uiDist = 2f;

    // Use this for initialization
    void Start() {
        health = maxHealth;
        view = photonView;

        List<MeshRenderer> meshes = new List<MeshRenderer>();

        GetComponentsInChildren<MeshRenderer>(meshes);

        print(string.Format("Meshes: {0}", meshes.Count));

        foreach (MeshRenderer mr in meshes) {
            if (!view.isMine) {
                Material[] mats = mr.materials;

                print(string.Format("Materials: {0}", mats.Length));

                for (int i = 0; i < mats.Length; ++i) {
                    mats[i] = invisible;
                }

                mr.materials = mats;
            }
        }

        head = transform.Find("Avatar_Head");
        body = transform.Find("Avatar_Body");
        right = transform.Find("GrenadeLauncher");
        left = transform.Find("Bracelet");

        if (view.isMine) {
            UpdateBracelet();

            player = this;
            GameManager.GM.ReportJoin(photonView.viewID);

            audio = GetComponent<AudioSource>();

            ui = GameObject.Instantiate(canvas, transform.position + transform.forward * uiDist, Quaternion.identity);
            ui.transform.SetParent(transform.Find("Avatar_Head"));
            ui.transform.localScale = new Vector3(0.007f, 0.007f, 0.007f);
        }
    }

    // Update is called once per frame
    void Update() {
        // update gameobject of we don't own it
        if (!view.isMine) {
            SyncPuppet();

            // make sure we respawn once we are waiting for a new game
            if (GameManager.GM.GetState(photonView.viewID) == GameManager.PlayerState.Wait) {
                Spawn();
            }
        }
        else {
            MovePuppet();
            UpdateBracelet();
            UpdateDeathCount();
        }
    }

    // updates the puppet position on the network copies
    void SyncPuppet() {
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
    }

    // binds puppet position to VR Rig in the master copy
    void MovePuppet() {
        head.position = pHead.position;
        head.rotation = pHead.rotation;

        head.parent.position = head.position;

        right.position = pRight.position;
        right.rotation = pRight.rotation;

        left.position = pLeft.position;
        left.rotation = pLeft.rotation;

        body.position = head.position + Vector3.down * bodyHeight;
    }

    // helper to handle damage feedback
    void HitFeedback(float t) {
        //print("buzz " + t);
        //VRTK_ControllerHaptics.TriggerHapticPulse(rightRef, 1f, 1f, 0f);
        //VRTK_ControllerHaptics.TriggerHapticPulse(leftRef, t);

        PP_HurtEffect h = PP_HurtEffect.HurtEffect;
        if (h != null) {
            h.Trigger();
        }

        StartCoroutine(PulseHaptics((int)(t * 500), 500));
    }

    IEnumerator PulseHaptics(int strength, int duration) {
        float dur = (float)duration / 1000f;
        float start = Time.time;

        while (Time.time < start + dur) {
            SteamVR_Controller.Input((int)rCont.index).TriggerHapticPulse((ushort)strength);
            SteamVR_Controller.Input((int)lCont.index).TriggerHapticPulse((ushort)strength);

            yield return new WaitForEndOfFrame();
        }
    }

    // public method to damage players - called by stuff like grenades, bullets, etc.
    [PunRPC]
    public void Damage(float amt) {
        if (photonView.isMine) {
            health -= amt;
            HitFeedback(amt / 20f);

            if (health <= 0) {
                Die();
            }
            else if (audio != null && hurtSound != null) {
                if (!audio.isPlaying && health <= lowHealth) {
                    audio.loop = true;
                    audio.Play();
                }
                audio.PlayOneShot(hurtSound);
            }
        }
        else {
            photonView.RPC("Damage", photonView.owner, amt);
        }
    }

    // updates the color of the bracelet in the master copy
    void UpdateBracelet() {
        if (photonView.isMine) {
            Color currentHealth = CalculateColor(health / maxHealth);

            UpdateColor(currentHealth);
        }
    }

    // calculates the color of the bracelet. takes into account the player's health,
    // puls time to make it blink when health is low
    Color CalculateColor(float t) {
        Color baseCol = healthGradient.Evaluate(t);
        Color blinkCol = blinkGradient.Evaluate(t);
        float p = blinkPeriod.Evaluate(t);
        float a = Time.time % p;
        a /= p;

        Color result = baseCol * (1 - a) + blinkCol * baseCol * a;
        result.a = 1f;

        return result;
    }

    // applies a color to the bracelet
    void UpdateColor(Color col) {
        // boost color to hdr levels
        Color em = col * (1 + glow);

        Material mat = left.GetComponentInChildren<MeshRenderer>().materials[2];
        //mat.SetColor("_Color", col);
        mat.color = col;
        mat.SetColor("_EmissionColor", em);
    }

    // updates ui to reflect numebr of players left
    void UpdateDeathCount() {
        Text t = ui.transform.Find("DeathCount").GetComponent<Text>();
        GameManager.PlayerState ps = GameManager.GM.GetState(photonView.viewID);

        switch (GameManager.State) {
            case GameManager.GameState.Lobby:
                t.text = string.Format("Waiting for players: {0}/4 joined", GameManager.GM.NumPlayers());
                break;
            case GameManager.GameState.Match:
                if (ps == GameManager.PlayerState.Alive) {
                    t.text = string.Format("Players Remaining: {0}/{1}", GameManager.GM.NumAlive(), GameManager.GM.NumPlayers());
                }
                else if (ps == GameManager.PlayerState.Dead) {
                    t.text = string.Format("ILLUMINATED");
                }

                break;
            case GameManager.GameState.Over:
                if (ps == GameManager.PlayerState.Over) {
                    t.text = string.Format("GAME OVER");
                }
                else if (ps == GameManager.PlayerState.Winner) {
                    t.text = string.Format("WINNER");
                }

                break;
        }

    }

    // method to determine what we do once we are ded (becoming a spectator, for example)
    void Die() {
        // do stuff
        print("Ded");

        if (audio != null && deadSound != null) {
            audio.PlayOneShot(deadSound);
        }

        GameManager.GM.ReportDeath(photonView.viewID);
        VRInputManager.Instance.allowInput = false;
    }

    // opposite of die
    [PunRPC]
    void Spawn() {
        // rn it just disables shooting
        if (photonView.isMine) {
            VRInputManager.Instance.allowInput = true;
            health = maxHealth;
        }
        else {
            photonView.RPC("Spawn", photonView.owner);
        }
    }

    // utility to fetch player state from GM
    GameManager.PlayerState State() {
        return GameManager.GM.GetState(photonView.viewID);
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
