using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerInfo
{
    public static int KartLayer => LayerMask.NameToLayer("Karts");
    public static int KartBoxColliderLayer => LayerMask.NameToLayer("Kart Box Collider");
    public static int AllExcludingKarts => ~((1 << KartLayer) | (1 << KartBoxColliderLayer));
    public static int WaypointLayer => 1 << LayerMask.NameToLayer("Waypoints");
}
