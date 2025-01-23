using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepGame.Quota
{
    public class QuotaManager : MonoBehaviour
    {
        public Action OnQuotaFailed { get; set; }
        public Action OnDayFinished{ get; set; }
        public Action<float> OnNewDayGenerated { get; set; }
        public Action OnQuotaCompleted { get; set; }

        [SerializeField]
        private Quota _startingQuota;
        [SerializeField]
        private Quota _currentQuota;
        [SerializeField]
        private float _quotaMinPer = 0.2f;
        [SerializeField]
        private float _quotaMaxPer = 0.3f;
        [SerializeField]
        private float _quotaMaxDays = 6f;
        [SerializeField]
        private float _quotaDayMultiplier = 1.7f;

        private int _currentDay = 1;
        private float _currentQuotaValue = 0f;
        private float _previousQuotaValue = 0f;

        public void InitializeQuota()
        {
            _currentQuota = GenerateQuota();
            _previousQuotaValue = _currentQuota.quotaValue;
            OnNewDayGenerated?.Invoke(_currentQuota.quotaValue);
        }

        public Quota GetQuotaData()
        {
            return _currentQuota;
        }

        public void FinishDay(float quotaDelta)
        {
            if(_currentDay == _currentQuota.daysCount)
            {
                if(_currentQuotaValue >= _currentQuota.quotaValue)
                {
                    ResetParameters();
                    FinishQuota();
                    OnQuotaCompleted?.Invoke();
                }
                else
                {
                    ResetCurrentValues();
                    OnQuotaFailed?.Invoke();
                }
            }
            else
            {
                _currentDay++;
                _currentQuotaValue += quotaDelta;
                OnDayFinished?.Invoke();
            }
        }


        [ContextMenu("Regenerate")]
        public void GenerateNewDay()
        {
            OnNewDayGenerated?.Invoke(_currentQuota.quotaValue);
        }

        private void FinishQuota()
        {
            _currentQuota = GenerateQuota();
        }

        private void ResetCurrentValues()
        {
            ResetParameters();
            _currentQuota.quotaValue = 0f;
            _currentQuota = GenerateQuota();
            _previousQuotaValue = _currentQuota.quotaValue;
        }

        private void ResetParameters()
        {
            _currentDay = 1;
            _currentQuotaValue = 0f;
        }

        private Quota GenerateQuota()
        {
            Quota quota = null;

            if (_currentQuota.quotaValue == 0f)
            {
                quota = _startingQuota;
            }
            else
            {
                quota = new Quota();
                float increasePercentage = UnityEngine.Random.Range(_quotaMinPer, _quotaMaxPer);
                float newQuotaValue = _currentQuota.quotaValue * (1 + increasePercentage);
                int newDaysCount = _currentQuota.daysCount;

                if (newQuotaValue >= _previousQuotaValue * _quotaDayMultiplier)
                {
                    if(newDaysCount >= _quotaMaxDays)
                    {
                        _previousQuotaValue = _currentQuota.quotaValue;
                    }
                    else
                    {
                        newDaysCount += 1;
                        _previousQuotaValue = _currentQuota.quotaValue;
                    }
                }

                quota.quotaValue = Mathf.Round(newQuotaValue);
                quota.daysCount = newDaysCount;
            }

            return quota;
        }
    }
}
