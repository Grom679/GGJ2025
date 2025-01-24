using DeepGame.Quota;
using UnityEngine;
using VRSim.Core;

public class ShipMovement : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float _baseSpeed = 5f;
    [SerializeField] private float _baseRotationSpeed = 2f;
    
    [Header("Runtime Values (Debug)")]
    [SerializeField] private float _currentSpeed; 
    [SerializeField] private float _currentRotationSpeed; 
    [SerializeField] private float _effectiveInertia;

    private Vector3 _currentVelocity = Vector3.zero; 
    private float _currentRotationY = 0f;
    private Rigidbody _rigidbody;
    private ShipInventory _shipInventory;
    private QuotaManager _quotaManager;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _shipInventory = GetComponent<ShipInventory>();
        _quotaManager = ServiceLocator.Get<QuotaManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем столкновение со стеной
        if (collision.gameObject.CompareTag("Wall"))
        {
            _quotaManager.FinishDay(0);
        }
    }
    
    private void Update()
    {
        _effectiveInertia = 1f + Mathf.Log(1f + _shipInventory.InertiaKoef);
        _currentSpeed = _baseSpeed / (1f + _shipInventory.SpeedKoef);
        _currentRotationSpeed = _baseRotationSpeed / _effectiveInertia;

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        
        Vector3 targetVelocity = new Vector3(inputX, inputY, 0) * _currentSpeed;
        _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, Time.deltaTime * (1f / _effectiveInertia));
        
        if (_currentVelocity.magnitude > 0.1f)
        {
            float targetRotationY = _currentVelocity.x > 0 ? 0 : -180;
            float rotationLerpFactor = Mathf.Lerp(0.1f, 1f, 1f / _effectiveInertia);
            _currentRotationY = Mathf.LerpAngle(_currentRotationY, targetRotationY, Time.deltaTime * _currentRotationSpeed * rotationLerpFactor);
        }
        
        _rigidbody.MovePosition(_rigidbody.position + _currentVelocity * Time.deltaTime);
        _rigidbody.MoveRotation(Quaternion.Euler(0, _currentRotationY, 0));
    }
}
