using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MultiplayerScoreManager : MonoBehaviourPunCallbacks
{
    public TMP_Text blueText;
    public TMP_Text redText;
    public GameObject winnerBackground;
    public TMP_Text winnerText;
    public float restartDelay = 3f;

    const string BlueScoreKey = "BlueScore";
    const string RedScoreKey = "RedScore";

    int blueScore;
    int redScore;
    bool isRoundOver;

    public static MultiplayerScoreManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;
        if (props.ContainsKey(BlueScoreKey)) blueScore = (int)props[BlueScoreKey];
        if (props.ContainsKey(RedScoreKey))  redScore = (int)props[RedScoreKey];
        UpdateScoreUI();
        winnerText.gameObject.SetActive(false);
        winnerBackground.SetActive(false);
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            blueScore = 0;
            redScore = 0;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
                [BlueScoreKey] = 0,
                [RedScoreKey]  = 0
            });
        }
        UpdateScoreUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            var mgr = MultiplayerManager.Instance;
            var players = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();
            var available = Enumerable.Range(0, mgr.spawnPoints.Length).ToList();

            int[] actorNumbers = new int[players.Length];
            int[] spawnIndices = new int[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                actorNumbers[i] = players[i].ActorNumber;
                int pick = Random.Range(0, available.Count);
                spawnIndices[i] = available[pick];
                available.RemoveAt(pick);
            }

            photonView.RPC(
                nameof(RPC_AssignAndResetRound),
                RpcTarget.All,
                actorNumbers,
                spawnIndices
            );
        }
    }

    public void EndRound(bool winnerIsBlue)
    {
        photonView.RPC(nameof(RPC_HandleRoundEnd), RpcTarget.All, winnerIsBlue);
    }

    [PunRPC]
    void RPC_HandleRoundEnd(bool blueWon)
    {
        if (isRoundOver) return;
        isRoundOver = true;

        winnerBackground.SetActive(true);
        winnerText.color = blueWon
            ? new Color32(0x00, 0x4B, 0xFF, 0xFF)
            : new Color32(0xDB, 0x00, 0x00, 0xFF);

        if (blueWon) blueScore++; else redScore++;
        UpdateScoreUI();
        winnerText.text = blueWon ? "Blue won!" : "Red won!";
        winnerText.gameObject.SetActive(true);

        StartCoroutine(DelayedReset());
    }

    IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(restartDelay);

        winnerBackground.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            var mgr = MultiplayerManager.Instance;
            var players = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();
            var available = Enumerable.Range(0, mgr.spawnPoints.Length).ToList();

            int[] actorNumbers = new int[players.Length];
            int[] spawnIndices = new int[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                actorNumbers[i] = players[i].ActorNumber;
                int pick = Random.Range(0, available.Count);
                spawnIndices[i] = available[pick];
                available.RemoveAt(pick);
            }

            photonView.RPC(
                nameof(RPC_AssignAndResetRound),
                RpcTarget.All,
                actorNumbers,
                spawnIndices
            );
        }
    }

    [PunRPC]
    void RPC_AssignAndResetRound(int[] actorNumbers, int[] spawnIndices)
    {
        isRoundOver = false;
        winnerBackground.SetActive(false);
        winnerText.gameObject.SetActive(false);

        foreach (var h in FindObjectsOfType<MultiPlayerHealth>())
            if (h.photonView.IsMine)
                h.ResetHealth();

        int localActor = PhotonNetwork.LocalPlayer.ActorNumber;
        int idx = 0;
        for (int i = 0; i < actorNumbers.Length; i++)
            if (actorNumbers[i] == localActor)
            {
                idx = spawnIndices[i];
                break;
            }

        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new Hashtable { ["SpawnIndex"] = idx }
        );

        var mgr = MultiplayerManager.Instance;
        foreach (var ctrl in FindObjectsOfType<MultiPlayerController>())
            if (ctrl.photonView.IsMine)
                ctrl.ResetPosition(mgr.spawnPoints[idx]);
    }

    void UpdateScoreUI()
    {
        blueText.text = blueScore.ToString();
        redText.text = redScore.ToString();
    }

    public bool IsRoundOver => isRoundOver;
}
