using UnityEngine;
using Cinemachine;

public class CinemachineCameraTargetSetter : MonoBehaviour
{
    [Tooltip("The tag of the target GameObject to find.")]
    public string targetTag = "Player";

    [Tooltip("The delay in seconds before finding and setting the target.")]
    public float delay = 5f;

    [Tooltip("Reference to the Cinemachine Virtual Camera.")]
    public CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera is not assigned!");
            return;
        }

        StartCoroutine(SetCameraTargetAfterDelay());
    }

    private System.Collections.IEnumerator SetCameraTargetAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Find the GameObject with the specified tag
        GameObject targetObject = GameObject.FindWithTag(targetTag);

        if (targetObject != null)
        {
            // Set the Cinemachine camera's LookAt and Follow targets to the found GameObject
            virtualCamera.LookAt = targetObject.transform;
            virtualCamera.Follow = targetObject.transform;

            Debug.Log($"Virtual Camera target set to GameObject with tag '{targetTag}'.");
        }
        else
        {
            Debug.LogWarning($"No GameObject with tag '{targetTag}' found.");
        }
    }
}
