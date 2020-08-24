using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NeuralManager : MonoBehaviour
{
    [Header("Población")]
    public int poblacion;
    [Range(0, 1)]
    public float probMutacion;


    [Header("Red neuronal")]
    public int capas;
    public int neuronas;
    public float2 randomValues;

    [Header("Genetic")]
    public int bestNeurons;
    public int worstNeurons;

    [Header("Auto")]
    public float velocity;
    public float rotationSpeed;
    public float3 sensorDistances;
    public LayerMask mask;
    public GameObject carPrefab;
    Transform startPosition;
    public Color childColor;
    public Color parentColor;

    List<CarMove> carList;
    List<GameObject> cameras;
    int currentCamera;

    int generations;
    void Start()
    {
        currentCamera = 0;
        generations = 0;
        carList = new List<CarMove>();
        cameras = new List<GameObject>();
        startPosition = carPrefab.transform;
        CarMove temp;
        for(int i = 0; i < poblacion; i++)
        {
            GameObject g = GameObject.Instantiate(carPrefab);
            g.name = "Car(" + i + ")";
            g.SetActive(true);
            temp = g.GetComponent<CarMove>();
            carList.Add(temp);
            temp.SetValues(sensorDistances, velocity, rotationSpeed, capas, neuronas, mask, randomValues);
            g.GetComponent<Renderer>().material.color = childColor;
            cameras.Add(temp.cam);
        }
        if(bestNeurons + worstNeurons >= poblacion)
        {
            bestNeurons = poblacion / 3;
            worstNeurons = poblacion / 4;
        }
        cameras[currentCamera].SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeCamera(1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeCamera(-1);

        foreach (CarMove c in carList)
        {
            if (c.gameObject.activeInHierarchy)
                return;
        }
        NextGeneration();
    }

    void ChangeCamera(int c)
    {
        for(int i = currentCamera + c, t = 0; t < cameras.Count; i += c, t++)
        {
            if (i == cameras.Count)
                i = 0;
            else if (i < 0)
                i = cameras.Count - 1;

            if (cameras[i].transform.parent.gameObject.activeInHierarchy)
            {
                cameras[i].SetActive(true);
                cameras[currentCamera].SetActive(false);
                currentCamera = i;
                return;
            }
        }
    }

    void PrepareNextGeneration()
    {
        foreach (CarMove c in carList)
            c.CalculateFitness();

        carList.Sort(CompareFitness);
        List<CarMove> parents = new List<CarMove>();
        
        for(int i = 0; i < bestNeurons; i++)
        {
            parents.Add(carList[i]);
            carList[i].isParent = true;
            carList[i].gameObject.GetComponent<Renderer>().material.color = parentColor;
        }
        for(int i = 0; i < worstNeurons; i++)
        {
            parents.Add(carList[carList.Count - (1 + i)]);
            carList[carList.Count - (1 + i)].isParent = true;
            carList[carList.Count - (1 + i)].gameObject.GetComponent<Renderer>().material.color = parentColor;
        }
        foreach (CarMove c in carList)
        {
            if (c.isParent)
                continue;
            c.SetAsChild(parents[UnityEngine.Random.Range(0, parents.Count)], parents[UnityEngine.Random.Range(0, parents.Count)], UnityEngine.Random.Range(0, 1.0f) <= probMutacion);
            c.gameObject.GetComponent<Renderer>().material.color = childColor;
        }

    }
    private int CompareFitness(CarMove c1, CarMove c2)
    {
        if (c1.fitness < c2.fitness)
            return 1;
        if (c1.fitness > c2.fitness)
            return -1;
        return 0;
    }

    void RestartAllCars()
    {
        foreach (CarMove c in carList)
        {
            c.RestartValues();
            c.gameObject.SetActive(true);
            c.gameObject.transform.position = startPosition.position;
            c.gameObject.transform.rotation = startPosition.rotation;
            c.isParent = false;
        }
    }

    public void NextGeneration()
    {
        foreach (CarMove c in carList)
            c.gameObject.SetActive(false);
        generations++;
        Debug.Log(generations);
        PrepareNextGeneration();
        RestartAllCars();
    }

}
