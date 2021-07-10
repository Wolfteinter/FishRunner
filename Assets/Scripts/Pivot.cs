using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pivot : MonoBehaviour{

    const int threadGroupSize = 1024;
    public float spawnRadius = 10;
    public int spawnCount = 30;
    public float windowSizeX = 450f;
    public float windowSizeYDown = 20f;
    public float windowSizeYUp = 200f;

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
            Vector3 pos = transform.position + UnityEngine.Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate(prefab);
            boid.transform.Find("PezDorado").gameObject.GetComponent<Renderer>().sharedMaterial = prefab.transform.Find("PezDorado").GetComponent<Renderer>().sharedMaterial;
            boid.transform.position = pos;
            boid.transform.forward = UnityEngine.Random.insideUnitSphere;
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
        camera.transform.LookAt(pivot.transform);
    }
    private static float OutsideLimitedMove(float current, float move, float bottomLimit, float topLimit){
        if(current >= -200 && current <= 200) 
        return 0f;
        float predictedY = current + move;
        if(current < bottomLimit && predictedY > bottomLimit) 
            return Math.Min(bottomLimit - current, move);
        if (current > topLimit && predictedY < topLimit)
            return Math.Max(topLimit - current, move);
        return move;
    }
    void GetInputs(){
        if(boids != null){
            /*if(pivot.transform.position.x < -windowSizeX 
                || pivot.transform.position.x > windowSizeX 
                || pivot.transform.position.y > windowSizeY 
                || pivot.transform.position.y < -windowSizeY
            )*/
            Vector3 currentPosition = pivot.transform.position;
            //pivot.transform.Translate(new Vector3(joystick.Horizontal*Time.deltaTime*300,joystick.Vertical*Time.deltaTime*300,Time.deltaTime*0f), Space.World);

            //float x = OutsideLimitedMove(currentPosition.x, joystick.Horizontal*Time.deltaTime*300,-200, 200);
            //float y = OutsideLimitedMove(currentPosition.y, joystick.Vertical*Time.deltaTime*300,-200, 200);
            //Debug.Log("Position" + x.ToString() + y.ToString());
            pivot.transform.position = new Vector3(Mathf.Clamp(pivot.transform.position.x + joystick.Horizontal*Time.deltaTime*300, -windowSizeX, windowSizeX), Mathf.Clamp(pivot.transform.position.y + joystick.Vertical*Time.deltaTime*300, windowSizeYDown, windowSizeYUp), currentPosition.z);
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
