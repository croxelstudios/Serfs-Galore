using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeactivateOutOfBounds : MonoBehaviour
{
    List<DeactivableArea> objects;
    Collider col;
    Camera cam;

    //TO DO: Add support for newly created objects
    [SerializeField]
    string[] tags = null;
    [SerializeField]
    [Tooltip("This feature ONLY supports static objects")]
    Vector3 subAreasLength = Vector3.zero;
    [SerializeField]
    bool parentedAreas = true;
    [SerializeField]
    TimeMode timeMode = TimeMode.Update;
    //[SerializeField] //TO DO
    BoundsMode getBoundsFrom = BoundsMode.Position;

    enum TimeMode { Update, FixedUpdate }
    enum BoundsMode { Position, Collider, Renderer }

    bool useCam;

    void Awake()
    {
        col = GetComponent<Collider>();
        if (col == null)
        {
            cam = GetComponent<Camera>();
            useCam = true;
        }
        GetObjects();
    }

    void GetObjects()
    {
        objects = new List<DeactivableArea>();
        if ((subAreasLength.x <= 0f) || (subAreasLength.y <= 0f) || (subAreasLength.z <= 0f))
        {
            //Not using subAreas
            for (int i = 0; i < tags.Length; i++)
            {
                GameObject[] array = GameObject.FindGameObjectsWithTag(tags[i]);
                foreach (GameObject gObj in array)
                    objects.Add(new DeactivableArea(new GameObject[] { gObj }, GetBounds(gObj)));
            }
        }
        else
        {
            //Using subAreas
            Dictionary<Vector3Int, List<GameObject>> dictionary = new Dictionary<Vector3Int, List<GameObject>>();

            for (int i = 0; i < tags.Length; i++)
            {
                GameObject[] array = GameObject.FindGameObjectsWithTag(tags[i]);

                //Get subAreas in grid
                foreach (GameObject gObj in array)
                {
                    if (gObj.isStatic)
                    {
                        Vector3 position = gObj.transform.position;
                        Vector3Int gridPos = new Vector3Int(Mathf.RoundToInt(position.x / subAreasLength.x),
                            Mathf.RoundToInt(position.y / subAreasLength.y), Mathf.RoundToInt(position.z / subAreasLength.z));
                        if (!dictionary.ContainsKey(gridPos)) dictionary.Add(gridPos, new List<GameObject>());
                        dictionary[gridPos].Add(gObj);
                    }
                    else objects.Add(new DeactivableArea(new GameObject[] { gObj }, GetBounds(gObj)));
                }
            }

            //Set subAreas data
            foreach (KeyValuePair<Vector3Int, List<GameObject>> pair in dictionary)
            {
                Bounds objBounds = new Bounds(Vector3.Scale(pair.Key, subAreasLength), subAreasLength);
                foreach (GameObject go in pair.Value) objBounds.Encapsulate(GetBounds(go));
                if (parentedAreas)
                {
                    GameObject parent = new GameObject("Area");
                    SceneManager.MoveGameObjectToScene(parent, gameObject.scene); //TO DO: Is this really necessary?
                    parent.transform.position = objBounds.center;

                    foreach (GameObject child in pair.Value) child.transform.parent = parent.transform;
                    objects.Add(new DeactivableArea(new GameObject[] { parent }, objBounds, true));
                }
                else objects.Add(new DeactivableArea(pair.Value, objBounds, true));
            }
        }
    }

    Bounds GetBounds(GameObject obj)
    {
        switch (getBoundsFrom) //TO DO
        {
            default: return new Bounds(obj.transform.position, Vector2.zero);
        }
    }

    void Update()
    {
        if (timeMode == TimeMode.Update) UpdateVisible();
    }

    void FixedUpdate()
    {
        if (timeMode == TimeMode.FixedUpdate) UpdateVisible();
    }

    void UpdateVisible()
    {
        if (objects != null)
        {
            foreach (DeactivableArea area in objects)
            {
                if (!area.CheckNull())
                {
                    if (IsAreaVisible(area)) foreach (GameObject obj in area.objs) obj?.SetActive(true);
                    else foreach (GameObject obj in area.objs) obj?.SetActive(false);
                }
            }
        }
    }

    bool IsAreaVisible(DeactivableArea area)
    {
        if (useCam) return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), area.isStatic ? area.bounds : GetBounds(area.objs[0]));
        else return col.bounds.Intersects(area.isStatic ? area.bounds : GetBounds(area.objs[0]));
    }

    struct DeactivableArea
    {
        public GameObject[] objs;
        public Bounds bounds;
        public bool isStatic;

        public DeactivableArea(IEnumerable<GameObject> objs, Bounds bounds, bool isStatic = false)
        {
            this.objs = objs.ToArray();
            this.bounds = bounds;
            this.isStatic = isStatic;
        }

        public bool CheckNull()
        {
            if ((objs == null) || (objs.Length == 0) || (objs[0] == null)) return true;
            else return false;
        }
    }
}
