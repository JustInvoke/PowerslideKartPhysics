// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class for detecting wall collisions in different ways
    public abstract class WallCollision
    {
        public enum CollisionType { Normal, Layer, Tag, Component };

        // Create wall collision instance based on the type of collision detection
        public static WallCollision CreateFromType(CollisionType colType) {
            switch (colType) {
                case CollisionType.Normal:
                    return new NormalWallCollision();
                case CollisionType.Layer:
                    return new LayerWallCollision();
                case CollisionType.Tag:
                    return new TagWallCollision();
                case CollisionType.Component:
                    return new ComponentWallCollision();
            }
            return new NormalWallCollision();
        }

        // Actual wall detection method
        public abstract bool WallTest(WallCollisionProps props);
    }

    // Normal-based collision
    public class NormalWallCollision : WallCollision
    {
        public override bool WallTest(WallCollisionProps props) {
            return Mathf.Abs(Vector3.Dot(props.contact.normal, props.upDir)) < props.dotLimit;
        }
    }

    // Layer-based collision
    public class LayerWallCollision : WallCollision
    {
        public override bool WallTest(WallCollisionProps props) {
            return props.mask.Contains(props.contact.otherCollider.gameObject.layer);
        }
    }

    // Tag-based collision
    public class TagWallCollision : WallCollision
    {
        public override bool WallTest(WallCollisionProps props) {
            if (!string.IsNullOrEmpty(props.tag)) {
                return props.contact.otherCollider.CompareTag(props.tag);
            }
            else {
                Debug.LogWarning("Wall tag is null or empty.");
                return false;
            }
        }
    }

    // Component-based collision
    public class ComponentWallCollision : WallCollision
    {
        public override bool WallTest(WallCollisionProps props) {
            return props.contact.otherCollider.IsWall();
        }
    }

    // Struct for passing collision data
    public struct WallCollisionProps
    {
        public ContactPoint contact;
        public Vector3 upDir;
        public float dotLimit;
        public LayerMask mask;
        public string tag;

        public WallCollisionProps(ContactPoint cp, Vector3 up, float dot, LayerMask lm, string t) {
            contact = cp;
            upDir = up;
            dotLimit = dot;
            mask = lm;
            tag = t;
        }
    }

    [System.Serializable]
    // Struct for setting collision parameters in inspector
    public struct WallDetectProps
    {
        public WallCollision.CollisionType wallDetectionType;
        [Range(-1.0f, 1.0f)]
        public float wallDotLimit;
        public LayerMask wallMask;
        public string wallTag;

        public static WallDetectProps Default {
            get { return new WallDetectProps(WallCollision.CollisionType.Normal, 0.5f, 1, ""); }
        }

        public WallDetectProps(WallCollision.CollisionType detectionType, float dotLimit, LayerMask mask, string tag) {
            wallDetectionType = detectionType;
            wallDotLimit = dotLimit;
            wallMask = mask;
            wallTag = tag;
        }
    }
}
