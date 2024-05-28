using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] footstepClips;
    private AudioSource audioSource;
    private Rigidbody rigidbody;
    public float footstepThreshold; //rigidbody에서 움직이고 있는지를 받아와서 사용할 것;
    public float footstepRate;
    private float footStepTime;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(rigidbody.velocity.y)<0.1f){
            if(rigidbody.velocity.magnitude>footstepThreshold){
                if(Time.time-footStepTime>footstepRate){
                    footStepTime=Time.time;
                    audioSource.PlayOneShot(footstepClips[Random.Range(0,footstepClips.Length)]);
                }
            }
        }
    }
}
