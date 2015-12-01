//@
// Generate City map by using 2D pixel Texture.

// Updated 2015.11.9
// Byunghwan Lee
//-----------------------------------------------------

using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]


//@
//Basic Tile == city basic unit
[Serializable]
public class Tile { 

	[SerializeField] int c_size;
	[SerializeField] int c_property;  /* 0 = default , 1 = road */
	[SerializeField] public float c_posX;
	[SerializeField] public float c_posZ;
	[SerializeField] bool c_bIsObstacle;
	
	public Tile(int _prop, float _x, float _z){
		c_property = _prop;
		c_posX = _x;
		c_posZ = _z;
		c_bIsObstacle = false;
		c_size = 1;
	}
	public bool GetIsObstacle(){
		return c_bIsObstacle;
	}
	public void setIsObstacle(bool _tof){
		c_bIsObstacle = _tof;
	}
	public Vector3 GetPos(){
		return new Vector3 (c_posX, 0, c_posZ);
	}

	public int GetSize(){
		return c_size;
	}
	public void SetSize(int _size){
		c_size = _size;
	}
	public void SetProperty(int _prop){
		c_property = _prop;  
	}
	public int GetProperty(){
		return c_property;  
	}
	
}

//@
//Group of Tiles...
[Serializable]
public class TileMap : MonoBehaviour { 
	
	public int size_x = 100;
	public int size_z = 50;
	public float tileSize = 1.0f;

	public bool bIsCompleted = false;

	public Texture2D UnitTileTx;
	public int tileResolution;
	public Texture2D bluePrintTx;

	public Material[] buildingOuterMtrl;
	public Material[] buildingWindowMtrl;

	[SerializeField] public Tile[] c_Tiles;

	// Use this for initialization
	void Start () {
		/*
		DestoyBuildings ();
		BuildMesh();
		BuildTexture();
		BuildCity ();
		*/
	}	

	//Basic Set selected tile info
	public int[] GetTileSet(Vector3 _posVector){
		int x = Mathf.FloorToInt( _posVector.x / tileSize);
		int z = Mathf.FloorToInt( _posVector.z / tileSize);

		int[] posArr = new int[] {x, z};
		return posArr;
	}
	//Basic Get selected tile info
	public Vector3 GetTilePos(int _currX, int _currZ){
		float currPosX = _currX * tileSize + tileSize/2;
		float currPosZ = _currZ * tileSize + tileSize/2;
		Vector3 currPosVec = new Vector3 (currPosX, 0, currPosZ);
		return currPosVec;
	}

	public bool GetCityBuildStatus(){
		return bIsCompleted;
	}


	public Tile[] GetTileArray(){
		return c_Tiles;
	}
	public void GetTileArray_ref(ref Tile[] _tileArray){
		_tileArray =  c_Tiles;
	}

	//Construct building in here
	public void BuildCity(){

		if (bluePrintTx == null)
			return;

		c_Tiles = new Tile[size_x * size_z];
		//c_Tiles1 = new Tile[6];
		
		for (int x =0; x < size_x; ++x) {
			for(int z = 0; z <size_z; ++z){
				//속성 넣어주기
				//Map Generator 생성하기;;			
				c_Tiles[x + z * size_z] = new Tile(0, this.transform.position.x + x * tileSize, this.transform.position.z + z * tileSize);
				
				Color pickColor = bluePrintTx.GetPixel(x,z);
				//For blueprint Texture genearating, Using no-compressed file format(DDS)
				// and RGBA 32 bit on photoshop

				Debug.Log("Idx: (" + x + ", " + z + ") ::" + bluePrintTx.GetPixel(x,z));

				string buildingName = "null";
				Quaternion buldingRot = Quaternion.AngleAxis(90, Vector3.left);
				
				// Road Checking
				if(pickColor ==Color.white){
					AssingRoadBlock(x,z, ref buildingName, ref buldingRot);
					c_Tiles[x + z * size_z].SetProperty(1);

				}else{

					int randNum_type = UnityEngine.Random.Range(0,3);

					if(pickColor == Color.red){ //장애물
						if(randNum_type % 2 == 0){ buildingName= "ModelAsset/prefab/B_Antena"; }
						else{buildingName= "ModelAsset/prefab/B_Multiple"; }

					}else if(pickColor == Color.blue){
						if(randNum_type % 2 == 0){ buildingName= "ModelAsset/prefab/B_Basic_A"; }
						else{buildingName= "ModelAsset/prefab/B_Basic_B"; }

					}else if(pickColor == Color.green){
						if(randNum_type % 2 == 0){ buildingName= "ModelAsset/prefab/B_GridBank"; }
						else{buildingName= "ModelAsset/prefab/B_Library"; }

					}else if(pickColor ==Color.gray){
						if(randNum_type % 2 == 0){ buildingName= "ModelAsset/prefab/B_GridOffice"; }
						else{buildingName= "ModelAsset/prefab/B_GridBank"; }

					}else if(pickColor ==Color.black){

						if(randNum_type == 0){ buildingName= "ModelAsset/prefab/B_Cluster_A"; }
						else if(randNum_type == 1){ buildingName= "ModelAsset/prefab/B_Cluster_B";}
						else{ buildingName = "ModelAsset/prefab/B_Cluster_C";}
					}

					int randomNum_rot = UnityEngine.Random.Range(0,3);
					buldingRot *= Quaternion.Euler(0, 0, 90 * randomNum_rot); // this add a 90 degrees Z rotation

					//Basic Ground
					GameObject InstantPrefab_null =  (GameObject)Instantiate(Resources.Load("ModelAsset/prefab/R_Null"), c_Tiles[x + z * size_z].GetPos(), buldingRot );
					InstantPrefab_null.transform.parent = this.transform;

				}


				if(buildingName != "null"){
					GameObject InstantPrefab = (GameObject)Instantiate(Resources.Load(buildingName), c_Tiles[x + z * size_z].GetPos(), buldingRot );
					//GameObject InstantPrefab = (GameObject)Instantiate(Resources.Load("ModelAsset/prefab/B_Antena"), transform.position + c_Tiles[x + z * size_z].GetPos(), buldingRot );
					InstantPrefab.transform.parent = this.transform;
					float randomNum_scale = UnityEngine.Random.Range(1.0f,1.5f);
					Vector3 InstantPrefab_Scale = InstantPrefab.transform.localScale;
					InstantPrefab.transform.localScale  = new Vector3(InstantPrefab_Scale.x, InstantPrefab_Scale.y , InstantPrefab_Scale.z *randomNum_scale );

					if(InstantPrefab.tag == "building"){
						Building buildScript = InstantPrefab.GetComponent<Building>();

						int randColor = UnityEngine.Random.Range(0, (buildingOuterMtrl.Length -1 ));
						int windowColor = UnityEngine.Random.Range(0,(buildingWindowMtrl.Length -1 ));
						buildScript.Initialzie(buildingName, buildingOuterMtrl[randColor], buildingWindowMtrl[windowColor]);
					}
				}
					
				
			}
		}

		bIsCompleted = true;
	}

	void AssingRoadBlock(int _x, int _z , ref string _buildingName,  ref Quaternion _buildingRot){
		//Road Curve,Intersection, Straight Decision..
		// @@ needed function seperation...~~
		//다양한 타입의 블럭을 불러오기 위함이다
		//Road Checking--------------------------------
		// Up, Left, Right, Down
		bool bIsRoad_Up, bIsRoad_Down, bIsRoad_Left, bIsRoad_Right;
		bIsRoad_Up = bIsRoad_Down = bIsRoad_Left = bIsRoad_Right = false;
		int negiborRoadCnt = 0;
		
		// 1. Checking Neighbor road count for what type of road needed
		//up check
		if( (_z + 1 ) >= 0 && (_z + 1) < size_z ){ 
			if(bluePrintTx.GetPixel(_x, _z+1) == Color.white ){
				bIsRoad_Up = true;
				negiborRoadCnt++;
			}
		} 
		//down check
		if( (_z - 1 ) >= 0 && (_z - 1) < size_z ){ 
			if(bluePrintTx.GetPixel(_x, _z-1) == Color.white ){
				bIsRoad_Down  = true;
				negiborRoadCnt++;
			}
		} 
		// left check
		if( (_x - 1 ) >= 0 && (_x - 1) < size_x ){  
			if(bluePrintTx.GetPixel(_x-1, _z) == Color.white ){
				bIsRoad_Left  = true;
				negiborRoadCnt++;
			}
		} 
		//right check
		if( (_x + 1 ) >= 0 && (_x + 1) < size_x ){ 
			if(bluePrintTx.GetPixel(_x+1 ,_z) == Color.white ){
				bIsRoad_Right = true;
				negiborRoadCnt++;
			}
		}
		
		// 2. For rotating to proper angle .. O = neighbor road / T = current road;
		//Debug.Log ("RoadType>>Idx: (" + _x + ", " + z + ") ::" + negiborRoadCnt + "/ check: Up-" +  bIsRoad_Up + ", Down-" + bIsRoad_Down +  ", left-" + bIsRoad_Left + ", right-" + bIsRoad_Right);
		switch(negiborRoadCnt){
			case 1:{
				if(bIsRoad_Up  || bIsRoad_Down  ){
					//   O 
					//   T
					//   O
					_buildingRot *= Quaternion.Euler(0, 0, 90); // this add a 90 degrees Z rotation
					_buildingName= "ModelAsset/prefab/R_Strait";
					
				} else if( bIsRoad_Left || bIsRoad_Right){
					//   O T O
					_buildingName= "ModelAsset/prefab/R_Strait";
				} 

			}break;
		
			case 2:{
				//Curve 
				if(!bIsRoad_Up && bIsRoad_Down && bIsRoad_Left && !bIsRoad_Right){
					// O T
					//   O
					_buildingRot *= Quaternion.Euler(0, 0, -90); // this add a 90 degrees Z rotation
					_buildingName= "ModelAsset/prefab/R_Curve";
				} else if(!bIsRoad_Up && bIsRoad_Down && !bIsRoad_Left && bIsRoad_Right){
					// T O
					// O  
					_buildingRot *= Quaternion.Euler(0, 0, 180); // this add a 90 degrees Z rotation
					_buildingName= "ModelAsset/prefab/R_Curve";
				} else if(bIsRoad_Up && !bIsRoad_Down && !bIsRoad_Left && bIsRoad_Right){
					// O 
					// T O
					_buildingRot *= Quaternion.Euler(0, 0, 90); // this add a 90 degrees Z rotation 
					_buildingName= "ModelAsset/prefab/R_Curve";
				} else if(bIsRoad_Up && !bIsRoad_Down && bIsRoad_Left && !bIsRoad_Right){
					//   O 
					// O T 
					_buildingName= "ModelAsset/prefab/R_Curve";
				} 
				//Straight----------
				else if(bIsRoad_Up && bIsRoad_Down && !bIsRoad_Left && !bIsRoad_Right){
					//   O 
					//   T
					//   O
					_buildingRot *= Quaternion.Euler(0, 0, 90); // this add a 90 degrees Z rotation
					_buildingName= "ModelAsset/prefab/R_Strait";
			
				} else if(!bIsRoad_Up && !bIsRoad_Down && bIsRoad_Left && bIsRoad_Right){
					//   O T O
					_buildingName= "ModelAsset/prefab/R_Strait";
				} 
				
			}break;
				
			case 3: {
				if(bIsRoad_Up && bIsRoad_Down && !bIsRoad_Left && bIsRoad_Right){
					// O
					// T O
					// O
					_buildingRot *= Quaternion.Euler(0, 0, 90); // this add a 90 degrees Z rotation 
					
				} else if(!bIsRoad_Up && bIsRoad_Down && bIsRoad_Left && bIsRoad_Right){
					// O T O
					//   O
					_buildingRot *= Quaternion.Euler(0, 0, 180); // this add a 90 degrees Z rotation 
					
				} else if(bIsRoad_Up && bIsRoad_Down && bIsRoad_Left && !bIsRoad_Right){
					//   O
					// O T 
					//   O
					_buildingRot *= Quaternion.Euler(0, 0, -90); // this add a 90 degrees Z rotation 
					
				} else if(bIsRoad_Up && !bIsRoad_Down && bIsRoad_Left && bIsRoad_Right){
					//   O
					// O T O  
				}
				_buildingName= "ModelAsset/prefab/R_Triangle";
			}break;
				
			case 4: {
				_buildingName= "ModelAsset/prefab/R_Intersection";
			}break;
			
		}
	}

	Color[][] ChopUpTiles() {
		int numTilesPerRow = UnitTileTx.width / tileResolution;
		int numRows = UnitTileTx.height / tileResolution;
		
		Color[][] tiles = new Color[numTilesPerRow*numRows][];
		
		for(int y=0; y<numRows; y++) {
			for(int x=0; x<numTilesPerRow; x++) {
				tiles[y*numTilesPerRow + x]  = UnitTileTx.GetPixels( x*tileResolution , y*tileResolution, tileResolution, tileResolution );
			}
		}
		return tiles;
	}
	
	public void BuildTexture() {

		int texWidth = size_x * tileResolution;
		int texHeight = size_z * tileResolution;
		Texture2D texture = new Texture2D(texWidth, texHeight);
		
		Color[][] tiles = ChopUpTiles();
		
		for(int y=0; y < size_z; y++) {
			for(int x=0; x < size_x; x++) {
				Color[] p = tiles[0];
				texture.SetPixels(x*tileResolution, y*tileResolution, tileResolution, tileResolution, p);
			}
		}
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();
		
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = texture;
	}
	
	public void BuildMesh() {
		int numTiles = size_x * size_z;
		int numTris = numTiles * 2;
		
		int vsize_x = size_x + 1;
		int vsize_z = size_z + 1;
		int numVerts = vsize_x * vsize_z;
		
		// Generate the mesh data
		Vector3[] vertices = new Vector3[ numVerts ];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[ numTris * 3 ];

		int x, z;
		for(z=0; z < vsize_z; z++) {
			for(x=0; x < vsize_x; x++) {
				vertices[ z * vsize_x + x ] = new Vector3( x*tileSize, 0, z*tileSize );
				normals[ z * vsize_x + x ] = Vector3.up;
				uv[ z * vsize_x + x ] = new Vector2( (float)x / vsize_x, (float)z / vsize_z );
			}
		}
		
		for(z=0; z < size_z; z++) {
			for(x=0; x < size_x; x++) {
				int squareIndex = z * size_x + x;
				int triOffset = squareIndex * 6;

				triangles[triOffset + 0] = z * vsize_x + x + 		   0;
				triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 0;
				triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 1;
				
				triangles[triOffset + 3] = z * vsize_x + x + 		   0;
				triangles[triOffset + 4] = z * vsize_x + x + vsize_x + 1;
				triangles[triOffset + 5] = z * vsize_x + x + 		   1;

				//가끔 뒤집여서 안볼일때 있음
				/* 
				triangles[triOffset + 0] = z * vsize_x + x + 		   0;
				triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 1;
				triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 0;
				
				triangles[triOffset + 3] = z * vsize_x + x + 		   0;
				triangles[triOffset + 4] = z * vsize_x + x + vsize_x + 1;
				triangles[triOffset + 5] = z * vsize_x + x + 		   1;
				*/
			}				
		}

		// Create a new Mesh and populate with the data ~~
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		
		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();
		
		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;

	}

	public void DestoyBuildings(){
		GameObject[] arr_object_building =  GameObject.FindGameObjectsWithTag("building");
		GameObject[] arr_object_road =  GameObject.FindGameObjectsWithTag("road");

		for (int i =0; i< arr_object_building.Length; ++i) {
			//In Edit mode, we need to use DestoyImmediate instead of Destory
			DestroyImmediate(arr_object_building[i]);
		}
		for (int i =0; i< arr_object_road.Length; ++i) {
			//In Edit mode, we need to use DestoyImmediate instead of Destory
			DestroyImmediate(arr_object_road[i]);
		}

	}
	
}
