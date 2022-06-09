using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Image _AimCursor;
    [SerializeField]
    private Camera _Camera;
    [SerializeField]
    private Rigidbody _RigidBody;
    [SerializeField]
    private float _Speed;
    [SerializeField]
    private float _MouseSensitivity = 0.6f;
    [SerializeField]
    private Collider _Collider;
    [SerializeField]
    private LayerMask _LayersToDestroyWithBullets;
    [SerializeField]
    private LayerMask _LayersToDestroyBullet;
    [SerializeField]
    private Transform _WeaponPointTransform;
    [SerializeField]
    private BulletController _Bullet;
    [SerializeField]
    private ParticleSystem _Explosion;
    [SerializeField]
    float _BulletSpeed = 10f;

    private Quaternion _OriginRotation;
    private float _MouseX;
    private Vector3 _MovementVector;
    private Vector2 _InputVector;
    private Logger _LoggerInstance;
    private RaycastHit _CurrentTarget;

    private void Start()
    {
        _LoggerInstance = Logger.Instance;
        Cursor.visible = false;
        _OriginRotation = transform.rotation;
    }
    private void FixedUpdate()
    {
        if (_MovementVector != Vector3.zero && IsOnGround())
        {
            _RigidBody.velocity = transform.TransformDirection(_MovementVector * _Speed);
        }
        SetTarget();
    }
    private void SetTarget()
    {
        if (Physics.Raycast(_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out RaycastHit hit, 100f))
        {
            _CurrentTarget = hit;
            SetCursorColor(hit);
        }
        else if (_AimCursor.color == Color.yellow)
        {
            _AimCursor.color = Color.red;
        }
    }
    private bool CheckLayerWithHit(LayerMask layerMask, Collider collider) 
    {
        return (layerMask.value & (1 << collider.gameObject.layer)) > 0;
    }
    private void SetCursorColor(RaycastHit hit)
    {
        if (CheckLayerWithHit(_LayersToDestroyWithBullets, hit.collider))
        {
            _AimCursor.color = Color.yellow;
        }
        else
        {
            _AimCursor.color = Color.red;
        }
    }
    public void OnLook(InputValue input)
    {
        _MouseX += input.Get<Vector2>().x;
        Quaternion rotationY = Quaternion.AngleAxis(_MouseX * _MouseSensitivity, Vector3.up);
        transform.rotation = _OriginRotation * rotationY;
    }
    public void OnMove(InputValue input)
    {
        _InputVector = input.Get<Vector2>();
        _MovementVector = new Vector3(_InputVector.x, 0, _InputVector.y);
    }
    public void OnFire()
    {
        _LoggerInstance.WriteLogMessage("Shooted " + _CurrentTarget.collider.gameObject.name + "!");
        ShootWithBullet();
    }
    private void ShootWithBullet()
    {
        BulletController bullet = Instantiate(_Bullet, _WeaponPointTransform.position, Quaternion.identity);
        bullet.RigidBody.AddForce((_CurrentTarget.point - _WeaponPointTransform.position).normalized * _BulletSpeed, ForceMode.Impulse);
        bullet.OnCollide += Bullet_OnCollide;
    }
    private void Bullet_OnCollide(BulletController bullet, Collision target)
    {
        if (CheckLayerWithHit(_LayersToDestroyBullet, target.collider))
        {
            ExplodeBullet(bullet, target.GetContact(0).point);

            if (CheckLayerWithHit(_LayersToDestroyWithBullets, target.collider))
            {
                Logger.Instance.WriteLogMessage("Destroyed " + target.gameObject.name + "!");
                Destroy(target.gameObject);
            } 
        }
    }
    private void ExplodeBullet(BulletController bullet, Vector3 explosionPoint)
    {
        bullet.OnCollide -= Bullet_OnCollide;
        Instantiate(_Explosion, explosionPoint, Quaternion.identity);
        Destroy(bullet.gameObject);
    }
    public void DeactivateCursor()
    {
        _AimCursor.enabled = false;
    }
    public bool IsOnGround()
    {
        return Physics.BoxCast(transform.position + Vector3.up, new Vector3(_Collider.bounds.extents.x / 2 + 0.1f, 0.1f, _Collider.bounds.extents.z / 2 + 0.1f), -Vector3.up * 2, Quaternion.identity,2f, _LayersToDestroyBullet);
    }
}
