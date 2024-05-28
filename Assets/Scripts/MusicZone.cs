using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeTime;
    public float maxVolume;
    public float targetVolume;

    // Start is called before the first frame update
    void Start()
    {
        targetVolume =0;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = targetVolume;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Mathf.Approximately(audioSource.volume,targetVolume)){ //근사값 구하기 1.00001, 1.0000을 같은값으로 인식
            //근사값이 아닐때
            audioSource.volume = Mathf.MoveTowards(audioSource.volume,targetVolume,(maxVolume/fadeTime)*Time.deltaTime);//점진적으로 늘어나게 한다.
        }
    }
    private void OnTriggerEnter(Collider other) {
        
        if(other.gameObject.CompareTag("Player")){
            targetVolume = maxVolume;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            targetVolume = 0;
        }
    }
}
