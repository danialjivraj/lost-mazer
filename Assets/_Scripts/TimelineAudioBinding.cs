using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineAudioBinding : MonoBehaviour
{
    public AudioSource  ;

    private PlayableDirector director;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();

        if (director.playableAsset == null)
            return;

        if (timelineAudioSource == null)
        {
            Debug.LogWarning("TimelineAudioSource not assigned in the inspector!");
        }

        foreach (var output in director.playableAsset.outputs)
        {
            if (output.outputTargetType == typeof(AudioSource))
            {
                director.SetGenericBinding(output.sourceObject, timelineAudioSource);
            }
        }
    }
}
