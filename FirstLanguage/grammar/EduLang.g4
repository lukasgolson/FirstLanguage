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
    load_instr
    | pop_instr
    | push_instr
    | store_instr
    | add_instr
    | sub_instr
    | jumpz_instr
    | print_instr
    | halt_instr
    | label_instr
    ;

// Specific parser rules for each EduLang instruction.
// Their structure remains the same, but they are now part of a 'statement'
// which must end with a NEWLINE.

// Stack Manipulation Instructions
push_instr  : KW_PUSH val=INTEGER_LITERAL;
pop_instr   : KW_POP;

load_instr  : KW_LOAD id=IDENTIFIER;
store_instr : KW_STORE id=IDENTIFIER;

// Arithmetic Operations
add_instr   : KW_ADD;
sub_instr   : KW_SUB;

// Control Flow Instructions
label_instr : KW_LABEL id=IDENTIFIER;
jumpz_instr : KW_JUMPZ id=IDENTIFIER;


// Misc
print_instr : KW_PRINT;
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

KW_LOAD : L O A D;
KW_STORE: S T O R E;

KW_ADD  : A D D;
KW_SUB  : S U B;

KW_LABEL : L A B E L;
KW_JUMPZ : J U M P Z;

KW_PRINT: P R I N T;
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
