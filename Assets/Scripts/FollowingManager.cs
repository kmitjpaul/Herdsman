using System.Collections.Generic;
using UnityEngine;

public class FollowingManager : MonoBehaviour
{
    public List<Collider2D> agents;

    public float followSpeed = 10f;

    public float minRadius = 5f;
    public LayerMask rayCastTarget;

    private void FixedUpdate()
    {
        for (var index = 1; index < agents.Count; index++) FollowTarget(new[] {index - 1, 0}, index);
    }

    private void FollowTarget(IEnumerable<int> targetIndexes, int followerIndex)
    {
        var follower = agents[followerIndex];
        foreach (var targetIndex in targetIndexes)
        {
            var target = agents[targetIndex];

            var positionA = target.attachedRigidbody.position;
            var positionB = follower.attachedRigidbody.position;

            var diff = positionA - positionB;

            if (Physics2D.Linecast(positionB, positionA, rayCastTarget))
                continue;

            if (diff.magnitude <= minRadius) return;

            follower.attachedRigidbody.velocity += diff.normalized * followSpeed;

            return;
        }
    }
}