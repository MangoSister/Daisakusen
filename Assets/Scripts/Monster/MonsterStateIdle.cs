using UnityEngine;
using System.Collections;

public class MonsterStateIdle : StateBase<MonsterAction>
{
    //play idle animation
    public bool finish { get { return false; } }
    public void Enter(MonsterAction monster)
    {

    }

    public void Execute(MonsterAction monster)
    {

    }

    public void Exit(MonsterAction monster)
    {

    }

}
