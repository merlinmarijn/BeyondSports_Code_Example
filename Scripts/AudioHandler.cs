using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [Header("Audio list")]
    [Tooltip("List starts at index 0 so -1 from the visual index it in to reference specific audio clip")]
    public List<AudioClip> Audio;
    public AudioClip music;

    public AudioClip getAudio(int id)
    {
        return Audio[id];
    }


    public void PlaySound(int id, GameObject obj, float vol = 1f, float pitch = 1f, bool RndPitch = false)
    {
        AudioSource Source = obj.GetComponent<AudioSource>();
        if (RndPitch)
        {
            pitch = pitch + Random.Range(-(pitch / 16), (pitch / 16));
        }
        //Debug.Log(pitch);
        Source.pitch = pitch;
        Source.clip = Audio[id];
        Source.volume = 1 * vol;
        Source.Play();
    }
}
