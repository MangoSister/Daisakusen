using UnityEngine;
using System;
using System.Collections;

public sealed class MonsterStateRageStage : StateBase<MonsterAction>
{
    private MonsterRageParam _param;

    private StateEngine<MonsterAction> _rageStateEngine;

    public MonsterStateRageStage(MonsterRageParam param)
    {
        _param = param;
    }

    public bool finish { get { return false; } }

    public void Enter(MonsterAction monster)
    {
        _rageStateEngine = new StateEngine<MonsterAction>();
        _rageStateEngine.debug = true;

        _rageStateEngine.Init(monster,
            new MonsterStateFurySprint
            (_param.dashMaxAcc, 
            _param.dashMaxSpeed, _param.overDashFactor,
            _param.dashTruncateSpeed, _param.dashRestTimer, 
            _param.steerAngularSpeed));
    }

    public void Execute(MonsterAction monster)
    {
        //state execute
        _rageStateEngine.Execute();

        //state transition
        StateBase<MonsterAction> curr = _rageStateEngine.currState;
        //first check stun
        bool stun = monster.hitCount > _param.criticalHitNum && !(curr is MonsterStateStun);
        if (stun)
        {
            monster.hitCount = 0;
            _rageStateEngine.ChangeState(new MonsterStateStun(_param.stunTime));
            return;
        }

        float targetDist = Vector2.Distance(new Vector2(monster.transform.position.x, monster.transform.position.z),
                                new Vector2(monster.target.transform.position.x, monster.target.transform.position.z));
        bool near = targetDist <= _param.nearRange;

        if (curr is MonsterStateFurySprint && curr.finish)
        {
            if (near)
                _rageStateEngine.ChangeState(new MonsterStateFight(_param.minFightCd, _param.maxFightCd, 
                    _param.attackTime, _param.defenseTime, _param.fightRatio, _param.steerAngularSpeed));
        }

        else if (curr is MonsterStateFight)
        {
            if (!near)
                _rageStateEngine.ChangeState(new MonsterStateFurySprint
            (_param.dashMaxAcc,
            _param.dashMaxSpeed, _param.overDashFactor,
            _param.dashTruncateSpeed, _param.dashRestTimer,
            _param.steerAngularSpeed));
        }
        else if (curr is MonsterStateStun)
        {
            if (curr.finish)
                _rageStateEngine.RevertToPrevState();
        }

    }

    public void Exit(MonsterAction monster)
    {
        _rageStateEngine.ChangeState(new MonsterStateIdle());
    }
}

[Serializable]
public class MonsterRageParam
{
    public float dashMaxAcc;
    public float dashMaxSpeed;
    public float overDashFactor;
    public float dashTruncateSpeed;
    public float dashRestTimer;
    public float steerAngularSpeed;
    public float nearRange;

    public float attackTime;
    public float defenseTime;
    public float fightRatio;
    public float minFightCd;
    public float maxFightCd;

    public float stunTime;
    public int criticalHitNum;

}