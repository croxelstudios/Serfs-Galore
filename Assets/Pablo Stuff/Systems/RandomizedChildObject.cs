using UnityEngine;

public class RandomizedChildObject : MonoBehaviour
{
    [SerializeField]
    GameObject[] prefabs = null;
    [SerializeField]
    Vector3[] rotations = null;

    void Awake()
    {
        int p = Random.Range(0, prefabs.Length);
        Quaternion rotation = Quaternion.identity;
        if ((rotations != null) && (rotations.Length > 0))
        {
            int r = Random.Range(0, rotations.Length);
            rotation = Quaternion.Euler(rotations[r]);
        }
        GameObject child = Instantiate(prefabs[p], transform.position +
            transform.TransformVector(prefabs[p].transform.position), Quaternion.identity, transform);
        child.transform.localRotation = rotation * prefabs[p].transform.rotation;
    }
}
