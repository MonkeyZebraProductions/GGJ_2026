using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MusicController : MonoBehaviour
{
    [System.Serializable]
    public class Track
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("Audio")]
    [SerializeField] private AudioSource source;

    [Header("AudioCry")]
    [SerializeField] private AudioSource crySource;

    [Header("Tracks")]
    [SerializeField] private List<Track> tracks = new();

    [Header("Fade In")]
    [SerializeField] private float fadeInSeconds = 1f;
    [SerializeField] private bool loop = true;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (source == null) source = GetComponent<AudioSource>();
    }

    private Track FindTrack(string trackName)
    {
        trackName = trackName.Trim().Trim('"');

        for (int i = 0; i < tracks.Count; i++)
            if (tracks[i].name == trackName)
                return tracks[i];

        return null;
    }

    [YarnCommand("playCry")]
    public void PlayCry()
    {
        crySource.Play();
    }

    [YarnCommand("stopCry")]
    public void StopCry()
    {
        crySource.Stop();
    }


    [YarnCommand("music")]
    public void Play(string trackName)
    {
        var t = FindTrack(trackName);

        if (source.isPlaying && source.clip == t.clip)
            return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        source.Stop();
        source.clip = t.clip;
        source.loop = loop;
        source.volume = 0f;
        source.Play();

        fadeRoutine = StartCoroutine(FadeInTo(t.volume));
    }

    private IEnumerator FadeInTo(float targetVolume)
    {
        float t = 0f;
        float dur = Mathf.Max(0.01f, fadeInSeconds);

        while (t < dur)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, t / dur);
            yield return null;
        }

        source.volume = targetVolume;
        fadeRoutine = null;
    }
}
