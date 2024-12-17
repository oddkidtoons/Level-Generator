using UnityEngine;
using UnityEngine.Playables;

public class CitySampleAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public PlayableDirector Timeline; // Timeline for animations and events
    public AnimationClip AnimationClip; // Animation clip for manual playback
    public float AnimationSpeed = 1.0f; // Default animation speed (1 = normal)
    public bool RandomizeSpeed = false; // Toggle to randomize animation speed
    public float MinSpeed = 0.5f; // Minimum random speed
    public float MaxSpeed = 2.0f; // Maximum random speed

    [Header("Visual Effects")]
    public ParticleSystem VFX; // Optional VFX

    [Header("Audio")]
    public AudioClip SpawnSound; // Optional sound to play on spawn
    private AudioSource audioSource;

    private Animator animator;

    private void Awake()
    {
        // Set up AudioSource
        if (SpawnSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = SpawnSound;
        }

        // Set up Animator if AnimationClip is provided
        if (AnimationClip != null)
        {
            animator = gameObject.GetComponent<Animator>();
            if (animator == null)
                animator = gameObject.AddComponent<Animator>();

            animator.runtimeAnimatorController = CreateRuntimeAnimatorController();
        }

        // Prepare VFX
        if (VFX != null)
        {
            VFX.Stop();
        }
    }

    private void OnEnable()
    {
        // Set animation speed
        SetAnimationSpeed();

        // Play Timeline or Animation
        PlayAnimation();

        // Trigger VFX and Sound
        PlayVFX();
        PlaySound();
    }

    private void SetAnimationSpeed()
    {
        if (RandomizeSpeed)
        {
            // Randomize animation speed between MinSpeed and MaxSpeed
            AnimationSpeed = Random.Range(MinSpeed, MaxSpeed);
        }

        // Adjust the speed of PlayableDirector if it exists
        if (Timeline != null)
        {
            Timeline.playableGraph.GetRootPlayable(0).SetSpeed(AnimationSpeed);
        }
        else if (animator != null)
        {
            animator.speed = AnimationSpeed; // Set the speed for Animator
        }
    }

    private void PlayAnimation()
    {
        if (Timeline != null)
        {
            Timeline.Play(); // Play assigned Timeline asset
        }
        else if (AnimationClip != null && animator != null)
        {
            animator.Play(AnimationClip.name); // Play the animation clip
        }
    }

    private void PlayVFX()
    {
        if (VFX != null)
        {
            VFX.Play();
        }
    }

    private void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    private RuntimeAnimatorController CreateRuntimeAnimatorController()
    {
        // Create a runtime Animator Controller for the animation clip
        var animatorController = new UnityEditor.Animations.AnimatorController();
        var state = animatorController.AddMotion(AnimationClip);
        return animatorController;
    }
}
