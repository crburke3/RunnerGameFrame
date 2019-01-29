using UnityEngine;
using UnityEditor;
using System.Collections;

// Custom editor panel for the scenery layer settings.
// This allows us to hide the additional parameters for generating a set number of blocks from the layer 
// unless they are needed.

[CustomEditor(typeof(SceneryLayer))]
public class SceneryLayerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		SceneryLayer layer = (SceneryLayer)target;
		layer.parallaxSpeed = EditorGUILayout.FloatField(new GUIContent("Parallax Speed", "Relative speed that this layer scrolls at"), layer.parallaxSpeed);
		layer.isInfinite = EditorGUILayout.Toggle(new GUIContent("Is Infinite", "Whether to continue generating blocks from this layer forever"), layer.isInfinite);

		// only show maxBlockCount and nextLayer parameters if the layer is not set to generate indefinitely.
		if(!layer.isInfinite)
		{
			layer.maxBlockCount = EditorGUILayout.IntField(new GUIContent("Max Block Count", "maxmimum number of blocks that will be generated before moving to next layer"), layer.maxBlockCount);
			layer.nextLayer = (SceneryLayer)EditorGUILayout.ObjectField(new GUIContent("Next Layer", "Layer to activate after this layer has generated its maximum count"), layer.nextLayer, typeof(SceneryLayer), true);
		}
	}
}
