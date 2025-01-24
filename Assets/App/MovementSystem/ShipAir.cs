using System;
using System.Collections;
using System.Collections.Generic;
using DeepGame.Quota;
using UnityEngine;
using VRSim.Core;

public class ShipAir : MonoBehaviour
{
    [SerializeField] private float _maxAir = 100f;
    [SerializeField] private float _airKoef = 1; 
    
    private float _currentAir;
    private bool _isAlive = true;
    private QuotaManager _quotaManager;

    private void Awake()
    {
        _currentAir = _maxAir;
        _quotaManager = ServiceLocator.Get<QuotaManager>();
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
}
