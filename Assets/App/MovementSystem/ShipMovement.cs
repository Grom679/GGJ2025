using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSim.Core;

namespace DeepGame.Quota
{
    public class ShipMovement : MonoBehaviour
    {
        [Header("Movement Settings")] [SerializeField]
        private float _baseSpeed = 5f;

        [SerializeField] private float _baseRotationSpeed = 2f;
        [SerializeField] private float _maxY = 2f;

        [Header("Runtime Values (Debug)")] [SerializeField]
        private float _currentSpeed;

        [SerializeField] private float _currentRotationSpeed;
        [SerializeField] private float _effectiveInertia;
        [SerializeField] private GameObject _deathParticle;
        [SerializeField] private GameObject _engineParticle;
        
        private Vector3 _currentVelocity = Vector3.zero;
        private float _currentRotationY = 0f;
        private Rigidbody _rigidbody;
        private ShipInventory _shipInventory;
        private QuotaManager _quotaManager;
        private Vector3 _newPosition;
        private bool _isRestarted = false;
        private GameObject _triggeredWall;
        private MeshRenderer _meshRenderer;
        
        

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _rigidbody = GetComponent<Rigidbody>();
            _shipInventory = GetComponent<ShipInventory>();
        }

        private void Start()
        {
            _quotaManager = ServiceLocator.Get<QuotaManager>();
            _quotaManager.OnDayFinished += ResetPosition;
        }

        private void OnDisable()
        {
            _quotaManager.OnDayFinished -= ResetPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Wall") && !_isRestarted)
            {
                if (_triggeredWall == null)
                {
                    _triggeredWall = other.gameObject;
                    _isRestarted = true;
                    if (_quotaManager != null)
                    {
                        StartCoroutine(DeathCoroutine());
                    }
                }
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
            _currentVelocity =
                Vector3.Lerp(_currentVelocity, targetVelocity, Time.deltaTime * (1f / _effectiveInertia));

            if (_currentVelocity.magnitude > 0.1f)
            {
                float targetRotationY = _currentVelocity.x > 0 ? 0 : -180;
                float rotationLerpFactor = Mathf.Lerp(0.1f, 1f, 1f / _effectiveInertia);
                _currentRotationY = Mathf.LerpAngle(_currentRotationY, targetRotationY,
                    Time.deltaTime * _currentRotationSpeed * rotationLerpFactor);
            }

            _rigidbody.MoveRotation(Quaternion.Euler(0, _currentRotationY, 0));

            _newPosition = _rigidbody.position + _currentVelocity * Time.deltaTime;

            if (_newPosition.y >= _maxY)
            {
                _newPosition.y = _maxY;
            }

            _rigidbody.MovePosition(_newPosition);
        }

        private void ResetPosition()
        {
            _isRestarted = false;
            _currentVelocity = Vector3.zero;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        public void Movement(bool enable)
        {
            enabled = enable;
        }

        private IEnumerator DeathCoroutine()
        {
            GameObject particle = Instantiate(_deathParticle, transform.position, Quaternion.identity);
            Movement(false);
            _meshRenderer.enabled = false;
            _engineParticle.SetActive(false);
            yield return new WaitForSeconds(2f);
            
            Destroy(particle);
            Movement(true);
            _meshRenderer.enabled = true;
            _engineParticle.SetActive(true);
            ResetPosition();
            _quotaManager.FinishDay(0);
        }
    }
}
