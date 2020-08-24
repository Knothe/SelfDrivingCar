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
        temp.values[1, 0] = TanH(temp.values[1, 0]);
        return temp;
    }

    void ApplyFunctionTemp()
    {
        for (int i = 0; i < temp.x; i++)
            for (int j = 0; j < temp.y; j++)
                temp.values[i, j] = TanH(temp.values[i, j]);
    }

    float Sigmoid(float v)
    {
        float k = (float)Math.Exp(v);
        return k / (1 + k);
    }

    float TanH(float f)
    {
        return (float)(Math.Sinh(f) / Math.Cosh(f));
    }

    void MultiplyNeurons(Matrix m, int n)
    {
        if (n == matrices.Length)
            return;
        temp = matrices[n] * m;
        MultiplyNeurons(temp, n + 1);
    }

    public void BreedNeural(NeuralNetwork n1, bool mod)
    {
        float m = UnityEngine.Random.Range(-1, 1);
        int n = neuronas / 10;
        for (int i = 0; i < matrices.Length; i++)
        {
            for(int j = 0; j < matrices[i].x; j++)
            {
                for(int k = 0; k < matrices[i].y; k++)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        matrices[i].values[j, k] = n1.matrices[i].values[j, k];
                    if (mod && UnityEngine.Random.Range(0, neuronas) <= n)
                        matrices[i].values[j, k] += m;
                }
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
