using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using URandom = UnityEngine.Random;
using PowerUpType = ParachuteItem.PowerUpType;

public class ParachuteManager : MonoBehaviour
{
    public List<ParachuteItem> items;

    private Vector2 leftBottomBound
    { get { return new Vector2(GameController.Instance.cityLowerBound.x, GameController.Instance.cityLowerBound.z); } }
    private Vector2 rightTopBound
    { get { return new Vector2(GameController.Instance.cityUpperBound.x, GameController.Instance.cityUpperBound.z); } }

    public float ceilingMinHeight;
    public float ceilingMaxHeight;

    public float spawnInterval;
    private float _spawnTimer;

    public float lifeSpan;

	public Transform robotTransform;

    private bool _init = false;


    // Update is called once per frame
    private void Update()
    {
        if (!_init)
            return;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer > spawnInterval)
        {
            _spawnTimer = 0f;
            Vector2 spawnPos2d = GameController.Instance.mapMgr.GetDropPoint_Random();
            //spawn
            ParachuteItem newItem = Instantiate(items[URandom.Range(0, items.Count)], new Vector3(spawnPos2d.x, URandom.Range(ceilingMinHeight, ceilingMaxHeight), spawnPos2d.y), Quaternion.identity)
                as ParachuteItem;
            newItem.lifeSpan = lifeSpan;
            newItem.StartCycling();
            //Array vals = Enum.GetValues(typeof(ParachuteItem.PowerUpType));
            //Instantiate(items[(ParachuteItem.PowerUpType)vals.GetValue(URandom.Range(0, vals.Length))]
            newItem.itemOpen += OnItemOpen;

        }
    }

    private bool SearchReleasePos(ref Vector2 result)
    {
        RaycastHit info;
        Ray ray = new Ray(new Vector3(URandom.Range(leftBottomBound.x, rightTopBound.x),
                                       ceilingMaxHeight,
                                       URandom.Range(leftBottomBound.y, rightTopBound.y)),
                                       Vector3.down);
        if (Physics.Raycast(ray, out info))
        {
            if (info.collider.gameObject.CompareTag("Ground"))
            {
                result = new Vector2(info.point.x, info.point.z);
                return true;
            }
        }

        return false;
    }

	private void OnItemOpen(PowerUpType type)
	{
		switch(type)
		{
			case PowerUpType.SuperBlast:
			{	PoseManager.Instance.CreateNextPose(PoseManager.PoseType.SuperBlast, robotTransform); break;}
			case PowerUpType.SpeedUp:
			{	PoseManager.Instance.CreateNextPose(PoseManager.PoseType.SpeedUp, robotTransform); break;}
            default: break;
		}
	}

    public void Init()
    {
        _spawnTimer = 0f;
        _init = true;
    }
}
