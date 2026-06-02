using UnityEngine;
using System.Collections;

public class InterrogationRoomAudio : MonoBehaviour
{
    [Header("Ses Kaynaklarý")]
    public AudioSource mainCasinoMusic; 
    public AudioSource roomBuzzSound;   

    [Header("Ses Ayarlarý")]
    public float fadeDuration = 1.5f; 
    public float loweredCasinoVolume = 0.15f; 

    private float _originalCasinoVolume;
    private Coroutine _fadeCoroutine;

    private void Start()
    {
        if (mainCasinoMusic != null)
        {
            _originalCasinoVolume = mainCasinoMusic.volume;
        }

        if (roomBuzzSound != null)
        {
            roomBuzzSound.volume = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeAudio(loweredCasinoVolume, 1f));

            if (roomBuzzSound != null && !roomBuzzSound.isPlaying)
            {
                roomBuzzSound.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeAudio(_originalCasinoVolume, 0f));
        }
    }

    private IEnumerator FadeAudio(float targetCasinoVolume, float targetRoomVolume)
    {
        float startCasinoVolume = mainCasinoMusic != null ? mainCasinoMusic.volume : 0f;
        float startRoomVolume = roomBuzzSound != null ? roomBuzzSound.volume : 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeDuration;

            if (mainCasinoMusic != null)
                mainCasinoMusic.volume = Mathf.Lerp(startCasinoVolume, targetCasinoVolume, progress);

            if (roomBuzzSound != null)
                roomBuzzSound.volume = Mathf.Lerp(startRoomVolume, targetRoomVolume, progress);

            yield return null;
        }

        if (roomBuzzSound != null && targetRoomVolume == 0f)
        {
            roomBuzzSound.Stop();
        }
    }
}