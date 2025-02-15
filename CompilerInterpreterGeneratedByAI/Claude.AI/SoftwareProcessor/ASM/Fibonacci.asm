; Program to compute Fibonacci sequence with text output
.data
    welcome_msg     DB ""Fibonacci Sequence Calculator""
    number_msg      DB ""Fibonacci number: ""
    done_msg        DB ""Calculation complete!""

PRINTS ""*** Fibonacci Sequence Calculator ***""

; Registers used:
; R0 - First number (previous)
; R1 - Second number (current)
; R2 - Next number in sequence
; R3 - Counter
; R4 - Limit (number of Fibonacci numbers to generate)

    ; Initialize the first two numbers of sequence
    MOV R0, 0       ; First number
    MOV R1, 1       ; Second number
    MOV R3, 0       ; Initialize counter
    MOV R4, 10      ; Calculate first 10 numbers

    ; Print initial values
    PRINT R0        ; Print first number (0)
    PRINT R1        ; Print second number (1)
    ADD R3, 2       ; Increment counter by 2 (we've printed 2 numbers)

loop:
    ; Calculate next Fibonacci number
    MOV R2, R1      ; Copy current number
    ADD R2, R0      ; Add previous number to get next number
    
    ; Print the calculated number
    PRINTS number_msg
    PRINT R2        ; Display next Fibonacci number
    
    ; Shift numbers for next iteration
    MOV R0, R1      ; Previous becomes current
    MOV R1, R2      ; Current becomes next
    
    ; Increment counter and check if we're done
    ADD R3, 1       ; Increment counter
    CMP R3, R4      ; Compare with limit
    JL loop         ; If counter < limit, continue loop
    
    ; End program
    PRINTS done_msg
    HLT
