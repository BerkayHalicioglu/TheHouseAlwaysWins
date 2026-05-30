using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BartenderManager : MonoBehaviour
{
    public static BartenderManager Instance;

    [Header("Barmen Görsel ve Animasyon")]
    public Animator bartenderAnimator;

    [Header("Arayüz ve Efektler")]
    public GameObject lowStaminaNotification;
    public Volume globalVolume;
    public Image staminaFillBar;

    [Header("Yorgunluk (Körlük) Ayarlarý")]
    public float maxBlindness = 0.9f;
    public float blindnessSpeed = 0.05f;

    [Header("Ses Efektleri (Audio)")]
    public AudioSource heartbeatSound;
    public AudioSource breathingSound;
    public AudioSource environmentSound;

    private float originalEnvVolume = 1f;

    private Vignette vignette;
    private Color originalBarColor;
    private Color drinkBarColor = new Color(0f, 0.5f, 1f);

    private bool isDrinking = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (globalVolume != null && globalVolume.profile.TryGet(out Vignette v))
            vignette = v;

        if (lowStaminaNotification != null) lowStaminaNotification.SetActive(false);
        if (staminaFillBar != null) originalBarColor = staminaFillBar.color;

        if (environmentSound != null) originalEnvVolume = environmentSound.volume;
        if (heartbeatSound != null) heartbeatSound.volume = 0f;
        if (breathingSound != null) breathingSound.volume = 0f;
    }

    private void OnEnable()
    {
        StaminaManager.OnStaminaChanged += CheckStaminaEffects;
    }

    private void OnDisable()
    {
        StaminaManager.OnStaminaChanged -= CheckStaminaEffects;
    }

    private void Update()
    {
        if (isDrinking || StaminaManager.Instance == null) return;

        if (StaminaManager.Instance.CurrentStamina <= 0f)
        {
            if (heartbeatSound != null && !heartbeatSound.isPlaying)
            {
                heartbeatSound.volume = 1f;
                heartbeatSound.Play();
            }
            if (breathingSound != null && !breathingSound.isPlaying)
            {
                breathingSound.volume = 1f;
                breathingSound.Play();
            }

            if (vignette != null && vignette.intensity.value < maxBlindness)
            {
                vignette.intensity.value += blindnessSpeed * Time.deltaTime;

                if (environmentSound != null)
                    environmentSound.volume = Mathf.Lerp(originalEnvVolume, originalEnvVolume * 0.2f, vignette.intensity.value / maxBlindness);
            }
        }
    }

    private void CheckStaminaEffects(float currentStamina, float maxStamina)
    {
        if (isDrinking) return;

        float staminaPercentage = currentStamina / maxStamina;

        if (currentStamina > 0 && staminaPercentage <= 0.25f)
        {
            if (lowStaminaNotification != null) lowStaminaNotification.SetActive(true);
            if (vignette != null) vignette.intensity.value = Mathf.Lerp(0.6f, 0f, staminaPercentage / 0.25f);
        }
        else if (currentStamina > 0 && staminaPercentage > 0.25f)
        {
            if (lowStaminaNotification != null) lowStaminaNotification.SetActive(false);
            if (vignette != null) vignette.intensity.value = 0f;
        }
        else if (currentStamina <= 0)
        {
            if (lowStaminaNotification != null) lowStaminaNotification.SetActive(true);
        }
    }

    public void DrinkCocktail()
    {
        if (isDrinking || StaminaManager.Instance.CurrentStamina >= StaminaManager.Instance.MaxStamina)
            return;

        // Animasyonu tetikle
        if (bartenderAnimator != null)
        {
            bartenderAnimator.SetTrigger("IckiHazirla");
        }

        StartCoroutine(RefillStaminaOverTime());
    }

    private IEnumerator RefillStaminaOverTime()
    {
        isDrinking = true;
        if (lowStaminaNotification != null) lowStaminaNotification.SetActive(false);
        if (staminaFillBar != null) staminaFillBar.color = drinkBarColor;

        float duration = 5f;
        float elapsed = 0f;
        float startStamina = StaminaManager.Instance.CurrentStamina;
        float targetStamina = StaminaManager.Instance.MaxStamina;
        float startVignette = vignette != null ? vignette.intensity.value : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            float newStamina = Mathf.Lerp(startStamina, targetStamina, progress);
            float amountToAdd = newStamina - StaminaManager.Instance.CurrentStamina;

            if (amountToAdd > 0)
            {
                StaminaManager.Instance.RefillStamina(amountToAdd);
            }

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(startVignette, 0f, progress);

                if (environmentSound != null)
                    environmentSound.volume = Mathf.Lerp(originalEnvVolume * 0.2f, originalEnvVolume, progress);
            }

            yield return null;
        }

        StaminaManager.Instance.RefillStamina(StaminaManager.Instance.MaxStamina);

        if (vignette != null) vignette.intensity.value = 0f;
        if (staminaFillBar != null) staminaFillBar.color = originalBarColor;

        if (environmentSound != null) environmentSound.volume = originalEnvVolume;

        if (heartbeatSound != null) heartbeatSound.Stop();
        if (breathingSound != null) breathingSound.Stop();

        isDrinking = false;
    }
}