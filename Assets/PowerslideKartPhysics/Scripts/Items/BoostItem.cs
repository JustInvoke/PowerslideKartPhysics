// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class for boost item
    public class BoostItem : Item
    {
        public float boostAmount = 1.0f;
        public float boostForce = 1.0f;

        // Award boost to kart upon activation
        public override void Activate(ItemCastProperties props) {
            base.Activate(props);
            if (props.castKart != null) {
                if (props.castKart.canBoost) {
                    props.castKart.AddBoost(boostAmount, boostForce);
                    props.castKart.boostStartEvent.Invoke();
                }
            }
        }
    }
}