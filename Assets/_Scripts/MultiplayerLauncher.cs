using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerLauncher : MonoBehaviourPunCallbacks
{
    public AudioSource buttonSound;

    public void Connect()
    {
        if (buttonSound) buttonSound.Play();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short code, string msg)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MultiplayerMap");
    }
}
