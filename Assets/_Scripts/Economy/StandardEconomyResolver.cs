using UnityEngine;

public class StandardEconomyResolver : IEconomyResolver
{
    private readonly float npcWinChance;

    public StandardEconomyResolver(float npcWinChance)
    {
        this.npcWinChance = npcWinChance;
    }

    public void Resolve(float betAmount)
    {
        if (Random.value < npcWinChance)
            EconomyManager.Instance.DeductMoney(betAmount);
        else
            EconomyManager.Instance.AddMoney(betAmount);
    }
}
