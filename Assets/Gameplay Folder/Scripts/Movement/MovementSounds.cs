using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSounds : MonoBehaviour
{
    public CharacterController characterController;
    float Timer = 0.0f;

    public AudioClip walkAudio;
    public AudioSource walkSource;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (characterController.isGrounded == true && characterController.velocity.magnitude > 1.0f)
        {
            if (Timer > 0.3f)
            {
                if (walkSource.isPlaying == false)
                {
                    walkSource.PlayOneShot(walkAudio);
                    Timer = 0.0f;
                }
                else if(walkSource.isPlaying == true)
                {
                    Timer = 0.0f;
                }
            }

            Timer += Time.deltaTime;
        }
        else if(characterController.isGrounded == true && characterController.velocity.magnitude < 1.0f)
        {
            walkSource.Stop();
        }
    }
}