using UnityEngine;
using System.Collections;

public class MonsterAnimHandler : MonoBehaviour
{
    public Animator monsterAnim;

    private Vector3 lastPos;

    public void TriggerLeftAttackAnim()
    {
        monsterAnim.SetTrigger("LeftAttack");
    }

    public void TriggerRightAttackAnim()
    {
        monsterAnim.SetTrigger("RightAttack");

    }

    public void TriggerSpinAnim()
    {
        monsterAnim.SetTrigger("Spin");
    }

    public void SetMovePlaySpeed(float speed = 1.0f)
    {
        monsterAnim.SetFloat("MovePlaySpeed", speed);
    }

    public void Awake()
    {
        lastPos = transform.position;
    }

    private void Update()
    {
        Vector3 currPos = transform.position;
        float speed = (currPos - lastPos).magnitude / Time.deltaTime;
        monsterAnim.SetFloat("MoveSpeed", speed);
        lastPos = currPos; 
    }
}
