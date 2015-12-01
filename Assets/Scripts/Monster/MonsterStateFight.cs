using UnityEngine;
using System.Collections;
using PunchType = MonsterAction.PunchType;

public class MonsterStateFight : StateBase<MonsterAction>
{
    public float minCoolDownTime;
    public float maxCoolDownTime;
    public float attackTime;
    public float defenseTime;

    public float actionRatio;

    public float steerAngularSpeed;

    private float _coolDownTimer;
    private bool _nextAction;
    private StateEngine<MonsterAction> _fightStateEngine;

    public bool finish { get { return false; } }

    public MonsterStateFight(float minCd, float maxCd, 
        float atkTime, float dfsTime, float ratio, float angularSpeed)
    {
        minCoolDownTime = minCd;
        maxCoolDownTime = maxCd;
        attackTime = atkTime;
        defenseTime = dfsTime;
        actionRatio = ratio;
        steerAngularSpeed = angularSpeed;
    }


    public void Enter(MonsterAction monster)
    {
        _fightStateEngine = new StateEngine<MonsterAction>();
        _nextAction = Random.value > actionRatio ? false : true;
        Vector3 offset = monster.target.transform.position - monster.transform.position;
        offset.y = 0f;
        Quaternion nextRot;
        if (offset == Vector3.zero)
            nextRot = monster.transform.rotation;
        else
            nextRot = Quaternion.LookRotation(offset);
        _fightStateEngine.Init(monster, new MonsterStateSteer(nextRot, steerAngularSpeed));
        //_fightStateEngine.Init(monster, new MonsterStateAttack(attackTime, Random.value > 0.5f ? PunchType.Left : PunchType.Right));

    }

    public void Execute(MonsterAction monster)
    {
        _fightStateEngine.Execute();
        StateBase<MonsterAction> curr = _fightStateEngine.currState;

        if (curr is MonsterStateSteer)
        {
            _coolDownTimer -= Time.deltaTime;
            if (curr.finish)
                _fightStateEngine.ChangeState(new MonsterStateIdle());

        }
        else if (curr is MonsterStateIdle)
        {
            _coolDownTimer -= Time.deltaTime;
            if (_coolDownTimer <= 0f)
            {
                _fightStateEngine.ChangeState(_nextAction ?
                (new MonsterStateAttack(attackTime, Random.value > 0.5f ? PunchType.Left : PunchType.Right)) as StateBase<MonsterAction> :
                (new MonsterStateDefense(defenseTime)) as StateBase<MonsterAction>);
            }
        }
        else if ((curr is MonsterStateAttack || curr is MonsterStateDefense) && curr.finish)
        {
            float waitTime;
            bool action;
            GenNextAction(out waitTime, out action);
            _coolDownTimer = waitTime;
            _nextAction = action;

            Vector3 offset = monster.target.transform.position - monster.transform.position;
            offset.y = 0f;
            Quaternion nextRot;
            if (offset == Vector3.zero)
                nextRot = monster.transform.rotation;
            else
                nextRot = Quaternion.LookRotation(offset);
            _fightStateEngine.Init(monster, new MonsterStateSteer(nextRot, steerAngularSpeed));
        }

    }

    public void Exit(MonsterAction monster)
    {
        _fightStateEngine.ChangeState(new MonsterStateIdle());
    }

    private void GenNextAction(out float waitTime, out bool action)
    {
        waitTime = Random.Range(minCoolDownTime, maxCoolDownTime);
        action = Random.value > actionRatio ? false : true;
    }
}
