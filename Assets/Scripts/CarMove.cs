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
    //int2 values;             // randomValues
    LayerMask mask;
    int notMoveCount;
    NeuralNetwork network;
    public float fitness { get; private set; }

    private int CompareFitness(CarMove c1, CarMove c2)
    {
        if (c1.fitness > c2.fitness)
            return 1;
        if (c1.fitness < c2.fitness)
            return -1;
        return 0;
    }

    public void SetValues(float3 d, float m, float r, int c, int n, LayerMask l, float2 randomValues)
    {
        distances = d;
        moveSpeed = m;
        rotationSpeed = r;
        mask = l;
        network = new NeuralNetwork(c, n);
        network.SetRandomValues(randomValues.x, randomValues.y);
    }

    void Update()
    {
        KeyboardMove();
        Debug.Log("");
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
    }

    bool Move(float m, float r)
    {
        if (float.IsNaN(m))
            return true;

        if (m <= .05f || m > 1)
            notMoveCount++;
        else
            notMoveCount = 0;

        if (notMoveCount >= 10)
            return true;
        Debug.Log(m);
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

    void KeyboardMove()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.Translate(Vector3.forward * moveSpeed);
            if (Input.GetKey(KeyCode.LeftArrow))
                gameObject.transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.eulerAngles.y - rotationSpeed, 0);
            else if (Input.GetKey(KeyCode.RightArrow))
                gameObject.transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.eulerAngles.y + rotationSpeed, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            return;
        gameObject.SetActive(false);
    }
}
