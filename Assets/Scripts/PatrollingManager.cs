using System.Collections.Generic;
using Enums;
using UnityEngine;
using static UnityEngine.Random;

public class PatrollingManager : MonoBehaviour
{
    public List<Agent> agents;
    public float rayCastLength = 5f;
    public LayerMask rayCastTarget;

    private void FixedUpdate()
    {
        ManageStateTransition();
        ProcessStates();
    }

    private void ManageStateTransition()
    {
        foreach (var flockAgent in agents)
        {
            flockAgent.TimeToSpendInCurrentState -= Time.deltaTime;

            if (flockAgent.TimeToSpendInCurrentState > 0f) continue;

            switch (flockAgent.state)
            {
                case AgentState.Idling:
                    flockAgent.state = AgentState.MovingAround;
                    break;
                case AgentState.MovingAround:
                    flockAgent.state = AgentState.Idling;
                    break;
            }

            flockAgent.TimeToSpendInCurrentState = Range(0, 1f);
        }
    }

    private void ProcessStates()
    {
        foreach (var flockAgent in agents)
        {
            switch (flockAgent.state)
            {
                case AgentState.Idling:
                    flockAgent.patrollingVelocity = Vector2.zero;
                    break;

                case AgentState.MovingAround when flockAgent.patrollingVelocity == Vector2.zero:
                    flockAgent.patrollingVelocity = insideUnitCircle;
                    break;

                case AgentState.MovingAround:

                    var h = Physics2D.Raycast(flockAgent.rb.position, flockAgent.patrollingVelocity, rayCastLength,
                        rayCastTarget);

                    if (h)
                        flockAgent.patrollingVelocity = Vector2.Reflect(flockAgent.patrollingVelocity, h.normal);

                    break;
                case AgentState.Following:
                    flockAgent.patrollingVelocity = Vector2.zero;
                    break;
            }

            flockAgent.rb.velocity += flockAgent.patrollingVelocity;
        }
    }
}