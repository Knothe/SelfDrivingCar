using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NeuralManager : MonoBehaviour
{
    [Header("Población")]
    public int poblacion;
    public float probMutacion;


    [Header("Red neuronal")]
    public int capas;
    public int neuronas;
    public float2 randomValues;

    [Header("Auto")]
    public float velocity;
    public float rotationSpeed;
    public float3 sensorDistances;
    public LayerMask mask;
    public GameObject carPrefab;
    Transform startPosition;

    List<CarMove> carList;

    void Start()
    {
        carList = new List<CarMove>();
        startPosition = carPrefab.transform;
        for(int i = 0; i < poblacion; i++)
        {
            GameObject g = GameObject.Instantiate(carPrefab);
            carList.Add(g.GetComponent<CarMove>());
            carList[carList.Count - 1].SetValues(sensorDistances, velocity, rotationSpeed, capas, neuronas, mask, randomValues);
        }
    }

    void Update()
    {
        foreach(CarMove c in carList)
        {
            if (c.gameObject.activeInHierarchy)
                return;
        }
        Debug.Log("All Finished");
        

    }

    void PrepareNextGeneration()
    {

    }

    void RestartAllCars()
    {
        foreach (CarMove c in carList)
        {
            c.gameObject.SetActive(true);
            c.gameObject.transform.position = startPosition.position;
            c.gameObject.transform.rotation = startPosition.rotation;
        }
    }

}
