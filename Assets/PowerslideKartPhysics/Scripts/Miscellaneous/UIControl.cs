// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PowerslideKartPhysics
{
    // This class controls the demo UI while driving a kart
    public class UIControl : MonoBehaviour
    {
        public Kart targetKart;
        ItemCaster caster;
        public Slider boostMeter;
        public Image boostMeterFill;
        public Color boostReadyColor = Color.green;
        public Color boostNotReadyColor = Color.red;
        public Slider boostReserveMeter;
        public float boostReserveCap = 10f;
        public Slider airTimeMeter;
        public float airTimeCap = 3.0f;
        public Text itemText;
        public Text ammoText;

        private void Awake() {
            Initialize(targetKart);
        }

        // Set up references to a kart and its item caster
        public void Initialize(Kart kart) {
            targetKart = kart;
            if (targetKart != null) {
                caster = targetKart.GetComponent<ItemCaster>();
            }
        }

        private void Update() {
            if (targetKart == null) { return; }

            // Set the boost meter fill amount and color
            if (boostMeter != null) {
                boostMeter.value = targetKart.GetBoostValue();

                if (boostMeterFill != null) {
                    boostMeterFill.color = targetKart.IsBoostReady() ? boostReadyColor : boostNotReadyColor;
                }
            }

            // Set the boost reserve meter fill amount
            if (boostReserveMeter != null) {
                boostReserveMeter.value = targetKart.boostReserve / Mathf.Max(0.01f, targetKart.boostReserveLimit < Mathf.Infinity ? targetKart.boostReserveLimit : boostReserveCap);
            }

            // Set the air time meter fill amount
            if (airTimeMeter != null) {
                airTimeMeter.value = targetKart.GetJumpedAirTime() / Mathf.Max(0.01f, airTimeCap);
            }

            // Set the item text to show the name of the equipped item and the ammo count
            if (caster != null) {
                if (itemText != null) {
                    if (caster.item != null) {
                        itemText.text = caster.ammo > 0 ? caster.item.itemName : "No Item";
                    }
                    else {
                        itemText.text = "No Item";
                    }
                }

                if (ammoText != null) {
                    ammoText.text = caster.ammo > 0 ? "x" + caster.ammo : "";
                }
            }
        }
    }
}
