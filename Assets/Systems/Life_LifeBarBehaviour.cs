using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Life_LifeBarBehaviour : MonoBehaviour
{
    SimpleLife life;
    [SerializeField]
    Image lifeBarContent = null;

    List<Image> frameUI;
    
    [SerializeField]
    public Color fullLifeColor = Color.green;
    [SerializeField]
    public Color emptyLifeColor = Color.red;
    [SerializeField]
    Color alertColor = Color.white;

    [Range(0,1)]
    [SerializeField]
    float alertPercentage = 0.1f;
    [SerializeField]
    float alertSpeed = 5;
    [SerializeField]
    bool reactiveFrame = false;

    Color currentlifeBarColor;

    public UnityEvent OnLowLife;
    bool eventLaunched = false;

    void OnEnable()
    {
        life = GetComponent<SimpleLife>();
        //lifeBarContent = GameObject.FindGameObjectWithTag("UILife").GetComponent<Image>();
    }

    void OnDestroy()
    {
        if (enabled) Update();
    }

    void Update()
    {
        if (lifeBarContent && lifeBarContent.fillAmount <= alertPercentage) {
            lifeBarContent.color = Color.Lerp(currentlifeBarColor, alertColor, Mathf.PingPong(Time.time * alertSpeed, 1f));
            if (reactiveFrame)
            {
                foreach (Image i in frameUI)
                {
                    i.color = Color.Lerp(currentlifeBarColor, alertColor, Mathf.PingPong(Time.time * alertSpeed, 1f));
                }

                if (lifeBarContent.fillAmount <= alertPercentage && !eventLaunched)
                {
                    OnLowLife?.Invoke();
                    //Debug.Log("low life");
                    eventLaunched = true;
                }
            }
        }
        UpdateLifeBar();
    }

    public void UpdateLifeBar()
    {
        if (enabled && life && lifeBarContent)
        {
            lifeBarContent.fillAmount = life.life / life.maxLife;
            lifeBarContent.color = currentlifeBarColor = Color.Lerp(emptyLifeColor, fullLifeColor, lifeBarContent.fillAmount);
            if (reactiveFrame)
            {
                foreach(Image i in frameUI)
                {
                    i.color = Color.Lerp(emptyLifeColor, fullLifeColor, lifeBarContent.fillAmount);
                }

                if(lifeBarContent.fillAmount <= alertPercentage && !eventLaunched)
                {
                    OnLowLife?.Invoke();
                    //Debug.Log("low life");
                    eventLaunched = true;
                }

            }
        }
    }

    private void Start()
    {
        if (reactiveFrame)
        {
            frameUI = new List<Image>();
            foreach (Image i in GameObject.FindGameObjectWithTag("UIFrame").GetComponentsInChildren<Image>())
            {
                if (i.tag != "DialogSpeaker")
                {
                    frameUI.Add(i);
                }
            }
        }
        UpdateLifeBar();
    }
    private void OnDisable()
    {
        lifeBarContent = null;
    }
}
