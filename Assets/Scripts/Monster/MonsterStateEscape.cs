using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterStateEscape : MonsterStateNavigation
{
    public float minRedirectInterval;
    public float maxRedirectInterval;
    public float safeRange;
    public GameObject predator;

    private float _redirectInterval;
    private float _redirectTimer;

    private Color _oldColor;

    public MonsterStateEscape(Vector2 lb, Vector2 rt, float speed, float angularSpeed, float minRdir, float maxRdir, GameObject predator, float range)
    {
        leftBottomBound = lb;
        rightTopBound = rt;
        this.speed = speed;
        steerAngularSpeed = angularSpeed;
        minRedirectInterval = minRdir;
        maxRedirectInterval = maxRdir;
        this.predator = predator;
        safeRange = range;
    }

    public override void Enter(MonsterAction monster)
    {
        base.Enter(monster);
        _redirectTimer = 0f;
        _redirectInterval = Random.Range(minRedirectInterval, maxRedirectInterval);
        if (monster.GetComponent<MeshRenderer>() != null)
        {
            _oldColor = monster.GetComponent<MeshRenderer>().material.color;
            monster.GetComponent<MeshRenderer>().material.color = Color.blue;
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
        if (Vector2.Distance(new Vector2(monster.transform.position.x, monster.transform.position.z),
            new Vector2(predator.transform.position.x, predator.transform.position.z)) > safeRange)
            return Vector3.zero;
        else
        {
            Vector3 dir = monster.transform.position - predator.transform.position;
            dir.y = 0f;
            dir = dir.normalized;
            Vector3 target = monster.transform.position + dir * safeRange;
            float[] outOfBoundt = new float[4] { 1f, 1f, 1f, 1f };
            if(target.x < leftBottomBound.x)
                outOfBoundt[0] = Mathf.InverseLerp(monster.transform.position.x, target.x, leftBottomBound.x);
            if (target.x > rightTopBound.x)
                outOfBoundt[1] = Mathf.InverseLerp(monster.transform.position.x, target.x, rightTopBound.x);
            if (target.z < leftBottomBound.y)
                outOfBoundt[2] = Mathf.InverseLerp(monster.transform.position.z, target.z, leftBottomBound.y);
            if (target.z > rightTopBound.y)
                outOfBoundt[3] = Mathf.InverseLerp(monster.transform.position.z, target.z, rightTopBound.y);
            float mint = 1f;
            foreach (float t in outOfBoundt)
            {
                if (t < mint)
                    mint = t;
            }
            target = Vector3.Lerp(monster.transform.position, target, mint);
            return target - monster.transform.position;
        }
    }

    protected override bool RedirectCondition
    {
        get
        {
            _redirectTimer += Time.deltaTime;
            if (_redirectTimer >= _redirectInterval)
            {
                _redirectTimer = 0f;
                _redirectInterval = Random.Range(minRedirectInterval, maxRedirectInterval);
                return true;
            }
            else return false;
        }
    }
}


[System.Serializable]
public class MonsterEscapeParam
{
    public float escapeSpeed;
    public float escapeAngularSpeed;
    public float minRDir;
    public float maxRDir;
    public float safeRange;
}