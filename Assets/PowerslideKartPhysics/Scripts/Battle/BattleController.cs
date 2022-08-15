// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    // Class for controlling a race with its logic and events
    [DisallowMultipleComponent]
    public class BattleController : MonoBehaviour
    {
        [System.NonSerialized]
        public BattleAgent[] allKarts = new BattleAgent[0]; // All karts in the scene
        List<BattleAgent> activeKarts = new List<BattleAgent>(); // Karts actively participating in the battle
        List<BattleAgent> finishedKarts = new List<BattleAgent>(); // Karts that have been eliminated
        BattleAgent playerAgent; // The player's kart
        public int maxHealth = 3; // Number of hit points to start with, -1 for infinite health
        public int maxPoints = -1; // Number of hit points needed to win, -1 to disable
        public float timeLimit = -1.0f; // Maximum time limit for battle, -1 to disable
        float battleTimer = 0.0f;
        BattleWaypoint[] allWaypoints; // All battle waypoints

        public GameObject[] aiKartsToSpawn = new GameObject[0]; // AI karts that will be spawned
        public int countdownDuration = 3; // Duration in seconds for the battle starting countdown

        public UnityEvent battleStartEvent; // Event invoked when the battle is started
        public UnityEvent battleEndEvent; // Event invoked when the battle ends
        [System.NonSerialized]
        public bool battleActive = false; // Whether a battle is in progress (true if the player kart has not finished)
        public Events.SingleString countDownUpdate; // Event invoked for changing the countdown text

        private void Awake() {
            FetchAllKarts();
            allWaypoints = FindObjectsOfType<BattleWaypoint>();
        }

        // Finds all karts and adds them to the appropriate lists
        public void FetchAllKarts() {
            allKarts = FindObjectsOfType<BattleAgent>();
            activeKarts.Clear();
            activeKarts.AddRange(allKarts);
        }

        // Spawns the player and AI karts on the starting grid
        public void SpawnKarts(Transform playerKart) {
            List<BattleWaypoint> availablePoints = allWaypoints.ToList();
            if (availablePoints.Count > 0) {
                // Spawn the player kart
                int spawnPointIndex = Random.Range(0, availablePoints.Count);
                if (playerKart != null) {
                    playerKart.position = availablePoints[spawnPointIndex].transform.position;
                    playerKart.rotation = Quaternion.LookRotation(new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)), Vector3.up);
                    KartInput ki = playerKart.GetComponent<KartInput>();
                    if (ki != null) {
                        ki.enabled = false;
                    }

                    BattleAgent ba = playerKart.GetComponent<BattleAgent>();
                    if (ba != null) {
                        ba.currentPoint = availablePoints[spawnPointIndex];
                    }

                    playerAgent = playerKart.GetComponent<BattleAgent>();
                    availablePoints.RemoveAt(spawnPointIndex);
                }

                // Spawn AI karts
                int spawnedKarts = 0;
                while (spawnedKarts < aiKartsToSpawn.Length && availablePoints.Count > 0) {
                    spawnPointIndex = Random.Range(0, availablePoints.Count);
                    GameObject newKart = Instantiate(aiKartsToSpawn[spawnedKarts], availablePoints[spawnPointIndex].transform.position, Quaternion.LookRotation(new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)), Vector3.up));
                    BattleAgent ba = newKart.GetComponent<BattleAgent>();
                    if (ba != null) {
                        ba.currentPoint = availablePoints[spawnPointIndex];
                    }

                    KartInput ki = newKart.GetComponent<KartInput>();
                    if (ki != null) {
                        ki.enabled = false;
                    }
                    availablePoints.RemoveAt(spawnPointIndex);
                    spawnedKarts++;
                }
            }

            GlobalManager.FetchAllKarts();
            FetchAllKarts();
            StartCoroutine(StartCountdown()); // Start the battle countdown
        }

        // Coroutine for the battle start countdown
        IEnumerator StartCountdown() {
            // Update text with numbers counting down
            for (int i = countdownDuration; i > 0; i--) {
                countDownUpdate.Invoke(i.ToString());
                yield return new WaitForSeconds(1.0f);
            }
            countDownUpdate.Invoke("Go!");

            // Activate all karts so they can drive
            for (int i = 0; i < activeKarts.Count; i++) {
                BattleAgent ba = activeKarts[i].GetComponent<BattleAgent>();
                KartInput ki = activeKarts[i].GetComponent<KartInput>();
                if (ki != null && ba != null) {
                    ki.enabled = true;
                    if (ki is BattleWaypointFollower) {
                        ((BattleWaypointFollower)ki).targetPoint = ba.currentPoint;
                    }
                }
            }

            // Start the actual battle
            battleActive = true;
            battleTimer = timeLimit;
            battleStartEvent.Invoke();

            // Remove the go! text from the countdown UI after the race has started
            yield return new WaitForSeconds(1.0f);
            countDownUpdate.Invoke("");
        }

        //if (activeKarts.Contains(agent)) {
        //    activeKarts.Remove(agent);
        //}
        //finishedKarts.Add(agent);

        // Returns the position of the given kart in a race
        public int GetPositionOfKart(BattleAgent kart) {
            if (activeKarts.Contains(kart)) {
                return activeKarts.IndexOf(kart) + finishedKarts.Count;
            }
            if (finishedKarts.Contains(kart)) {
                return finishedKarts.IndexOf(kart);
            }
            return -1;
        }

        // Returns all karts sorted by their positions
        public BattleAgent[] GetAllKarts() {
            List<BattleAgent> agents = new List<BattleAgent>();
            agents.AddRange(finishedKarts);
            agents.AddRange(activeKarts);
            return agents.ToArray();
        }

        private void Update() {
            // Sorts the karts by their battle positions
            activeKarts.Sort(new BattleAgent.BattleAgentComparer());

            if (battleActive) {
                if (timeLimit > 0 && battleTimer > 0) {
                    timeLimit -= Time.deltaTime;
                }
                else if ((timeLimit > 0 && battleTimer <= 0) || activeKarts.Count == 0) {
                    for (int i = 0; i < activeKarts.Count; i++) {
                        activeKarts[i].finishedBattle = true;
                        finishedKarts.Add(activeKarts[i]);

                        // Player-specific logic
                        if (activeKarts[i] == playerAgent) {
                            battleActive = false;
                            battleEndEvent.Invoke();
                        }
                    }
                    activeKarts.Clear();
                }
            }
        }
    }
}
