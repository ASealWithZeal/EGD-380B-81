using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectObject : MonoBehaviour
{
    public AudioSource source;

    public void Init(AudioClip clip, float volume, bool shiftPitch)
    {
        source.clip = clip;
        source.volume = volume;
        if (shiftPitch)
            source.pitch += Random.Range(-0.05f, 0.05f);
        source.Play();

        Invoke("DestroyObj", clip.length + 0.1f);
    }

    private void DestroyObj()
    {
        Destroy(gameObject);
    }
}
