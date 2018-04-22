using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class GameManager : Photon.MonoBehaviour {

    private int currentNumberOfPlayers; // How many players are here and alive in the arena at this instant
    
    public int playersNeeded; // How many players need to be spawned into the arena before we are ready to play
    public static GameManager instance = null;  // Singleton Design Pattern  
    private enum playerState { NOTREADY, READY, DEAD, ALIVE, INACTIVE };
    private playerState hostPlayerState;
    private playerState clientPlayerState;
    [SerializeField]
    List<PlayerManager> players;
    public bool playersReady;
    [SerializeField]
    private int playersAliveForGameOver = 1; // Should normally be 1, but for debugging, who knows, could be anything
    [SerializeField]
    private bool gameRunning;


    void Awake() {
        // Default to not ready so that we can then check if ready
        hostPlayerState = playerState.NOTREADY;
        clientPlayerState = playerState.NOTREADY;

        playersReady = false;
        gameRunning = false;
        //Call the InitGame function to initialize the first level 

    }

    //Initializes the actual deathmatch game mode.
    void InitGame() {
        Debug.Log("INIT!!!");
        playersReady = true;
        hostPlayerState = playerState.READY;
        clientPlayerState = playerState.READY;
        Debug.Log("This is where the fun begins!");
        gameRunning = true;
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject playerObj in playerObjects) {
            PlayerManager player = playerObj.GetComponent<PlayerManager>();
            players.Add(player);
        }
    }

    // Game is over if only one player is alive
    bool IsGameOver() {
        return currentNumberOfPlayers == 1;
    }

    public GameManager getInstance() {
        return this;
    }

    //Update is called every frame.
    void Update() {
        if (photonView.isMine && Input.GetKeyDown("s") && (hostPlayerState == playerState.NOTREADY) && (clientPlayerState == playerState.NOTREADY)) {
            InitGame();
            return;
        }
        
        
        
            
            
        


    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }
}

