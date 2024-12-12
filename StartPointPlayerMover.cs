using UnityEngine;
using PLAYERTWO.PlatformerProject;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("PLAYER TWO/Platformer Project/Misc/Start Point Player Mover")]
public class StartPointPlayerMover : MonoBehaviour
{
    [Tooltip("The transform to which the player will be moved and respawned.")]
    public Transform playerStartLocation;

    public AudioClip activationClip; // Optional: sound when the start point is activated.
    public UnityEvent OnStartPointActivated; // Optional: event when the start point is activated.

    private AudioSource audioSource;

    private void Start()
    {
        if (playerStartLocation == null)
        {
            Debug.LogError("Player Start Location is not assigned. Please assign an empty GameObject in the prefab.");
            return;
        }

        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("Player GameObject not found. Make sure the Player is in the scene and tagged as 'Player'.");
            return;
        }

        if (playerObject.TryGetComponent<Player>(out var player))
        {
            // Activate the player if it is inactive
            if (!playerObject.activeSelf)
            {
                playerObject.SetActive(true);
            }

            // Move the player to the start location
            player.transform.position = playerStartLocation.position;
            player.transform.rotation = playerStartLocation.rotation;

            // Set the respawn point for the player
            player.SetRespawn(playerStartLocation.position, playerStartLocation.rotation);

            Debug.Log($"Player moved to start location at {playerStartLocation.position} and respawn point set.");

            // Play activation sound if provided
            if (activationClip != null)
            {
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }

                audioSource.PlayOneShot(activationClip);
            }

            // Invoke any additional events
            OnStartPointActivated?.Invoke();
        }
        else
        {
            Debug.LogError("The Player does not have a Player component. Ensure the Player script is attached.");
        }
    }
}
