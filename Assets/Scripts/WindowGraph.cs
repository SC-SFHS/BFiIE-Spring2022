using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    public static bool showData; //for testing purposes
    public TMPro.TMP_Text myText;
    public double RMSSD1;
    public double RMSSD2;
    public double RMSSD3;
    public double RMSSD4;
    public double RMSSD5;


    public static bool resetGraph;


    private void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("XTemplate").GetComponent<RectTransform>();
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(1, 1); //DOT SIZE
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private void showGraph(List<Double> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        //float graphHeight = 1000; //figure out dynamics
        float yMaximum = 140f; //highest HR
        //float xSize = ((graphContainer.sizeDelta.x) / (float) valueList.Count) - graphContainer.sizeDelta.x; //learn to make dynamic based on size of list and dimension chosen
        float xSize = ((float)graphContainer.sizeDelta.x / (valueList.Count - 1));
        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = i * xSize; //will be in two lists instead...
            float yPosition = (float)(valueList[i] / yMaximum) * graphHeight; //normalize Value
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            if (lastCircleGameObject != null)
            {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
        }
        //hard put this
        int seperatorCount = 10;
        for (int i = 0; i <= seperatorCount; i++)
        {
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.transform.SetParent(graphContainer, false);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            float normalizedValue = i * 1f / seperatorCount;
            labelX.anchoredPosition = new Vector2(-95f, normalizedValue * graphHeight);
            labelX.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();
            labelX.transform.localScale = new Vector3(1, 1, 1);
        }



    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1f, 1f, 0.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 6f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;

        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        rectTransform.localEulerAngles = new Vector3(0, 0, n);

    }

    public double CalculateRMSDD(List<int> ListOfRMSDD)
    {
        double returnRMSSD = 0;
        for (int i = 1; i < ListOfRMSDD.Count; i++)
        {
            double currSum = Math.Pow(ListOfRMSDD[i - 1] - ListOfRMSDD[i], 2);
            returnRMSSD += currSum;
            Debug.Log(returnRMSSD + "boom");
        }
        returnRMSSD = Math.Sqrt(returnRMSSD);
        return returnRMSSD;
    }

    // Start is called before the first frame update
    void Start()
    {
        //List<int> RMSSDList1 = new List<int>() { 744, 427, 498, 931};
        RMSSD1 = CalculateRMSDD(UDPListener.IBIList1);
        RMSSD2 = CalculateRMSDD(UDPListener.IBIList2);
        RMSSD3 = CalculateRMSDD(UDPListener.IBIList3);
        RMSSD4 = CalculateRMSDD(UDPListener.IBIList4);
        RMSSD5 = CalculateRMSDD(UDPListener.IBIList5);
        showData = true;
        resetGraph = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (showData)
        {
            //List<Double> valueList = new List<Double>() { 50, 70, 68, 102, 100, 82, 100, 130, 120, 118, 117, 85, 78 };
            if(UDPListener.heartRateList.Count <  4)
            {
                //for testing purposes when we don't have HR thing
                List<Double> valueList = new List<Double>();
                System.Random rnd = new System.Random();
                for(int i = 0; i<50; i++)
                {
                    valueList.Add(rnd.NextDouble() * 60);
                    RMSSD1 = rnd.NextDouble() * 80;
                    RMSSD2 = rnd.NextDouble() * 300;
                    RMSSD3 = rnd.NextDouble() * 100;
                    RMSSD4 = rnd.NextDouble() * 100;
                }
                showGraph(valueList);
                myText.text = "Your RMSDD Score (Estimated due to sensor problem)\nBiome 1: " + RMSSD1 + "\nBiome 2: " + RMSSD2 + "\nBiome 3: " + RMSSD3 + "\nBiome 4: " + RMSSD4;
            } else
            {
                showGraph(UDPListener.heartRateList);
                myText.text = "Your RMSDD Scores\nBiome 1: " + RMSSD1 + "\nBiome 2: " + RMSSD2 + "\nBiome 3: " + RMSSD3 + "\nBiome 4: " + RMSSD4;
            }
            showData = false;
            
        }

        if (resetGraph)
        {
            foreach (Transform child in graphContainer.transform)

            {
                if (child.name == "dotConnection" || child.name == "circle")
                {

                    Destroy(child.gameObject);
                }
                if (child.name == "XTemplate(Clone)")
                {
                    Destroy(child.gameObject);
                }

            }
            myText.text = "Your RMSDD Scores\nBiome 1: " + RMSSD1 + "\nBiome 2: " + 0 + "\nBiome 3: " + 0 + "\nBiome 4: " + 0;
        }
    }
}