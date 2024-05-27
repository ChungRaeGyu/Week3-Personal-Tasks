using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLightCycle : MonoBehaviour
{
    [Range(0.0f,1.0f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f;  //0.5가 정오
    private float timeRate;
    public Vector3 noon; // 90 0 0

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity; //키값에 따라 변화되는 값
    
    [Header("Moon")]
    public Light moon;
    public Gradient moonColor; //Gradient색상을 정의한다.
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;  //반사각
    // Start is called before the first frame update
    void Start()
    {
        timeRate = 1.0f/fullDayLength;
        time = startTime;
        Debug.Log($"noon = {noon} , noon*0.25 = {noon*0.25f}");
    }

    // Update is called once per frame
    void Update()
    {
        time = (time+timeRate*Time.deltaTime) % 1.0f;
        UpdateLighting(sun,sunColor,sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);

    }


    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve){
        float intensity = intensityCurve.Evaluate(time); //원하는 시간대의 값을 추출 한다.
        //달과 해가 같이 돌게 하면서 기준값을 정오로 하고 *4를 통해서 그 값을 맞춰준다.
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f:0.75f))*noon*4f;
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity; //intensity 강도
        GameObject go = lightSource.gameObject;
        if(lightSource.intensity==0 && go.activeInHierarchy){
            go.SetActive(false);
        }else if(lightSource.intensity>0&&!go.activeInHierarchy){
            go.SetActive(true);
        }
    }
}
