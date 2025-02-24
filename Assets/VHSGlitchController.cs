using UnityEngine;

public class VHSGlitchController : MonoBehaviour
{
    public Material vhsMaterial;
    public float glitchSpeed = 1.0f;
    public float glitchIntensity = 0.1f;

    private void Update()
    {
        float glitchAmount = Mathf.PingPong(Time.time * glitchSpeed, glitchIntensity);
        vhsMaterial.SetFloat("_GlitchAmount", glitchAmount);
    }
}