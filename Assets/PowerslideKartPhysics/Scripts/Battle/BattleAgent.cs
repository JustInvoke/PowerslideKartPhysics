// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class attached to a kart that participates in a battle
    public class BattleAgent : ModeAgent
    {
        BattleController bc;
        [System.NonSerialized]
        public int health = -1;
        [System.NonSerialized]
        public int points; // Current battle point score
        [System.NonSerialized]
        public bool finishedBattle = false; // True if the kart has reached the maximum points or run out of health

        protected override void Awake() {
            base.Awake();
            bc = FindObjectOfType<BattleController>();
            if (kart != null) {
                kart.spinOutEvent.AddListener(() => { if (health > 0) health--; });
            }
        }

        private void OnTriggerEnter(Collider other) {
            BattleWaypoint point = other.GetComponent<BattleWaypoint>();
            if (point != null) {
                currentPoint = point;
            }
        }

        private void Update() {
            if (health == 0 && kart != null) {
                kart.active = false;
            }
        }

        // Respawns the kart by moving it to the last touched waypoint
        public override void Respawn() {
            base.Respawn();
            if (kart != null && kart.rotator != null) {
                kart.rotator.rotation = Quaternion.LookRotation(new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)), Vector3.up);
            }

            if (health > 0) {
                health--;
            }
        }

        // Comparer for sorting karts by their position in the race
        public class BattleAgentComparer : IComparer<BattleAgent>
        {
            public int Compare(BattleAgent x, BattleAgent y) {
                if (x == null || y == null || x.points == y.points) { return 0; }
                return y.points > x.points ? 1 : -1;
            }
        }
    }
}
