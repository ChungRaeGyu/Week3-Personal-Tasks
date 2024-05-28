using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipTool : Equip
{
    public float attackRate;
    private bool attacking;
    public float attackDistance;
    public float useStamina;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    private Animator animator;
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        camera = Camera.main;
    }

    public override void OnAttackInput()
    {
        Debug.Log("오버라이드");
        if(!attacking){
            if(CharacterManager.Instance.Player.condition.UseStamina(useStamina))
            attacking = true;
            animator.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);
        }
    }

    void OnCanAttack(){
        attacking = false;

    }
    void OnHit(){
        Debug.Log("OnHit");
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,attackDistance)){
            if(doesGatherResources&&hit.collider.TryGetComponent(out Resource resource)){
                resource.Gather(hit.point,hit.normal);
            }
            if(hit.collider.TryGetComponent(out IDamaged damaged)){
                damaged.TakePhysicalDamage(damage);
            }
        }
    }
}
