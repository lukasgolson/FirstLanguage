Just a small hobbyist project slowly building up to a fully-fledged array-based programming language.

This initial implementation contains a Turing complete stack-based VM and operators. 


The language currently supports the following instructions:

| **Instruction** | **Description** | **Example** |
|-----------------|-----------------|-------------|
| `PUSH <integer>` | Pushes an integer literal onto the top of the stack. | `PUSH 123` |
| `POP`           | Removes the top element from the stack. | `POP` |
| `ADD`           | Pops the top two elements, adds them, and pushes the result back onto the stack. | `ADD` |
| `SUB`           | Pops the top two elements, subtracts the second popped from the first popped, and pushes the result. | `SUB` |
| `LOAD <id>`     | Loads the value from the register named `<id>` and pushes it onto the stack. | `LOAD myVar` |
| `STORE <id>`    | Pops the top element from the stack and stores it in the register named `<id>`. | `STORE myVar` |
| `LABEL <id>`    | Defines a jump target named `<id>`. This instruction itself performs no operation during execution. | `LABEL loopEnd` |
| `JUMPZ <id>`    | Pops the top element from the stack. If the value is `0`, jumps to the instruction after the label `<id>`. | `JUMPZ endLoop` |
| `PRINT`         | Prints the top element of the stack to the console. The element remains on the stack. | `PRINT` |
| `HALT`          | Stops program execution. | `HALT` |
