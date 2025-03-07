using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance { get; private set; }
    public TextMeshProUGUI subtitleText;
    public Image background;
    public float fadeDuration = 0.5f;

    private Queue<SubtitleMessage> subtitleQueue = new Queue<SubtitleMessage>();
    private Coroutine processingCoroutine = null;
    private Coroutine displayCoroutine = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ResetSubtitles()
    {
        subtitleQueue.Clear();

        if (processingCoroutine != null)
        {
            StopCoroutine(processingCoroutine);
            processingCoroutine = null;
        }

        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }

        subtitleText.text = "";
        subtitleText.color = new Color(subtitleText.color.r, subtitleText.color.g, subtitleText.color.b, 0f);
        
        if (background != null)
            background.gameObject.SetActive(false);
    }

    public void EnqueueSubtitle(SubtitleData data)
    {
        subtitleQueue.Enqueue(new SubtitleMessage(data.text, data.displayTime, data.spawnDelay));
        if (processingCoroutine == null)
        {
            processingCoroutine = StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        bool isFirst = true;
        while (subtitleQueue.Count > 0)
        {
            SubtitleMessage message = subtitleQueue.Dequeue();
            float delay = isFirst ? message.spawnDelay : 0f;
            displayCoroutine = StartCoroutine(DisplaySubtitle(message.text, message.displayTime, delay));
            yield return displayCoroutine;
            displayCoroutine = null;
            isFirst = false;
        }
        processingCoroutine = null;
    }

    private IEnumerator DisplaySubtitle(string text, float displayTime, float spawnDelay)
    {
        if (spawnDelay > 0f)
            yield return new WaitForSeconds(spawnDelay);

        subtitleText.text = text;
        
        if (background != null)
            background.gameObject.SetActive(true);

        // fades in
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / fadeDuration);
            subtitleText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // holds the subtitle
        yield return new WaitForSeconds(displayTime);

        // fades out
        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (time / fadeDuration));
            subtitleText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        
        if (background != null)
            background.gameObject.SetActive(false);
    }

    private class SubtitleMessage
    {
        public string text;
        public float displayTime;
        public float spawnDelay;

        public SubtitleMessage(string text, float displayTime, float spawnDelay)
        {
            this.text = text;
            this.displayTime = displayTime;
            this.spawnDelay = spawnDelay;
        }
    }
}
