
using UnityEditor;
using UnityEngine;

namespace ArtNotes.SimpleCityGenerator
{
    [CustomEditor(typeof(SimpleCityGenerator))]
    public class CustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SimpleCityGenerator gen = (SimpleCityGenerator)target;
            if (GUILayout.Button("Generate City")) gen.Generate();
        }
    }
}