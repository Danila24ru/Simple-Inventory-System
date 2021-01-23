using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Create Item", order = 0)]
    public class ItemData : ScriptableObject
    {
        public int id;
        public string name;
        public ItemType itemType;
        public float weight;

        public Sprite icon;
        public GameObject itemPrefab;
    }
}

