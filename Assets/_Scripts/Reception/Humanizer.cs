using UnityEngine;

public class Humanizer : MonoBehaviour
{
    private void Start()
    {
        Animator anim = GetComponentInChildren<Animator>();

        if (anim != null)
        {
            anim.speed = Random.Range(0.85f, 1.15f);
        }
    }
}