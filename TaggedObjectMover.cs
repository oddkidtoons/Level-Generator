using UnityEngine;
using System.Collections;

public class TaggedObjectMover : MonoBehaviour
{
    [Tooltip("The tag of the GameObject to move.")]
    public string targetTag = "Player";

    [Tooltip("The transform to which the tagged object will be moved.")]
    public Transform targetLocation;

    [Tooltip("Delay (in seconds) before attempting to find the tagged GameObject.")]
    public float findDelay = 0.5f;

    [Tooltip("Number of retries to find the tagged GameObject.")]
    public int retryCount = 10;

    [Tooltip("Delay (in seconds) between each retry.")]
    public float retryInterval = 0.5f;

    private void Start()
    {
        StartCoroutine(FindAndMoveTaggedObject());
    }

    private IEnumerator FindAndMoveTaggedObject()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(findDelay);

        GameObject targetObject = null;

        for (int i = 0; i < retryCount; i++)
        {
            // Attempt to find the GameObject with the tag
            targetObject = GameObject.FindWithTag(targetTag);

            // If not found, check for inactive GameObjects
            if (targetObject == null)
            {
                foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
                {
                    if (obj.CompareTag(targetTag))
                    {
                        targetObject = obj;
                        break;
                    }
                }
            }

            if (targetObject != null)
                break;

            Debug.LogWarning($"Attempt {i + 1}/{retryCount}: No GameObject found with tag '{targetTag}'. Retrying in {retryInterval} seconds...");
            yield return new WaitForSeconds(retryInterval);
        }

        if (targetObject == null)
        {
            Debug.LogError($"Failed to find GameObject with tag '{targetTag}' after {retryCount} attempts. Ensure the object exists and is properly tagged.");
            yield break;
        }

        if (targetLocation == null)
        {
            Debug.LogError("Target Location is not assigned. Please assign an empty GameObject as the destination.");
            yield break;
        }

        // Activate the object if it is inactive
        if (!targetObject.activeSelf)
        {
            targetObject.SetActive(true);
        }

        // Move the entire hierarchy by manipulating the parent transform
        MoveHierarchy(targetObject.transform, targetLocation);

        Debug.Log($"GameObject with tag '{targetTag}' (and its children) moved to {targetLocation.position}.");
    }

    private void MoveHierarchy(Transform targetTransform, Transform newLocation)
    {
        // Move the parent object
        targetTransform.position = newLocation.position;
        targetTransform.rotation = newLocation.rotation;

        // Iterate through and move all children relative to the parent
        foreach (Transform child in targetTransform)
        {
            Vector3 relativePosition = child.localPosition;
            Quaternion relativeRotation = child.localRotation;

            child.position = newLocation.TransformPoint(relativePosition);
            child.rotation = newLocation.rotation * relativeRotation;
        }
    }
}
