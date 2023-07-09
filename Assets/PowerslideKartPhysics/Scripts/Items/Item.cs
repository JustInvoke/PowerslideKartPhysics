// Copyright (c) 2023 Justin Couch / JustInvoke
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class for different items to be used
    public abstract class Item : MonoBehaviour
    {
        public string itemName = "Item";
        protected ItemCastProperties castProps;
        protected Kart[] allKarts = new Kart[0];

        protected virtual void Awake() {
            allKarts = FindObjectsOfType<Kart>();
        }

        // Called upon activation
        public virtual void Activate(ItemCastProperties props) {
            props.allKarts = allKarts;
            castProps = props;
        }

        // Called upon deactivation
        public virtual void Deactivate() {
        }
    }
}