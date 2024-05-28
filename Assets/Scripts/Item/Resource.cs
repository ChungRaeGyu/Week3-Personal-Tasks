using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive; //지급할 아이템의 정보
    public int quantityPerHit =1; //몇번 맞았을때 아이템을 지급할 지
    public int capacy; //몇번때리면 사라질지
    
    public void Gather(Vector3 hitPoint, Vector3 hitNormal){
        for(int i=0; i<quantityPerHit;i++){
            if(capacy<=0)break;
            capacy -=1;
            Instantiate(itemToGive.dropPrefab,hitPoint+Vector3.up,Quaternion.LookRotation(hitNormal,Vector3.up));
        }
        
    }
}
