using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;
using System.Linq;

public class BarChart : MonoBehaviour
{

    public Bar barPrefab;
    public int[] inputValues;
    public string[] labels;
    public Color[] colors;

    List<Bar> bars = new List<Bar>();
    float ChartHeight;
    void start()
    {
        ChartHeight = Screen.height + GetComponent<RectTransform>().sizeDelta.y;
        DisplayGraph(inputValues);
    }
    void DisplayGraph(int[] vals)
    {
        int maxValue = vals.Max();

        for (int i = 0; i < vals.Length; i++)
        {
            Bar newBar = Instantiate(barPrefab) as Bar;
            newBar.transform.SetParent(transform);
            RectTransform rt = newBar.bar.GetComponent<RectTransform>();
            float normalizedValue = ((float)vals[i] / (float)maxValue) * 0.95f;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, ChartHeight * normalizedValue);
            newBar.bar.color = colors[i % colors.Length];




            if (labels.Length <= i)
            {
                newBar.label.text = "undefined";

            }
            else
            {
                newBar.label.text = labels[i];
            }
            newBar.barValue.text = vals[i].ToString();
            if (rt.sizeDelta.y < 30f)
            {
                newBar.barValue.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                newBar.barValue.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
        }
    }
}
