using UnityEngine;

public class PrefabInstancer : MonoBehaviour
{
    [SerializeField]
    GameObject[] prefabs = null;
    [SerializeField]
    bool inheritRotation = true;
    [SerializeField]
    SpawnMode spawnMode = SpawnMode.World;

    void OnDisable()
    {
    }

    public void InstantiatePrefab()
    {
        if (isActiveAndEnabled) foreach (GameObject prefab in prefabs)
        {
            switch (spawnMode)
            {
                case SpawnMode.Sibling:
                    Instantiate(prefab, transform.position,
                        inheritRotation ? transform.rotation : Quaternion.identity, transform.parent);
                    break;
                case SpawnMode.Child:
                    Instantiate(prefab, transform.position,
                        inheritRotation ? transform.rotation : Quaternion.identity, transform);
                    break;
                default:
                    Instantiate(prefab, transform.position,
                        inheritRotation ? transform.rotation : Quaternion.identity);
                    break;
            }
            
        }
        //TO DO: Support for keeping momentum of original object
    }

    enum SpawnMode { World, Sibling, Child }
}
