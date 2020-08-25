using UnityEngine;
using UnityEngine.UI;

public class IntToCanvas : MonoBehaviour
{
    [SerializeField]
    public Text text;
    [SerializeField]
    bool updateOnChange = false;

    CrowdItem item;

    // Start is called before the first frame update
    void Start()
    {
        item = GetComponent<CrowdItem>();
        text.text = item.amountToGrab.ToString();
    }

    private void Update()
    {
        if (updateOnChange)
        {
            text.text = (item.amountToGrab - item.GetIteractingCount()).ToString();
        }
    }
}
