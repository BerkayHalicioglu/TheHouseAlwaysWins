using UnityEngine;

public interface IGuardStrategy
{
    void EnterStrategy(SecurityGuardAI guard);
    void UpdateStrategy(SecurityGuardAI guard);
    void ExitStrategy(SecurityGuardAI guard);
}