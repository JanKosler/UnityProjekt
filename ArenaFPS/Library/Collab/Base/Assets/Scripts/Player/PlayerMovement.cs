using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [SerializeField] public float runSpeed, walkSpeed;
    [SerializeField] private float runBuildUpSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private float crouchHeight;
    [SerializeField] private float originalHeight;

    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;

    [SerializeField] [ReadOnly] private Vector3 velocity;

    [SerializeField] private KeyCode runKey;
    [SerializeField] private KeyCode jumpKey;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isRunning;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isCrouching;
    [SerializeField] private bool wasJumping;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool inMidAir;

    [SerializeField] private bool canJump;
    [SerializeField] private bool canRun;
    [SerializeField] private bool canCrouch;
    /*
    [SerializeField] public AudioClip[] walkStepSounds, runStepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] public AudioClip jumpSound;           // the sound played when character leaves the ground.
    [SerializeField] public AudioClip landSound;           // the sound played when character touches back on ground.
    */
    private AudioSource audioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip jumpSound;

    private float horizAxis;
    private float vertAxis;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        originalHeight = controller.height;
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
                                

        //jump
        if (isGrounded)
        {
            canJump = true;
            canRun = true;
            canCrouch = true;
            inMidAir = false;
            if (Input.GetButtonDown("Jump") && canJump)
            {
                canRun = false;
                wasJumping = true;
                isJumping = true;
                velocity.y = Mathf.Sqrt(jumpForce * -2f * -19.6f);
                audioSource.PlayDelayed(1.8f);
            }
            //standing on ground after fall
            if (velocity.y < 0)
            {
                velocity.y = 0f;
                isJumping = false;
                isFalling = false;
            }
        }
        else
        {
            //bashing head in ceiling
            if ((controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                velocity.y = -2f;
            }
            //check if falling
            if (velocity.y < 0)
            {
                isFalling = true;
            }
            canJump = false;
            canRun = false;
            movementSpeed *= 0.75f;
        }

        if (canCrouch)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCrouching = !isCrouching;
                CheckCrouch();
            }
        }

        horizAxis = Input.GetAxis("Horizontal");
        vertAxis = Input.GetAxis("Vertical");

        Vector3 vertMove = transform.right * horizAxis;
        Vector3 horizMove = transform.forward * vertAxis;

        isWalking = false;

        if ((horizAxis != 0 || vertAxis != 0) && !isJumping)
        {
            isWalking = true;
            if(GetComponent<AudioSource>().isPlaying == false)
            audioSource.Play();                       
        }
        controller.Move(Vector3.ClampMagnitude(vertMove + horizMove, 1f) * movementSpeed * Time.deltaTime);

        if ((horizAxis != 0 || vertAxis != 0) && OnSlope())
        {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }
        
        
        
        velocity.y += -19.6f * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        ChangeMovementSpeed();
    }
    private void CheckCrouch()
    {
        if (isCrouching)
        {
            controller.height = crouchHeight;
        }
        else
        {
            controller.height = originalHeight;
        }
    }
    private void ChangeMovementSpeed()
    {
        if (Input.GetKey(runKey) && canRun)
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
            isWalking = false; isRunning = true;
        }
        else
        {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
            isRunning = false;
        }
    }
    private bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
                return true;

        return false;
    }
}
/*
    private void PlayLandingSound()
    {
        audioSource.clip = jumpSound;
        audioSource.Play();
        //m_NextStep = m_StepCycle + .5f;
    }
    private void PlayJumpSound()
    {
        audioSource.clip = landSound;
        audioSource.Play();
    }*/
        /*
        private void ProgressStepCycle(float speed)
        {
            if (controller.velocity.sqrMagnitude > 0 && (x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (controller.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }
        */

//walk footstep sounds
/*
private void PlayRunStepAudio()
{
    if (!isGrounded)
    {
        return;
    }
    // pick & play a random footstep sound from the array,
    // excluding sound at index 0 
    int n = Random.Range(1, runStepSounds.Length);
    audioSource.clip = runStepSounds[n];
    audioSource.PlayOneShot(audioSource.clip);
    // move picked sound to index 0 so it's not picked next time
    runStepSounds[n] = runStepSounds[0];
    runStepSounds[0] = audioSource.clip;
}
//running footstep sounds
private void PlayWalkStepAudio()
{
    if (!isGrounded)
    {
        return;
    }
    // pick & play a random footstep sound from the array,
    // excluding sound at index 0 
    int n = Random.Range(1, walkStepSounds.Length);
    audioSource.clip = walkStepSounds[n];
    audioSource.PlayOneShot(audioSource.clip);
    // move picked sound to index 0 so it's not picked next time
    walkStepSounds[n] = walkStepSounds[0];
    walkStepSounds[0] = audioSource.clip;
}*/
