using System;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    [Serializable]
    public class GoCollisionEvent : UnityEvent<Collider2D>
    {
    }
}