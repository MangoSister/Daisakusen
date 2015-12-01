using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class ParachuteItem : MonoBehaviour
{
    public float constResistAcc;
    public float parachuteResistFactor;
    public float openParachuteTime;
    public PowerUpType powerUpType;
    public float lifeSpan;

    private Rigidbody _rigidBody { get { return GetComponent<Rigidbody>(); } }
    private float _resist;
    private bool _onGround;

    public delegate void ItemOpenHandler(PowerUpType type);
    public event ItemOpenHandler itemOpen;

    private void Awake()
    {
        _onGround = false;
        _resist = constResistAcc;
        StartCoroutine(FallingCoroutine());
    }

    private void FixedUpdate()
    {
        if (!_onGround)
            _rigidBody.AddForce(Vector3.up * _resist * Mathf.Abs(Mathf.Min(_rigidBody.velocity.y, 0f)), ForceMode.Acceleration);
    }

    private IEnumerator FallingCoroutine()
    {
        float timer = 0f;
        while (timer < openParachuteTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        //maybe parachute animation

        _resist *= parachuteResistFactor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !_onGround)
        {
            _onGround = true;
        }
        else if (collision.gameObject.CompareTag("Robot"))
        {
            //trigger pose
            if (itemOpen != null)
                itemOpen(powerUpType);
            Destroy(gameObject);
        }
    }

    public void StartCycling()
    {
        StartCoroutine(SelfDestroyCoroutine());
    }

    private IEnumerator SelfDestroyCoroutine()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }

    public enum PowerUpType
    {
        SuperBlast,
        SpeedUp,
    }
}