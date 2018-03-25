using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


// Class to manage player input
public class InputManager : Photon.MonoBehaviour {

    public float moveSpeed = 5;
    public float angularSpeed = 120;
    public bool invertY = true;

    Transform head;

	// Use this for initialization
	void Start () {
        // Lock cursor to center of the screen to make our lives easier
        Cursor.lockState = CursorLockMode.Locked;
        // locate our head
        head = transform.Find("Head");
        // for now, let's grab the main camera and attach it
        Transform mainCam = Camera.main.transform;
        mainCam.position = head.position;
        mainCam.rotation = head.rotation;
        mainCam.SetParent(head);
	}
	
	// Update is called once per frame
	void Update () {
        // allow input only if we own the gameobject
		if (photonView.isMine) {
            HandleInput();
        }
	}

    void HandleInput() {
        Move();
        Turn();
    }

    void Move() {
        // make vector to store movement input
        Vector3 move = new Vector3();
        // capture WASD/ArrowKey input
        move.x = Input.GetAxis("Horizontal");
        move.z = Input.GetAxis("Vertical");
        // normalize and multiply by our speed
        move = move.normalized * moveSpeed;
        // translate scaled by deltaTime
        transform.Translate(move * Time.deltaTime);
    }

    void Turn() {
        // make vector to store rotation input
        Vector3 turn = new Vector3();
        // horizontal mouse -> rotate around y-axis
        turn.y = Input.GetAxis("Mouse X");
        // vertical mouse -> rotate around x-axis
        turn.x = Input.GetAxis("Mouse Y") * (invertY ? -1 : 1);
        // scale by our angular speed
        turn *= angularSpeed * Time.deltaTime;

        // horizontal turn is in world space
        transform.Rotate(0, turn.y, 0, Space.World);
        // vertical turn is in local space
        head.Rotate(turn.x, 0, 0, Space.Self);
    }
}
