using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.UI;

public class SubtitleTrackMixer : PlayableBehaviour
{
    private Image textBox;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI text = playerData as TextMeshProUGUI;
        if (!text) return;

        if (textBox == null)
        {
            textBox = text.transform.parent.GetComponent<Image>();
        }
        
        if (textBox == null) return;

        string currentText = "";
        float currentAlpha = 0f;

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0f)
            {
                ScriptPlayable<SubtitleBehaviour> inputPlayable = 
                    (ScriptPlayable<SubtitleBehaviour>)playable.GetInput(i);

                SubtitleBehaviour input = inputPlayable.GetBehaviour();
                currentText = input.subtitleText;
                currentAlpha = inputWeight;
            }
        }

        text.text = currentText;
        text.color = new Color(1f, 1f, 1f, currentAlpha);

        textBox.gameObject.SetActive(currentAlpha > 0f);
    }
}
