// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    // Class for controlling a battle with its logic and events
    public class BattleController : ModeController
    {
        public int maxHealth = 3; // Number of hit points to start with, -1 for infinite health
        public int maxPoints = -1; // Number of hit points needed to win, -1 to disable
        public float timeLimit = -1.0f; // Maximum time limit for battle, -1 to disable
        float battleTimer = 0.0f;
        BattleWaypoint[] allWaypoints; // All battle waypoints

        protected override void Awake() {
            base.Awake();
            allWaypoints = FindObjectsOfType<BattleWaypoint>();
        }

        public void CalculateWaypointConnections() {
            if (allWaypoints == null || !Application.isPlaying) {
                allWaypoints = FindObjectsOfType<BattleWaypoint>();
            }

            foreach (BattleWaypoint waypoint in allWaypoints) {
                waypoint.CalculateConnections(allWaypoints);
            }
        }

        public void SnapWaypointsToGround() {
            if (allWaypoints == null || !Application.isPlaying) {
                allWaypoints = FindObjectsOfType<BattleWaypoint>();
            }

            foreach (BattleWaypoint waypoint in allWaypoints) {
                waypoint.SnapToGround();
            }
        }

        // Spawns the player and AI karts on the starting grid
        public override void SpawnKarts(Transform playerKart) {
            List<BattleWaypoint> availablePoints = allWaypoints.ToList();
            if (availablePoints.Count > 0) {
                // Spawn the player kart
                int spawnPointIndex = Random.Range(0, availablePoints.Count);
                BattleWaypoint spawnPoint = availablePoints[spawnPointIndex];
                if (playerKart != null) {
                    playerKart.position = spawnPoint.transform.position;
                    playerKart.rotation = Quaternion.LookRotation((spawnPoint.GetNextPoint().transform.position - spawnPoint.transform.position).normalized, Vector3.up);
                    KartInput ki = playerKart.GetComponent<KartInput>();
                    if (ki != null) {
                        ki.enabled = false;
                    }

                    BattleAgent ba = playerKart.GetComponent<BattleAgent>();
                    if (ba != null) {
                        ba.currentPoint = spawnPoint;
                        ba.health = maxHealth;
                        ba.mc = this;
                    }

                    playerAgent = ba;
                    availablePoints.RemoveAt(spawnPointIndex);
                }

                // Spawn AI karts
                int spawnedKarts = 0;
                while (spawnedKarts < aiKartsToSpawn.Length && availablePoints.Count > 0) {
                    spawnPointIndex = Random.Range(0, availablePoints.Count);
                    spawnPoint = availablePoints[spawnPointIndex];
                    GameObject newKart = Instantiate(aiKartsToSpawn[spawnedKarts], spawnPoint.transform.position, Quaternion.LookRotation((spawnPoint.GetNextPoint().transform.position - spawnPoint.transform.position).normalized, Vector3.up));
                    BattleAgent ba = newKart.GetComponent<BattleAgent>();
                    KartInput ki = newKart.GetComponent<KartInput>();
                    if (ki != null) {
                        ki.enabled = false;
                    }

                    if (ba != null) {
                        ba.currentPoint = spawnPoint;
                        ba.health = maxHealth;
                        ba.mc = this;
                    }
                    availablePoints.RemoveAt(spawnPointIndex);
                    spawnedKarts++;
                }
            }

            base.SpawnKarts(playerKart);
        }

        protected override void StartCountdownCompleted() {
            // Activate all karts so they can drive
            for (int i = 0; i < activeKarts.Count; i++) {
                BattleAgent ba = activeKarts[i].GetComponent<BattleAgent>();
                KartInput ki = activeKarts[i].GetComponent<KartInput>();
                if (ki != null && ba != null) {
                    ki.enabled = true;
                    if (ki is BattleWaypointFollower) {
                        ((BattleWaypointFollower)ki).targetPoint = ba.currentPoint.GetNextPoint();
                        ((BattleWaypointFollower)ki).Initialize();
                    }
                }
            }

            battleTimer = timeLimit;
            base.StartCountdownCompleted();
        }

        protected override void Update() {
            base.Update();

            if (modeActive) {
                if (timeLimit > 0 && battleTimer > 0) {
                    timeLimit -= Time.deltaTime;
                }
                else if ((timeLimit > 0 && battleTimer <= 0) || activeKarts.Count == 0) {
                    for (int i = 0; i < activeKarts.Count; i++) {
                        activeKarts[i].finishedMode = true;
                        finishedKarts.Add(activeKarts[i]);

                        // Player-specific logic
                        if (activeKarts[i] == playerAgent) {
                            modeActive = false;
                            modeEndEvent.Invoke();
                        }
                    }
                    activeKarts.Clear();
                }
            }
        }

        protected override void SortKarts() {
            List<BattleAgent> battleAgents = activeKarts.OfType<BattleAgent>().ToList();
            battleAgents.Sort(new BattleAgent.BattleAgentComparer());
            activeKarts = battleAgents.OfType<ModeAgent>().ToList();
        }
    }
}
