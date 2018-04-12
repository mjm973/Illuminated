using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;       // Because lists

public class GameManager : MonoBehaviour {

    public int numberOfPlayers;
    public static GameManager instance = null;  // Singleton Design Pattern
    private BoardManager boardScript;
    private int playersAlive;               

    
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

        // Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        //C all the InitGame function to initialize the first level 
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame() {
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);

    }



    //Update is called every frame.
    void Update() {

    }
