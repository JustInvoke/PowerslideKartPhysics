// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // This class manages items to be used by karts, deriving items from child objects
    public class ItemManager : MonoBehaviour
    {
        Item[] items = new Item[0];

        private void Awake() {
            items = GetComponentsInChildren<Item>();
        }

        // Return a random item from the list of items
        public Item GetRandomItem() {
            if (items.Length == 0) { return null; }
            return items[Random.Range(0, items.Length)];
        }

        // Return an item of a specific type from the list if it exists
        public Item GetItem<T>() where T : Item {
            for (int i = 0; i < items.Length; i++) {
                if (items[i] is T) {
                    return items[i];
                }
            }
            return null;
        }

        // Return an item by its object name if it exists in the list
        public Item GetItem(string itemName) {
            for (int i = 0; i < items.Length; i++) {
                if (string.Compare(itemName, items[i].itemName, true) == 0 || string.Compare(itemName, items[i].transform.name, true) == 0) {
                    return items[i];
                }
            }
            return null;
        }
    }
}
