using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFog : MonoBehaviour
{

    [SerializeField]
    string cameraTag = "MainCamera";
    [SerializeField]
    string playerTag = "Player";

    [SerializeField]
    float defaultDistance = 12;

    float currentDistance;
    float defaultFogStart;
    float defaultFogEnd;

    Transform player;
    Transform cameraFog;

    private void Awake()
    {
        defaultFogStart = RenderSettings.fogStartDistance;
        defaultFogEnd = RenderSettings.fogEndDistance;
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        cameraFog = GameObject.FindGameObjectWithTag(cameraTag).transform;
    }

    // Update is called once per frame
    void Update()
    {
        currentDistance = Vector3.Distance(player.position, cameraFog.position);

        RenderSettings.fogStartDistance = defaultFogStart + (currentDistance - defaultDistance);
        RenderSettings.fogEndDistance = defaultFogEnd + (currentDistance - defaultDistance);
    }
}
