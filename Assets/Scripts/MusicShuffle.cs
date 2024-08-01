
using System.Collections.Generic;
using UnityEngine;

public class MusicShuffle : MonoBehaviour
{
    public AudioClip[] musicClips;
    private AudioSource audioSource;
    private List<int> shuffleOrder;
    private int currentTrackIndex;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ShuffleTracks();
        PlayNextTrack();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    void ShuffleTracks()
    {
        shuffleOrder = new List<int>();
        for (int i = 0; i < musicClips.Length; i++)
        {
            shuffleOrder.Add(i);
        }
        for (int i = 0; i < shuffleOrder.Count; i++)
        {
            int temp = shuffleOrder[i];
            int randomIndex = Random.Range(i, shuffleOrder.Count);
            shuffleOrder[i] = shuffleOrder[randomIndex];
            shuffleOrder[randomIndex] = temp;
        }
        currentTrackIndex = 0;
    }

    void PlayNextTrack()
    {
        if (shuffleOrder.Count == 0)
        {
            return;
        }

        int clipIndex = shuffleOrder[currentTrackIndex];
        audioSource.clip = musicClips[clipIndex];
        audioSource.Play();

        currentTrackIndex++;
        if (currentTrackIndex >= shuffleOrder.Count)
        {
            ShuffleTracks();
        }
    }
}