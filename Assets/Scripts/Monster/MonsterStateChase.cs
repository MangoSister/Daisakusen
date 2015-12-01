using UnityEngine;
using System.Collections;

public class MonsterStateChase : MonsterStateNavigation
{
    public float minRedirectInterval;
    public float maxRedirectInterval;
    public GameObject target;

    private float _redirectInterval;
    private float _redirectTimer;

    private Color _oldColor;


    public MonsterStateChase(Vector2 lb, Vector2 rt, float speed, float angularSpeed, float minRdir, float maxRdir, GameObject target)
    {
        leftBottomBound = lb;
        rightTopBound = rt;
        this.speed = speed;
        steerAngularSpeed = angularSpeed;
        minRedirectInterval = minRdir;
        maxRedirectInterval = maxRdir;
        this.target = target;
    }

    public override void Enter(MonsterAction monster)
    {
        base.Enter(monster);
        _redirectTimer = 0f;
        _redirectInterval = Random.Range(minRedirectInterval, maxRedirectInterval);
        if (monster.GetComponent<MeshRenderer>() != null)
        {
            _oldColor = monster.GetComponent<MeshRenderer>().material.color;
            monster.GetComponent<MeshRenderer>().material.color = Color.magenta;
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
        Vector3 targetPos = target.transform.position;
        targetPos.x = Mathf.Min(Mathf.Max(targetPos.x, leftBottomBound.x), rightTopBound.x);
        targetPos.z = Mathf.Min(Mathf.Max(targetPos.z, leftBottomBound.y), rightTopBound.y);
        return new Vector3(targetPos.x - monster.transform.position.x, 0f,
                            targetPos.z - monster.transform.position.z);
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
