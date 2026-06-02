using UnityEngine;
using TMPro; 
using System.Collections; 

public class CroupierReaction : MonoBehaviour
{
    [Header("UI Ayarlarý")]
    public TextMeshProUGUI reactionText; 
    public GameObject bubbleContainer; 
    public float displayTime = 4f; 

    private Coroutine _currentReactionRoutine;

    
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
  
        if (reactionText != null) reactionText.text = "";
        if (bubbleContainer != null) bubbleContainer.SetActive(false);
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
        string chosenText = punishReactions[Random.Range(0, punishReactions.Length)];
        ShowReaction(chosenText, Color.red);
    }

    private void ReactToRelease()
    {
        string chosenText = releaseReactions[Random.Range(0, releaseReactions.Length)];
        ShowReaction(chosenText, Color.green);
    }

    private void ShowReaction(string message, Color textColor)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        if (_currentReactionRoutine != null)
        {
            StopCoroutine(_currentReactionRoutine);
        }

        _currentReactionRoutine = StartCoroutine(ReactionCoroutine(message, textColor));
    }

    private IEnumerator ReactionCoroutine(string message, Color textColor)
    {
        if (reactionText != null)
        {
            reactionText.text = message;
            reactionText.color = textColor;
        }

        if (bubbleContainer != null)
        {
            bubbleContainer.SetActive(true);
        }

        yield return new WaitForSeconds(displayTime);

        if (bubbleContainer != null)
        {
            bubbleContainer.SetActive(false);
        }

        if (reactionText != null)
        {
            reactionText.text = "";
        }
    }
}