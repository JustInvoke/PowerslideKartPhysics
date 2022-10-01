// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    // Class for controlling a race with its logic and events
    public class RaceController : ModeController
    {
        public int maxLaps = 3; // Number of laps in the race
        public BasicWaypoint startPoint; // Starting waypoint for a lap

        public Transform startingGrid; // The transform representing the orientation of the starting grid for spawning karts
        public Vector2 startingGridSize = Vector2.one; // Dimensions of the starting grid
        public int kartsPerRow = 4; // Maximum karts in a single row of the starting grid
        public float raceEndDuration = 300f; // Time limit for all karts to finish a race after the winner does
        [System.NonSerialized]
        public float raceEndTimer = -1.0f; // Current time left at the end of a race, negative indicates it's disabled

        float raceStartTime = 0.0f; // Time when the race started
        [System.NonSerialized]
        public float finalRaceTime = 0.0f; // Final time for a completed race
        [System.NonSerialized]
        public List<float> lapTimes = new List<float>(); // Lap times completed by the player
        float lastLapTime = 0.0f; // The time of the last completed lap by the player

        public UnityEvent raceTimeoutEvent; // Event invoked when the race end timer runs out

        protected override void Awake() {
            base.Awake();
        }

        // Finds all karts and adds them to the appropriate lists
        public override void FetchAllKarts() {
            base.FetchAllKarts();
            for (int i = 0; i < allKarts.Length; i++) {
                RaceAgent ra = allKarts[i].GetComponent<RaceAgent>();
                if (ra != null) {
                    ra.lapCompletedEvent.RemoveListener(CountLap);
                    ra.lapCompletedEvent.AddListener(CountLap);
                }
            }
        }

        // Spawns the player and AI karts on the starting grid
        public override void SpawnKarts(Transform playerKart) {
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
                    if (playerAgent != null) {
                        playerAgent.mc = this;
                    }
                }

                // Spawn AI karts
                for (int i = 0; i < Mathf.Min(spawnPoints.Length - 1, aiKartsToSpawn.Length); i++) {
                    GameObject newKart = Instantiate(aiKartsToSpawn[i], spawnPoints[i + 1], startingGrid.rotation);
                    KartInput ki = newKart.GetComponent<KartInput>();
                    if (ki != null) {
                        ki.enabled = false;
                    }

                    RaceAgent ra = newKart.GetComponent<RaceAgent>();
                    if (ra != null) {
                        ra.mc = this;
                    }
                }
            }

            base.SpawnKarts(playerKart);
        }

        protected override void StartCountdownCompleted() {
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

            raceStartTime = Time.timeSinceLevelLoad;
            lastLapTime = raceStartTime;
            base.StartCountdownCompleted();
        }

        // Returns the current race time
        public float GetRaceTime() {
            return modeActive ? Time.timeSinceLevelLoad - raceStartTime : 0.0f;
        }

        // Returns the time of the last completed lap
        public float GetLastLapTime() {
            return modeActive && lapTimes.Count > 0 ? lapTimes[lapTimes.Count - 1] : 0.0f;
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
                    modeActive = false;
                    modeEndEvent.Invoke();
                }

                if (raceEndTimer < 0) {
                    raceEndTimer = raceEndDuration;
                }

                if (activeKarts.Contains(agent)) {
                    activeKarts.Remove(agent);
                }

                if (!finishedKarts.Contains(agent)) {
                    finishedKarts.Add(agent);
                }
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

        protected override void Update() {
            base.Update();

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
                    activeKarts[i].finishedMode = true;
                    finishedKarts.Add(activeKarts[i]);

                    // Player-specific logic
                    if (activeKarts[i] == playerAgent) {
                        finalRaceTime = GetRaceTime();
                        modeActive = false;
                        modeEndEvent.Invoke();
                    }
                }
                activeKarts.Clear();
                raceTimeoutEvent.Invoke();
            }
        }

        protected override void SortKarts() {
            List<RaceAgent> raceAgents = activeKarts.OfType<RaceAgent>().ToList();
            raceAgents.Sort(new RaceAgent.RaceAgentComparer());
            activeKarts = raceAgents.OfType<ModeAgent>().ToList();
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
