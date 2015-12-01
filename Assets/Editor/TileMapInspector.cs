using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileMap))]
public class TileMapInspector : Editor {
	
	public override void OnInspectorGUI() {
		//base.OnInspectorGUI();
		DrawDefaultInspector();
		
		if(GUILayout.Button("Regenerate")) {
			TileMap tileMap = (TileMap)target;

			//Erasing function;;

			tileMap.DestoyBuildings();
			tileMap.BuildMesh();
			tileMap.BuildTexture();
			tileMap.BuildCity ();
		}

		if(GUILayout.Button("DeleteAll")) {
			TileMap tileMap = (TileMap)target;
			tileMap.DestoyBuildings();
		}
	}
}
