// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    // Class for controlling a race with its logic and events
    [DisallowMultipleComponent]
    public class RaceController : MonoBehaviour
    {
        [System.NonSerialized]
        public RaceAgent[] allKarts = new RaceAgent[0]; // All karts in the scene
        List<RaceAgent> activeKarts = new List<RaceAgent>(); // Karts actively participating in the race
        List<RaceAgent> finishedKarts = new List<RaceAgent>(); // Karts that have finished the race
        RaceAgent playerAgent; // The player's kart
        public int maxLaps = 3; // Number of laps in the race
        public BasicWaypoint startPoint; // Starting waypoint for a lap

        public GameObject[] aiKartsToSpawn = new GameObject[0]; // AI karts that will be spawned
        public Transform startingGrid; // The transform representing the orientation of the starting grid for spawning karts
        public Vector2 startingGridSize = Vector2.one; // Dimensions of the starting grid
        public int kartsPerRow = 4; // Maximum karts in a single row of the starting grid
        public int countdownDuration = 3; // Duration in seconds for the race starting countdown
        public float raceEndDuration = 300f; // Time limit for all karts to finish a race after the winner does
        [System.NonSerialized]
        public float raceEndTimer = -1.0f; // Current time left at the end of a race, negative indicates it's disabled

        float raceStartTime = 0.0f; // Time when the race started
        [System.NonSerialized]
        public float finalRaceTime = 0.0f; // Final time for a completed race
        [System.NonSerialized]
        public List<float> lapTimes = new List<float>(); // Lap times completed by the player
        float lastLapTime = 0.0f; // The time of the last completed lap by the player

        public UnityEvent raceStartEvent; // Event invoked when the race is started
        public UnityEvent raceEndEvent; // Event invoked when the race ends
        public UnityEvent raceTimeoutEvent; // Event invoked when the race end timer runs out
        [System.NonSerialized]
        public bool raceActive = false; // Whether a race is in progress (true if the player kart has not finished)
        public Events.SingleString countDownUpdate; // Event invoked for changing the countdown text

        private void Awake() {
            FetchAllKarts();
        }

        // Finds all karts and adds them to the appropriate lists
        public void FetchAllKarts() {
            allKarts = FindObjectsOfType<RaceAgent>();
            for (int i = 0; i < allKarts.Length; i++) {
                RaceAgent ra = allKarts[i].GetComponent<RaceAgent>();
                if (ra != null) {
                    ra.lapCompletedEvent.RemoveListener(CountLap);
                    ra.lapCompletedEvent.AddListener(CountLap);
                }
            }
            activeKarts.Clear();
            activeKarts.AddRange(allKarts);
        }

        // Spawns the player and AI karts on the starting grid
        public void SpawnKarts(Transform playerKart) {
            Vector3[] spawnPoints = CalculateKartSpawnPoints();
            if (spawnPoints.Length > 0 && startingGrid != null) {
                // Spawn the player kart
                if (playerKart != null) {
                    playerKart.position = spawnPoints[0];
                    playerKart.rotation = startingGrid.rotation;
                    KartInput ki = playerKart.GetComponent<KartInput>();
                    if (ki != null) {
                        ki.enabled = false;
                    }

                    playerAgent = playerKart.GetComponent<RaceAgent>();
                }

                // Spawn AI karts
                for (int i = 0; i < Mathf.Min(spawnPoints.Length - 1, aiKartsToSpawn.Length); i++) {
                    GameObject newKart = Instantiate(aiKartsToSpawn[i], spawnPoints[i + 1], startingGrid.rotation);
                    KartInput ki = newKart.GetComponent<KartInput>();
                    if (ki != null) {
                        ki.enabled = false;
                    }
                }
            }

            GlobalManager.FetchAllKarts();
            FetchAllKarts();
            StartCoroutine(StartCountdown()); // Start the race countdown
        }

        // Coroutine for the race start countdown
        IEnumerator StartCountdown() {
            // Update text with numbers counting down
            for (int i = countdownDuration; i > 0; i--) {
                countDownUpdate.Invoke(i.ToString());
                yield return new WaitForSeconds(1.0f);
            }
            countDownUpdate.Invoke("Go!");

            // Activate all karts so they can drive
            for (int i = 0; i < activeKarts.Count; i++) {
                RaceAgent ra = activeKarts[i].GetComponent<RaceAgent>();
                if (ra != null) {
                    ra.currentPoint = startPoint;
                }

                KartInput ki = activeKarts[i].GetComponent<KartInput>();
                if (ki != null) {
                    ki.enabled = true;
                    if (ki is BasicWaypointFollower) {
                        ((BasicWaypointFollower)ki).targetPoint = startPoint;
                    }
                    else if (ki is BasicWaypointFollowerDrift) {
                        ((BasicWaypointFollowerDrift)ki).targetPoint = startPoint;
                        ((BasicWaypointFollowerDrift)ki).Initialize();
                    }
                }
            }

            // Start the actual race
            raceStartTime = Time.timeSinceLevelLoad;
            lastLapTime = raceStartTime;
            raceActive = true;
            raceStartEvent.Invoke();

            // Remove the go! text from the countdown UI after the race has started
            yield return new WaitForSeconds(1.0f);
            countDownUpdate.Invoke("");
        }

        // Returns the current race time
        public float GetRaceTime() {
            return raceActive ? Time.timeSinceLevelLoad - raceStartTime : 0.0f;
        }

        // Returns the time of the last completed lap
        public float GetLastLapTime() {
            return raceActive && lapTimes.Count > 0 ? lapTimes[lapTimes.Count - 1] : 0.0f;
        }

        // Counts a lap completed by a kart
        public void CountLap(int lap, RaceAgent agent) {
            // Track lap times completed by the player
            if (agent == playerAgent) {
                lapTimes.Add(Time.timeSinceLevelLoad - lastLapTime);
                lastLapTime = Time.timeSinceLevelLoad;
            }

            // A kart finished the race if it completed all laps
            if (lap >= maxLaps && !agent.finishedRace) {
                agent.finishedRace = true;
                agent.lapsCompleted = maxLaps;

                // End the race when the player finishes all laps
                if (agent == playerAgent) {
                    finalRaceTime = GetRaceTime();
                    raceActive = false;
                    raceEndEvent.Invoke();
                }

                if (raceEndTimer < 0) {
                    raceEndTimer = raceEndDuration;
                }

                if (activeKarts.Contains(agent)) {
                    activeKarts.Remove(agent);
                }
                finishedKarts.Add(agent);
            }
        }

        // Calculates the spawn points for all karts on the starting grid
        public Vector3[] CalculateKartSpawnPoints() {
            List<Vector3> spawnPoints = new List<Vector3>();
            if (startingGrid != null) {
                kartsPerRow = Mathf.Max(1, kartsPerRow);
                int maxRows = Mathf.CeilToInt((aiKartsToSpawn.Length + 1) * 1.0f / kartsPerRow);
                float rowSize = 1.0f / maxRows;
                float colSize = 1.0f / Mathf.Min(kartsPerRow, aiKartsToSpawn.Length + 1);
                int row = 0;
                int col = 0;
                for (int i = 0; i < aiKartsToSpawn.Length + 1; i++) {
                    spawnPoints.Add(startingGrid.TransformPoint(
                        startingGridSize.x * (-0.5f + colSize * (col + 0.5f)),
                        0.0f,
                        startingGridSize.y * (0.5f - rowSize * (row + 0.5f))
                        ));

                    if (col < kartsPerRow - 1) {
                        col++;
                    }
                    else {
                        col = 0;
                        row++;
                    }
                }
            }
            return spawnPoints.ToArray();
        }

        // Returns the position of the given kart in a race
        public int GetPositionOfKart(RaceAgent kart) {
            if (activeKarts.Contains(kart)) {
                return activeKarts.IndexOf(kart) + finishedKarts.Count;
            }
            if (finishedKarts.Contains(kart)) {
                return finishedKarts.IndexOf(kart);
            }
            return -1;
        }

        // Returns all karts sorted by their positions
        public RaceAgent[] GetAllKarts() {
            List<RaceAgent> agents = new List<RaceAgent>();
            agents.AddRange(finishedKarts);
            agents.AddRange(activeKarts);
            return agents.ToArray();
        }

        private void Update() {
            // Sorts the karts by their race positions
            activeKarts.Sort(new RaceAgent.RaceAgentComparer());

            // Decrements the race end timer and forces all karts to stop racing when the timer runs out
            if (raceEndTimer > 0) {
                raceEndTimer = Mathf.Max(0.0f, raceEndTimer - Time.deltaTime);
                if (activeKarts.Count == 0) {
                    raceEndTimer = 0.0f;
                }
            }
            else if (raceEndTimer == 0) // When the timer runs out
            {
                raceEndTimer = -1.0f;
                for (int i = 0; i < activeKarts.Count; i++) {
                    activeKarts[i].finishedRace = true;
                    finishedKarts.Add(activeKarts[i]);

                    // Player-specific logic
                    if (activeKarts[i] == playerAgent) {
                        finalRaceTime = GetRaceTime();
                        raceActive = false;
                        raceEndEvent.Invoke();
                    }
                }
                activeKarts.Clear();
                raceTimeoutEvent.Invoke();
            }
        }

        private void OnDrawGizmos() {
            // Visualizes the starting grid layout
            if (startingGrid != null) {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(startingGrid.TransformPoint(0.0f, 0.0f, startingGridSize.y * 0.5f - 1.0f), startingGrid.TransformPoint(0.0f, 0.0f, startingGridSize.y * 0.5f + 1.0f));
                Gizmos.DrawLine(startingGrid.TransformPoint(startingGridSize.x * 0.5f, 0.0f, startingGridSize.y * 0.5f), startingGrid.TransformPoint(startingGridSize.x * 0.5f, 0.0f, startingGridSize.y * -0.5f));
                Gizmos.DrawLine(startingGrid.TransformPoint(startingGridSize.x * 0.5f, 0.0f, startingGridSize.y * -0.5f), startingGrid.TransformPoint(startingGridSize.x * -0.5f, 0.0f, startingGridSize.y * -0.5f));
                Gizmos.DrawLine(startingGrid.TransformPoint(startingGridSize.x * -0.5f, 0.0f, startingGridSize.y * -0.5f), startingGrid.TransformPoint(startingGridSize.x * -0.5f, 0.0f, startingGridSize.y * 0.5f));
                Gizmos.DrawLine(startingGrid.TransformPoint(startingGridSize.x * -0.5f, 0.0f, startingGridSize.y * 0.5f), startingGrid.TransformPoint(startingGridSize.x * 0.5f, 0.0f, startingGridSize.y * 0.5f));

                Vector3[] spawnPoints = CalculateKartSpawnPoints();
                for (int i = 0; i < spawnPoints.Length; i++) {
                    Gizmos.color = i > 0 ? Color.red : new Color(1.0f, 0.5f, 0.0f); // The player's spawn point is drawn in orange
                    Gizmos.DrawWireSphere(spawnPoints[i], 0.5f);
                }
            }
        }
    }
}
