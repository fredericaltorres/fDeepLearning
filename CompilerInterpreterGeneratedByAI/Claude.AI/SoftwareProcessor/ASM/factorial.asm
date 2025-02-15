; Simple program to calculate factorial of a number
    MOV R0 5       ; Input number
    MOV R1 1       ; Result
factorial_loop:
    CMP R0 1
    JLE done
    MUL R1 R0      ; Multiply result by current number
    SUB R0 1       ; Decrement counter
    JMP factorial_loop
done:
    PRINT R1        ; Print result
    HLT