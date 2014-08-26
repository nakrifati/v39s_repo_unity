using UnityEngine;
using System.Collections.Generic;

public class AudioTracks : MonoBehaviour
{
    public AudioSource aSource;

    public List<AudioClip> aClips = new List<AudioClip>();

    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!aSource.isPlaying)
        {
            aSource.clip = aClips[Random.Range(0, aClips.Count)];
            aSource.Play();
        }
    }
}
