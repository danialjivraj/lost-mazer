using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavKeypad
{
    public class SlidingDoor : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        public bool IsOpoen => isOpen;
        private bool isOpen = false;
        public AudioSource openingSound;

        void Start()
        {
            if (SaveLoadManager.SaveExists())
            {
                GameStateData data = SaveLoadManager.LoadGame();
                if(data != null)
                {
                    LoadState(data.slidingDoorIsOpen, data.slidingDoorAnimTime);
                }
            }
        }

        public void ToggleDoor()
        {
            if (!isOpen)
            {
                StartCoroutine(OpenDoorWithDelay());
            }
            else
            {
                CloseDoor();
            }
        }

        public void OpenDoor()
        {
            if (!isOpen)
            {
                StartCoroutine(OpenDoorWithDelay());
            }
        }

        private IEnumerator OpenDoorWithDelay()
        {
            yield return new WaitForSeconds(1f);
            
            isOpen = true;
            anim.SetBool("isOpen", isOpen);
            
            if (openingSound != null)
            {
                openingSound.Play();
            }
        }

        public void CloseDoor()
        {
            isOpen = false;
            anim.SetBool("isOpen", isOpen);
        }

        public void LoadState(bool savedOpen, float savedAnimTime)
        {
            isOpen = savedOpen;
            if (isOpen)
            {
                float normalizedTime = savedAnimTime < 1f ? savedAnimTime : 1f;
                anim.Play("SlidingDoor_Open", 0, normalizedTime);
            }
            else
            {
                anim.Play("SlidingDoor_Idle", 0, 0f);
            }
        }

        public float GetAnimNormalizedTime()
        {
            return anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
    }
}
