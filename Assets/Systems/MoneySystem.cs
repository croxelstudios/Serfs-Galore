using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneySystem : MonoBehaviour
{
    [SerializeField]
    string allyTag = "AllyInteraction";
    [SerializeField]
    int money = 0;
    [SerializeField]
    string canvasCoinsTag = "HudCoins";
    

    TextMeshProUGUI tmpCoins;
    

    private void Start()
    {
        tmpCoins = GameObject.FindGameObjectWithTag(canvasCoinsTag)?.GetComponent<TextMeshProUGUI>();
        
        if(tmpCoins)
            tmpCoins.text = money.ToString();
    }

    public void AddMoney(int quantity)
    {
        money += quantity;
        
    }

    private void FixedUpdate()
    {
        if (!tmpCoins)
            tmpCoins = GameObject.FindGameObjectWithTag(canvasCoinsTag)?.GetComponent<TextMeshProUGUI>();
        if (tmpCoins)
        {
            tmpCoins.text = money.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CrowdElement ce = other.GetComponentInParent<CrowdElement>();
        if (other.tag == allyTag && !ce.elementEnabled)
        {
            AddMoney(ce.Buy(money));
        }
        else if(other.tag == allyTag)
        {
            if (ce.sl.ded || !ce.elementEnabled) 
                ce.Buy(int.MaxValue);
        }
    }
}
