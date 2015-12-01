using UnityEngine;
using System.Collections;

public abstract class MonsterStateNavigation : StateBase<MonsterAction>
{
    public Vector2 leftBottomBound;
    public Vector2 rightTopBound;
    public float speed;
    public float steerAngularSpeed;

    public virtual bool finish { get { return false; } }

    protected Vector3 _nextOffset;
    protected StateEngine<MonsterAction> _moveStateEngine;

    public virtual void Enter(MonsterAction monster)
    {
        Vector3 currPos = monster.transform.position;
        //Debug.Assert(currPos.x > leftBottomBound.x && currPos.x < rightTopBound.x &&
        //        currPos.z > leftBottomBound.y && currPos.z < rightTopBound.y);

        _moveStateEngine = new StateEngine<MonsterAction>();
		//_moveStateEngine.debug = true;
        _nextOffset = GenNextOffset(monster, true);
        Quaternion nextRot = Quaternion.LookRotation(_nextOffset);
        _moveStateEngine.Init(monster, new MonsterStateSteer
            (nextRot, steerAngularSpeed));
    }

    public virtual void Execute(MonsterAction monster)
    {
        _moveStateEngine.Execute();

        if (RedirectCondition)
        {
            _nextOffset = GenNextOffset(monster, false);
            Quaternion nextRot;
            if (_nextOffset == Vector3.zero)
                nextRot = monster.transform.rotation;
            else nextRot = Quaternion.LookRotation(_nextOffset);
            _moveStateEngine.ChangeState(new MonsterStateSteer(nextRot, steerAngularSpeed));
        }

        if (_moveStateEngine.currState is MonsterStateSteer && _moveStateEngine.currState.finish)
        {
            _moveStateEngine.ChangeState(new MonsterStateMove(_nextOffset, speed));
        }

        //if (RedirectCondition)
        //{
        //    if (_moveStateEngine.currState is MonsterStateSteer)
        //        _moveStateEngine.ChangeState(new MonsterStateMove(_nextOffset, speed));
        //    else if (_moveStateEngine.currState is MonsterStateMove)
        //    {
        //        _nextOffset = GenNextOffset(monster, false);
        //        Quaternion nextRot = Quaternion.LookRotation(_nextOffset);
        //        _moveStateEngine.ChangeState(new MonsterStateSteer(nextRot, steerAngularSpeed));
        //    }
        //    else throw new UnityException("unexpected state");
        //}
    }

    public virtual void Exit(MonsterAction monster)
    {
        _moveStateEngine.ChangeState(new MonsterStateIdle());
    }

    protected abstract Vector3 GenNextOffset(MonsterAction monster, bool init);
    protected virtual bool RedirectCondition
    {
        get { return _moveStateEngine.currState is MonsterStateMove &&
                _moveStateEngine.currState.finish; }
    }
}
