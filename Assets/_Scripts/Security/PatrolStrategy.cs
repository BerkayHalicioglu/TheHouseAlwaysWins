using UnityEngine;

public class PatrolStrategy : IGuardStrategy
{
    public void EnterStrategy(SecurityGuardAI guard)
    {
        if (guard.Animator != null)
        {
            guard.Animator.SetBool("isWalking", true);
        }

        MoveToNextWaypoint(guard);
    }

    public void UpdateStrategy(SecurityGuardAI guard)
    {
        if (!guard.Agent.pathPending && guard.Agent.remainingDistance <= guard.Agent.stoppingDistance)
        {
            guard.ChangeStrategy(new IdleStrategy());
        }
    }

    public void ExitStrategy(SecurityGuardAI guard)
    {
        if (guard.Agent.isOnNavMesh)
        {
            guard.Agent.ResetPath();
        }
    }

    private void MoveToNextWaypoint(SecurityGuardAI guard)
    {
        if (guard.Waypoints == null || guard.Waypoints.Length == 0) return;

        Transform targetPoint = guard.Waypoints[guard.CurrentWaypointIndex];
        guard.Agent.SetDestination(targetPoint.position);

        guard.CurrentWaypointIndex = (guard.CurrentWaypointIndex + 1) % guard.Waypoints.Length;
    }
}