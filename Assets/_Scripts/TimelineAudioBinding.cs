using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineAudioBinding : MonoBehaviour
{
    public AudioMixerGroup cutsceneMixerGroup;

    private PlayableDirector director;
    private AudioSource timelineAudioSource;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();

        if (director.playableAsset == null)
            return;

        GameObject audioGO = new GameObject("TimelineAudioSource");
        audioGO.transform.SetParent(this.transform, false);

        timelineAudioSource = audioGO.AddComponent<AudioSource>();
        if (cutsceneMixerGroup != null)
            timelineAudioSource.outputAudioMixerGroup = cutsceneMixerGroup;

        foreach (var output in director.playableAsset.outputs)
        {
            if (output.outputTargetType == typeof(AudioSource))
            {
                director.SetGenericBinding(output.sourceObject, timelineAudioSource);
            }
        }
    }
}
