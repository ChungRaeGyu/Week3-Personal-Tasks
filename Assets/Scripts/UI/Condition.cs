using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue;
    public Image uiBar;

    void Start()
    {
        curValue = startValue;
    }

    // Update is called once per frame
    void Update()
    {
        //ui업데이트
        uiBar.fillAmount = GetPercentage();
    }

    float GetPercentage(){
        return curValue / maxValue;
    }
    public void Add(float value){
        curValue = Mathf.Min(curValue+value,maxValue);
    }
    public void Subtract(float value){
        curValue = Mathf.Max(curValue-value,0);
    }
}
