using DeepGame.Loot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepGame.Map
{
    public class MapSection : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _lootPoint;
        [SerializeField]
        private float _lootValueBorder;
        [SerializeField]
        private float _lootMaxPrice;
        [SerializeField]
        private float _valueOffset;

        private List<Transform> _filledPoints = new List<Transform>();
        private List<LootItem> _generatedLood = new List<LootItem>();
        private float _usedValue = 0f;

        public float SpawnLoot(List<LootItem> items)
        {
            float value = 0f;
            List<LootItem> correctItems = items.FindAll(x => x.Price <= _lootMaxPrice);

            while (_usedValue <= (_lootValueBorder - _valueOffset))
            {
                if (_filledPoints.Count == _lootPoint.Count)
                {
                    break;
                }

                int index = Random.Range(0, correctItems.Count);
                LootItem item = correctItems[index];

                Transform point;
                do
                {
                    int pointIndex = Random.Range(0, _lootPoint.Count);
                    point = _lootPoint[pointIndex];
                }
                while (_filledPoints.Contains(point)); // Keep finding a new point if already used

                _filledPoints.Add(point);
                _usedValue += item.Price;

                LootItem createdItem = Instantiate(item, point.position, Quaternion.identity);
                createdItem.transform.SetParent(transform);
                _generatedLood.Add(createdItem);
            }
            value = _usedValue;
            return value;
        }
    }
}

