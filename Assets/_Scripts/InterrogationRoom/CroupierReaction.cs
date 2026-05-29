using UnityEngine;
using TMPro;

public class CroupierReaction : MonoBehaviour
{
    [Header("Tepki Yazýsý")]
    public TextMeshPro reactionText;

    private string[] punishReactions = {
        "Patron bugün acýmasýz...",
        "Yine birini benzettiler galiba...",
        "Göz temasý kurma, sadece kartlara odaklan...",
        "Arka odadan pek iyi sesler gelmiyordu.",
        "Birisi kurallarý zor yoldan öðrendi.",
        "Halýlara kan sýçramamýþtýr umarým...",
        "Ben sadece maaþýma bakarým, gerisi beni bozmaz.",
        "En azýndan benim masamda yakalanmadý.",
        "Patronun damarýna basmamak lazýmdý.",
        "", "", "", "", "", "", "", "" 
    };

    private string[] releaseReactions = {
        "Patron bugün merhametli.",
        "Þanslý herif, ucuz yýrttý...",
        "Bugün iyi günündeyiz anlaþýlan.",
        "Ben olsam o kadar kolay býrakmazdým.",
        "Sadece sert bir uyarýydý herhalde.",
        "Demek ki arkasý saðlammýþ, yoksa çýkamazdý.",
        "Umarým dersini almýþtýr.",
        "Ensesi kalýn birine benziyordu zaten.",
        "Hadi bakalým, oyun devam ediyor...",
        "", "", "", "", "", "", "", "" 
    };

    private void Start()
    {
        if (reactionText != null)
        {
            reactionText.text = "";
        }
    }

    private void OnEnable()
    {
        InterrogationManager.OnSuspectPunished += ReactToPunish;
        InterrogationManager.OnSuspectReleased += ReactToRelease;
    }

    private void OnDisable()
    {
        InterrogationManager.OnSuspectPunished -= ReactToPunish;
        InterrogationManager.OnSuspectReleased -= ReactToRelease;
    }

    private void ReactToPunish()
    {
        if (reactionText != null)
        {
            string chosenText = punishReactions[Random.Range(0, punishReactions.Length)];
            reactionText.text = chosenText;
            reactionText.color = Color.red;
        }
    }

    private void ReactToRelease()
    {
        if (reactionText != null)
        {
            string chosenText = releaseReactions[Random.Range(0, releaseReactions.Length)];
            reactionText.text = chosenText;
            reactionText.color = Color.green;
        }
    }
}