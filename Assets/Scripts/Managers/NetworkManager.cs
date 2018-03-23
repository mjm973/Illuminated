using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

// Class to manage our network
public class NetworkManager : Photon.PunBehaviour {
    string roomName = "room";
    RoomInfo[] rooms;

    enum AvatarType {
        Demo,
        VR
    }

    [SerializeField]
    AvatarType avatarType;

    string avatarName = "";

    // Use this for initialization
    void Start() {
        switch (avatarType) {
            case AvatarType.Demo:
                avatarName = "Avatar_Demo";
                break;
            case AvatarType.VR:
                avatarName = "Avatar_VR";
                break;
        }

        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnGUI() {
        if (!PhotonNetwork.connected) {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else if (PhotonNetwork.room == null) {
            if (GUI.Button(new Rect(100, 100, 250, 100), "New Room")) {
                PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 6, IsVisible = true }, null);
            }

            if (rooms != null) {
                int i = 0;
                foreach (RoomInfo r in rooms) {
                    if (GUI.Button(new Rect(100, 250 + 110 * i, 250, 100), "Join " + r.Name)) {
                        PhotonNetwork.JoinRoom(r.Name);
                    }
                }
            }
        }
    }

    public override void OnJoinedRoom() {
        //base.OnJoinedRoom();

        PhotonNetwork.Instantiate(avatarName, Vector3.up * 2, Quaternion.identity, 0);
    }

    public override void OnReceivedRoomListUpdate() {
        rooms = PhotonNetwork.GetRoomList();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        //base.OnPhotonRandomJoinFailed(codeAndMsg);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 8 }, null);
    }

    public override void OnJoinedLobby() {
        //base.OnJoinedLobby();

    }
}
