using UnityEngine;
using System.Collections;

public class MonsterStateDie : StateBase<MonsterAction>
{
    //play die animation, fx, modify game stage, and finally destory itself
    public bool finish { get { return false; } }
    public void Enter(MonsterAction monster)
    {
        monster.Explode();
        monster.gameObject.SetActive(false);
        if (GameController.Instance != null)
            GameController.Instance.GameEnding(true);
    }

    public void Execute(MonsterAction monster)
    {

    }

    public void Exit(MonsterAction monster)
    {

    }
}
