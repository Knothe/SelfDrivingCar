using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class JsonConnect
{
    public JsonConnect()    {    }

    public static void SaveToJson(MatrixSaveData m)
    {
        string data = m.GetJsonData();
        System.IO.File.WriteAllText("Assets/NeuralSaves/NeuralNetwork" + m.capas + "," + m.neuronas + ".json", data);
    }

    public static NeuralNetwork SetFromJson(int c, int n)
    {
        try
        {
            var jsonSData = System.IO.File.ReadAllText("Assets/NeuralSaves/NeuralNetwork" + c + "," + n + ".json");
            char[] chars = { '\r', '\n', ' ', '[', ']', '{', '}', ':', ',' };
            List<string> temp = jsonSData.Split(chars).ToList<string>();
            for (int i = temp.Count - 1; i >= 0; i--)
            {
                if (temp[i] == "")
                    temp.RemoveAt(i);
            }
            temp.RemoveAt(0);
            NeuralNetwork neural = new NeuralNetwork(c, n);
            neural.SetValues(temp);
            return neural;
        }
        catch(DirectoryNotFoundException e)
        {
            return null;
        }
    }

}

public class MatrixSaveData
{
    public Matrix[] values;
    public int capas;
    public int neuronas;

    public MatrixSaveData(Matrix[] m, int c, int n)
    {
        values = m;
        capas = c;
        neuronas = n;
    }

    public string GetJsonData()
    {
        string data = "{";
        //data += " capas : " + capas + ",";
        //data += " neuronas : " + neuronas + ",";
        data += " values : [";

        data += "[";
        foreach(Matrix m in values)
        {
            data += "[";
            for(int i = 0; i < m.x; i++)
                for(int j = 0; j < m.y; j++)
                    data += m.values[i, j].ToString() + ",";

            data = data.Remove(data.Length - 1);
            data += "],";
        }
        data = data.Remove(data.Length - 1);
        data += "]";
        data += "]";
        data += "}";
        return data;
    }

}
