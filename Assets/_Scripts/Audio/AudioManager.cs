using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Müzik Listesi")]
    public AudioSource musicSource;
    public AudioClip[] musicPlaylist; 

    [Header("Ortam Sesleri")]
    public AudioSource ambianceSource;
    public AudioClip crowdAmbiance; 

    private void Awake() => Instance = this;

    private void Start()
    {
        ambianceSource.clip = crowdAmbiance;
        ambianceSource.loop = true;
        ambianceSource.Play();

        StartCoroutine(PlayPlaylist());
    }

    private IEnumerator PlayPlaylist()
    {
        while (true)
        {
            foreach (AudioClip clip in musicPlaylist)
            {
                musicSource.clip = clip;
                musicSource.Play();
                yield return new WaitForSeconds(clip.length); 
            }
        }
    }
}