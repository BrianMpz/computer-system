using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryEditor : MonoBehaviour
{
    public GameObject memoryEntryPrefab;  // Drag your prefab here in the Inspector
    public Transform contentParent;       // Reference to the Content object in the Scroll View
    public CentralProcessingUnit cpu;     // Reference to your CPU to access memory
    public int memorySize;          // Define the size of the memory array

    void Start()
    {
        PopulateMemory();
    }

    // Populate the scroll view with memory entries
    void PopulateMemory()
    {
        memorySize = cpu.memory.Memory.Length;
        for (int i = 0; i < memorySize; i++)
        {
            // Instantiate a new MemoryEntry prefab
            GameObject newEntry = Instantiate(memoryEntryPrefab, contentParent);

            // Find and set the address and value fields
            TMP_Text addressText = newEntry.transform.Find("MemoryAddress").GetComponent<TMP_Text>();
            TMP_InputField valueInput1 = newEntry.transform.Find("MemoryData1").GetComponent<TMP_InputField>();
            TMP_InputField valueInput2 = newEntry.transform.Find("MemoryData2").GetComponent<TMP_InputField>();

            // Set the memory address
            addressText.text = i.ToString();

            // Set the initial values for data1 and data2 from CPU memory
            Data memoryData = cpu.memory.Memory[i];
            
            valueInput1.text = memoryData.data1.ToString();  // data1
            valueInput2.text = memoryData.data2.ToString();  // data2

            // Add listeners to detect changes in the input fields and update memory
            int currentIndex = i; // Store the current index in a local variable for the lambda closure

            valueInput1.onEndEdit.AddListener((string newValue) => UpdateMemoryData1(currentIndex, newValue));
            valueInput2.onEndEdit.AddListener((string newValue) => UpdateMemoryData2(currentIndex, newValue));
        }
    }

    // This function updates data1 when the input field is changed
    void UpdateMemoryData1(int address, string newValue)
    {
        if (int.TryParse(newValue, out int parsedValue))
        {
            // Update only data1
            Data currentData = cpu.memory.Memory[address];
            cpu.memory.Write(address, new Data(parsedValue, currentData.data2));
        }
        else
        {
            Debug.LogError("Invalid input: Please enter a valid integer for data1.");
            Data currentData = cpu.memory.Memory[address];
            cpu.memory.Write(address, new Data(0, currentData.data2));
        }
    }

    // This function updates data2 when the input field is changed
    void UpdateMemoryData2(int address, string newValue)
    {
        if (int.TryParse(newValue, out int parsedValue))
        {
            // Update only data2
            Data currentData = cpu.memory.Memory[address];
            cpu.memory.Write(address, new Data(currentData.data1, parsedValue));
        }
        else
        {
            Debug.LogError("Invalid input: Please enter a valid integer for data2.");
        }
    }
}
