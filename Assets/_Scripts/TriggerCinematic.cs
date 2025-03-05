using UnityEngine;
using UnityEngine.Playables;

public class TriggerCinematic : MonoBehaviour
{
    public GameObject fadeFX;
    public PlayableDirector timelineDirector;
    private bool hasPlayed = false;

    void Start()
    {
        if (fadeFX != null) fadeFX.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            hasPlayed = true;

            if (fadeFX != null)
                fadeFX.SetActive(true);

            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.enabled = false;
            }

            timelineDirector.Play();

            timelineDirector.stopped += OnTimelineStopped;
        }
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
