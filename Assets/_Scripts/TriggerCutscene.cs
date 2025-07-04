using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;

public class TriggerCutscene : MonoBehaviour
{
    public string cutsceneId;
    public static bool isCutsceneActive = false;

    public GameObject fadeFX;
    public PlayableDirector timelineDirector;
    public float timelineStartDelay = 0.25f;
    public bool disableOtherSounds = true;

    public AudioMixerGroup menuSFX;
    public AudioSource echoesAudioSource;
    public bool freezeEnemiesDuringThisCutscene = false;
    public static bool freezeEnemiesActive = false;

    public bool hasPlayed = false;
    private bool isTimelinePlaying = false;
    private bool isSkipping = false;
    private List<AudioSource> ambientAudioSources = new List<AudioSource>();
    public bool isEndCutscene = false;
    public LevelCompleted levelCompletedScript;

    void Awake()
    {
        if (string.IsNullOrEmpty(cutsceneId))
            cutsceneId = gameObject.name + "_" + transform.position.x + "_" + transform.position.y + "_" + transform.position.z;

        if (SaveLoadManager.SaveExists())
        {
            GameStateData data = SaveLoadManager.LoadGame();
            if (data != null)
            {
                foreach (var cs in data.cutsceneStates)
                {
                    if (cs.cutsceneId == cutsceneId && cs.hasPlayed)
                    {
                        hasPlayed = true;
                        break;
                    }
                }
            }
        }
    }

    void Start()
    {
        if (fadeFX != null)
            fadeFX.SetActive(false);
    }

    void Update()
    {
        if (isCutsceneActive && isTimelinePlaying && Time.timeScale > 0 && Input.GetKeyDown(KeyCode.Tab))
        {
            if (isEndCutscene && levelCompletedScript != null)
            {
                timelineDirector.stopped -= OnTimelineStopped;
                timelineDirector.Stop();
                levelCompletedScript.StartLevelCompletion();
            }
            else if (!isSkipping)
            {
                isSkipping = true;
                StartCoroutine(SkipCutscene());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;

            GameStateData data;
            if(SaveLoadManager.SaveExists())
            {
                data = SaveLoadManager.LoadGame();
            }
            else
            {
                data = new GameStateData();
            }
            data.cutsceneStates.RemoveAll(cs => cs.cutsceneId == cutsceneId);
            data.cutsceneStates.Add(new CutsceneState { cutsceneId = cutsceneId, hasPlayed = true });

            if (fadeFX != null)
                fadeFX.SetActive(true);

            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.enabled = false;

            StartCoroutine(StartTimelineAfterDelay());
        }
    }

    private IEnumerator StartTimelineAfterDelay()
    {
        yield return new WaitForSeconds(timelineStartDelay);

        SubtitleManager.Instance.ResetSubtitles();

        if (disableOtherSounds)
        {
            AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();
            ambientAudioSources.Clear();
            foreach (AudioSource src in allSources)
            {
                if (src.transform.IsChildOf(timelineDirector.transform))
                    continue;
                
                if (src.outputAudioMixerGroup == menuSFX)
                    continue;
                
                ambientAudioSources.Add(src);
                src.mute = true;
            }
        }

        if (freezeEnemiesDuringThisCutscene)
            freezeEnemiesActive = true;
        timelineDirector.Play();

        isTimelinePlaying = true;
        isCutsceneActive = true;
        timelineDirector.stopped += OnTimelineStopped;
    }

    private IEnumerator SkipCutscene()
    {
        if (fadeFX != null)
        {
            fadeFX.SetActive(false);
            fadeFX.SetActive(true);
        }

        yield return new WaitForSeconds(timelineStartDelay);

        timelineDirector.time = timelineDirector.duration;
        timelineDirector.Evaluate();
        timelineDirector.Stop();

        OnTimelineStopped(timelineDirector);
        isTimelinePlaying = false;
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (freezeEnemiesDuringThisCutscene)
            freezeEnemiesActive = false;

        timelineDirector.stopped -= OnTimelineStopped;
        isCutsceneActive = false;

        if (!isSkipping)
        {
            StartCoroutine(ExitWithFade());
        }
        else
        {
            ReenablePlayer();
            isSkipping = false;
        }
    }

    private IEnumerator ExitWithFade()
    {
        if (fadeFX != null)
        {
            fadeFX.SetActive(false);
            fadeFX.SetActive(true);
        }

        yield return new WaitForSeconds(timelineStartDelay);

        ReenablePlayer();
    }

    private void ReenablePlayer()
    {
        if (disableOtherSounds)
        {
            foreach (AudioSource src in ambientAudioSources)
            {
                if (src != null)
                    src.mute = false;
            }
            ambientAudioSources.Clear();
        }

        if (echoesAudioSource != null)
        {
            echoesAudioSource.Stop();
            echoesAudioSource.Play();
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
                pc.enabled = true;
        }
    }
}
