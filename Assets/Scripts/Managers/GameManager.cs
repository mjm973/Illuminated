using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class GameManager : Photon.MonoBehaviour, IPunObservable {

    // static reference to the GM. private to avoid tampering.
    static GameManager gm = null;
    // static getter for the GM. no setter.
    public static GameManager GM {
        get { return gm; }
    }

    #region BGM

    AudioSource bgm;
    public AudioClip[] bgmList;

    float targetVolume = 0f;
    float lastTime = 0f;
    [Range(0, 10)]
    public float fadeTime;

    #endregion

    #region Synced States

    // enum to keep track of the game state
    public enum GameState {
        Lobby,
        Match,
        Over
    }
    // actual game state
    static GameState state = GameState.Lobby;
    // static getter ditto ditto
    public static GameState State {
        get { return state; }
    }

    #region Player tracking

    public enum PlayerState {
        None,
        Wait,
        Alive,
        Dead,
        Over
    }

    struct Player {
        public int id;
        public PlayerState state;
    }

    int nextPlayerToAdd = 0;
    List<Player> players = new List<Player>(new Player[4]);

    #endregion

    #endregion

    #region Unity Functions

    private void Start() {
        // initialize
        gm = this;
        state = GameState.Lobby;
        bgm = GetComponent<AudioSource>();
    }

    void Awake() {

    }

    //Update is called every frame.
    void Update() {
        if (Mathf.Abs(targetVolume - bgm.volume) > 0.01f) {
            bgm.volume = Mathf.Lerp(bgm.volume, targetVolume, (Time.time - lastTime) / fadeTime);
        }
    }

    #endregion

    #region PunRPCs

    [PunRPC] // called by a player to report that they have joined the game
    public void ReportJoin(int id) {

        // owner copy adds player to player list
        if (photonView.isMine) {

            // do not add past 4 players
            if (nextPlayerToAdd >= 4) {
                return;
            }

            Player p = new Player();
            p.state = PlayerState.Wait;
            p.id = id;

            players[nextPlayerToAdd++] = p;
        }
        // borrow copies call RPC on owner
        else {
            photonView.RPC("ReportJoin", photonView.owner, id);
        }
    }

    [PunRPC] // called by a player to report that they have been eliminated
    public void ReportDeath(int id) {

        // owner copy checks the players array to update player data to Dead
        if (photonView.isMine) {

            for (int i = 0; i < players.Count; ++i) {
                Player p = players[i];
                if (p.id == id) {
                    p.state = PlayerState.Dead;
                    break;
                }
            }

            if (IsGameOver()) {
                state = GameState.Over;

                for (int i = 0; i < players.Count; ++i) {
                    Player p = players[i];
                    p.state = PlayerState.Over;
                }
            }
        }
        // borrow copies call the RPC on the owner copy
        else {
            photonView.RPC("ReportDeath", photonView.owner, id);
        }
    }

    [PunRPC] // to trigger music changes
    void AudioFadeIn(int i) {
        bgm.clip = bgmList[i];
        bgm.loop = true;
        bgm.volume = 0;
        bgm.Play();

        targetVolume = 1f;
        lastTime = Time.time;
    }

    #endregion

    #region PUN Sync

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        /* Sync Stuffs:
         * 1) GameState   
         * 2) players
         */

        if (stream.isWriting) {
            SyncWrite(stream);
        }
        else {
            SyncRead(stream);
        }
    }

    void SyncWrite(PhotonStream s) {
        s.SendNext((int)state);

        for (int i = 0; i < 4; ++i) {
            s.SendNext(players[i].id);
            s.SendNext((int)players[i].state);

        }
    }

    void SyncRead(PhotonStream s) {
        state = (GameState)s.ReceiveNext();

        for (int i = 0; i < 4; ++i) {
            Player p = new Player();
            int id = (int)s.ReceiveNext();
            PlayerState state = (PlayerState)s.ReceiveNext();
            if (state != PlayerState.None) {
                p.id = id;
                p.state = state;
                players[i] = p;
            }
        }
    }

    #endregion

    #region Local Methods

    // method to start the game
    public void StartGame() {
        // can only be called if we own the GM - i.e. we are player 1
        if (photonView.isMine) {

            switch (state) {
                case GameState.Lobby:
                    // start match
                    state = GameState.Match;
                    // set player states to alive if not None
                    for (int i = 0; i < 4; ++i) {
                        Player p = players[i];

                        if (p.state == PlayerState.None) {
                            continue;
                        }

                        p.state = PlayerState.Alive;
                        players[i] = p;
                    }

                    photonView.RPC("AudioFadeIn", PhotonTargets.AllBuffered, 0);
                    break;
                case GameState.Match:

                    break;
                case GameState.Over:
                    // start match
                    state = GameState.Lobby;
                    // set player states to alive if not None
                    for (int i = 0; i < 4; ++i) {
                        Player p = players[i];

                        if (p.state == PlayerState.None) {
                            continue;
                        }

                        p.state = PlayerState.Wait;
                        players[i] = p;
                    }
                    break;
            }
        }
    }

    // get player state. states are synced so no need for RPC
    public PlayerState GetState(int id) {
        foreach (Player p in players) {
            if (p.id == id) {
                return p.state;
            }
        }
        // shouldn't happen unless we didn't properly join
        return PlayerState.None;
    }

    #endregion

    #region Utility

    bool IsGameOver() {
        //return NumPlayers () >= 2 && NumAlive () <= 1;
        return NumAlive() <= 1;
    }

    // how many players
    public int NumPlayers() {
        int num = 0;

        foreach (Player p in players) {
            PlayerState s = p.state;
            if (s != PlayerState.None) {
                ++num;
            }
        }

        return num;
    }

    // how many left
    public int NumAlive() {
        int num = 0;

        foreach (Player p in players) {
            PlayerState s = p.state;
            if (s == PlayerState.Alive) {
                ++num;
            }
        }

        return num;
    }



    void AudioFadeOut() {
        targetVolume = 0f;
        lastTime = Time.time;
    }

    #endregion
}