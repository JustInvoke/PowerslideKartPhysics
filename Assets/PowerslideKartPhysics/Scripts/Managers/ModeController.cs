using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    // Base class for controlling different modes
    [DisallowMultipleComponent]
    public abstract class ModeController : MonoBehaviour
    {
        [System.NonSerialized]
        public ModeAgent[] allKarts = new ModeAgent[0]; // All karts in the scene
        protected List<ModeAgent> activeKarts = new List<ModeAgent>(); // Karts actively participating in the mode
        protected List<ModeAgent> finishedKarts = new List<ModeAgent>(); // Karts that have finished the mode
        protected ModeAgent playerAgent; // The player's kart
        protected ModeAgent.ModeAgentComparer kartComparer;

        public GameObject[] aiKartsToSpawn = new GameObject[0]; // AI karts that will be spawned
        public int countdownDuration = 3; // Duration in seconds for the mode starting countdown

        public UnityEvent modeStartEvent; // Event invoked when the mode is started
        public UnityEvent modeEndEvent; // Event invoked when the mode ends
        [System.NonSerialized]
        public bool modeActive = false; // Whether a mode is in progress (true if the player kart has not finished)
        public Events.SingleString countDownUpdate; // Event invoked for changing the countdown text

        protected virtual void Awake() {
            FetchAllKarts();
        }

        // Finds all karts and adds them to the appropriate lists
        public virtual void FetchAllKarts() {
            allKarts = FindObjectsOfType<ModeAgent>();
            activeKarts.Clear();
            activeKarts.AddRange(allKarts);
        }

        // Spawns the player and AI karts on the starting grid
        public virtual void SpawnKarts(Transform playerKart) {
            GlobalManager.FetchAllKarts();
            FetchAllKarts();
            StartCoroutine(StartCountdown()); // Start the mode countdown
        }

        // Coroutine for the mode start countdown
        IEnumerator StartCountdown() {
            // Update text with numbers counting down
            for (int i = countdownDuration; i > 0; i--) {
                countDownUpdate.Invoke(i.ToString());
                yield return new WaitForSeconds(1.0f);
            }
            countDownUpdate.Invoke("Go!");

            StartCountdownCompleted();

            // Remove the go! text from the countdown UI after the mode has started
            yield return new WaitForSeconds(1.0f);
            countDownUpdate.Invoke("");
        }

        // Called when the start countdown finishes and the mode actually starts
        protected virtual void StartCountdownCompleted() {
            // Start the actual mode
            modeActive = true;
            modeStartEvent.Invoke();
        }

        // Returns the position of the given kart in a mode
        public int GetPositionOfKart(ModeAgent kart) {
            if (activeKarts.Contains(kart)) {
                return activeKarts.IndexOf(kart) + finishedKarts.Count;
            }
            if (finishedKarts.Contains(kart)) {
                return finishedKarts.IndexOf(kart);
            }
            return -1;
        }

        // Returns all karts sorted by their positions
        public ModeAgent[] GetAllKartsSorted() {
            List<ModeAgent> agents = new List<ModeAgent>();
            agents.AddRange(finishedKarts);
            agents.AddRange(activeKarts);
            return agents.ToArray();
        }

        protected virtual void Update() {
            // Sorts the karts by their mode positions
            activeKarts.Sort(kartComparer);
        }
    }
}
