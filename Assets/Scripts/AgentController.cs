using System.Collections;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField]
    private bool _StartToRight = true;
    [SerializeField]
    private float _Speed;
    [SerializeField]
    private bool _Movable;
    [SerializeField]
    private float _MaxTime;
    [SerializeField]
    private PlayerController _Player;
    [SerializeField]
    private LayerMask _LayersToShoot;
    [SerializeField]
    private BulletController _Bullet;
    [SerializeField]
    private Transform _WeaponPoint;
    [SerializeField]
    private float _PushForse = 1000f;    
    [SerializeField]
    private float _AimPeriod = 2f;

    private float _Distance = 3f;
    private Vector3 _OriginPos;
    private Rigidbody _RigidBody;
    private float _StartTime;
    private Vector3 _TargetHit;

    private Vector3 _Direction;
    // Update is called once per frame
    private void Awake()
    {
        _StartTime = Time.time;
        _OriginPos = transform.position;
        _RigidBody = GetComponent<Rigidbody>();
        _Direction = _StartToRight ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
        StartCoroutine(WaitForTargetAndStartShoot());
    }
    private void FixedUpdate()
    {
        SetTargetHit();
        if (_Movable)
        {
            Move();
        }
    }
    private void Move()
    {
        _RigidBody.velocity = _Direction;
        if ((transform.position.z - _OriginPos.z) >= _Distance && CheckMinimumTimeForSwitch())
        {
            _StartTime = Time.time;
            SwitchTargetPosition(false);
        }
        if ((transform.position.z - _OriginPos.z) == 0 && CheckMinimumTimeForSwitch())
        {
            _StartTime = Time.time;
            SwitchTargetPosition(true);
        }
        if ((Time.time - _StartTime) > _MaxTime)
        {
            if (_Direction.z > 0)
            {
                _StartTime = Time.time;
                SwitchTargetPosition(false);
            }
            else
            {
                _StartTime = Time.time;
                SwitchTargetPosition(true);
            }
        }
    }
    private void SetTargetHit()
    {
        if (Physics.Raycast(transform.position, (_Player.transform.position - transform.position), out RaycastHit hit, 100f, _LayersToShoot.value) && hit.collider.gameObject.layer == 6)
        {

            _TargetHit = hit.point;
        }
        else
        {
            _TargetHit = Vector3.zero;
        }
    }
    private void SwitchTargetPosition(bool isPositivDirection)
    {
        _Direction = isPositivDirection ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
    }
    bool CheckMinimumTimeForSwitch()
    {
        if (Time.time - _StartTime > 1)
        {
            return true;
        }
        return false;
    }

    IEnumerator WaitForTargetAndStartShoot()
    {
        while (true)
        {
            if (_TargetHit != Vector3.zero)
            {
                var vectorToTarget = (_TargetHit - transform.position).normalized;
                BulletController bullet = Instantiate(_Bullet, _WeaponPoint.position, Quaternion.identity);
                bullet.Collider.isTrigger = true;
                bullet.GetComponent<Rigidbody>().AddForce(vectorToTarget * 10f, ForceMode.VelocityChange);
                bullet.OnTrigger += Bullet_OnTrigger;
                yield return new WaitForSeconds(_AimPeriod);
            }
            else
            {
                yield return new WaitForSeconds(_AimPeriod);
            }
        }
    }

    private void Bullet_OnTrigger(BulletController bullet, Collider collider)
    {
        if (((_LayersToShoot.value & (1 << collider.gameObject.layer)) > 0))
        {
            bullet.OnTrigger -= Bullet_OnTrigger;
            Rigidbody otherRigidBody = collider.GetComponent<Rigidbody>();
            if (otherRigidBody != null)
            {
                otherRigidBody.AddForceAtPosition(bullet.RigidBody.velocity * _PushForse, _TargetHit);
            }
            Destroy(bullet.gameObject); 
        }
    }
}
