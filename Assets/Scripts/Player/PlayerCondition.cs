using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamaged{
    void TakePhysicalDamage(int damage);
}
public class PlayerCondition : MonoBehaviour, IDamaged
{
    public UICondition uiCondition;

    Condition health { get {return uiCondition.health;}}
    Condition hunger {get{return uiCondition.hunger;}}
    Condition stamina {get {return uiCondition.stamina;}}
    
    public float noHungerHealthDecay;

    public event Action onTakeDamge;
    // Update is called once per frame
    void Update()
    {
        hunger.Subtract(hunger.passiveValue*Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if(hunger.curValue==0){
            health.Subtract(noHungerHealthDecay*Time.deltaTime);
        }
        if(health.curValue==0){
            Die();
        }
    }

    public void Heal(float amount){
        health.Add(amount);
    }

    public void Eat(float amount){
        hunger.Add(amount);
    }
    private void Die()
    {
        Debug.Log("죽음");
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        onTakeDamge?.Invoke();
    }
}
