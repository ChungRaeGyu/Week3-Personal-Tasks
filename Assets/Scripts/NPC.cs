using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState{
    Idle,
    Wandering,
    Attacking
}
public class NPC : MonoBehaviour, IDamaged
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    private AIState aiState;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;

    public float fieldOfView = 120f; //시야각

    private Animator animator;
    private SkinnedMeshRenderer[] meshRendereres; //몬스터가 가지고 있는 각종 매쉬들의 정보

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();    
        animator = GetComponent<Animator>();
        meshRendereres = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Start(){
        SetState(AIState.Wandering);
    }
    void Update()
    {
        playerDistance = Vector3.Distance(transform.position,CharacterManager.Instance.Player.transform.position);

        animator.SetBool("Moving",aiState != AIState.Idle);

        switch(aiState){
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }

    public void  SetState(AIState state){
        aiState = state;

        switch(aiState){
            case AIState.Idle : 
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
        }
        animator.speed = agent.speed/walkSpeed;
    }

    void PassiveUpdate(){
        if(aiState == AIState.Wandering && agent.remainingDistance <0.1f){
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation",Random.Range(minWanderWaitTime,maxWanderWaitTime));
        }

        if(playerDistance < detectDistance){
            SetState(AIState.Attacking);
        }
    }
    void WanderToNewLocation(){
        if(aiState != AIState.Idle) return;

        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }      

    Vector3 GetWanderLocation(){
        NavMeshHit hit;
        //onUnitSphere : 반지름이 1인 구
        //SamplePosition : 선택한 범위 안에서 이동할 수 있는 경로 중 최단 경로를 hit로 반환한다. , 최고거리
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere*Random.Range(minWanderDistance,maxWanderDistance)), 
        out hit,maxWanderDistance,NavMesh.AllAreas);

        int i=0;
        while(Vector3.Distance(transform.position,hit.position)<detectDistance){
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)),
            out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if(i==30)break;
        }
        return hit.position;
    }
    void AttackingUpdate(){
        if(playerDistance < attackDistance && IsPlayerInFieldOfView()){
            agent.isStopped = true;
            if(Time.time - lastAttackTime>attackRate){
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.controller.GetComponent<IDamaged>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
        else{
            if(playerDistance < detectDistance){
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                if(agent.CalculatePath(CharacterManager.Instance.Player.transform.position,path)){
                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else{
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            else{
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    bool IsPlayerInFieldOfView(){
        //내위치에서 플레이어까지의 방향
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward,directionToPlayer);
        return angle<fieldOfView*0.5; //*0.5를 하는 이유는 내 시야에서 왼쪽으로 반 오른쪽으로 반씩 가서 총 각도가 나오기 때문
    }

    public void TakePhysicalDamage(int damage)
    {
        health -=damage;
        if(health <= 0){
            Die();
        }
        //데미지 효과
        StartCoroutine(DamageFlash());
    }

    void Die(){
        for(int  i=0; i<dropOnDeath.Length;i++){
            Instantiate(dropOnDeath[i].dropPrefab, transform.position + Vector3.up*2, Quaternion.identity);
        }
        Destroy(gameObject);

    }


    IEnumerator DamageFlash(){
        for(int i=0; i<meshRendereres.Length;i++){
            meshRendereres[i].material.color = new Color(1.0f,0.6f,0.6f);
        }

        yield return new WaitForSeconds(0.1f);

        for(int i=0; i<=meshRendereres.Length;i++){
            meshRendereres[i].material.color = Color.white;
        }
    }
}
