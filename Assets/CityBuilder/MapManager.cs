using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

	public TileMap TileMap;
	Tile[] c_Tiles;
	ArrayList c_RoadTiles;

	// Use this for initialization
	void Start () {
		if (TileMap != null && TileMap.bIsCompleted) {
			Initialize();
		} else {
			Debug.LogError("Error!! the city is not generated");
		}
	}

	void Initialize(){
		c_Tiles = TileMap.GetTileArray ();
		c_RoadTiles = new ArrayList ();

		int size_z = TileMap.size_z;
		for (int i = 0; i <TileMap.size_x; ++i) {
			for(int p = 0; p < size_z; ++p){
				//Distinguish Property
				// 1 -> road type
				if( c_Tiles[i + p * size_z ].GetProperty() == 1){
					c_RoadTiles.Add(c_Tiles[i + p *size_z]);
				}
			}
		}
	}
	
	public Vector2 GetDropPoint_Random(){
		int rand = Random.Range (0, c_RoadTiles.Count);
		return new Vector2 ( ((Tile)c_RoadTiles [rand]).c_posX, ((Tile)c_RoadTiles [rand]).c_posZ);
	}

	/*
	public Vector2 GetDropPoint_Adjacent(Transform _characterTransform, float _minDistance){
	
		Vector3 faceDir = (_characterTransform.forward).normalized;
		Vector3 pos = _characterTransform.position;
		int pos_idxX = pos.x / TileMap.tileSize;
		int pos_idxZ = pos.z / TileMap.tileSize;

		//Out of tile exception;
		if (pos_idxX < 0 && pos_idxX > TileMap.size_x 
			&& pos_idxZ < 0 && pos_idxZ > TileMap.size_z) {
			Debug.Log ("[Error] Out of Tile");
			return new Vector2(0,0);
		}

		bool bIsPlusX, bIsPlusZ;

		if (faceDir.x >= 0 && faceDir.z >= 0) { // 1사분면
			bIsPlusX = true;  bIsPlusZ = true;
		} else if (faceDir.x < 0 && faceDir.z >= 0) { // 2사분면
			bIsPlusX = false;  bIsPlusZ = true;
		} else if (faceDir.x < 0 && faceDir.z < 0) { // 3사분면
			bIsPlusX = false;  bIsPlusZ = false;
		} else if (faceDir.x >= 0 && faceDir.z < 0) { // 4사분면
			bIsPlusX = true;  bIsPlusZ = false;
		}

		//Searching Loop
		int searchDist = _minDistance;
		for (;;) {


			for(int i = C; i < searchDist; ){
				for(int p = searchDist; p < searchDist; ){


					if( c_Tiles[pos_idxX ,pos_idxZ ])
				}
			}
		
		}

	}
	*/
}
