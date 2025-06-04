Just a small hobbyist project slowly building up to a fully-fledged array-based programming language.

This initial implementation contains a Turing complete stack-based VM and operators. 


The language currently supports the following instructions:

| **Instruction** | **Description** | **Example** |
|-----------------|-----------------|-------------|
| `PUSH <integer>` | Pushes an integer literal onto the top of the stack. | `PUSH 123` |
| `ADD`           | Pops the top two elements, adds them, and pushes the result back onto the stack. | `ADD` |
| `SUB`           | Pops the top two elements, subtracts the second popped from the first popped, and pushes the result. | `SUB` |
| `GT`            | Pops the top two elements, pushes 1 if the first popped value is greater than the second popped value, otherwise pushes 0 | `GT` |
| `POP <id>*`     | Removes the top element from the provided stack. | `POP myVar` |
| `LOAD <id>`     | Loads the value from the stack named `<id>` and pushes it onto the main stack. | `LOAD myVar` |
| `STORE <id>`    | Pops the top element from the main stack and stores it in the stack named `<id>`. | `STORE myVar` |
| `LABEL <id>`    | Defines a jump target named `<id>`. This instruction itself performs no operation during execution. | `LABEL endLoop` |
| `JUMPZ <id>`    | Pops the top element from the stack. If the value is `0`, jumps to the label `<id>`. | `JUMPZ endLoop` |
| `PRINT`         | Prints the top element of the stack to the console. The element remains on the stack. | `PRINT` |
| `INPUT`         | Waits for user input. Pushes element to the top of the main stack.
| `HALT`          | Stops program execution. | `HALT` |
