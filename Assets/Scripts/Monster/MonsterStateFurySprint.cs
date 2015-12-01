using UnityEngine;
using System.Collections;

public class MonsterStateFurySprint : StateBase<MonsterAction>
{
    public Vector2 leftBottomBound;
    public Vector2 rightTopBound;

    private float _dashMaxAcc;
    private float _dashMaxSpeed;
    private float _overDashFactor;
    private float _dashTruncateSpeed;
    private float _dashRestTime;

    private float _steerAngularSpeed;

    private Vector3 _nextOffset;
    private StateEngine<MonsterAction> _moveStateEngine;

    public MonsterStateFurySprint(float dashMaxAcc, float dashMaxSpeed, float overDashFactor,
        float dashTruncateSpeed, float dashRestTime, float steerAngularSpeed)
    {
        _dashMaxAcc = dashMaxAcc;
        _dashMaxSpeed = dashMaxSpeed;
        _overDashFactor = overDashFactor;
        _dashTruncateSpeed = dashTruncateSpeed;
        _dashRestTime = dashRestTime;
        _steerAngularSpeed = steerAngularSpeed;
    }

    //play idle animation
    public bool finish { get; private set; }
    public void Enter(MonsterAction monster)
    {
        _moveStateEngine = new StateEngine<MonsterAction>();
        _nextOffset = monster.target.transform.position - monster.transform.position;
        _nextOffset.y = 0f;
        Quaternion nextRot = Quaternion.LookRotation(_nextOffset);
        _moveStateEngine.Init(monster, new MonsterStateSteer(nextRot, _steerAngularSpeed));
    }

    public void Execute(MonsterAction monster)
    {
        _moveStateEngine.Execute();
        StateBase<MonsterAction> curr = _moveStateEngine.currState;
        if (curr is MonsterStateDash && curr.finish)
        {
            finish = true;
            _nextOffset = monster.target.transform.position - monster.transform.position;
            _nextOffset.y = 0f;
            Quaternion nextRot;
            if(_nextOffset == Vector3.zero)
                nextRot = monster.transform.rotation;
            else nextRot = Quaternion.LookRotation(_nextOffset);
            _moveStateEngine.Init(monster, new MonsterStateSteer(nextRot, _steerAngularSpeed));
        }
        else if (curr is MonsterStateSteer && curr.finish)
        {
            _moveStateEngine.ChangeState(new MonsterStateDash(_nextOffset, _dashMaxAcc, _dashMaxSpeed, _dashTruncateSpeed, _overDashFactor, _dashRestTime));
            finish = false;
        }
    }

    public void Exit(MonsterAction monster)
    {
        _moveStateEngine.ChangeState(new MonsterStateIdle());
    }

}
