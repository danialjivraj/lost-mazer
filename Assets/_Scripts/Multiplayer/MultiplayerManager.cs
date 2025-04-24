using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static MultiplayerManager Instance { get; private set; }
    public Transform[] spawnPoints;
    const string SpawnIndexKey = "SpawnIndex";
    bool hasSpawned;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

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
        AssignTeam();

        if (SceneManager.GetActiveScene().name == "MultiplayerMap")
        {
            hasSpawned = false;
            TrySpawn();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MultiplayerMap") return;

        hasSpawned = false;

        var parent = GameObject.Find("spawnpoints");
        if (parent == null)
        {
            Debug.LogError("MultiplayerManager: no 'spawnpoints' object found");
            return;
        }
        spawnPoints = parent
            .GetComponentsInChildren<Transform>()
            .Where(t => t != parent.transform)
            .OrderBy(t => t.name)
            .ToArray();

        TrySpawn();
    }

    void AssignTeam()
    {
        int team;
        if (PhotonNetwork.PlayerListOthers.Length == 0)
        {
            team = 0;
        }
        else
        {
            var other = PhotonNetwork.PlayerListOthers[0];
            object raw;
            other.CustomProperties.TryGetValue("Team", out raw);
            int otherTeam = (raw is int) ? (int)raw : 0;
            team = 1 - otherTeam;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new ExitGames.Client.Photon.Hashtable { ["Team"] = team }
        );
    }

    void TrySpawn()
    {
        if (hasSpawned
            || !PhotonNetwork.InRoom
            || SceneManager.GetActiveScene().name != "MultiplayerMap"
            || spawnPoints == null
            || spawnPoints.Length == 0)
            return;

        int spawnIdx;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(SpawnIndexKey, out object raw))
        {
            spawnIdx = (int)raw;
        }
        else
        {
            spawnIdx = GetRandomAvailableSpawnIndex();
            PhotonNetwork.LocalPlayer.SetCustomProperties(
                new Hashtable { [SpawnIndexKey] = spawnIdx }
            );
        }

        if (spawnIdx < 0 || spawnIdx >= spawnPoints.Length)
            spawnIdx = GetRandomAvailableSpawnIndex();

        PhotonNetwork.Instantiate(
            "Player",
            spawnPoints[spawnIdx].position,
            spawnPoints[spawnIdx].rotation
        );
        hasSpawned = true;
    }

    int GetRandomAvailableSpawnIndex()
    {
        var taken = new HashSet<int>();
        foreach (var p in PhotonNetwork.PlayerList)
            if (p.CustomProperties.TryGetValue(SpawnIndexKey, out var v))
                taken.Add((int)v);

        var free = Enumerable
            .Range(0, spawnPoints.Length)
            .Where(i => !taken.Contains(i))
            .ToList();

        if (free.Count == 0)
            free.AddRange(Enumerable.Range(0, spawnPoints.Length));

        return free[Random.Range(0, free.Count)];
    }
}
