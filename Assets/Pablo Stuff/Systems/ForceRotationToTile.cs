using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ForceRotationToTile : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    Vector2 unitsLenght = Vector2.one;
    [SerializeField]
    Vector3 plane2DNormal = Vector3.up;
    [SerializeField]
    Vector3 plane2DUp = Vector3.forward;
    [SerializeField]
    OrientedPrefab[] orientedPrefabs = null;
    [SerializeField]
    bool stopExecute = false;

    bool pLeft = false;
    bool pTop = false;
    bool pRight = false;
    bool pBottom = false;
    bool pLeftTop = false;
    bool pRightTop = false;
    bool pRightBottom = false;
    bool pLeftBottom = false;

    Vector2Int myKey;

    static Dictionary<Vector2Int, ForceRotationToTile> allTiles;

    bool CanDo()
    {
        return (!Application.isPlaying) && (PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab) && (!stopExecute);
    }

    void OnEnable()
    {
        if (CanDo())
        {
            if (allTiles == null) allTiles = new Dictionary<Vector2Int, ForceRotationToTile>();

            SwitchChild(false, false, false, false, false, false, false, false);
            StartCoroutine(UpdateAfterFrame(true));
            EditorApplication.delayCall += EditorApplication.QueuePlayerLoopUpdate;
        }
    }

    private void OnDisable()
    {
        if (CanDo())
        {
            if (allTiles.ContainsKey(myKey)) allTiles.Remove(myKey);
            UpdateSurroundings();
        }
    }

    public void UpdateStateCo()
    {
        if (CanDo())
        {
            StartCoroutine(UpdateAfterFrame());
            EditorApplication.delayCall += EditorApplication.QueuePlayerLoopUpdate;
        }
    }

    //-----------------------------------vFunctionsv------------------------------------

    [ContextMenu("CleanDictionary")]
    public void CleanDictionary()
    {
        allTiles.Clear();
    }

    void UpdateState()
    {
        bool left = false;
        bool top = false;
        bool right = false;
        bool bottom = false;
        bool leftTop = false;
        bool rightTop = false;
        bool rightBottom = false;
        bool leftBottom = false;

        left = left || allTiles.ContainsKey(myKey + Vector2Int.left);
        top = top || allTiles.ContainsKey(myKey + Vector2Int.up);
        right = right || allTiles.ContainsKey(myKey + Vector2Int.right);
        bottom = bottom || allTiles.ContainsKey(myKey + Vector2Int.down);
        leftTop = leftTop || allTiles.ContainsKey(myKey + new Vector2Int(-1, 1));
        rightTop = rightTop || allTiles.ContainsKey(myKey + new Vector2Int(1, 1));
        rightBottom = rightBottom || allTiles.ContainsKey(myKey + new Vector2Int(1, -1));
        leftBottom = leftBottom || allTiles.ContainsKey(myKey + new Vector2Int(1, -1));

        if ((left != pLeft) || (top != pTop) || (right != pRight) || (bottom != pBottom)
            || (leftTop != pLeftTop) || (rightTop != pRightTop) || (rightBottom != pRightBottom) || (leftBottom != pLeftBottom))
            SwitchChild(left, top, right, bottom, leftTop, rightTop, rightBottom, leftBottom);

        pLeft = left;
        pTop = top;
        pRight = right;
        pBottom = bottom;
        pLeftTop = leftTop;
        pRightTop = rightTop;
        pRightBottom = rightBottom;
        pLeftBottom = leftBottom;
    }

    void SwitchChild(bool left, bool top, bool right, bool bottom,
        bool leftTop, bool rightTop, bool rightBottom, bool leftBottom)
    {
        foreach (Transform child in transform) DestroyImmediate(child.gameObject); //TO DO: Sure?

        OrientedPrefab[] op = (OrientedPrefab[])orientedPrefabs.Clone();
        int[] points = new int[op.Length];
        for (int i = 0; i < op.Length; i++)
        {
            if (left == op[i].left) points[i]++;
            if (top == op[i].top) points[i]++;
            if (right == op[i].right) points[i]++;
            if (bottom == op[i].bottom) points[i]++;
            if (leftTop == op[i].leftTop) points[i]++;
            if (rightTop == op[i].rightTop) points[i]++;
            if (rightBottom == op[i].rightBottom) points[i]++;
            if (leftBottom == op[i].leftBottom) points[i]++;
        }

        Array.Sort(points, op);
        Array.Reverse(op);
        if (op[0].prefab != null) Instantiate(op[0].prefab, transform);
    }

    void UpdateSurroundings()
    {
        if (allTiles.ContainsKey(myKey + Vector2Int.left)) allTiles[myKey + Vector2Int.left].UpdateStateCo();
        if (allTiles.ContainsKey(myKey + Vector2Int.up)) allTiles[myKey + Vector2Int.up].UpdateStateCo();
        if (allTiles.ContainsKey(myKey + Vector2Int.right)) allTiles[myKey + Vector2Int.right].UpdateStateCo();
        if (allTiles.ContainsKey(myKey + Vector2Int.down)) allTiles[myKey + Vector2Int.down].UpdateStateCo();
        if (allTiles.ContainsKey(myKey + new Vector2Int(-1, 1))) allTiles[myKey + new Vector2Int(-1, 1)].UpdateStateCo();
        if (allTiles.ContainsKey(myKey + new Vector2Int(1, 1))) allTiles[myKey + new Vector2Int(1, 1)].UpdateStateCo();
        if (allTiles.ContainsKey(myKey + new Vector2Int(1, -1))) allTiles[myKey + new Vector2Int(1, -1)].UpdateStateCo();
        if (allTiles.ContainsKey(myKey + new Vector2Int(1, -1))) allTiles[myKey + new Vector2Int(1, -1)].UpdateStateCo();
    }

    IEnumerator UpdateAfterFrame(bool updateSurroundings = false)
    {
        yield return new WaitForEndOfFrame();
        UpdateStates(updateSurroundings);
    }

    void UpdateStates(bool isOnEnable = false)
    {
        if (isOnEnable)
        {
            myKey = new Vector2Int(Mathf.FloorToInt(transform.position.x / unitsLenght.x),
                Mathf.FloorToInt(transform.position.z / unitsLenght.y)); //TO DO: Should use plane info to calculate the vector 2 from the transform
            if (allTiles.ContainsKey(myKey)) allTiles.Remove(myKey);
            allTiles.Add(myKey, this);

            UpdateState();
            UpdateSurroundings();
        }
        else UpdateState();
    }

    [Serializable]
    struct OrientedPrefab
    {
        public bool left;
        public bool top;
        public bool right;
        public bool bottom;
        public bool leftTop;
        public bool rightTop;
        public bool rightBottom;
        public bool leftBottom;
        public GameObject prefab;

        public OrientedPrefab(bool left, bool top, bool right, bool bottom,
            bool leftTop, bool rightTop, bool rightBottom, bool leftBottom, GameObject prefab)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.leftTop = leftTop;
            this.rightTop = rightTop;
            this.rightBottom = rightBottom;
            this.leftBottom = leftBottom;
            this.prefab = prefab;
        }
    }

    //TO DO: Theres probably a better mathematical way of doing this
    Vector3 InterpretVector2(Vector2 input, Vector3 planeNormal, Vector3 planeUp)
    {
        float vectorAngle = -Vector2.SignedAngle(Vector2.up, input);
        return Quaternion.AngleAxis(vectorAngle, planeNormal) * planeUp * input.magnitude;
    }
#endif
}
