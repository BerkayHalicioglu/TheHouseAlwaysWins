using UnityEngine;

public class IdleStrategy : IGuardStrategy
{
    private float _waitTimer;
    private float _targetWaitTime; 

    public void EnterStrategy(SecurityGuardAI guard)
    {
        _waitTimer = 0f;

        _targetWaitTime = guard.GetRandomWaitTime();

        guard.Agent.isStopped = true;

        if (guard.Animator != null)
        {
            guard.Animator.SetBool("isWalking", false);
        }
    }

    public void UpdateStrategy(SecurityGuardAI guard)
    {
        _waitTimer += Time.deltaTime;

        if (_waitTimer >= _targetWaitTime)
        {
            guard.ChangeStrategy(new PatrolStrategy());
        }
    }

    public void ExitStrategy(SecurityGuardAI guard)
    {
        guard.Agent.isStopped = false;
    }
}