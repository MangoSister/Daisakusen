using UnityEngine;
using System.Collections;

public class MonsterStatusCollector : MonoBehaviour
{
    public bool GetDataFromGameCtrl = false;
    private MonsterAction monsterAction { get { return GetComponent<MonsterAction>(); } }
    private static Vector2 leftBottomBound
    { get { return new Vector2(GameController.Instance.cityLowerBound.x, GameController.Instance.cityLowerBound.z); } }
    private static Vector2 rightTopBound
    { get { return new Vector2(GameController.Instance.cityUpperBound.x, GameController.Instance.cityUpperBound.z); } }

    private void Start()
    {
        if (GetDataFromGameCtrl)
        {
            monsterAction.leftBottomBound = leftBottomBound;
            monsterAction.rightTopBound = rightTopBound;
        }
    }

    // Update is called once per frame
    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            monsterAction.DealDamage(10f);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            monsterAction.RestoreHealth(10f);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            monsterAction.targetActive = true;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            monsterAction.targetActive = false;
        }
    }
}
