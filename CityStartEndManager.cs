using UnityEngine;
using ArtNotes.SimpleCityGenerator;

public class CityStartEndManager : MonoBehaviour
{
    public SimpleCityGenerator cityGenerator;
    public GameObject startPrefab;
    public GameObject endPrefab;

    void Start()
    {
        if (cityGenerator != null && cityGenerator.activationMode == SimpleCityGenerator.ActivationMode.LevelStart)
        {
            PlaceStartAndEnd();
        }
    }

    public void PlaceStartAndEnd()
    {
        if (startPrefab == null || endPrefab == null)
        {
            Debug.LogError("Start or End prefab is not assigned!");
            return;
        }

        Transform cityParent = GameObject.Find("City")?.transform;
        if (cityParent == null)
        {
            Debug.LogError("City parent object not found!");
            return;
        }

        Vector3 minPosition = Vector3.positiveInfinity;
        Vector3 maxPosition = Vector3.negativeInfinity;

        Transform minObject = null;
        Transform maxObject = null;

        foreach (Transform child in cityParent)
        {
            Vector3 childPosition = child.position;

            if (childPosition.x + childPosition.z < minPosition.x + minPosition.z)
            {
                minPosition = childPosition;
                minObject = child;
            }
            if (childPosition.x + childPosition.z > maxPosition.x + maxPosition.z)
            {
                maxPosition = childPosition;
                maxObject = child;
            }
        }

        if (minObject != null)
        {
            DestroyImmediate(minObject.gameObject);
        }
        if (maxObject != null)
        {
            DestroyImmediate(maxObject.gameObject);
        }

        Instantiate(startPrefab, minPosition, Quaternion.identity, cityParent);
        Instantiate(endPrefab, maxPosition, Quaternion.identity, cityParent);

        Debug.Log($"StartPrefab placed at: {minPosition}, original object destroyed.");
        Debug.Log($"EndPrefab placed at: {maxPosition}, original object destroyed.");
    }
}
