# Testing AI Engine to generate software assembler and compiler
- Claude.AI
- Mistral.AI
- ChatGPT

## ASSEMBLEUR 

Generate a C# class that execute assembly code of a software processor.
The processor support the following operations:
- Data movement (MOV). Do not use the coma as separator. The source and destination are separated by a space.
- Arithmetic operations (ADD, SUB, MUL, DIV)
- Logical operations (AND, OR, XOR, NOT)
- Bit shifting (SHL, SHR)
- Comparison and jumping (CMP, JMP, JE, JNE, JG, JL)
- Stack operations (PUSH, POP)
- Subroutine handling (CALL, RET)
- Program control (HLT)- It possible to define LABEL in the source code. The command JMP, JE, JNE, JG, JL can use the label as argument.

Registers:
- AX, BX, CX, DX of type int
 
Display:
- Add instruction to print a register to the screen or read a register to the screen.

Debugger:
- print to the screen for every instruction executed the state of the processor.

Source Code:
The source code is passed as text.

Strings:
- Add assembly language source code to define string constant with directive ".data" and "DB" 
- Add assembly language source code to print string constant

Jump:
implement methods JumpIfEqual  JumpIfNotEqual, JumpIfGreater, and JumpIfLess.
Provide additional logic to handle comparison flags or conditions in CMP instruction.

Comparison Flags: Introduce flags to store the result of the last comparison.
Compare Method: Modify the Compare method to set these flags.
Conditional Jumps: Implement the conditional jump methods to check these flags.
 
Implement all methods

Write a program in symbolic assembly language to 
- sum the number from 1 to 10


Write a program in symbolic assembly language to 
- compute the Fibonacci sequence

Write a program in symbolic assembly language to 
- compute the Fibonacci sequence


## COMPILER 

Write a C# compiler compiling numeric expression into the following assembly language

   - Data movement (MOV)
   - Arithmetic operations (ADD, SUB, MUL, DIV)
   - Logical operations (AND, OR, XOR, NOT)
   - Bit shifting (SHL, SHR)
   - Comparison and jumping (CMP, JMP, JE, JNE, JG, JL)
   - Stack operations (PUSH, POP)
   - Subroutine handling (CALL, RET)
   - Program control (HLT)

The registers are : AX, BX, CX, DX of type C# int   


