using DeepGame.Loot;
using UnityEngine;

namespace DeepGame.Quota
{
    public class ShipInventory : MonoBehaviour
    {
        [Header("Weight Settings")] [SerializeField]
        private float _baseWeight = 1f;

        [SerializeField] private float _additionalWeight = 0f;
        [SerializeField] private float _weightMultiplier = 50f;
        [SerializeField] private float _inertiaKoef;
        [SerializeField] private float _speedKoef;

        public float InertiaKoef => _inertiaKoef;
        public float SpeedKoef => _speedKoef;

        private float _totalWeight;

        private void Awake()
        {
            UpdateWeightParameters();
        }

        private void UpdateWeightParameters()
        {
            _totalWeight = _baseWeight + _additionalWeight;
            _inertiaKoef = _totalWeight / _weightMultiplier;
            _speedKoef = _additionalWeight / _weightMultiplier;
        }

        private void SetAdditionalWeight(float newAdditionalWeight)
        {
            _additionalWeight += newAdditionalWeight;
            UpdateWeightParameters();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Loot"))
            {
                LootItem item = other.GetComponent<LootItem>();
                if (item != null)
                {
                    SetAdditionalWeight(item.Weight);
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
