using UnityEngine;
using System.Collections;
using PunchType = MonsterAction.PunchType;

public class MonsterStateAttack : StateBase<MonsterAction>
{
    public float attackInterval;
    private float _attackTimer;
    private Color _oldColor;
    private PunchType _hand;

    public bool finish { get; private set; }

    public MonsterStateAttack(float interval, PunchType hand)
    {
        Debug.Assert(interval > 0f);
        attackInterval = interval;
        _hand = hand;
    }

    public void Enter(MonsterAction monster)
    {
        _attackTimer = 0f;

        //anim
        if (_hand == PunchType.Right && monster.rightPunchObj != null)
            monster._animHandler.TriggerRightAttackAnim();
        else if (_hand == PunchType.Left && monster.leftPunchObj != null)
            monster._animHandler.TriggerLeftAttackAnim();

        //fx
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.PoseFeedback(attackInterval, Color.red, 0.5f);
        }

        //sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.sfxPlay3D(SFXType.MONSTER_ATK, monster.transform);
        }

        if (PoseManager.Instance != null)
            PoseManager.Instance.CreateNextPose
                (_hand == PunchType.Left ? PoseManager.PoseType.LeftDodge : PoseManager.PoseType.RightDodge,
                monster.target.transform);

        if (monster.GetComponent<MeshRenderer>() != null)
        {
            _oldColor = monster.GetComponent<MeshRenderer>().material.color;
            monster.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    public void Execute(MonsterAction monster)
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= 0.5f) // hard code
            monster.ActivatePunch(_hand);
        if (_attackTimer >= attackInterval)
            finish = true;
    }

    public void Exit(MonsterAction monster)
    {
        monster.DeactivatePunch(_hand);
        if (PoseManager.Instance != null)
            PoseManager.Instance.CancelCurrDodgePose();

        if (monster.GetComponent<MeshRenderer>() != null)
            monster.GetComponent<MeshRenderer>().material.color = _oldColor;
    }
}
