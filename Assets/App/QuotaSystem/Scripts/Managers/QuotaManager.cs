using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepGame.Quota
{
    public class QuotaManager : MonoBehaviour
    {
        [SerializeField]
        private Quota _startingQuota;

        private Quota _currentQuota;

        public void InitializeQuota()
        {
            _currentQuota = GenerateQuota();
        }

        public void FinishDay()
        {

        }

        private void FinishQuota()
        {

        }

        private Quota GenerateQuota()
        {
            Quota quota = null;

            if (_currentQuota == null)
            {
                quota = _startingQuota;
            }
            else
            {
                quota = new Quota();
            }

            return quota;
        }
    }
}
