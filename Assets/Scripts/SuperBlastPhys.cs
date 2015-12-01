using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class SuperBlastPhys : MonoBehaviour
{
    public float damage = 30f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("monster"))
        {
            var monster = other.gameObject.GetComponent<MonsterAction>();
            if (monster != null)
                monster.DealDamage(damage);
        }
    }
}
