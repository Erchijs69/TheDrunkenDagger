using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform tavernCenter;

    public AudioSource tavernMusic;
    public AudioSource dayAmbience;
    public AudioSource nightAmbience;

    [Header("Tavern Music Settings")]
    public float fadeDistanceMin = 5f;
    public float fadeDistanceMax = 30f;
    public float tavernVolume = 1f;

    [Header("Fading Settings")]
    public float fadeDuration = 2f;

    private bool isNight = false;
    private bool dayAmbienceStarted = false;

    void Start()
    {
        if (tavernMusic != null)
        {
            tavernMusic.volume = tavernVolume;
            tavernMusic.loop = true;
            tavernMusic.Play();
        }

        if (dayAmbience != null)
        {
            dayAmbience.volume = 0f;
            dayAmbience.loop = true;
            dayAmbience.Stop();
        }

        if (nightAmbience != null)
        {
            nightAmbience.volume = 0f;
            nightAmbience.loop = true;
            nightAmbience.Stop();
        }
    }

    void Update()
    {
        if (player == null || tavernMusic == null || tavernCenter == null)
            return;

        // Calculate distance from player to tavern center
        float distance = Vector3.Distance(player.position, tavernCenter.position);

        // Calculate target volume: 1 at fadeDistanceMin or closer, 0 at fadeDistanceMax or farther
        float targetVolume = Mathf.InverseLerp(fadeDistanceMax, fadeDistanceMin, distance) * tavernVolume;

        // Smoothly fade tavern music volume towards target
        tavernMusic.volume = Mathf.Lerp(tavernMusic.volume, targetVolume, Time.deltaTime * 5f);
    }

    public void SwitchToDayAmbience()
    {
        if (isNight)
        {
            Debug.LogWarning("Cannot switch to day ambience because it is night.");
            return; // No going back to day once night starts
        }

        if (dayAmbienceStarted)
            return;

        dayAmbienceStarted = true;

        if (dayAmbience != null)
        {
            dayAmbience.Play();
            StartCoroutine(FadeAudio(dayAmbience, 1f, fadeDuration));
        }
    }

    public void SwitchToNightAmbience()
    {
        if (isNight)
            return;

        isNight = true;

        if (dayAmbience != null && dayAmbience.isPlaying)
            StartCoroutine(FadeAudio(dayAmbience, 0f, fadeDuration));

        if (nightAmbience != null)
        {
            nightAmbience.volume = 0f;
            nightAmbience.Play();
            StartCoroutine(FadeAudio(nightAmbience, 1f, fadeDuration));
        }
    }

    private IEnumerator FadeAudio(AudioSource audio, float targetVolume, float duration)
    {
        float startVolume = audio.volume;
        float time = 0f;

        while (time < duration)
        {
            audio.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        audio.volume = targetVolume;

        if (Mathf.Approximately(targetVolume, 0f))
            audio.Stop();
    }
}
