using UnityEngine;

public class CityPrefabConnector : MonoBehaviour
{
    public enum ConnectionSide
    {
        None = 0,
        North = 1,
        East = 2,
        South = 3,
        West = 4,
        All = 5 // Added "All" option
    }

    [System.Serializable]
    public struct Connection
    {
        public ConnectionSide Side;
        public bool IsEntry;
        public bool IsExit;
    }

    public Connection[] Connections;

    /// <summary>
    /// Checks if the connection on this prefab matches with a neighboring prefab.
    /// </summary>
    public bool IsMatchingConnection(ConnectionSide side, bool isEntry)
    {
        foreach (var connection in Connections)
        {
            if (connection.Side == ConnectionSide.All || connection.Side == side)
            {
                if ((isEntry && connection.IsExit) || (!isEntry && connection.IsEntry))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
