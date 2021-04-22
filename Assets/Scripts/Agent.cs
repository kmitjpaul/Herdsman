using Enums;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private float _timeToSpendInCurrentState;
    public new Collider2D collider;

    public Vector2 patrollingVelocity = Vector2.zero;

    public Rigidbody2D rb;
    public AgentState state = AgentState.Idling;

    public float TimeToSpendInCurrentState
    {
        get => _timeToSpendInCurrentState;
        set => _timeToSpendInCurrentState = Mathf.Clamp(value, 0f, Mathf.Infinity);
    }
}