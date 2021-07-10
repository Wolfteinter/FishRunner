using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour{
    GameObject prefab;
    Vector3 position;

    void Start(){
        transform.position = new Vector3(Random.Range(-350.0f, 350.0f), 1000, 600);

        transform.localScale += new Vector3(Random.Range(1,3),Random.Range(1,3),Random.Range(1,3));
    }
    void Update(){
        if(transform.position.y < -100){
            foreach (Transform child in transform) {
                GameObject.Destroy(child.gameObject);
            }
            GameObject.Destroy(this.gameObject);
        }
        transform.Translate(new Vector3(0,Random.Range(-100.0f, -80.0f)*Time.deltaTime,-50f*Time.deltaTime), Space.World);
    }
    
}
