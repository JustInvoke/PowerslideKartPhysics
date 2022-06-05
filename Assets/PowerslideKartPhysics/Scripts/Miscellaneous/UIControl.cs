// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PowerslideKartPhysics
{
    // This class controls the demo UI while driving a kart
    public class UIControl : MonoBehaviour
    {
        public Kart targetKart;
        ItemCaster caster;
        RaceAgent agent;
        RaceController raceController;
        public GameObject hudContainer;
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
        public Text timeText;
        public Text lapText;
        public Text positionText;
        public Text countdownText;
        bool showingLapTime = false;
        public float lastLapShowDuration = 5.0f;
        public GameObject raceSummary;
        public GameObject lapTimeHolder;
        public GameObject kartPositionHolder;
        List<Text> kartNames = new List<Text>();
        List<Text> kartLaps = new List<Text>();
        List<Text> kartPositions = new List<Text>();
        public Text raceEndTime;

        private void Awake() {
            Initialize(targetKart);

            // Connect race events to UI updates
            raceController = FindObjectOfType<RaceController>();
            if (raceController != null)
            {
                raceController.countDownUpdate.AddListener(UpdateCountdownText);
                raceController.raceEndEvent.AddListener(ShowRaceSummary);
            }

            if (raceSummary != null)
            {
                raceSummary.SetActive(false);
            }
        }

        // Set up references to a kart and its item caster
        public void Initialize(Kart kart) {
            targetKart = kart;
            if (targetKart != null) {
                caster = targetKart.GetComponent<ItemCaster>();
                agent = targetKart.GetComponent<RaceAgent>();
                if (agent != null)
                {
                    agent.lapCompletedEvent.AddListener(ShowLapTime);
                }
            }
        }

        // Updates the text showing the race start countdown
        public void UpdateCountdownText(string countdown)
        {
            if (countdownText != null)
            {
                countdownText.text = countdown;
            }
        }

        // Shows the last completed lap time for the lastLapShowDuration
        void ShowLapTime(int lap, RaceAgent ra)
        {
            StartCoroutine(LapShowCycle());
        }

        // Coroutine for showing the lap time
        IEnumerator LapShowCycle()
        {
            showingLapTime = true;
            yield return new WaitForSeconds(lastLapShowDuration);
            showingLapTime = false;
        }

        // Calling this shows all lap times and race summary at the end of a race
        public void ShowRaceSummary()
        {
            // Hide the HUD
            if (hudContainer != null)
            {
                hudContainer.SetActive(false);
            }

            // Show the race summary UI
            if (raceSummary != null)
            {
                raceSummary.SetActive(true);
            }

            // Get the total race time and lap times for the player
            if (raceController == null) { return; }
            List<float> times = raceController.lapTimes;
            times.Insert(0, raceController.finalRaceTime);

            // Duplicates the reference time holder and assigns each to show a different lap time
            if (lapTimeHolder != null && times.Count > 0)
            {
                for (int i = 0; i < times.Count; i++)
                {
                    GameObject curTimeHolder = i > 0 ? Instantiate(lapTimeHolder, lapTimeHolder.transform.parent, false) : lapTimeHolder;
                    Transform timeLabel = curTimeHolder.transform.Find("Lap");
                    Transform timeValue = curTimeHolder.transform.Find("Time");
                    if (timeLabel != null)
                    {
                        Text t1 = timeLabel.GetComponent<Text>();
                        if (t1 != null)
                        {
                            t1.text = i > 0 ? "Lap " + i.ToString() : "Total Time";
                        }
                        Text t2 = timeValue.GetComponent<Text>();
                        if (t2 != null)
                        {
                            t2.text = F.TimeFormat(times[i]);
                        }
                    }
                }
            }

            // Duplicates the reference kart position holder and keeps track of them
            if (kartPositionHolder != null)
            {
                for (int i = 0; i < raceController.allKarts.Length; i++)
                {
                    GameObject curKartHolder = i > 0 ? Instantiate(kartPositionHolder, kartPositionHolder.transform.parent, false) : kartPositionHolder;
                    Transform kartLabel = curKartHolder.transform.Find("Kart");
                    Transform lapLabel = curKartHolder.transform.Find("Laps");
                    Transform positionLabel = curKartHolder.transform.Find("Position");
                    if (kartLabel != null)
                    {
                        Text t = kartLabel.GetComponent<Text>();
                        if (t != null)
                        {
                            kartNames.Add(t);
                        }
                    }

                    if (lapLabel != null)
                    {
                        Text t = lapLabel.GetComponent<Text>();
                        if (t != null)
                        {
                            kartLaps.Add(t);
                        }
                    }

                    if (positionLabel != null)
                    {
                        Text t = positionLabel.GetComponent<Text>();
                        if (t != null)
                        {
                            t.text = IntToOrdinal(i + 1);
                            kartPositions.Add(t);
                        }
                    }
                }
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

            // Updates the lap count text and race position text of the player kart
            if (agent != null)
            {
                if (raceController != null && lapText != null)
                {
                    lapText.text = agent.lap.ToString() + " / " + raceController.maxLaps.ToString();
                }

                if (positionText != null)
                {
                    positionText.text = IntToOrdinal(agent.GetRacePosition() + 1);
                }
            }

            if (raceController != null)
            {
                // Shows either the current race time or last lap time
                if (timeText != null)
                {
                    timeText.enabled = !showingLapTime || Mathf.Sin(Time.time * 10f) > 0;
                    timeText.text = F.TimeFormat(showingLapTime ? raceController.GetLastLapTime() : raceController.GetRaceTime());
                }

                // Update the kart positions and laps on the race summary
                if (raceSummary != null)
                {
                    if (raceSummary.activeSelf && kartNames.Count == kartLaps.Count && kartLaps.Count == kartPositions.Count)
                    {
                        RaceAgent[] karts = raceController.GetAllKarts();
                        for (int i = 0; i < karts.Length; i++)
                        {
                            kartNames[i].text = karts[i].transform.name;
                            kartLaps[i].text = Mathf.Min(karts[i].lapsCompleted, raceController.maxLaps).ToString() + "/" + raceController.maxLaps.ToString();
                        }
                    }
                }

                // Update race end timer text
                if (raceEndTime != null)
                {
                    raceEndTime.enabled = raceController.raceEndTimer >= 0;
                    if (raceEndTime.enabled)
                    {
                        raceEndTime.text = F.TimeFormat(Mathf.Max(0.0f, raceController.raceEndTimer));
                    }
                }
            }
        }

        // Returns a string of an integer with an ordinal suffix appended
        public string IntToOrdinal(int n)
        {
            switch (n)
            {
                case 1:
                    return n.ToString() + "st";
                case 2:
                    return n.ToString() + "nd";
                case 3:
                    return n.ToString() + "rd";
                default:
                    return n.ToString() + "th";
            }
        }

        // Reloads the currently loaded level
        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
