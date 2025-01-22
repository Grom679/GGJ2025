using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepGame.Loot
{
    public class LootItem : MonoBehaviour
    {
        public float Price => _price;

        [SerializeField]
        private float _price;
    }
}

