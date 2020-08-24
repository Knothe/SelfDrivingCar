using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CarMove : MonoBehaviour
{
    float3 distances;        // sensorDistances
    float moveSpeed;         // velocity
    float rotationSpeed;     // rotationSpeed
    LayerMask mask;
    public NeuralNetwork network;
    public float fitness { get; private set; }
    public bool isChecked { get; private set; }
    public bool isParent;
    float distanceMoved;
    int totalFrameCount;
    float totalTime;        // totalFrameCount / 120

    

    public void SetValues(float3 d, float m, float r, int c, int n, LayerMask l, float2 randomValues)
    {
        distances = d;
        moveSpeed = m;
        rotationSpeed = r;
        mask = l;
        network = new NeuralNetwork(c, n);
        network.SetRandomValues(randomValues.x, randomValues.y);
        isChecked = false;
        RestartValues();
    }

    void Update()
    {
        
        GetRayValues();
    }

    void GetRayValues()
    {
        float f, r, l;
        f = Sigmoid(CheckRay(Vector3.forward), distances.y);
        r = Sigmoid(CheckRay(new Vector3(1, 0, 1)), distances.z);
        l = Sigmoid(CheckRay(new Vector3(-1, 0, 1)), distances.x);
        Matrix control = network.GetAction(f, l, r);
        if (Move(control.values[0, 0], control.values[1, 0]))
            gameObject.SetActive(false);
        else
        {
            distanceMoved += control.values[0, 0];
            totalFrameCount++;
            totalTime = totalFrameCount / 60.0f;
            if(totalTime >= 10)
            {
                if (totalTime > 100)
                    Debug.Log("Large");
                if(distanceMoved / totalTime <= 5)
                    gameObject.SetActive(false);
            }

        }
    }

    public void CalculateFitness()
    {
        fitness = distanceMoved;
        isChecked = true;
    }

    public void RestartValues()
    {
        totalFrameCount = 0;
        distanceMoved = 0;
        totalTime = 0;
        isParent = false;
    }

    bool Move(float m, float r)
    {
        if (float.IsNaN(m))
            return true;

        gameObject.transform.Translate(Vector3.forward * moveSpeed * m);
        if (r >= -1 && r <= 1)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.eulerAngles.y + rotationSpeed * r, 0);
        }

        return false;
    }

    float Sigmoid(float v, float add)
    {
        v -= (3 + add);
        float k = (float)Math.Exp(v);
        return k / (1 + k);
    }

    float CheckRay(Vector3 v)
    {
        float t = float.MaxValue;
        RaycastHit hit;
        Color c = Color.red;
        if (Physics.Raycast(transform.position, transform.TransformDirection(v), out hit, Mathf.Infinity, mask))
        {
            c = Color.green;
            t = hit.distance;
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(v) * 10, c);
        return t;
    }

    public void SetAsChild(CarMove c1, CarMove c2, bool b) 
    {
        isChecked = false;
        network = c1.network;
        if(c1 == c2 && !b)
            return;
        network.BreedNeural(c2.network, b);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            return;
        gameObject.SetActive(false);
    }
}
