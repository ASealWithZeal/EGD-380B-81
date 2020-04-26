using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour, ISelectHandler
{
    public bool canClick = true;
    public bool positiveSound = true;

    private void Start()
    {
        if (canClick)
            gameObject.GetComponent<Button>().onClick.AddListener(SoundOnClick);
    }

    public void OnSelect(BaseEventData baseData)
    {
        if (Managers.MenuButtons.Instance.lastselect != gameObject)
            Managers.SoundEffectManager.Instance.PlaySoundClip(SFX.CursorMove, 0.25f);
    }

    public void SoundOnClick()
    {
        if (canClick)
        {
            if (positiveSound)
                Managers.SoundEffectManager.Instance.PlaySoundClip(SFX.CursorSelect, 0.15f);
            else
                Managers.SoundEffectManager.Instance.PlaySoundClip(SFX.CursorExit, 0.25f);
        }
    }
}
