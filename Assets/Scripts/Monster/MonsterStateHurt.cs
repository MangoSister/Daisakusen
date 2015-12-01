using UnityEngine;
using System.Collections;

public class MonsterStateHurt : StateBase<MonsterAction>
{
    public float hurtInterval;
    private float _hurtTimer;
    private Color _oldColor;
    public bool finish { get; private set; }

    public MonsterStateHurt(float interval)
    {
        Debug.Assert(interval > 0f);
        hurtInterval = interval;
    }

    public void Enter(MonsterAction monster)
    {
        _hurtTimer = 0f;
        if (monster.GetComponent<MeshRenderer>() != null)
        {
            _oldColor = monster.GetComponent<MeshRenderer>().material.color;
            monster.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
    }

    public void Execute(MonsterAction monster)
    {
        _hurtTimer += Time.deltaTime;
        if (_hurtTimer >= hurtInterval)
            finish = true;
    }

    public void Exit(MonsterAction monster)
    {
        if (monster.GetComponent<MeshRenderer>() != null)
            monster.GetComponent<MeshRenderer>().material.color = _oldColor;
    }
}
