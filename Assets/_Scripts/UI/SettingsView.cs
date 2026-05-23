using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsView : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button backButton;

    [Header("Labels")]
    [SerializeField] private TMP_Text volumeValueText;
    [SerializeField] private TMP_Text sensitivityValueText;

    private const string VOLUME_KEY = "Volume";
    private const string SENSITIVITY_KEY = "Sensitivity";
    private const string FULLSCREEN_KEY = "Fullscreen";

    private void Awake()
    {
        gameObject.SetActive(false);

        if (backButton != null)
            backButton.onClick.AddListener(Hide);

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
    }

    public void Show()
    {
        LoadSettings();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        SaveSettings();
        gameObject.SetActive(false);
    }

    private void LoadSettings()
    {
        float volume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        float sensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, 0.25f);
        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;

        if (volumeSlider != null) volumeSlider.value = volume;
        if (sensitivitySlider != null) sensitivitySlider.value = sensitivity;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;

        ApplyVolume(volume);
        ApplySensitivity(sensitivity);
        ApplyFullscreen(fullscreen);
    }

    private void SaveSettings()
    {
        if (volumeSlider != null)
            PlayerPrefs.SetFloat(VOLUME_KEY, volumeSlider.value);

        if (sensitivitySlider != null)
            PlayerPrefs.SetFloat(SENSITIVITY_KEY, sensitivitySlider.value);

        if (fullscreenToggle != null)
            PlayerPrefs.SetInt(FULLSCREEN_KEY, fullscreenToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();
    }

    private void OnVolumeChanged(float value)
    {
        ApplyVolume(value);
        if (volumeValueText != null)
            volumeValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    private void OnSensitivityChanged(float value)
    {
        ApplySensitivity(value);
        if (sensitivityValueText != null)
            sensitivityValueText.text = value.ToString("F2");
    }

    private void OnFullscreenChanged(bool value)
    {
        ApplyFullscreen(value);
    }

    private void ApplyVolume(float value)
    {
        AudioListener.volume = value;
        if (volumeValueText != null)
            volumeValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    private void ApplySensitivity(float value)
    {
        if (sensitivityValueText != null)
            sensitivityValueText.text = value.ToString("F2");

        PlayerLook playerLook = Object.FindFirstObjectByType<PlayerLook>();
        if (playerLook != null)
            playerLook.SetSensitivity(value);
    }

    private void ApplyFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }
}