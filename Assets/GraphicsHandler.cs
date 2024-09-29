using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsHandler : MonoBehaviour
{
    public CentralProcessingUnit cpu;
    public RandomAccessMemory memory;
    public int width = 32; // Width of the monitor (number of pixels)
    public int height = 32; // Height of the monitor
    public Texture2D displayTexture; // The texture to represent the pixels
    public Color[] pixelData; // Store pixel colors
    public int startAddress;

    public void Boot(int startingAddress)
    {
        startAddress = startingAddress;
        displayTexture = new Texture2D(width, height);
        pixelData = new Color[width * height];
        GetComponent<Renderer>().material.mainTexture = displayTexture;

        StartCoroutine(UpdateMonitor());
    }

    IEnumerator UpdateMonitor()
    {
        while(cpu.endProcess == false)
        {
            for (int i = 0; i < pixelData.Length; i++)
            {
                if(memory.Memory[i + startAddress].data2 > 0)
                {
                    pixelData[i] = Color.white;
                }
                else
                {
                    pixelData[i] = Color.black;
                }
            }
            displayTexture.SetPixels(pixelData);
            displayTexture.Apply();
            yield return null;
        }
    }
}

