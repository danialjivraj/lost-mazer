using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerLauncher : MonoBehaviourPunCallbacks
{
    public AudioSource buttonSound;

    public void Connect()
    {
        if (buttonSound != null) 
            buttonSound.Play();

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MultiplayerMap");
    }
}
