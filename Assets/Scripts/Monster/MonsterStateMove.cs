using UnityEngine;
using System.Collections;

public class MonsterStateMove : StateBase<MonsterAction>
{
    private Vector3 _offset;
    private float _speed;

    private Vector3 _startPos;
    private float _startTime;

    public bool finish { get; private set; }

    public MonsterStateMove(Vector3 offset, float speed)
    {
        _offset = offset;
        _speed = speed;
    }

    public void Enter(MonsterAction monster)
    {
        _startPos = monster.transform.position;
        _startTime = Time.time;
        //monster.transform.LookAt(_startPos + _offset);
    }

    public void Execute(MonsterAction monster)
    {
        float percent;
        if (_offset.magnitude != 0f)
            percent = Mathf.Clamp01((Time.time - _startTime) / (_offset.magnitude / _speed));
        else
            percent = 1f;
        if (percent < 1f)
            monster.transform.position = Vector3.Lerp(_startPos, _startPos + _offset, percent);
        else monster.transform.position = _startPos + _offset;
        finish = percent >= 1f;
    }

    public void Exit(MonsterAction monster)
    {
        
    }
}
