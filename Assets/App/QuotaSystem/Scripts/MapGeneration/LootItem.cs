using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepGame.Loot
{
    public class LootItem : MonoBehaviour
    {
        public float Price => _price;
        public float Weight => _weight;
        public float Dificulty => _pickupDificulty;

        [SerializeField]
        private float _price;

        [SerializeField] 
        private float _weight;

        [SerializeField]
        private float _pickupDificulty;
    }
}

