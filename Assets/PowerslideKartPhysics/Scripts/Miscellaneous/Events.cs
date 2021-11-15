// Copyright (c) 2022 Justin Couch / JustInvoke
using System;
using UnityEngine;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    // This class contains special types of Unity Events, allowing for event invocations with dynamic parameters
    public class Events
    {
        [Serializable]
        public class SingleBool : UnityEvent<bool> { }
        [Serializable]
        public class DoubleBool : UnityEvent<bool, bool> { }
        [Serializable]
        public class SingleFloat : UnityEvent<float> { }
        [Serializable]
        public class DoubleFloat : UnityEvent<float, float> { }
        [Serializable]
        public class FloatBool : UnityEvent<float, bool> { }
        [Serializable]
        public class SingleVector3 : UnityEvent<Vector3> { }
        [Serializable]
        public class DoubleVector3 : UnityEvent<Vector3, Vector3> { }
    }
}