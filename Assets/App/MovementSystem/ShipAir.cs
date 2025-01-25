using System;
using UnityEngine;
using VRSim.Core;

namespace DeepGame.Quota
{
    public class ShipAir : MonoBehaviour
    {
        [SerializeField] private float _maxAir = 100f;
        [SerializeField] private float _airKoef = 1;

        [SerializeField] private float _currentAir;
        private bool _isAlive = true;
        private QuotaManager _quotaManager;

        private void Start()
        {
            ResetAir();
            _quotaManager = ServiceLocator.Get<QuotaManager>();
            _quotaManager.OnDayFinished += ResetAir;
        }

        private void OnDisable()
        {
            _quotaManager.OnDayFinished -= ResetAir;
        }

        private void Update()
        {
            if (_currentAir > 0 && _isAlive)
            {
                _currentAir -= Time.deltaTime * _airKoef;
                //Debug.LogError("_currentAir " + _currentAir);
            }
            else if (_currentAir <= 0 && _isAlive)
            {
                _isAlive = false;
                _quotaManager.FinishDay(0);
            }
        }

        public void UpgradeMaxAir(float additionalAir)
        {
            _maxAir += additionalAir;
        }

        public void AddAir(float air)
        {
            if (_currentAir + air >= _maxAir)
            {
                _currentAir = _maxAir;
            }
            else
            {
                _currentAir += air;
            }
        }

        private void ResetAir()
        {
            _currentAir = _maxAir;
        }
    }
}
