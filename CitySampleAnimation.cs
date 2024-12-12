using UnityEngine;
using UnityEngine.Playables;

public class CitySampleAnimation : MonoBehaviour
{
    public PlayableDirector Timeline; // Timeline for animations and events
    public AnimationClip AnimationClip; // Animation clip for manual playback
    public ParticleSystem VFX; // Optional VFX
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
        // Play Timeline or Animation
        PlayAnimation();

        // Trigger VFX and Sound
        PlayVFX();
        PlaySound();
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
