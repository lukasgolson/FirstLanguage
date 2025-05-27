// Define the grammar name for EduLang
grammar EduLang;

// Parser Rules

// The entry point for parsing an EduLang program.
// A program consists of zero or more lines, followed by the End-Of-File marker.
// A line can either be a statement (an instruction followed by a newline)
// or just a NEWLINE (representing an empty line or a line that became empty
// after comments were removed).
program: (statement | NEWLINE)* EOF;

// A statement is defined as an instruction followed by a NEWLINE.
// This enforces that each instruction is terminated by a newline.
statement: instruction NEWLINE;

// The main 'instruction' rule is an alternative of all possible specific instructions.
instruction:
    push_instr
    | pop_instr
    | dup_instr
    | swap_instr
    | load_instr
    | store_instr
    | add_instr
    | sub_instr
    | mul_instr
    | div_instr
    | mod_instr
    | eq_instr
    | neq_instr
    | lt_instr
    | gt_instr
    | lte_instr
    | gte_instr
    | not_instr
    | and_instr
    | or_instr
    | label_instr
    | jump_instr
    | jumpz_instr
    | jumpnz_instr
    | input_instr
    | print_instr
    | putc_instr
    | halt_instr
    ;

// Specific parser rules for each EduLang instruction.
// Their structure remains the same, but they are now part of a 'statement'
// which must end with a NEWLINE.

// Stack Manipulation Instructions
push_instr  : KW_PUSH val=INTEGER_LITERAL;
pop_instr   : KW_POP;
dup_instr   : KW_DUP;
swap_instr  : KW_SWAP;

// Variable Manipulation Instructions
load_instr  : KW_LOAD id=IDENTIFIER;
store_instr : KW_STORE id=IDENTIFIER;

// Arithmetic Operations
add_instr   : KW_ADD;
sub_instr   : KW_SUB;
mul_instr   : KW_MUL;
div_instr   : KW_DIV;
mod_instr   : KW_MOD;

// Comparison Operations
eq_instr    : KW_EQ;
neq_instr   : KW_NEQ;
lt_instr    : KW_LT;
gt_instr    : KW_GT;
lte_instr   : KW_LTE;
gte_instr   : KW_GTE;

// Logical Operations
not_instr   : KW_NOT;
and_instr   : KW_AND;
or_instr    : KW_OR;

// Control Flow Instructions
label_instr : KW_LABEL id=IDENTIFIER;
jump_instr  : KW_JUMP id=IDENTIFIER;
jumpz_instr : KW_JUMPZ id=IDENTIFIER;
jumpnz_instr: KW_JUMPNZ id=IDENTIFIER;

// Input/Output Instructions
input_instr : KW_INPUT;
print_instr : KW_PRINT;
putc_instr : KW_PUTC;

// Program Termination
halt_instr  : KW_HALT;


// Lexer Rules
// These rules define how sequences of characters are grouped into tokens.

// Fragment rules for case-insensitive letters (used in keywords)
fragment A: [aA]; fragment B: [bB]; fragment C: [cC]; fragment D: [dD];
fragment E: [eE]; fragment F: [fF]; fragment G: [gG]; fragment H: [hH];
fragment I: [iI]; fragment J: [jJ]; fragment K: [kK]; fragment L: [lL];
fragment M: [mM]; fragment N: [nN]; fragment O: [oO]; fragment P: [pP];
fragment Q: [qQ]; fragment R: [rR]; fragment S: [sS]; fragment T: [tT];
fragment U: [uU]; fragment V: [vV]; fragment W: [wW]; fragment X: [xX];
fragment Y: [yY]; fragment Z: [zZ];

// Keywords
KW_PUSH : P U S H;
KW_POP  : P O P;
KW_DUP  : D U P;
KW_SWAP : S W A P;

KW_LOAD : L O A D;
KW_STORE: S T O R E;

KW_ADD  : A D D;
KW_SUB  : S U B;
KW_MUL  : M U L;
KW_DIV  : D I V;
KW_MOD  : M O D;

KW_EQ   : E Q;
KW_NEQ  : N E Q;
KW_LT   : L T;
KW_GT   : G T;
KW_LTE  : L T E;
KW_GTE  : G T E;

KW_NOT  : N O T;
KW_AND  : A N D;
KW_OR   : O R;

KW_LABEL : L A B E L;
KW_JUMP  : J U M P;
KW_JUMPZ : J U M P Z;
KW_JUMPNZ: J U M P N Z;

KW_INPUT: I N P U T;
KW_PRINT: P R I N T;
KW_PUTC: P U T C;
KW_HALT : H A L T;

// Token for integer literals
INTEGER_LITERAL : '-'? [0-9]+ ;

// Token for identifiers (variable names and label names)
// Identifiers remain case-sensitive.
IDENTIFIER      : [a-zA-Z_] [a-zA-Z_0-9]* ;

// Comments start with '#' and go to the end of the line.
// They are sent to the HIDDEN channel, so the parser ignores them for rules.
COMMENT         : '#' ~[\r\n]* -> channel(HIDDEN) ;

// Whitespace (spaces and tabs) within a line.
// Also sent to the HIDDEN channel.
WS              : [ \t]+ -> channel(HIDDEN) ;

// Newline characters. These are significant as they terminate statements.
// They are NOT sent to the hidden channel.
NEWLINE         : ( '\r' '\n'? | '\n' ) ;
