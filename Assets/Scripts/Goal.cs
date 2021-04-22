using Events;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GoCollisionEvent ev = new GoCollisionEvent();

    private void OnTriggerEnter2D(Collider2D other)
    {
        ev.Invoke(other);
    }
}