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

    List<CarMove> carList;
    int generations;
    void Start()
    {
        generations = 0;
        carList = new List<CarMove>();
        startPosition = carPrefab.transform;
        for(int i = 0; i < poblacion; i++)
        {
            GameObject g = GameObject.Instantiate(carPrefab);
            g.SetActive(true);
            carList.Add(g.GetComponent<CarMove>());
            g.GetComponent<CarMove>().SetValues(sensorDistances, velocity, rotationSpeed, capas, neuronas, mask, randomValues);
        }
        if(bestNeurons + worstNeurons >= poblacion)
        {
            bestNeurons = poblacion / 3;
            worstNeurons = poblacion / 4;
        }
    }

    void Update()
    {
        foreach(CarMove c in carList)
        {
            if (c.gameObject.activeInHierarchy)
                return;
        }
        generations++;
        Debug.Log(generations);
        PrepareNextGeneration();
        RestartAllCars();

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
        }
        for(int i = 0; i < worstNeurons; i++)
        {
            parents.Add(carList[carList.Count - (1 + i)]);
            carList[carList.Count - (1 + i)].isParent = true;
        }
        foreach (CarMove c in carList)
        {
            if (c.isParent)
                continue;
            c.SetAsChild(parents[UnityEngine.Random.Range(0, parents.Count)], parents[UnityEngine.Random.Range(0, parents.Count)], UnityEngine.Random.Range(0, 1.0f) <= probMutacion);
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
            if (c.isChecked)
            {
                c.isParent = false;
                continue;
            }
            c.gameObject.SetActive(true);
            c.gameObject.transform.position = startPosition.position;
            c.gameObject.transform.rotation = startPosition.rotation;
            c.RestartValues();
        }
    }

}
