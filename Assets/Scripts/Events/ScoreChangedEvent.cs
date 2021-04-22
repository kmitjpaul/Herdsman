using System;
using UnityEngine.Events;

namespace Events
{
    [Serializable]
    public class ScoreChangedEvent : UnityEvent<uint>
    {
    }
}