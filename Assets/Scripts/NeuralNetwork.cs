using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    int capas;
    int neuronas;

    //List<Matrix> matrices;
    Matrix[] matrices;
    Matrix temp;

    public NeuralNetwork(int c, int n) // capas y neuronas
    {
        //matrices = new List<Matrix>();
        neuronas = n;
        capas = c;
        matrices = new Matrix[c + 1];
        int prev = 3;
        int cur;
        for (int i = 0; i < c + 1; i++)
        {
            if (i == c)
                cur = 2;
            else if (i == c - 1)
                cur = (n / c) + n % c;
            else
                cur = n / c;

            matrices[i] = new Matrix(cur, prev);
            prev = cur;
        }
    }

    public void SetRandomValues(float min, float max)
    {
        foreach (Matrix m in matrices)
        {
            for (int i = 0; i < m.x; i++)
            {
                for (int j = 0; j < m.y; j++)
                {
                    m.values[i, j] = UnityEngine.Random.Range(min, max);
                }
            }
        }
    }

    public void SetRandomValues(int min, int max)
    {
        foreach (Matrix m in matrices)
        {
            for (int i = 0; i < m.x; i++)
            {
                for (int j = 0; j < m.y; j++)
                {
                    m.values[i, j] = UnityEngine.Random.Range(min, max);
                }
            }
        }
    }

    public Matrix GetAction(float f, float l, float r)
    {
        temp = new Matrix(3, 1);
        temp.values[0, 0] = f;
        temp.values[1, 0] = l;
        temp.values[2, 0] = r;

        for (int i = 0; i < matrices.Length; i++)
        {
            temp = matrices[i] * temp;
            if(i != matrices.Length - 1)
                ApplyFunctionTemp();
        }
        temp.values[0, 0] = Sigmoid(temp.values[0,0]);
        temp.values[1, 0] = (float)Math.Tanh(temp.values[1,0]);
        return temp;
    }

    void ApplyFunctionTemp()
    {
        for (int i = 0; i < temp.x; i++)
            for (int j = 0; j < temp.y; j++)
                temp.values[i, j] = (float)Math.Tanh(temp.values[i, j]);
    }

    float Sigmoid(float v)
    {
        float k = 1.0f / (1.0f + (float)Math.Exp(-v));
        if (float.IsInfinity(k))
            return 1;
        return k;
    }

    void MultiplyNeurons(Matrix m, int n)
    {
        if (n == matrices.Length)
            return;
        temp = matrices[n] * m;
        MultiplyNeurons(temp, n + 1);
    }

    public void BreedNeural(NeuralNetwork n1, NeuralNetwork n2)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            for(int j = 0; j < matrices[i].x; j++)
            {
                for(int k = 0; k < matrices[i].y; k++)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        matrices[i].values[j, k] = n1.matrices[i].values[j, k];
                    else
                        matrices[i].values[j, k] = n2.matrices[i].values[j, k];
                }
            }
        }
    }

    public void Mutate()
    {
        int nPoints, rColumn, rRow;
        for(int i = 0; i < matrices.Length; i++)
        {
            nPoints = UnityEngine.Random.Range(1, (matrices[i].x * matrices[i].y) / 5);
            for(int j = 0; j < nPoints; j++)
            {
                rColumn = UnityEngine.Random.Range(0, matrices[i].y);
                rRow = UnityEngine.Random.Range(0, matrices[i].x);
                matrices[i].values[rRow, rColumn] = Mathf.Clamp(matrices[i].values[rRow, rColumn], -1, 1);
            }

        }
    }

    public void Print()
    {
        string s = "";
        for (int i = 0; i < temp.x; i++)
        {
            for (int j = 0; j < temp.y; j++)
            {
                s += temp.values[i, j].ToString() +", ";
            }
        }
        Debug.Log(s);
    }
}
