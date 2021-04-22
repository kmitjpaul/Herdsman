using System.Collections.Generic;
using UnityEngine;
using static System.Double;
using static UnityEngine.Random;

public class AvoidanceManager : MonoBehaviour
{
    public List<Agent> agents;
    public float avoidSpeed = 5f;

    public float radius = 2f;

    private void FixedUpdate()
    {
        for (var index = 1; index < agents.Count; index++) AvoidEachOther(index);
    }

    private void AvoidEachOther(int index)
    {
        var b = agents[index].rb;

        var vel = Vector2.zero;

        for (var i = 0; i < index; i++)
        {
            var a = agents[i].rb;

            var diff = a.position - b.position;
            var magnitude = diff.magnitude;

            if (magnitude > radius) continue;

            vel -= magnitude < Epsilon ? insideUnitCircle : diff.normalized;
        }

        b.velocity += vel.normalized * avoidSpeed;
    }
}