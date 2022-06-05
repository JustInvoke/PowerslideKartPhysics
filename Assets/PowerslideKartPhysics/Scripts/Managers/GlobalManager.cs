// Copyright (c) 2022 Justin Couch / JustInvoke

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    public class GlobalManager : MonoBehaviour
    {
        private static GlobalManager _instance;
        public static GlobalManager Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<GlobalManager>();
                }
                return _instance;
            }
        }

        private Kart[] _allKarts = new Kart[0];
        public static Kart[] AllKarts {
            get {
                if (Instance._allKarts.Length == 0) {
                    FetchAllKarts();
                }
                return Instance._allKarts;
            }
            set {
                Instance._allKarts = value;
            }
        }

        public static void FetchAllKarts() {
            Instance._allKarts = FindObjectsOfType<Kart>();
        }
    }
}
