using UnityEngine;
using System.Collections;

public class GameController : GenericSingleton<GameController> {

	public float attackPower = 10.0f;
	public float attackMultiplier = 1.0f;

	public TileMap cityMap;
	public MapManager mapMgr;

	public Material lightningMat;
	public Material windMat;

	private Vector3 upBound;
	private Vector3 lowBound;

	public Transform robotTransform;
	public MonsterAction ma;

	public ParticleSystem hitFeedback;
	
	public bool gameStarting = false;
	public GameFlowManager gfm;
	public ParachuteManager pcm;

	public Vector3 cityLowerBound {
		get {
			return lowBound;
		}
	}

	public Vector3 cityUpperBound {
		get {
			return upBound;
		}
	}
	
	// Use this for initialization
	void Start () {
		InputProxy.Instance.isEnable = false;
		lowBound = cityMap.transform.position;
		upBound = cityMap.transform.position + new Vector3 (cityMap.size_x, 0, cityMap.size_z) * cityMap.tileSize;
	}

	public void GameEnding(bool good) {
		InputProxy.Instance.isEnable = false;
		gfm.ActivateEnd (good);
	}

	public void GameStart() {
		ma.Init ();
		pcm.Init ();
		InputProxy.Instance.isEnable = true;
		gameStarting = true;
		robotTransform.gameObject.GetComponent<RobotCtrl> ().Begin ();
		SoundManager.Instance.bgmPlay (BGMStage.FIGHT_STAGE_1);
	}

}
