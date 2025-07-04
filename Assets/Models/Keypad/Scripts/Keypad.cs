using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace NavKeypad
{
    public class Keypad : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private UnityEvent onAccessGranted;
        [SerializeField] private UnityEvent onAccessDenied;
        [Header("Combination Code (9 Numbers Max)")]
        [SerializeField] private int keypadCombo = 12345;

        public UnityEvent OnAccessGranted => onAccessGranted;
        public UnityEvent OnAccessDenied => onAccessDenied;

        [Header("Settings")]
        [SerializeField] private string accessGrantedText = "Granted";
        [SerializeField] private string accessDeniedText = "Denied";

        [Header("Visuals")]
        [SerializeField] private float displayResultTime = 1f;
        [Range(0, 5)]
        [SerializeField] private float screenIntensity = 2.5f;
        [Header("Colors")]
        [SerializeField] private Color screenNormalColor = new Color(0.98f, 0.50f, 0.032f, 1f); //orangy
        [SerializeField] private Color screenDeniedColor = new Color(1f, 0f, 0f, 1f); //red
        [SerializeField] private Color screenGrantedColor = new Color(0f, 0.62f, 0.07f); //greenish
        [Header("SoundFx")]
        [SerializeField] private AudioClip buttonClickedSfx;
        [SerializeField] private AudioClip accessDeniedSfx;
        [SerializeField] private AudioClip accessGrantedSfx;
        [Header("Component References")]
        [SerializeField] private Renderer panelMesh;
        [SerializeField] private TMP_Text keypadDisplayText;
        [SerializeField] private AudioSource audioSource;


        private string currentInput;
        private bool displayingResult = false;
        private bool deniedState = false;
        private bool accessWasGranted = false;
        public AudioMixerGroup generalSFX;

        public List<SubtitleData> successSubtitles;
        public List<SubtitleData> failureSubtitles;


        private void Awake()
        {
            ClearInput();
            panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
        }

        void Start()
        {
            // random password
            keypadCombo = PasswordManager.CurrentPassword;
            Debug.Log($"[KEYPAD] Password for this keypad is: {keypadCombo}");

            if (SaveLoadManager.SaveExists())
            {
                GameStateData data = SaveLoadManager.LoadGame();
                if(data != null)
                {
                    LoadState(data.keypadCurrentInput, data.keypadAccessWasGranted, data.keypadDeniedState);
                }
            }

            if (generalSFX != null)
            {
                audioSource.outputAudioMixerGroup = generalSFX;
            }
        }

        //Gets value from pressedbutton
        public void AddInput(string input)
        {
            audioSource.PlayOneShot(buttonClickedSfx);
            if (displayingResult || accessWasGranted) return;
            switch (input)
            {
                case "enter":
                    CheckCombo();
                    break;
                default:
                    if (currentInput != null && currentInput.Length == 9) // 9 max passcode size 
                    {
                        return;
                    }
                    currentInput += input;
                    keypadDisplayText.text = currentInput;
                    break;
            }

        }
        public void CheckCombo()
        {
            if (int.TryParse(currentInput, out var currentKombo))
            {
                bool granted = currentKombo == keypadCombo;
                if (!displayingResult)
                {
                    StartCoroutine(DisplayResultRoutine(granted));
                }
            }
            else
            {
                Debug.LogWarning("Couldn't process input for some reason..");
            }

        }

        //mainly for animations 
        private IEnumerator DisplayResultRoutine(bool granted)
        {
            displayingResult = true;

            if (granted) AccessGranted();
            else AccessDenied();

            yield return new WaitForSeconds(displayResultTime);
            displayingResult = false;
            if (granted) yield break;
            ClearInput();
            panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);

        }

        private void AccessDenied()
        {
            deniedState = true;
            keypadDisplayText.text = accessDeniedText;
            onAccessDenied?.Invoke();
            panelMesh.material.SetVector("_EmissionColor", screenDeniedColor * screenIntensity);
            audioSource.PlayOneShot(accessDeniedSfx);

            if (failureSubtitles != null && failureSubtitles.Count > 0)
            {
                SubtitleManager.Instance.ResetSubtitles();
                foreach (SubtitleData subtitle in failureSubtitles)
                {
                    SubtitleManager.Instance.EnqueueSubtitle(subtitle);
                }
            }
        }

        private void ClearInput()
        {
            currentInput = "";
            keypadDisplayText.text = currentInput;
            deniedState = false;
        }

        private void AccessGranted()
        {
            accessWasGranted = true;
            keypadDisplayText.text = accessGrantedText;
            onAccessGranted?.Invoke();
            panelMesh.material.SetVector("_EmissionColor", screenGrantedColor * screenIntensity);
            audioSource.PlayOneShot(accessGrantedSfx);

            if (successSubtitles != null && successSubtitles.Count > 0)
            {
                SubtitleManager.Instance.ResetSubtitles();
                foreach (SubtitleData subtitle in successSubtitles)
                {
                    SubtitleManager.Instance.EnqueueSubtitle(subtitle);
                }
            }
        }

        public string GetCurrentInput() 
        {
            return currentInput;
        }

        public bool GetAccessWasGranted() 
        {
            return accessWasGranted;
        }

        public bool GetDeniedState() 
        {
            return deniedState;
        }

        private IEnumerator ResetDeniedStateRoutine()
        {
            yield return new WaitForSeconds(displayResultTime);
            if (deniedState && !accessWasGranted)
            {
                ClearInput();
                panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
            }
        }

        public void LoadState(string savedInput, bool savedAccessGranted, bool savedDenied)
        {
            currentInput = savedInput;
            accessWasGranted = savedAccessGranted;
            deniedState = savedDenied;
            
            if (accessWasGranted)
            {
                keypadDisplayText.text = accessGrantedText;
                panelMesh.material.SetVector("_EmissionColor", screenGrantedColor * screenIntensity);
            }
            else if (deniedState)
            {
                keypadDisplayText.text = accessDeniedText;
                panelMesh.material.SetVector("_EmissionColor", screenDeniedColor * screenIntensity);
                StartCoroutine(ResetDeniedStateRoutine());
            }
            else
            {
                keypadDisplayText.text = currentInput;
            }
        }
    }
}