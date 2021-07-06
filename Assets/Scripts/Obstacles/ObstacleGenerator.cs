using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour{
    public GameObject prefab;
    public GameObject terrain;
    GameObject obstacle;
    GameObject background;
    GameObject backgroundAux;
    GameObject camera;
    void Start(){
        camera = GameObject.Find("Main Camera");
        //obstacle = Instantiate(prefab);
        background = Instantiate(terrain);
        backgroundAux = Instantiate(terrain);
        //obstacle.transform.position = new Vector3(0, 100, 100);
        background.transform.position = new Vector3(0, -60, 0);
        backgroundAux.transform.position = new Vector3(0, -60, 1400);
    }

    // Update is called once per frame
    void Update(){
        if(background.transform.position.z < camera.transform.position.z - 1400){
            background.transform.position = new Vector3(0, -60, 1400);
        }
        if(backgroundAux.transform.position.z < camera.transform.position.z - 1400){
            backgroundAux.transform.position = new Vector3(0, -60, 1400);
        }
        //obstacle.transform.Translate(new Vector3(0,0,-50f*Time.deltaTime), Space.World);
        background.transform.Translate(new Vector3(0,0,-50f*Time.deltaTime), Space.World);
        backgroundAux.transform.Translate(new Vector3(0,0,-50f*Time.deltaTime), Space.World);
        
    }
}
