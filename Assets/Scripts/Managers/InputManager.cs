using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


// Class to manage player input
public class InputManager : Photon.MonoBehaviour {

    public float moveSpeed = 5;
    public float angularSpeed = 120;
    public bool invertY = true;
    //private float dodging = 0;
    bool dodging = false;
    [Range(0, 2)]
    public float dodgeDistance = 1f;
    [Range(0, 0.2f)]
    public float dodgeFactor = 0.1f;
    Vector3 dodgeTarget = Vector3.zero;
    //public float initDodgeSpeed = 150;
    //public float decceleration = 2;
    //public float dodgeSpeed = 0;

    public bool snatchMainCam = false;


    [Range(0, 10)]
    public float jumpForce = 100f;

    Transform head;
    Rigidbody rb;

    GrenadeGunManager gun;

    PhotonView view;

    // Use this for initialization
    void Start() {
        // Lock cursor to center of the screen to make our lives easier
        Cursor.lockState = CursorLockMode.Locked;
        // locate our head
        head = transform.Find("Head");
        // if we are running on our machine, find the main camera and attach it
        if (photonView.isMine && snatchMainCam) {
            Transform mainCam = Camera.main.transform;
            mainCam.position = head.position;
            mainCam.rotation = head.rotation;
            mainCam.SetParent(head);
        }
        // get our rigidbody to work with physicz
        rb = GetComponent<Rigidbody>();
        // get our gun to fire
        gun = GetComponentInChildren<GrenadeGunManager>();
    }

    // Update is called once per frame
    void Update() {
        // allow input only if we own the gameobject
        if (photonView.isMine) {
            HandleInput();
        }
    }

    void HandleInput() {
        Move();
        Turn();
        Jump();
        Shoot();
        Dodge();
    }

    void Move() {
        if (dodging) {
            return;
        }

        // make vector to store movement input
        Vector3 move = new Vector3();
        // capture WASD/ArrowKey input
        move.x = Input.GetAxis("Horizontal");
        move.z = Input.GetAxis("Vertical");
        // make sure we don't overwrite our y movement
        float ySpeed = rb.velocity.y;
        // normalize and multiply by our speed and deltaTime
        move = move.normalized * moveSpeed;// * Time.deltaTime;
        // factor our y speed back in
        move.y = ySpeed;
        // move using the rigidbody's physics
        rb.velocity = transform.rotation*move;
    }

    void Dodge() {
        if (!dodging) {
            if (Input.GetKeyDown(KeyCode.Q)) {
                dodgeTarget = transform.position - transform.right*dodgeDistance;
            } else if (Input.GetKeyDown(KeyCode.E)) {
                dodgeTarget = transform.position + transform.right*dodgeDistance;
            } else {
                return;
            }

            Debug.Log("nyoom");
            dodging = true;
        } else {
            dodgeTarget.y = transform.position.y;

            rb.MovePosition(Vector3.Lerp(transform.position, dodgeTarget, dodgeFactor));

            if ((dodgeTarget - transform.position).sqrMagnitude < 0.1f) {
                dodging = false;
            }
        }
    }

	//void Dodge(){
	//	if (dodging == 0) {
	//		if (Input.GetKeyDown (KeyCode.Q)) {
	//			if (dodging == 0) {
	//				dodgeSpeed = -initDodgeSpeed;
	//			}
	//			dodging = 1;
	//		} else if (Input.GetKeyDown (KeyCode.E)) {
	//			if (dodging == 0) {
	//				dodgeSpeed = initDodgeSpeed;
	//			}
	//			dodging = 2;

	//		}
	//	}

	//	if (dodging == 1) {
			
	//		rb.AddRelativeForce (dodgeSpeed,0,0);
	//		dodgeSpeed += decceleration;

	//		if (dodgeSpeed == 0) {
	//			dodging = 0;
	//		}

	//		}
	//	if (dodging == 2) {

	//		rb.AddRelativeForce (dodgeSpeed,0,0);
	//		dodgeSpeed -= decceleration;

	//		if (dodgeSpeed == 0) {
	//			dodging = 0;
	//		}
	//	}
	//}

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

    void Jump() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Shoot() {
        if (Input.GetMouseButtonDown(0)) {
            gun.Fire();
        }
    }

    private void OnCollisionStay(Collision collision) {
        dodging = false;
    }
}
