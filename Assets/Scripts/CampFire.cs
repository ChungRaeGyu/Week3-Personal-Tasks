using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;

    List<IDamaged> things = new List<IDamaged>();
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DealDamage",0, damageRate);
    }

    void DealDamage()
    {
        for(int i=0; i<things.Count;i++){
            things[i].TakePhysicalDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out IDamaged damagable)){
            things.Add(damagable);
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.TryGetComponent(out IDamaged damagable)){
            things.Remove(damagable);
        }
    }
}
