using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : SceneSingleton<ItemHolder>
{
    [Header("Resources")]
    [SerializeField] private GameObject[] _itemPrefabs;

    public GameObject GetRandomItem()
    {
        int randomIndex = Random.Range(0, _itemPrefabs.Length);
        GameObject randomItemPrefab = _itemPrefabs[randomIndex];
        return randomItemPrefab;
    }
}
