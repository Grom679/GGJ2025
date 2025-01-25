using System;
using DeepGame.Loot;
using DeepGame.Quota;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSim.Core;
using Random = UnityEngine.Random;

namespace DeepGame.Map
{
    public class MapGenerator : MonoBehaviour
    {
        public List<LootItem> LootItem => _loot;

        [SerializeField]
        private Transform _ship;
        [SerializeField]
        private MapData _mapData;
        [SerializeField]
        private float _middleValue;
        [SerializeField]
        private float _downValue;
        [SerializeField]
        private float _deadZone = 5f;
        [SerializeField]
        private float _sectionHeight = 2f;
        [SerializeField]
        private List<LootItem> _loot;
        [SerializeField]
        private QuotaManager _quotaManager;
        private float _quotaValue;
        private List<MapSection> _createdSections = new List<MapSection>();
        private int _activeSection = 0;
        private bool _isInit = false;

        private void Awake()
        {
            _quotaManager = ServiceLocator.Get<QuotaManager>();
            _quotaManager.OnNewDayGenerated += GenerateStartingLocation;
        }
        
        private void OnDisable()
        {
            _quotaManager.OnNewDayGenerated -= GenerateStartingLocation;
        }

        private void Update()
        {
            if (_isInit)
            {
                // Check if the ship has moved below the last section
                if (_createdSections.Count == 1)
                {
                    if (_ship.position.y <= _createdSections[0].transform.position.y)
                    {
                        GenerateNewSection();
                    }
                }
                else
                {
                    if (_ship.position.y < _createdSections[_activeSection - 1].transform.position.y - (_sectionHeight/2f))
                    {
                        GenerateNewSection();
                    }
                }

                HideSectionsBelowShip();
            }
        }

        private void HideSectionsBelowShip()
        {
            for (int i = 0; i < _createdSections.Count; i++)
            {
                MapSection section = _createdSections[i];
                if (section.transform.position.y < _ship.position.y - _deadZone || section.transform.position.y > _ship.position.y + _deadZone)
                {
                    // Set the section inactive
                    section.gameObject.SetActive(false);
                }
                else
                {
                    section.gameObject.SetActive(true);
                }
            }
        }

        private void GenerateNewSection()
        {
            MapSection newSection = Instantiate(GetNewSection(), transform);
            newSection.SpawnLoot(_loot);
            // Position the new section below the last one
            newSection.transform.localPosition = new Vector3(0f, _createdSections[_activeSection].transform.localPosition.y - _sectionHeight, 0f);
            // Add the new section to the list of created sections
            _createdSections.Add(newSection);
            _activeSection++;
        }

        private void GenerateStartingLocation(float quota)
        {
            ClearLocation();
            _quotaValue = quota;
            MapSection firstSection = Instantiate(_mapData.upperSections[GetRandomIndex(_mapData.upperSections.Count)], transform);
            _createdSections.Add(firstSection);
            _isInit = true;
        }

        private void ClearLocation()
        {
            _quotaValue = 0f;
            _activeSection = 0;
            _isInit = false;

            foreach(MapSection section in _createdSections)
            {
                Destroy(section.gameObject);
            }

            _createdSections.Clear();
        }

        private MapSection GetNewSection()
        {
            MapSection section;

            if(_ship.position.y < _middleValue && _ship.position.y > _downValue)
            {
               section = _mapData.middleSections[GetRandomIndex(_mapData.middleSections.Count)];
            }
            else if(_ship.position.y < _downValue)
            {
                section = _mapData.downSections[GetRandomIndex(_mapData.downSections.Count)];
            }
            else
            {
                section = _mapData.upperSections[GetRandomIndex(_mapData.upperSections.Count)];
            }

            return section;
        }

        private int GetRandomIndex(int maxValue)
        {
            int index = Random.Range(0, maxValue);
            return index;
        }
    }
}