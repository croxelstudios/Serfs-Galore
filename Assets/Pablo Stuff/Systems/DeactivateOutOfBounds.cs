using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DeactivateOutOfBounds : MonoBehaviour
{
    GameObject[][] gameObjects;
    BoxCollider col;

    //[SerializeField]
    bool update = false; //TO DO: Support for this (doesnt work properly right now)
    [SerializeField]
    string[] tags = null;

    void Awake()
    {
        GetObjects();
        col = GetComponent<BoxCollider>();
    }

    void GetObjects()
    {
        gameObjects = new GameObject[tags.Length][];
        for (int i = 0; i < gameObjects.Length; i++)
            gameObjects[i] = GameObject.FindGameObjectsWithTag(tags[i]);
    }

    void FixedUpdate()
    {
        if (update) GetObjects();
        if (gameObjects != null)
        {
            foreach (GameObject[] objs in gameObjects)
                if (objs != null) foreach (GameObject obj in objs)
                        if (obj != null) obj.SetActive(col.bounds.Contains(obj.transform.position));
        }
    }
}
