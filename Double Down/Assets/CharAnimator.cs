using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationClips
{
    Idle = 0,
    Move,
    CombatIdle,
    Attack,
    Magic,
    Defend,
}

public class CharAnimator : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayAnimations(AnimationClips clips)
    {
        switch (clips)
        {
            case AnimationClips.Idle:
                anim.SetBool("idling", true);
                anim.SetBool("combatIdling", false);
                anim.SetBool("moving", false);
                break;
            case AnimationClips.Move:
                anim.SetBool("idling", false);
                anim.SetBool("combatIdling", false);
                anim.SetBool("moving", true);
                break;
            case AnimationClips.CombatIdle:
                anim.SetBool("moving", false);
                anim.SetBool("idling", false);
                anim.SetBool("combatIdling", true);
                break;
        }
    }
}
