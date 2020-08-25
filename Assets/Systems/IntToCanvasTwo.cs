using UnityEngine;
using UnityEngine.UI;

public class IntToCanvasTwo : MonoBehaviour
{
    [SerializeField]
    public Text text = null;
    [SerializeField]
    GameObject deactivateThis = null;

    CrowdElement element;

    // Start is called before the first frame update
    void Start()
    {
        element = GetComponent<CrowdElement>();
        if (element.price <= 0) deactivateThis.SetActive(false);
        else text.text = element.price.ToString();
    }
}
