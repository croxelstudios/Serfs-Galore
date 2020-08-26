using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MouseRaycast : MonoBehaviour
{
    [SerializeField]
    string leftMouseInput = "Fire1";
    [SerializeField]
    string mainCameraTag = "MainCamera";
    [SerializeField]
    string[] interactionTagsInPriorityOrder = null;
    [SerializeField]
    Crowd crowdSystem = null;


    [SerializeField]
    GameObject crosshairPrefab = null;

    GameObject crosshairInstance;

    Camera mainCamera;

    bool hovering = false;
    [SerializeField]
    Texture2D defaultCursor = null;
    [SerializeField]
    Texture2D aimCursor = null;
    [SerializeField]
    bool justChangeCursor = true;

    [SerializeField]
    UnityEvent onElementClicked = null;
    [SerializeField]
    UnityEvent onElementHover = null;
    [SerializeField]
    UnityEvent onElementUnhover = null;

    private void OnEnable()
    {
        mainCamera = GameObject.FindGameObjectWithTag(mainCameraTag).GetComponent<Camera>();
        if (!justChangeCursor)
        {
            crosshairInstance = Instantiate(crosshairPrefab);
            SceneManager.MoveGameObjectToScene(crosshairInstance, gameObject.scene);
            crosshairInstance.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        RaycastHit[] hits;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        bool tagDetected = false;

        hits = Physics.RaycastAll(ray, 100);
        if (hits.Length > 0)
        {
            int currentPriority = int.MaxValue;
            foreach (RaycastHit hit in hits)
            {
                int priority = System.Array.IndexOf(interactionTagsInPriorityOrder, hit.transform.tag);
                if (priority >= 0 && priority < currentPriority)
                {
                    currentPriority = priority;
                    tagDetected = true;
                }
            }
            if (tagDetected)
            {
                string tagPrioritary = null;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].transform.tag == interactionTagsInPriorityOrder[currentPriority])
                    {
                        Transform objectHit = hits[i].transform;
                        if (!justChangeCursor)
                        {
                            crosshairInstance.transform.position = hits[i].point;
                        }
                        tagPrioritary = hits[i].transform.tag;

                        if (Sinput.GetButtonDown(leftMouseInput))
                        {
                            crowdSystem.AssignTargetToRandomElement(objectHit);
                            onElementClicked?.Invoke();
                        }
                        break;
                    }
                }
                //Debug.Log(tagPrioritary);
                if (!hovering && tagPrioritary != "RaycastMinion")
                {
                    onElementHover?.Invoke();
                    hovering = true;
                    //Debug.Log("HOVER");
                    if (!justChangeCursor)
                        crosshairInstance.SetActive(true);
                    else
                        Cursor.SetCursor(aimCursor, new Vector2(aimCursor.width/2,aimCursor.height/2), CursorMode.Auto);
                }
            }
            else
            {
                if (hovering)
                {
                    
                    onElementUnhover.Invoke();
                    //Debug.Log("UNHOVER");
                    hovering = false;
                    if (!justChangeCursor)
                        crosshairInstance.SetActive(false);
                    else
                        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                }
            }
        }
    }
    private void OnDisable()
    {
        Destroy(crosshairInstance);
    }
}
