// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // This class controls the menu in the demo and spawns karts
    public class DemoMenu : MonoBehaviour
    {
        public Vector3 spawnPoint = Vector3.zero;
        public Vector3 spawnDir = Vector3.forward;
        public GameObject uiContainer;
        public KartCamera kartCam;
        public KartGravityPreset antiGravPreset;
        bool useAntiGrav = false;

        private void Awake() {
            // Hide UI when showing the menu
            if (uiContainer != null) {
                uiContainer.SetActive(false);
            }
        }

        public void SpawnKart(GameObject kart) {
            // Spawn a given kart at the spawn point
            Kart newKart = null;
            if (kart != null) {
                newKart = Instantiate(kart, spawnPoint, Quaternion.LookRotation(spawnDir.normalized, Vector3.up)).GetComponent<Kart>();

                // Set anti-gravity mode
                if (useAntiGrav && newKart.GetComponent<KartPresetControl>() != null) {
                    newKart.GetComponent<KartPresetControl>().LoadGravityPreset(antiGravPreset);
                    newKart.GetComponent<Rigidbody>().useGravity = false;
                }
            }

            // Show the UI and connect it to the spawned kart
            if (uiContainer != null) {
                UIControl uiController = uiContainer.GetComponent<UIControl>();
                if (uiController != null) {
                    uiController.Initialize(newKart);
                }

                uiContainer.SetActive(true);
            }

            // Connect the camera to the spawned kart
            if (kartCam != null) {
                kartCam.Initialize(newKart);
            }

            gameObject.SetActive(false);
        }

        // Set whether to use anti-gravity preset for the spawned kart
        public void SetAntiGrav(bool grav) {
            useAntiGrav = grav;
        }

        // Visualize the kart spawn point
        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPoint, 0.5f);
            Gizmos.DrawRay(spawnPoint + spawnDir.normalized * 0.5f, spawnDir.normalized);
        }
    }
}
