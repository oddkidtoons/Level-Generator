using UnityEngine;
using UnityEngine.AI;

namespace Unity.AI.Navigation
{
    public class DynamicNavMeshBaker : MonoBehaviour
    {
        [Tooltip("The NavMeshSurface component responsible for baking the NavMesh.")]
        public NavMeshSurface navMeshSurface;

        [Tooltip("The name of the parent GameObject containing the generated level.")]
        public string cityParentName = "City";

        public void BakeNavMesh()
        {
            // Find or assign NavMeshSurface dynamically if not already assigned
            if (navMeshSurface == null)
            {
                navMeshSurface = FindObjectOfType<NavMeshSurface>();
                if (navMeshSurface == null)
                {
                    Debug.LogError("NavMeshSurface component is not found in the scene!");
                    return;
                }
            }

            // Find the City parent
            GameObject cityParent = GameObject.Find(cityParentName);
            if (cityParent == null)
            {
                Debug.LogError($"City parent object with name '{cityParentName}' not found.");
                return;
            }

            Debug.Log("City parent found. Clearing and rebaking NavMesh...");

            // Calculate bounds based on the City
            Bounds cityBounds = CalculateBounds(cityParent);
            navMeshSurface.center = cityBounds.center;
            navMeshSurface.size = cityBounds.size;

            // Clear existing NavMesh and bake
            navMeshSurface.RemoveData();
            navMeshSurface.BuildNavMesh();

            Debug.Log("NavMesh successfully rebaked.");
        }

        private Bounds CalculateBounds(GameObject parent)
        {
            Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                return new Bounds(parent.transform.position, Vector3.zero);
            }

            Bounds bounds = renderers[0].bounds;

            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }
    }
}
