using UnityEngine;
using TMPro;

public class UI_NeonPulse : MonoBehaviour
{
    [Header("Görsel Ayarlar")]
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1f;

    void Start()
    {
        if (targetText == null) targetText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (targetText != null)
        {
            Color textColor = targetText.color;
            float wave = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            textColor.a = Mathf.Lerp(minAlpha, maxAlpha, wave);
            targetText.color = textColor;
        }
    }
}