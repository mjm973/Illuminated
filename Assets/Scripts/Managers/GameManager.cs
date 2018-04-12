using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;       // Because lists

public class GameManager : MonoBehaviour {

    private int currentNumberOfPlayers; // How many players are here and alive in the arena at this instant
    public int playersNeeded; // How many players need to be spawned into the arena before we are ready to play
    public static GameManager instance = null;  // Singleton Design Pattern  
    public bool Player1Alive;
    public bool Player2Alive;
    public bool Player3Alive;
    public bool Player4Alive;


    void Awake() {
        // Singleton Design Pattern 
        if (instance == null)
            instance = this;

        // Sanity check for Singleton Design Pattern. 
        // Somehow if we end up with more than one GameManager, enforce singleton rule
        else if (instance != this)
            Destroy(gameObject);

        // Game Manager Persists
        DontDestroyOnLoad(gameObject);
        
        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //Initializes the actual deathmatch game mode.
    void InitGame() {

        Player1Alive = true;
        Player2Alive = true;
        Player3Alive = true;
        Player4Alive = true;

    }

    // Game is over if only one player is alive
    bool IsGameOver() {
        return currentNumberOfPlayers == 1;
    }

    void SetPlayerCount() {

    }

    //Update is called every frame.
    void Update() {

    }
}
