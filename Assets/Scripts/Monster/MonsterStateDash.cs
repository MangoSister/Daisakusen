using UnityEngine;
using System.Collections;

public class MonsterStateDash : StateBase<MonsterAction>
{
    private Vector3 _offset;
    private float _maxAcc;
    private float _maxSpeed;
    private float _overDashFactor;
    private float _truncateSpeed;
    private float _restTime;

    private Vector3 _target;
    private Vector3 _startPos;
    private Vector3 _currVelo;
    private Vector3 _currAcc;
    private bool _dashEnd;
    private float _restTimer;

    public MonsterStateDash(Vector3 offset, float maxAcc, float maxSpeed, 
        float truncateSpeed, float overDashFactor, float restTime)
    {
        _offset = offset;
        _maxAcc = maxAcc;
        _maxSpeed = maxSpeed;
        _truncateSpeed = truncateSpeed;
        _overDashFactor = overDashFactor;
        _restTime = restTime;
    }

    public bool finish { get; private set; }

    public void Enter(MonsterAction monster)
    {
        _startPos = monster.transform.position;
        _currVelo = Vector3.zero;
        _currAcc = Vector3.zero;
        _target = _startPos + _offset;
        finish = false;
        _dashEnd = false;
        _restTimer = 0f;

        monster.ActivateDash();
    }

    public void Execute(MonsterAction monster)
    {
        if (finish)
            return;
        if (!_dashEnd)
        {
            //a variation of seek & arrive behavior
            Vector3 desired = _target - monster.transform.position;
            float d = desired.magnitude;
            desired.Normalize();
            bool arrive = Vector3.Dot(monster.transform.position - _startPos, _offset.normalized) > _offset.magnitude;
            if (arrive)
                desired *= Mathf.Lerp(0, _maxSpeed, d / _overDashFactor);
            else
                desired *= _maxSpeed;

            Vector3 steer = desired - _currVelo;
            if (steer.magnitude > _maxAcc)
                steer = steer.normalized * _maxAcc;

            _currAcc = steer;
            _currVelo += _currAcc * Time.deltaTime;
            monster.transform.position += _currVelo * Time.deltaTime;

            if (arrive && _currVelo.magnitude < _truncateSpeed)
            {
                _currVelo = Vector3.zero;
                _dashEnd = true;
            }
        }
        else
        {
            _restTimer += Time.deltaTime;
            if (_restTimer > _restTime)
            {
                finish = true;
            }
        }
    }

    public void Exit(MonsterAction monster)
    {
        monster.DeactivateDash();
    }
}
