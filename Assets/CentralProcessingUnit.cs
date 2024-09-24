using System;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class CentralProcessingUnit : MonoBehaviour
{
    public int programCounter;
    public int memoryAddressRegister;
    public Data memoryDataRegister;
    public Data currentInstructionRegister;
    public int accumulator;
    public RandomAccessMemory memory;
    public GraphicsHandler graphics;

    public enum DecodeUnit_Instructions
    {
        HLT = 0,
        ADD = 1,
        SUB = 2,
        STA = 3,
        LDA = 4,
        OUT = 5,
        INP = 6,
        JMP = 7,
        BRA = 8,
        BRZ = 9,
        BRP = 10,
        MUL = 11,
        DIV = 12,
        WIT = 13,
        GRAB = 14,
    }

    public DecodeUnit_Instructions decodedInstruction;
    public bool endProcess;
    public bool debug;
    public bool frameStep;
    public int cycles;

    private int operand; // Cache operand for quick access

    public TMP_Text acc;

    private void OnDisable() {
        StopAllCoroutines();
    }
    public void Run()
    {
        StopAllCoroutines();
        programCounter = 0;
        cycles = 0;
        endProcess = false;
        StartCoroutine(ControlUnit());
    }
    IEnumerator ControlUnit()
    {
        acc.text = $"Accumulator: {accumulator}";
        Stopwatch stopwatch = new Stopwatch();  // Start stopwatch to measure time
        stopwatch.Start();

        while (!endProcess)
        {
            cycles++;  // Increment cycle counter
            Fetch();   // Fetch the next instruction
            Decode();  // Decode the instruction
            Execute(); // Execute the instruction
            CheckForInterrupts(); // Check for any interrupts
            if (frameStep) yield return new WaitForSeconds(0.02f);
        }

        stopwatch.Stop();
        double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        double mhz = cycles / elapsedSeconds / 1000000d;  // Convert cycles per second to MHz
        double secondsPerCycle = elapsedSeconds / cycles; // Calculate seconds per cycle

        print($"Execution Time: {elapsedSeconds} seconds");
        print($"{mhz} MHz");
        print($"Seconds per cycle: {secondsPerCycle} seconds");

        acc.text = $"Accumulator: {accumulator}";

    }

    private void Fetch()
    {
        // Point to data
        memoryAddressRegister = programCounter;
        memoryDataRegister = memory.Read(memoryAddressRegister);
        programCounter++;
        if (programCounter < 0 || programCounter >= memory.Memory.Length)
            throw new IndexOutOfRangeException("Program counter out of bounds");
    }

    private void Decode()
    {
        currentInstructionRegister = memoryDataRegister;
        int opcode = currentInstructionRegister.data1;
        decodedInstruction = (DecodeUnit_Instructions)opcode;
        operand = currentInstructionRegister.data2; // Cache operand
    }

    private void Execute()
    {
        if (debug) print($"{decodedInstruction} {operand}");

        switch (decodedInstruction)
        {
            case DecodeUnit_Instructions.HLT:
                endProcess = true;
                break;

            case DecodeUnit_Instructions.ADD:
                PerformALUOperation(ArithmeticLogicUnit.Operation.Add);
                break;

            case DecodeUnit_Instructions.SUB:
                PerformALUOperation(ArithmeticLogicUnit.Operation.Subtract);
                break;

            case DecodeUnit_Instructions.LDA:
                accumulator = memory.Read(operand).value();  // Load directly to accumulator
                break;

            case DecodeUnit_Instructions.STA:
                (int a, int b) = ToBytes(accumulator);
                memory.Write(operand, new Data(a, b));  // Store accumulator value to memory
                break;

            case DecodeUnit_Instructions.OUT:
                print($"Output: {accumulator}");
                break;

            case DecodeUnit_Instructions.BRA:
                programCounter = operand;
                break;

            case DecodeUnit_Instructions.BRZ:
                if (accumulator == 0) programCounter = operand;
                break;

            case DecodeUnit_Instructions.BRP:
                if (accumulator >= 0) programCounter = operand;
                break;

            case DecodeUnit_Instructions.MUL:
                PerformALUOperation(ArithmeticLogicUnit.Operation.Multiply);
                break;

            case DecodeUnit_Instructions.DIV:
                PerformALUOperation(ArithmeticLogicUnit.Operation.Divide);
                break;

            case DecodeUnit_Instructions.GRAB:
                graphics.Boot(operand);
                break;
        }
    }

    private void PerformALUOperation(ArithmeticLogicUnit.Operation operation)
    {
        int value = memory.Read(operand).value();
        accumulator = ArithmeticLogicUnit.PerformOperation(accumulator, value, operation);  // Perform operation in ALU
    }

    private void CheckForInterrupts()
    {
        // Placeholder for interrupt handling logic
    }

    public (int, int) ToBytes(int value)
    {
        return ((value >> 16) & 0xFFFF, value & 0xFFFF);
    }
}

public static class ArithmeticLogicUnit
{
    public enum Operation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public static int PerformOperation(int a, int b, Operation op)
    {
        switch (op)
        {
            case Operation.Add:
                return Add(a, b);
            case Operation.Subtract:
                return Subtract(a, b);
            case Operation.Multiply:
                return Multiply(a, b);
            case Operation.Divide:
                return Divide(a, b);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static int Add(int a, int b) => a + b;
    public static int Subtract(int a, int b) => a - b;
    public static int Multiply(int a, int b) => a * b;
    public static int Divide(int a, int b)
    {
        if (b == 0)
        {
            UnityEngine.Debug.LogError("Divide by zero error");
            return 0;
        }
        return a / b;
    }
}
