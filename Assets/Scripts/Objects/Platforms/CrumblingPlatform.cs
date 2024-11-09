using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    
    private float timer = 0f;
    public float reapearSpeed = 1.6f;
    public float waitTime = 1.2f;
    public bool hiddenPlat;
    public bool onPlat = false;
    public bool startPlat = false;
    public Animator animatorC;

    // Start is called before the first frame update
    void Start()
    {
        hiddenPlat = false;
        onPlat = false;
        animatorC = this.gameObject.GetComponent<Animator>();
    }

     private void OnCollisionStay2D(Collision2D other) {

        if(other.gameObject.name == "Player"){
            onPlat = true;
            StartPlat();
        
        }
        
        }

        private void OnCollisionExit2D(Collision2D other) {

        if(other.gameObject.name == "Player")
        {
              onPlat = false;
            }
        }
    void StartPlat(){
        startPlat = true;
    }

    // Update is called once per frame
    void Update()
    {
        
                    if(!hiddenPlat && startPlat){
                        timer = timer + Time.deltaTime;
                        animatorC.SetBool("Tremble", true);
                        CheckforWait(timer);
                        return;
                    }

                    if(hiddenPlat){
                        startPlat = false;
                        timer = timer + Time.deltaTime;
                        CheckforWait(timer);
                        return;
                    }

    }

     public void CheckforWait(float timerRuning){

                if(timerRuning >= reapearSpeed){
                    
                    if (hiddenPlat){
                        timer = 0;
                        animatorC.SetBool("Tremble", false);
                        animatorC.SetBool("Crumble", false);
                        hiddenPlat = false;
                    }
        }

        
        if(timerRuning >= waitTime && animatorC.GetBool("Tremble")){

                 if(!hiddenPlat){
                        timer = 0;
                        animatorC.SetBool("Crumble", true);
                        hiddenPlat = true;
                    }

        }

    }

}
