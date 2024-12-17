using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

using ArtNotes.SimpleCityGenerator;

public class CityStartEndManager : MonoBehaviour
{
    [Header("City Prefabs")]
    public SimpleCityGenerator cityGenerator;
    public GameObject startPrefab;
    public GameObject endPrefab;
    public GameObject selected2x2Prefab; // Selected 2x2 prefab to place
    public bool useSelected2x2 = true; // Toggle to enable or disable using the selected 2x2 prefab

    [Header("Boss Level Settings")]
    public GameObject bossPrefab; // Boss level prefab
    public float bossHeightAboveCity = 50f; // Height to spawn the boss above the city

    void Start()
    {
        if (cityGenerator != null && cityGenerator.activationMode == SimpleCityGenerator.ActivationMode.LevelStart)
        {
            PlaceStartAndEnd();

            // Only call PlaceSelected2x2 if the toggle is enabled and a prefab is assigned
            if (useSelected2x2 && selected2x2Prefab != null)
            {
                PlaceSelected2x2();
            }

            // Place the Boss Level above the city center
            if (bossPrefab != null)
            {
                PlaceBossLevel();
            }
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

    public void PlaceSelected2x2()
    {
        if (selected2x2Prefab == null)
        {
            Debug.LogWarning("Selected 2x2 prefab is not assigned!");
            return;
        }

        Transform cityParent = GameObject.Find("City")?.transform;
        if (cityParent == null)
        {
            Debug.LogError("City parent object not found!");
            return;
        }

        Vector3 startPosition = Vector3.positiveInfinity;
        Vector3 endPosition = Vector3.negativeInfinity;

        foreach (Transform child in cityParent)
        {
            Vector3 childPosition = child.position;

            if (childPosition.x + childPosition.z < startPosition.x + startPosition.z)
            {
                startPosition = childPosition;
            }
            if (childPosition.x + childPosition.z > endPosition.x + endPosition.z)
            {
                endPosition = childPosition;
            }
        }

        Vector3 centerPosition = (startPosition + endPosition) / 2;

        Transform closest2x2Object = null;
        float closestDistance = float.MaxValue;

        foreach (Transform child in cityParent)
        {
            Vector3 childPosition = child.position;

            if (child.name.Contains("2x2"))
            {
                float distance = Vector3.Distance(centerPosition, childPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest2x2Object = child;
                }
            }
        }

        if (closest2x2Object != null)
        {
            Vector3 replacementPosition = closest2x2Object.position;
            Quaternion replacementRotation = closest2x2Object.rotation;

            DestroyImmediate(closest2x2Object.gameObject);

            Instantiate(selected2x2Prefab, replacementPosition, replacementRotation, cityParent);
            Debug.Log($"Selected 2x2 prefab placed at: {replacementPosition}, replacing the closest 2x2 to center.");
        }
        else
        {
            Debug.LogWarning("No matching 2x2 object found to replace in the city!");
        }
    }

    public void PlaceBossLevel()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("Boss prefab is not assigned!");
            return;
        }

        Transform cityParent = GameObject.Find("City")?.transform;
        if (cityParent == null)
        {
            Debug.LogError("City parent object not found!");
            return;
        }

        // Calculate the center of the city
        Vector3 centerPosition = Vector3.zero;
        int childCount = 0;

        foreach (Transform child in cityParent)
        {
            centerPosition += child.position;
            childCount++;
        }

        if (childCount > 0)
        {
            centerPosition /= childCount; // Calculate average position
        }

        // Adjust the position to be above the city
        Vector3 bossPosition = new Vector3(centerPosition.x, centerPosition.y + bossHeightAboveCity, centerPosition.z);

        // Spawn the boss prefab
        Instantiate(bossPrefab, bossPosition, Quaternion.identity, cityParent);
        Debug.Log($"Boss prefab placed at: {bossPosition}, above the city center.");
    }
}
