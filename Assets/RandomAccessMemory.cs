using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAccessMemory : MonoBehaviour
{
    public Data[] Memory = new Data[256]; // address = index
    public Data Read(int address)
    {
        return Memory[address];
    }
    public void Write(int address, Data bytes4)
    {
        Memory[address] = bytes4; 
    }
}

[Serializable]
public class Data // 32 bit
{
    public int data1 = 0;
    public int data2 = 0;
    public Data(int d1 = 0, int d2 = 0)
    {
        data1 = d1;
        data2 = d2;
    }
    public int value()
    {
        return (data1 << 16) + data2;
    }
}