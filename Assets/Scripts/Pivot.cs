using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour{

    const int threadGroupSize = 1024;
    public float spawnRadius = 10;
    public int spawnCount = 30;
    public int windowSizeX = 500;
    public int windowSizeY = 80;

    public Boid prefab;
    public Color colour;
    public GameObject camera;
    protected Joystick joystick;
    //List<Boid> boids;
    public BoidSettings settings;
    public ComputeShader compute;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    Boid[] boids;
    Boid pivot;
    void Start(){
        joystick = FindObjectOfType<Joystick>();
        //boids = new List<Boid>();
        for (int i = 0; i < spawnCount; i++) {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate(prefab);
            boid.transform.Find("PezDorado").gameObject.GetComponent<Renderer>().sharedMaterial = prefab.transform.Find("PezDorado").GetComponent<Renderer>().sharedMaterial;
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;
            boid.SetColour (colour);
            //boids.Add(boid);
        }
        boids = FindObjectsOfType<Boid>();
        foreach (Boid b in boids) {
            b.Initialize(settings, boids[0].transform);
        }
        pivot = boids[0];
    }

    void FixedUpdate(){
        GetInputs();
        UpdatePivotDiraction();
        UpdateBoids();
        UpdateCamera();  

    }
    void UpdatePivotDiraction(){
        pivot.transform.forward = Vector3.left;
        pivot.transform.Rotate(new Vector3(joystick.Horizontal, joystick.Vertical, 0) * Time.deltaTime *10.0f , Space.World);
    }
    void UpdateCamera(){
        Vector3 desiredPosition = pivot.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(camera.transform.position, desiredPosition, smoothSpeed);
        camera.transform.position = smoothedPosition;
        camera.transform.LookAt(camera.transform);
    }
    void GetInputs(){
        if(boids != null){
            //Vector3 newPos = pivot.transform.position;
            pivot.transform.Translate(new Vector3(joystick.Horizontal*Time.deltaTime*300,joystick.Vertical*Time.deltaTime*300,Time.deltaTime*0f), Space.World);
            /*if(pivot.transform.position.x < -windowSizeX 
                || pivot.transform.position.x > windowSizeX 
                || pivot.transform.position.y > windowSizeY 
                || pivot.transform.position.y < -windowSizeY
            ){
                pivot.transform.position = newPos;
            }*/
        }
    }

    void UpdateBoids(){
        if (boids != null) {

            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];
            
            for (int i = 0; i < boids.Length; i++) {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData (boidData);

            compute.SetBuffer (0, "boids", boidBuffer);
            compute.SetInt ("numBoids", boids.Length);
            compute.SetFloat ("viewRadius", settings.perceptionRadius);
            compute.SetFloat ("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt (numBoids / (float) threadGroupSize);
            compute.Dispatch (0, threadGroups, 1, 1);

            boidBuffer.GetData (boidData);
            for (int i = 1; i < boids.Length; i++) {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;
                boids[i].UpdateBoid();
            }
            boidBuffer.Release();
        }
    }
    public struct BoidData {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size {
            get {
                return sizeof (float) * 3 * 5 + sizeof (int);
            }
        }
    }
}
