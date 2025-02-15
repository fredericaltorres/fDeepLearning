using SoftwareProcessorNP;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        var processor = new SoftwareProcessor();

        processor.LoadProgram(File.ReadAllText(@".\ASM\Fibonacci.asm"));
        processor.Execute();
        Console.ReadLine();
    }
}

