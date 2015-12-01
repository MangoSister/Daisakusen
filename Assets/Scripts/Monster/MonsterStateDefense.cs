using UnityEngine;
using System.Collections;

public class MonsterStateDefense : StateBase<MonsterAction>
{
    public float defenseInterval;
    private float _defenseTimer;
    private Color _oldColor;
    public bool finish { get; private set; }

    public MonsterStateDefense(float interval)
    {
        Debug.Assert(interval > 0f);
        defenseInterval = interval;
    }


    public void Enter(MonsterAction monster)
    {
        _defenseTimer = 0f;
        if (monster.GetComponent<MeshRenderer>() != null)
        {
            _oldColor = monster.GetComponent<MeshRenderer>().material.color;
            monster.GetComponent<MeshRenderer>().material.color = Color.green;
        }

        monster.StartTornado(defenseInterval);
    }

    public void Execute(MonsterAction monster)
    {
        _defenseTimer += Time.deltaTime;
        if (_defenseTimer >= defenseInterval)
            finish = true;
    }

    public void Exit(MonsterAction monster)
    {
        if (monster.GetComponent<MeshRenderer>() != null)
            monster.GetComponent<MeshRenderer>().material.color = _oldColor;
    }
}
