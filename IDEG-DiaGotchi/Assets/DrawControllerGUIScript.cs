using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class DrawControllerGUIScript : MonoBehaviour
{
    private static readonly int BorderWidth = 10;

    private List<double> PastIG = new List<double>();
    private static readonly int MaxIGRecords = 10;

    private Color NormoglycaemiaStripColor = new Color(0.6f, 1.0f, 0.6f, 1.0f);

    Texture2D outTexture;

    public void PushIG(double value)
    {
        PastIG.Add(value);
        while (PastIG.Count > MaxIGRecords)
            PastIG.RemoveAt(0);

        Redraw();
    }

    public void DrawLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col)
    {
        Vector2 t = p1;
        float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
        {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            tex.SetPixel((int)t.x, (int)t.y, col);
        }
    }

    public void DrawDot(Texture2D tex, Vector2 p, int size, Color col)
    {
        for (int y = (int)p.y - size; y < (int)p.y + size; y++)
        {
            for (int x = (int)p.x - size; x < (int)p.x + size; x++)
                tex.SetPixel(x, y, col);
        }
    }

    public void DrawRect(Texture2D tex, Vector2 p, int width, int height, Color col)
    {
        for (int y = (int)p.y; y < (int)p.y + height; y++)
        {
            for (int x = (int)p.x; x < (int)p.x + width; x++)
                tex.SetPixel(x, y, col);
        }
    }

    void Redraw()
    {
        var dispRect = GetComponent<RectTransform>().rect;

        if (outTexture == null)
            outTexture = new Texture2D((int)dispRect.width, (int)dispRect.height);
        GetComponent<Image>().material.mainTexture = outTexture;

        // clear
        for (int y = 0; y < outTexture.height; y++)
            for (int x = 0; x < outTexture.width; x++)
                outTexture.SetPixel(x, y, Color.white);

        for (int y = 0; y < outTexture.height; y++)
        {
            for (int x = 0; x < BorderWidth; x++)
            {
                outTexture.SetPixel(x, y, Color.black);
                outTexture.SetPixel(outTexture.width - x - 1, y, Color.black);
            }
        }
        for (int y = 0; y < BorderWidth; y++)
        {
            for (int x = BorderWidth; x < outTexture.width - BorderWidth; x++)
            {
                outTexture.SetPixel(x, y, Color.black);
                outTexture.SetPixel(x, outTexture.height - y - 1, Color.black);
            }
        }

        float startX = 20;
        float endX = dispRect.width - 20;
        float sizeX = endX - startX;

        float startY = 60;
        float endY = dispRect.height - 120;
        float sizeY = endY - startY;

        DrawLine(outTexture, new Vector2(startX - 1, startY), new Vector2(startX - 1, endY), Color.blue);
        DrawLine(outTexture, new Vector2(startX - 1, startY), new Vector2(endX + 1, startY), Color.red);

        float minVal = 3.0f;
        float maxVal = 15.0f;
        // TODO: find real min/max?

        float normYStart = startY + sizeY * ((3.5f - minVal) / (maxVal - minVal));
        float normYEnd = startY + sizeY * ((10.0f - minVal) / (maxVal - minVal));

        DrawRect(outTexture, new Vector2(startX, normYStart), (int)sizeX, (int)(-normYStart + normYEnd), NormoglycaemiaStripColor);

        float lastX = startX;
        float lastY = 0;

        float posCounter = startX;
        float posOffset = sizeX / (float)MaxIGRecords;
        foreach (double ig in PastIG)
        {
            float fig = (float)ig;

            float y = startY + sizeY * ((fig - minVal) / (maxVal - minVal));

            DrawDot(outTexture, new Vector2(posCounter, y), 2, Color.blue);

            if (posCounter != startX)
                DrawLine(outTexture, new Vector2(lastX, lastY), new Vector2(posCounter, y), Color.blue);

            lastX = posCounter;
            lastY = y;

            posCounter += posOffset;
        }

        outTexture.Apply();
    }

    void Start()
    {
        Redraw();
    }
}
