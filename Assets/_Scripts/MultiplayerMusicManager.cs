using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MultiplayerMusicManager : MonoBehaviour
{
    public static MultiplayerMusicManager Instance { get; private set; }

    public string multiplayerSceneName = "MultiplayerMap";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var src = GetComponent<AudioSource>();
        if (src && !src.isPlaying)
            src.Play();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != multiplayerSceneName)
            Destroy(gameObject);
    }
}
