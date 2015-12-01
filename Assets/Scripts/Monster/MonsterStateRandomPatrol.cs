using UnityEngine;
using System.Collections;

public class MonsterStateRandomPatrol : MonsterStateNavigation
{
    public float minDistEachMove;
    public float maxDistEachMove;

    private Color _oldColor;

    public MonsterStateRandomPatrol(Vector2 lb, Vector2 rt, float speed, float angularSpeed,
        float minMove, float maxMove)
    {
        leftBottomBound = lb;
        rightTopBound = rt;
        this.speed = speed;
        steerAngularSpeed = angularSpeed;
        minDistEachMove = minMove;
        maxDistEachMove = maxMove;
    }

    public override void Enter(MonsterAction monster)
    {
        base.Enter(monster);
        if (monster.GetComponent<MeshRenderer>() != null)
        {
            _oldColor = monster.GetComponent<MeshRenderer>().material.color;
            monster.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
    }


    public override void Exit(MonsterAction monster)
    {
        base.Exit(monster);
        if (monster.GetComponent<MeshRenderer>() != null)
            monster.GetComponent<MeshRenderer>().material.color = _oldColor;
    }

    protected override Vector3 GenNextOffset(MonsterAction monster, bool init)
    {
        Vector3 currPos = monster.transform.position;
        //absolutely need some improvement...
        Vector2 offset;
        Vector2 target;
        do
        {
            float dist = Random.Range(minDistEachMove, maxDistEachMove);
            float angle = Random.Range(0, Mathf.PI * 2f);
            offset = (new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))) * dist;
            target = new Vector2(currPos.x, currPos.z) + offset;
        }
        while (target.x < leftBottomBound.x || target.x > rightTopBound.x ||
                target.y < leftBottomBound.y || target.y > rightTopBound.y);
        return new Vector3(offset.x, 0f, offset.y);
    }
}