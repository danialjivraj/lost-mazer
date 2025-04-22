using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerPauseMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private AudioSource buttonSound;

    private bool isPaused = false;

    void Start()
    {
        pauseCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);

        foreach (var ctrl in FindObjectsOfType<MultiPlayerController>())
            if (ctrl.photonView.IsMine)
                ctrl.IsPaused = isPaused;

        Cursor.lockState = isPaused
            ? CursorLockMode.None
            : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        if (buttonSound) buttonSound.Play();
    }

    public void ContinueGame()
    {
        if (buttonSound)
        {
            buttonSound.Play();
        }
        TogglePause();
    }

    public void OnBackToMainMenuPressed()
    {
        if (buttonSound)
        {
            buttonSound.Play();
            StartCoroutine(DisconnectAfterSound());
        }
        else
        {
            PhotonNetwork.Disconnect();
        }
    }

    private IEnumerator DisconnectAfterSound()
    {
        yield return new WaitForSecondsRealtime(buttonSound.clip.length);
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("MainMenu");
    }
}
