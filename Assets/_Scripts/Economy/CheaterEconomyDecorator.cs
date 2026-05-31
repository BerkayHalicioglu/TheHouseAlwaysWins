using UnityEngine;

// Decorator: hileci müşteri normal çözümlemenin üzerine ek kazanma şansı ekler.
// İç resolver (StandardEconomyResolver) değiştirilmeden yeni davranış katmanlanır.
public class CheaterEconomyDecorator : IEconomyResolver
{
    private readonly IEconomyResolver inner;
    private const float ExtraWinChance = 0.40f; // hileci her eldede %40 ek kazanma şansı

    public CheaterEconomyDecorator(IEconomyResolver inner)
    {
        this.inner = inner;
    }

    public void Resolve(float betAmount)
    {
        // Hile sayesinde ek bir kazanç penceresi — inner'ı devre dışı bırakır
        if (Random.value < ExtraWinChance)
        {
            EconomyManager.Instance.DeductMoney(betAmount);
            return;
        }

        // Hile tutmadıysa normal çözümlemeye düşer
        inner.Resolve(betAmount);
    }
}
