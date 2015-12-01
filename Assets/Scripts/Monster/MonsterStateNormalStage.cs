using System;
using UnityEngine;
using System.Collections;

public sealed class MonsterStateNormalStage : StateBase<MonsterAction>
{
    private MonsterNormalParam _param;

    private StateEngine<MonsterAction> _normalStateEngine;

    public MonsterStateNormalStage(MonsterNormalParam param)
    {
        _param = param;
    }

    public bool finish { get { return false; } }

    public void Enter(MonsterAction monster)
    {
        _normalStateEngine = new StateEngine<MonsterAction>();
        _normalStateEngine.debug = true;
        _normalStateEngine.Init(monster, new MonsterStateRandomPatrol
                            (monster.leftBottomBound,
                            monster.rightTopBound,
                            _param.patrolSpeed,
                            _param.patrolAngularSpeed,
                            (monster.rightTopBound - monster.leftBottomBound).magnitude * 0.25f,
                            (monster.rightTopBound - monster.leftBottomBound).magnitude * 0.5f
                            ));
        
    }

    public void Execute(MonsterAction monster)
    {
        //state execute
        _normalStateEngine.Execute();

        //state transition
        StateBase<MonsterAction> curr = _normalStateEngine.currState;
        //first check stun
        bool stun = monster.hitCount > _param.criticalHitNum && !(curr is MonsterStateStun);
        if (stun)
        {
            monster.hitCount = 0;
            _normalStateEngine.ChangeState(new MonsterStateStun(_param.stunTime));
            return;
        }


        float targetDist = Vector2.Distance(new Vector2(monster.transform.position.x, monster.transform.position.z),
                                        new Vector2(monster.target.transform.position.x, monster.target.transform.position.z));
        bool near = targetDist <= _param.nearRange;
        

        if (curr is MonsterStateRandomPatrol)
        {
            if (monster.targetActive)
            {
                _normalStateEngine.ChangeState(new MonsterStateChase
                                        (monster.leftBottomBound,
                                        monster.rightTopBound,
                                        _param.chaseSpeed,
                                        _param.chaseAngularSpeed,
                                        _param.minRDir, _param.maxRDir, monster.target));
            }
        }

        else if (curr is MonsterStateChase)
        {
            if (!monster.targetActive)
            {
                _normalStateEngine.ChangeState(new MonsterStateRandomPatrol
                        (monster.leftBottomBound,
                        monster.rightTopBound,
                        _param.patrolSpeed,
                        _param.patrolAngularSpeed,
                        (monster.rightTopBound - monster.leftBottomBound).magnitude * 0.25f,
                        (monster.rightTopBound - monster.leftBottomBound).magnitude * 0.5f
                        ));
            }
            else if (near)
            {
                _normalStateEngine.ChangeState
                    (new MonsterStateFight(_param.minFightCd, _param.maxFightCd, 
                    _param.attackTime, _param.defenseTime, _param.fightRatio, _param.chaseAngularSpeed));
            }

        }

        else if (curr is MonsterStateFight)
        {
            if (!monster.targetActive)
            {
                _normalStateEngine.ChangeState(new MonsterStateRandomPatrol
                        (monster.leftBottomBound,
                        monster.rightTopBound,
                        _param.patrolSpeed,
                        _param.patrolAngularSpeed,
                        (monster.rightTopBound - monster.leftBottomBound).magnitude * 0.25f,
                        (monster.rightTopBound - monster.leftBottomBound).magnitude * 0.5f
                        ));
            }
            else if (!near)
            {
                _normalStateEngine.ChangeState(new MonsterStateChase
                        (monster.leftBottomBound,
                        monster.rightTopBound,
                        _param.chaseSpeed,
                        _param.chaseAngularSpeed,
                        _param.minRDir, _param.maxRDir, monster.target));
            }
        }

        else if (curr is MonsterStateStun)
        {
            if (curr.finish)
                _normalStateEngine.RevertToPrevState();
        }
    }

    public void Exit(MonsterAction monster)
    {
        _normalStateEngine.ChangeState(new MonsterStateIdle());
    }
}

[Serializable]
public class MonsterNormalParam
{
    public float patrolSpeed;
    public float patrolAngularSpeed;

    public float chaseSpeed;
    public float chaseAngularSpeed;
    public float minRDir;
    public float maxRDir;
    public float nearRange;

    public float attackTime;
    public float defenseTime;
    public float fightRatio;
    public float minFightCd;
    public float maxFightCd;

    public float stunTime;
    public int criticalHitNum;
}
