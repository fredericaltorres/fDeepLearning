using SoftwareProcessor;
using SoftwareProcessorNP;
using System;
using System.IO;

class Program
{
    
    class Q
    {
        public string Parameters { get; set; } = null;
    }
    static void Main()
    {
        //var processor = new SoftwareProcessor_ClaudeAI();
        //processor.LoadProgram(File.ReadAllText(@".\ASM\Fibonacci.asm"));
        //processor.Execute();
        //Console.ReadLine();

        var q = new Q();
        var jobExtraParameters = "a=1;";

        //if (jobExtraParameters != null)
        //{
        //    if (!string.IsNullOrEmpty(q.Parameters) && !q.Parameters.EndsWith(";"))
        //    {
        //        q.Parameters += ";";
        //    }
        //    q.Parameters += jobExtraParameters;
        //}

        if (jobExtraParameters != null)
        {
            q.Parameters = q.Parameters?.TrimEnd(';') + ";" + jobExtraParameters.TrimStart(';');
        }

        SoftwareProcessor.SoftwareProcessor.Main2(null);
        Console.ReadLine();
    }
}

