using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameMusicHandler : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;
    private int lastIndex = -1;
    private bool isPaused = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomClip();
    }

    void Update()
    {
        if (!audioSource.isPlaying && !isPaused) PlayRandomClip();
    }

    void PlayRandomClip()
    {
        if (audioClips.Length == 0) return;

        int index;
        do
        {
            index = Random.Range(0, audioClips.Length);
        } while (audioClips.Length > 1 && index == lastIndex);

        lastIndex = index;
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (isPaused)
        {
            audioSource.Play();
            isPaused = false;
        }
    }

    public void RestartMusic()
    {
        audioSource.Stop();
        audioSource.Play();
    }
}
