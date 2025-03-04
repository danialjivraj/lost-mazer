using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavKeypad
{
    public class KeypadInteractionFPV : MonoBehaviour
    {
        private Camera cam;
        private bool inReach;

        private void Awake() => cam = Camera.main;

        private void Update()
        {
            if (Time.timeScale == 0) return;

            var ray = cam.ScreenPointToRay(Input.mousePosition);

            if (inReach && Input.GetButtonDown("Interact"))
            {
                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.collider.TryGetComponent(out KeypadButton keypadButton))
                    {
                        keypadButton.PressButton();
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Reach"))
            {
                inReach = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Reach"))
            {
                inReach = false;
            }
        }
    }
}