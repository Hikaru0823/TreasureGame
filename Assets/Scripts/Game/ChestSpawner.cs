using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private GameObject _chestPrefab;
    [SerializeField] private Renderer _spawnAreaRenderer;

    [Header("Settings")]
    [SerializeField] private float _spawnRotRangeY = 20f;


    void Start()
    {
        SpawnChest();
    }

    public void SpawnChest()
    {
        Bounds spawnBounds = _spawnAreaRenderer.bounds;

        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnBounds.min.x, spawnBounds.max.x),
            _spawnAreaRenderer.transform.position.y,
            Random.Range(spawnBounds.min.z, spawnBounds.max.z));

        float rotY = Random.Range(180f - _spawnRotRangeY, 180f + _spawnRotRangeY);
        var chest = Instantiate(_chestPrefab, spawnPosition, Quaternion.Euler(0f, rotY, 0f)).GetComponent<Chest>();
        chest.OnAttentionedItem -= SpawnChest;
        chest.OnAttentionedItem += SpawnChest;
    }
}
