using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;
    bool hasSpawned = false;

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        base.OnDisable();
    }

    public override void OnJoinedRoom()
    {
        TrySpawn();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MultiplayerMap")
            TrySpawn();
    }

    void TrySpawn()
    {
        if (hasSpawned) return;
        
        if (!PhotonNetwork.IsConnectedAndReady) return;
        
        int idx = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
        var spawnPt = spawnPoints[idx];
        
        GameObject player = PhotonNetwork.Instantiate("Player", spawnPt.position, spawnPt.rotation);
        Debug.Log($"Player spawned: {player.GetPhotonView().ViewID} for actor {PhotonNetwork.LocalPlayer.ActorNumber}");
        
        hasSpawned = true;
    }
}
