using UnityEngine;
using System.Collections;

public class MonsterStateSteer : StateBase<MonsterAction>
{
    private Quaternion _target;
    private float _angularSpeed;

    public bool finish { get; private set; }

    public MonsterStateSteer(Quaternion target, float angularSpeed)
    {
        _target = target;
        _angularSpeed = angularSpeed;
    }

    public void Enter(MonsterAction monster)
    {

    }

    public void Execute(MonsterAction monster)
    {
        monster.transform.rotation = Quaternion.RotateTowards(monster.transform.rotation, _target, _angularSpeed * Time.deltaTime);
        finish = Quaternion.Angle(monster.transform.rotation, _target) < 0.1f;
        if(finish)
            monster.transform.rotation = _target;
    }


    public void Exit(MonsterAction monster)
    {

    }
}
