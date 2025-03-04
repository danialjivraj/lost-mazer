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
    }
}
