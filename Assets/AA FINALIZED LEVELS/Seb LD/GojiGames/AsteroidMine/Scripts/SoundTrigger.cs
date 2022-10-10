using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : StateMachineBehaviour
{
    public AudioClip Sound;
    private AudioSource SoundSource;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SoundSource == null)
            SoundSource = animator.GetComponent<AudioSource>();

        if (Sound == null || SoundSource == null)
            return;

        SoundSource.clip = Sound;
        SoundSource.Play();
    }
}
