using System.Collections.Generic;
using UnityEngine;

namespace ArtNotes.SimpleCityGenerator
{
    [ExecuteInEditMode]
    public class SimpleCityGenerator : MonoBehaviour
    {
        public enum ActivationMode { LevelStart, Event, Trigger }

        [Header("Activation Settings")]
        public ActivationMode activationMode = ActivationMode.LevelStart;
        public UnityEngine.Events.UnityEvent OnGenerateEvent; // Event to trigger generation
        public Collider triggerCollider; // Collider for trigger activation

        [Header("City Generation Settings")]
        public int CityZoneCount = 200, BigSectorCount = 500, SqareLength = 20;
        public Vector3Int CityCentre;
        public CitySample[] List1x1, List1x2, ListAngle, ListSqare;

        public bool DeletePreviousCity = true;
        public bool useSelected2x2 = true; // Toggle to enable or disable using the selected 2x2 prefab
        public bool useBossLevel = true;

        private int MapLength, CurrentBigSectorCount;
        private int[,] IntMap;
        private List<Vector2Int> Vacant;

        private void Start()
        {
            if (activationMode == ActivationMode.LevelStart)
            {
                Generate();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (activationMode == ActivationMode.Trigger && triggerCollider != null && other == triggerCollider)
            {
                Generate();
            }
        }

        public void Generate()
{
    try
    {
        if (DeletePreviousCity)
        {
            GameObject existingCity = GameObject.Find("City");
            if (existingCity != null)
            {
                DestroyImmediate(existingCity);
                Debug.Log("Previous city destroyed.");
            }
        }

        Vacant = new List<Vector2Int>();
        Debug.Log("Generating in game object : " + gameObject.name);

        MapLength = Mathf.FloorToInt(Mathf.Sqrt(CityZoneCount * 4));
        IntMap = new int[MapLength, MapLength];
        IntMap[MapLength / 2, MapLength / 2] = 1;

        Debug.Log("Length of square map : " + MapLength + ", city centre : " + MapLength / 2 + " : " + MapLength / 2);

        CurrentBigSectorCount = BigSectorCount;
        CheckNeighbour(MapLength / 2, MapLength / 2);

        CalculateCityZone();
        CalculateBigSectors();
        BuildCity();

        CityStartEndManager cityManager = GetComponent<CityStartEndManager>();
        if (cityManager != null)
        {
            cityManager.PlaceStartAndEnd();
            if (useSelected2x2)
            {
                cityManager.PlaceSelected2x2();
        
            }
               if (useBossLevel)
            {
                cityManager.PlaceBossLevel();
            }
            
        }
        else
        {
            Debug.LogError("CityStartEndManager not found on the GameObject!");
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Error during city generation: {ex.Message}. Stopping generation.");
    }
}


        private void CalculateCityZone()
        {
            for (int i = 0; i < CityZoneCount; i++)
            {
                Vector2Int Pos = Vacant[Random.Range(0, Vacant.Count - 1)];
                Vacant.Remove(Pos);

                IntMap[Pos.x, Pos.y] = 1;
                CheckNeighbour(Pos.x, Pos.y);
            }
            Debug.Log("City territory calculated");
        }

        private void CalculateBigSectors()
        {
            for (int x = 0; x < IntMap.GetLength(0); x++)
            {
                for (int y = 0; y < IntMap.GetLength(1); y++)
                {
                    if (IntMap[x, y] == 1) Vacant.Add(new Vector2Int(x, y));
                }
            }

            int limit = CityZoneCount;
            while (CurrentBigSectorCount > 0 && limit > 0)
            {
                int i = Random.Range(0, 3);
                switch (i)
                {
                    case (0): InsertLong1x2(); break;
                    case (1): InsertAngle(); break;
                    case (2): InsertSqare(); break;
                }
                limit--;
                if (limit < 1) Debug.Log("Big Sectors Limit achieved");
            }
            Debug.Log("Big sectors calculated");
        }

        private void InsertLong1x2()
        {
            int k = Random.Range(0, Vacant.Count - 1);
            Vector2Int Pos = Vacant[k];
            bool rotate = Random.Range(0, 2) == 1; // 90 degrees if true
            if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x + 1, Pos.y] == 1 && !rotate)
            {
                IntMap[Pos.x, Pos.y] = 21;
                IntMap[Pos.x + 1, Pos.y] = 9;
                Vacant.Remove(Pos);
                Vacant.Remove(new Vector2Int(Pos.x + 1, Pos.y));
                CurrentBigSectorCount--;
            }
            else if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x, Pos.y + 1] == 1 && rotate)
            {
                IntMap[Pos.x, Pos.y] = 22;
                IntMap[Pos.x, Pos.y + 1] = 0;
                Vacant.Remove(Pos);
                Vacant.Remove(new Vector2Int(Pos.x, Pos.y + 1));
                CurrentBigSectorCount--;
            }
        }

        private void InsertAngle()
        {
            int k = Random.Range(0, Vacant.Count - 1);
            Vector2Int Pos = Vacant[k];
            bool rotate = Random.Range(0, 2) == 1; // -90 degrees if true
            if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x + 1, Pos.y] == 1 && IntMap[Pos.x, Pos.y + 1] == 1 && !rotate)
            {
                IntMap[Pos.x, Pos.y] = 31;
                IntMap[Pos.x + 1, Pos.y] = 0;
                IntMap[Pos.x, Pos.y + 1] = 0;
                Vacant.Remove(Pos);
                Vacant.Remove(new Vector2Int(Pos.x + 1, Pos.y));
                Vacant.Remove(new Vector2Int(Pos.x, Pos.y + 1));
                CurrentBigSectorCount--;
            }
            else if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x, Pos.y + 1] == 1 && IntMap[Pos.x - 1, Pos.y] == 1 && rotate)
            {
                IntMap[Pos.x, Pos.y] = 32;
                IntMap[Pos.x, Pos.y + 1] = 0;
                IntMap[Pos.x - 1, Pos.y] = 0;
                Vacant.Remove(Pos);
                Vacant.Remove(new Vector2Int(Pos.x, Pos.y + 1));
                Vacant.Remove(new Vector2Int(Pos.x - 1, Pos.y));
                CurrentBigSectorCount--;
            }
        }

        private void InsertSqare()
        {
            int k = Random.Range(0, Vacant.Count - 1);
            Vector2Int Pos = Vacant[k];
            bool rotate = Random.Range(0, 2) == 1; // -90 degrees if true
            if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x + 1, Pos.y] == 1 && IntMap[Pos.x, Pos.y + 1] == 1 && IntMap[Pos.x + 1, Pos.y + 1] == 1 && !rotate)
            {
                IntMap[Pos.x, Pos.y] = 41;
                IntMap[Pos.x + 1, Pos.y] = 0;
                IntMap[Pos.x, Pos.y + 1] = 0;
                IntMap[Pos.x + 1, Pos.y + 1] = 0;
                Vacant.Remove(Pos);
                Vacant.Remove(new Vector2Int(Pos.x + 1, Pos.y));
                Vacant.Remove(new Vector2Int(Pos.x, Pos.y + 1));
                Vacant.Remove(new Vector2Int(Pos.x + 1, Pos.y + 1));
                CurrentBigSectorCount--;
            }
            else if (IntMap[Pos.x, Pos.y] == 1 && IntMap[Pos.x - 1, Pos.y] == 1 && IntMap[Pos.x, Pos.y - 1] == 1 && IntMap[Pos.x - 1, Pos.y - 1] == 1 && rotate)
            {
                IntMap[Pos.x, Pos.y] = 42;
                IntMap[Pos.x - 1, Pos.y] = 0;
                IntMap[Pos.x, Pos.y - 1] = 0;
                IntMap[Pos.x - 1, Pos.y - 1] = 0;
                Vacant.Remove(Pos);
                Vacant.Remove(new Vector2Int(Pos.x, Pos.y - 1));
                Vacant.Remove(new Vector2Int(Pos.x - 1, Pos.y));
                Vacant.Remove(new Vector2Int(Pos.x - 1, Pos.y - 1));
                CurrentBigSectorCount--;
            }
        }

        private void BuildCity()
        {
            var cityObject = new GameObject("City");
            var city = cityObject.transform;
            city.position = CityCentre;

            for (int x = 1; x < IntMap.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < IntMap.GetLength(1) - 1; y++)
                {
                    if (List1x1.Length == 0 && IntMap[x, y] == 1)
                    {
                        Debug.LogWarning("Skipping 1x1 prefab instantiation because the list is empty.");
                        continue;
                    }
                    int selected;
                    Vector3 position = new Vector3Int(x - MapLength / 2, 0, y - MapLength / 2) * SqareLength + CityCentre;

                    switch (IntMap[x, y])
                    {
                        case 1:
                            selected = SelectPrefab(List1x1);
                            Instantiate(List1x1[selected].gameObject, position, Quaternion.identity, city);
                            break;
                        case 21:
                            selected = SelectPrefab(List1x2);
                            Instantiate(List1x2[selected].gameObject, position, Quaternion.identity, city);
                            break;
                        case 22:
                            selected = SelectPrefab(List1x2);
                            Instantiate(List1x2[selected].gameObject, position, Quaternion.Euler(0, -90, 0), city);
                            break;
                        case 31:
                            selected = SelectPrefab(ListAngle);
                            Instantiate(ListAngle[selected].gameObject, position, Quaternion.identity, city);
                            break;
                        case 32:
                            selected = SelectPrefab(ListAngle);
                            Instantiate(ListAngle[selected].gameObject, position, Quaternion.Euler(0, -90, 0), city);
                            break;
                        case 41:
                            selected = SelectPrefab(ListSqare);
                            Instantiate(ListSqare[selected].gameObject, position, Quaternion.identity, city);
                            break;
                        case 42:
                            selected = SelectPrefab(ListSqare);
                            Instantiate(ListSqare[selected].gameObject, position, Quaternion.Euler(0, 180, 0), city);
                            break;
                    }
                }
            }
            Debug.Log("City was built");
        }

        private void CheckNeighbour(int x, int y)
        {
            if (y + 1 < MapLength && IntMap[x, y + 1] == 0)
                Vacant.Add(new Vector2Int(x, y + 1));
            else if (y + 1 < MapLength)
                Vacant.Remove(new Vector2Int(x, y + 1));

            if (y - 1 >= 0 && IntMap[x, y - 1] == 0)
                Vacant.Add(new Vector2Int(x, y - 1));
            else if (y - 1 >= 0)
                Vacant.Remove(new Vector2Int(x, y - 1));

            if (x + 1 < MapLength && IntMap[x + 1, y] == 0)
                Vacant.Add(new Vector2Int(x + 1, y));
            else if (x + 1 < MapLength)
                Vacant.Remove(new Vector2Int(x + 1, y));

            if (x - 1 >= 0 && IntMap[x - 1, y] == 0)
                Vacant.Add(new Vector2Int(x - 1, y));
            else if (x - 1 >= 0)
                Vacant.Remove(new Vector2Int(x - 1, y));
        }

        private int SelectPrefab(CitySample[] List)
        {
            if (List == null || List.Length == 0)
            {
                Debug.LogError("Prefab list is null or empty!");
                return 0; // Default to the first prefab or safe fallback
            }

            int VeritySumm = 0;
            for (int k = 0; k < List.Length; k++)
                VeritySumm += List[k].Verity;

            if (VeritySumm == 0)
            {
                Debug.LogError("All prefab Verity values are zero! Defaulting to the first prefab.");
                return 0; // Default to the first prefab or safe fallback
            }

            int CheckSumm = 0, i = 0;
            int IntRandom = Random.Range(1, VeritySumm);
            while (CheckSumm < IntRandom && i < List.Length)
            {
                CheckSumm += List[i].Verity;
                i++;
            }

            return Mathf.Clamp(i - 1, 0, List.Length - 1); // Ensure the index is valid
        }

        public void TriggerGeneration()
        {
            if (activationMode == ActivationMode.Event)
            {
                Generate();
            }
        }
    }
}
