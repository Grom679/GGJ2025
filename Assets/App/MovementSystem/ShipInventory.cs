using System;
using DeepGame.Loot;
using UnityEngine;
using VRSim.Core;

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
        [SerializeField] private ReactionMinigame _rewardMinigame;
        [SerializeField] private GameObject _miniGameRoot;
        [SerializeField] private float _price;
        public float InertiaKoef => _inertiaKoef;
        public float SpeedKoef => _speedKoef;
        public float Price => _price;

        private float _totalWeight;
        private ShipMovement _movement;
        private ShipAir _air;
        private Collider _currentLoot;
        private QuotaManager _quotaManager;

        private void Awake()
        {
            UpdateWeightParameters();
            _air = GetComponent<ShipAir>();
            _movement = GetComponent<ShipMovement>();
        }

        private void Start()
        {
            _quotaManager = ServiceLocator.Get<QuotaManager>();
            //_quotaManager.OnDayFinished += ResetWeight;
            _quotaManager.OnNewDayGenerated += ResetWeight;
        }
        
        private void OnDestroy()
        {
            _quotaManager.OnNewDayGenerated += ResetWeight;
            //_quotaManager.OnDayFinished -= ResetWeight;
        }

        private void OnEnable()
        {
            _rewardMinigame.OnSuccess += PickUpLoot;
            _rewardMinigame.OnFailure += FialLootPickup;
            _rewardMinigame.OnClose += FinishGame;
            _air.OnDeath += Die;
        }

        private void OnDisable()
        {
            _rewardMinigame.OnSuccess -= PickUpLoot;
            _rewardMinigame.OnFailure -= FialLootPickup;
            _rewardMinigame.OnClose -= FinishGame;
            _air.OnDeath -= Die;
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
                _currentLoot = other;
                _movement.enabled = false;
                StartGame();
            }
        }

        private void FialLootPickup()
        {
            _air.AddAir(-10f);  
        }

        private void Die()
        {
            if (_rewardMinigame.isActiveAndEnabled)
            {
                _rewardMinigame.Close();
                FinishGame();
            }
        }

        private void FinishGame()
        {
            _movement.enabled = true;
            _miniGameRoot.SetActive(false);
        }

        private void PickUpLoot()
        {
            LootItem item = _currentLoot.GetComponent<LootItem>();
            if (item != null)
            {
                SetAdditionalWeight(item.Weight);
                _price += item.Price;
                Destroy(_currentLoot.gameObject);
                _rewardMinigame.Close();
                _miniGameRoot.SetActive(false);
                _currentLoot = null;
                _movement.enabled = true;
            }
        }

        private void StartGame()
        {
            LootItem item = _currentLoot.GetComponent<LootItem>();
            if (item != null)
            {
                _miniGameRoot.SetActive(true);
                _rewardMinigame.ActivateGame(item.Dificulty);
            }
        }

        private void ResetWeight(float quota)
        {
            _additionalWeight = 0;
            _price = 0;
            UpdateWeightParameters();
        }
    }
}
