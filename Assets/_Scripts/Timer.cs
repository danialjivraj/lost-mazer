using UnityEngine;
using UnityEngine.UI;

// this script is being used by Timer object but it's not active because it is no longer needed. i'll keep it here just in case I build a future level who might make use of it
public class Timer : MonoBehaviour
{
    public Text timerText;
    private float startTime;
    private bool isRunning;

    void Start()
    {
        startTime = Time.time;
        isRunning = true;
    }

    void Update()
    {
        if (isRunning)
        {
            float elapsedTime = Time.time - startTime;

            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StartTimer()
    {
        isRunning = true;
        startTime = Time.time;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        startTime = Time.time;
    }
}