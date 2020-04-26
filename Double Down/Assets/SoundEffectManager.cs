using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX
{
    ScreenTransition = 0,

    CursorMove,
    CursorSelect,
    CursorExit,

    GainEXP,
    GainLevel,

    BossFlash,
    BossFade,
    CharDeath,

    ScreenTransition2,
    
    SkillCharge,
    FireCharge,
}

namespace Managers
{
    public class SoundEffectManager : Singleton<SoundEffectManager>
    {
        public GameObject audioObj;
        public AudioClip[] clipList;

        public void PlaySoundClip(AudioClip clip, float volume)
        {
            GameObject g = Instantiate(audioObj, transform);
            g.GetComponent<SoundEffectObject>().Init(clip, volume / transform.childCount, true);
        }

        public void PlaySoundClip(SFX index, float volume)
        {
            GameObject g = Instantiate(audioObj, transform);
            g.GetComponent<SoundEffectObject>().Init(clipList[(int)index], volume / transform.childCount, false);
        }
    }
}