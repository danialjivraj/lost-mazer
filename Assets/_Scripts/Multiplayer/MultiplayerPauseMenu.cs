using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerPauseMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject audioSettingsMenu;
    [SerializeField] private GameObject buttonsContainer;

    [SerializeField] private AudioSource buttonSound;

    bool isPaused = false;

    void Start()
    {
        pauseCanvas.SetActive(false);
        audioSettingsMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (audioSettingsMenu.activeSelf)
                    CloseAudioSettings();
                else
                    TogglePause();
            }
            else
            {
                TogglePause();
            }
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);

        // audio panel is closed on every pause toggle
        audioSettingsMenu.SetActive(false);
        buttonsContainer.SetActive(true);

        // notify the local player controller
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
        if (buttonSound) buttonSound.Play();
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

    public void OpenAudioSettings()
    {
        if (buttonSound) buttonSound.Play();
        buttonsContainer.SetActive(false);
        audioSettingsMenu.SetActive(true);
    }

    public void CloseAudioSettings()
    {
        if (buttonSound) buttonSound.Play();
        audioSettingsMenu.SetActive(false);
        buttonsContainer.SetActive(true);
    }
}
