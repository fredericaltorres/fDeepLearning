using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class SoftwareProcessor
    {
        // Registers
        public int AX { get; set; }
        public int BX { get; set; }
        public int CX { get; set; }
        public int DX { get; set; }

        // Flags for comparison
        private bool ZeroFlag;
        private bool SignFlag;

        // Stack for PUSH and POP operations
        private Stack<int> Stack = new Stack<int>();

        // Labels and their corresponding instruction indices
        private Dictionary<string, int> Labels = new Dictionary<string, int>();

        // Instructions and their corresponding methods
        private Dictionary<string, Action<string[]>> Instructions = new Dictionary<string, Action<string[]>>();

        public SoftwareProcessor()
        {
            // Initialize instructions
            Instructions["MOV"] = MOV;
            Instructions["ADD"] = ADD;
            Instructions["SUB"] = SUB;
            Instructions["MUL"] = MUL;
            Instructions["DIV"] = DIV;
            Instructions["AND"] = AND;
            Instructions["OR"] = OR;
            Instructions["XOR"] = XOR;
            Instructions["NOT"] = NOT;
            Instructions["SHL"] = SHL;
            Instructions["SHR"] = SHR;
            Instructions["CMP"] = CMP;
            Instructions["JMP"] = JMP;
            Instructions["JE"] = JumpIfEqual;
            Instructions["JNE"] = JumpIfNotEqual;
            Instructions["JG"] = JumpIfGreater;
            Instructions["JL"] = JumpIfLess;
            Instructions["PUSH"] = PUSH;
            Instructions["POP"] = POP;
            Instructions["CALL"] = CALL;
            Instructions["RET"] = RET;
            Instructions["HLT"] = HLT;
            Instructions["PRINT"] = PRINT;
            Instructions["READ"] = READ;
        }

        public void Execute(string[] sourceCode)
        {
            // First pass: Identify labels
            for (int i = 0; i < sourceCode.Length; i++)
            {
                string line = sourceCode[i].Trim();
                if (line.EndsWith(":"))
                {
                    string label = line.Substring(0, line.Length - 1);
                    Labels[label] = i;
                }
            }

            // Second pass: Execute instructions
            for (int i = 0; i < sourceCode.Length; i++)
            {
                string line = sourceCode[i].Trim();
                if (string.IsNullOrEmpty(line) || line.EndsWith(":")) continue;

                string[] parts = line.Split(new char[] { ' ' }, 2);
                string instruction = parts[0].ToUpper();
                string[] args = parts.Length > 1 ? parts[1].Split() : new string[0];

                if (Instructions.ContainsKey(instruction))
                {
                    Instructions[instruction](args);
                }
                else
                {
                    throw new InvalidOperationException($"Unknown instruction: {instruction}");
                }

                // Debugger: Print state of the processor
                Console.WriteLine($"AX: {AX}, BX: {BX}, CX: {CX}, DX: {DX}, ZeroFlag: {ZeroFlag}, SignFlag: {SignFlag}");
            }
        }

        private void MOV(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("MOV requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetValue(src));
        }

        private void ADD(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("ADD requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetRegister(dest) + GetValue(src));
        }

        private void SUB(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("SUB requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetRegister(dest) - GetValue(src));
        }

        private void MUL(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("MUL requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetRegister(dest) * GetValue(src));
        }

        private void DIV(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("DIV requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetRegister(dest) / GetValue(src));
        }

        private void AND(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("AND requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetRegister(dest) & GetValue(src));
        }

        private void OR(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("OR requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetRegister(dest) | GetValue(src));
        }

        private void XOR(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("XOR requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetRegister(dest) ^ GetValue(src));
        }

        private void NOT(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("NOT requires one argument.");
            string dest = args[0];
            SetRegister(dest, ~GetRegister(dest));
        }

        private void SHL(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("SHL requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetRegister(dest) << GetValue(src));
        }

        private void SHR(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("SHR requires two arguments.");
            string dest = args[0];
            string src = args[1];
            SetRegister(dest, GetRegister(dest) >> GetValue(src));
        }

        private void CMP(string[] args)
        {
            if (args.Length != 2) throw new ArgumentException("CMP requires two arguments.");
            string dest = args[0];
            string src = args[1];
            int result = GetRegister(dest) - GetValue(src);
            ZeroFlag = (result == 0);
            SignFlag = (result < 0);
        }

        private void JMP(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("JMP requires one argument.");
            string label = args[0];
            if (Labels.ContainsKey(label))
            {
                // Jump to the label
                // This is a simplified version; actual implementation would require more context
            }
            else
            {
                throw new InvalidOperationException($"Label not found: {label}");
            }
        }

        private void JumpIfEqual(string[] args)
        {
            if (ZeroFlag) JMP(args);
        }

        private void JumpIfNotEqual(string[] args)
        {
            if (!ZeroFlag) JMP(args);
        }

        private void JumpIfGreater(string[] args)
        {
            if (!SignFlag && !ZeroFlag) JMP(args);
        }

        private void JumpIfLess(string[] args)
        {
            if (SignFlag) JMP(args);
        }

        private void PUSH(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("PUSH requires one argument.");
            string src = args[0];
            Stack.Push(GetValue(src));
        }

        private void POP(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("POP requires one argument.");
            string dest = args[0];
            SetRegister(dest, Stack.Pop());
        }

        private void CALL(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("CALL requires one argument.");
            string label = args[0];
            if (Labels.ContainsKey(label))
            {
                // Push return address and jump to the label
                // This is a simplified version; actual implementation would require more context
            }
            else
            {
                throw new InvalidOperationException($"Label not found: {label}");
            }
        }

        private void RET(string[] args)
        {
            // Pop return address and jump
            // This is a simplified version; actual implementation would require more context
        }

        private void HLT(string[] args)
        {
            // Halt the processor
            Environment.Exit(0);
        }

        private void PRINT(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("PRINT requires one argument.");
            string src = args[0];
            Console.WriteLine(GetValue(src));
        }

        private void READ(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("READ requires one argument.");
            string dest = args[0];
            Console.Write($"Enter value for {dest}: ");
            string input = Console.ReadLine();
            SetRegister(dest, int.Parse(input));
        }

        private int GetRegister(string name)
        {
            switch(name.ToUpper() )
            {
                case "AX": return AX;
                case "BX": return BX;
                case "CX": return CX;
                case "DX": return DX;
                default: throw new ArgumentException($"Invalid register: {name}");
            }
        }

        private void SetRegister(string name, int value)
        {
            switch (name.ToUpper())
            {
                case "AX": AX = value; break;
                case "BX": BX = value; break;
                case "CX": CX = value; break;
                case "DX": DX = value; break;
                default: throw new ArgumentException($"Invalid register: {name}");
            }
        }

        private int GetValue(string src)
        {
            if (int.TryParse(src, out int value))
            {
                return value;
            }
            else
            {
                return GetRegister(src);
            }
        }

        public static void Main2(string[] args)
        {
            SoftwareProcessor processor = new SoftwareProcessor();
            string[] sourceCode = {
            "MOV AX 0",
            "MOV CX 1",
            "LOOP_START:",
            "ADD AX CX",
            "PRINT AX",
            "CMP CX 10",
            "JE LOOP_END",
            "ADD CX 1",
            "JMP LOOP_START",
            "LOOP_END:",
            "PRINT AX",
            "HLT"
        };
            processor.Execute(sourceCode);
        }
    }


}
