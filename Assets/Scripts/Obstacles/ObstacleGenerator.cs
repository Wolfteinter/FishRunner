using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObstacleGenerator : MonoBehaviour{
    public GameObject prefab;
    public GameObject terrain;
    GameObject background;
    GameObject backgroundAux;
    GameObject camera;
    Boolean flag;
    
    void Start(){
        camera = GameObject.Find("Main Camera");
        background = Instantiate(terrain);
        backgroundAux = Instantiate(terrain);
        background.transform.position = new Vector3(0, -60, 0);
        backgroundAux.transform.position = new Vector3(0, -60, 1400);
        flag = true;
    }
    void Update(){
        if((int)Math.Abs(background.transform.position.z) % 50 == 0){
            if(flag == true){
                GameObject obstacle = Instantiate(prefab);
                flag = false;
            }
        }else{
            flag = true;
        }

        if(background.transform.position.z < camera.transform.position.z - 1400){
            background.transform.position = new Vector3(0, -60, 1400);
        }
        if(backgroundAux.transform.position.z < camera.transform.position.z - 1400){
            backgroundAux.transform.position = new Vector3(0, -60, 1400);
        }
        background.transform.Translate(new Vector3(0,0,-50f*Time.deltaTime), Space.World);
        backgroundAux.transform.Translate(new Vector3(0,0,-50f*Time.deltaTime), Space.World);
        
    }
}
