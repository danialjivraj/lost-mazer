using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class TriggerCinematic : MonoBehaviour
{
    public GameObject fadeFX;
    public PlayableDirector timelineDirector;

    public float timelineStartDelay = 0.25f;

    private bool hasPlayed = false;

    void Start()
    {
        if (fadeFX != null) fadeFX.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;

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

        timelineDirector.Play();

        timelineDirector.stopped += OnTimelineStopped;
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.enabled = true;
            }
        }
        timelineDirector.stopped -= OnTimelineStopped;
    }
}
