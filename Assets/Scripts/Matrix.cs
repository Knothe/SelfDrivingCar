using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Matrix
{
    public float[,] values;
    public int x { get; private set; }
    public  int y { get; private set; }

    public Matrix(int2 size)
    {
        values = new float[size.x, size.y];
        x = size.x;
        y = size.y;
    }

    public Matrix(int xs, int ys)
    {
        values = new float[xs, ys];
        x = xs;
        y = ys;
    }

    public Matrix(float[,] v, int2 size)
    {
        values = v;
        x = size.x;
        y = size.y;
    }

    public static Matrix operator *(Matrix m1, Matrix m2)
    {
        if (m1.y != m2.x)
        {
            Debug.LogError("Cant multiply");
            return null;
        }

        int n = m1.y;
        float[,] temp = new float[m1.x, m2.y];
        float t;
        for (int i = 0; i < m1.x; i++)
        {
            for (int j = 0; j < m2.y; j++)
            {
                t = 0;
                for (int k = 0; k < n; k++)
                {
                    t += m1.values[i, k] * m2.values[k, j];
                }
                temp[i, j] = t;
            }
        }
        return new Matrix(temp, new int2(m1.x, m2.y));
    }

}
