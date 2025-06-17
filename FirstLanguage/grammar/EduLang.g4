// Define the grammar name for EduLang
grammar EduLang;

// Parser Rules

// The entry point for parsing an EduLang program.
// A program consists of zero or more lines, followed by the End-Of-File marker.
// A line can either be a statement (an instruction followed by a newline)
// or just a NEWLINE (representing an empty line or a line that became empty
// after comments were removed).
program: (unsafe_block | NEWLINE)* EOF;

// A raw statement is defined as a low-level instruction followed by a NEWLINE.
low_statement: (instruction | macro_def | macro_call) NEWLINE;

macro_def: macro_instr name=IDENTIFIER args+=IDENTIFIER* NEWLINE (low_statement | NEWLINE)* block_end_instr;

macro_call: name=IDENTIFIER args+=IDENTIFIER*;

unsafe_block: unsafe_instr NEWLINE (low_statement | NEWLINE)* block_end_instr;

// The main 'instruction' rule is an alternative of all possible specific instructions.
instruction:
    load_instr
    | pop_instr
    | push_instr
    | store_instr
    | add_instr
    | sub_instr
    | gt_instr
    | jumpz_instr
    | print_instr
    | input_instr
    | halt_instr
    | label_instr
    ;

// Specific parser rules for each EduLang instruction.

// Stack Manipulation Instructions
push_instr  : KW_PUSH val=INTEGER_LITERAL;
pop_instr   : KW_POP (id=IDENTIFIER)?;

load_instr  : KW_LOAD id=IDENTIFIER;
store_instr : KW_STORE id=IDENTIFIER;

// Arithmetic Operations
add_instr   : KW_ADD;
sub_instr   : KW_SUB;

gt_instr : KW_GT;

// Control Flow Instructions
label_instr : KW_LABEL id=IDENTIFIER;
jumpz_instr : KW_JUMPZ id=IDENTIFIER;


// Misc
print_instr : KW_PRINT;
input_instr : KW_INPUT;
halt_instr  : KW_HALT;

unsafe_instr : KW_UNSAFE;

macro_instr : KW_MACRO;
block_end_instr : KW_BLOCK_END;


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

fragment COLON: [:]; fragment AT: [@];

// Keywords
KW_PUSH : P U S H;
KW_POP  : P O P;

KW_LOAD : L O A D;
KW_STORE: S T O R E;

KW_ADD  : A D D;
KW_SUB  : S U B;

KW_GT : G T;

KW_LABEL : L A B E L;
KW_JUMPZ : J U M P Z;

KW_PRINT: P R I N T;
KW_INPUT: I N P U T;
KW_HALT : H A L T;

KW_MACRO: M A C R O;
KW_BLOCK_END: E N D; 
KW_UNSAFE: AT U N S A F E;

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