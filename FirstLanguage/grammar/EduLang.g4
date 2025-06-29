// Define the grammar name for EduLang
grammar EduLang;

// Parser Rules


program: (low_statement | NEWLINE)* EOF;


// A raw statement is defined as a low-level instruction followed by a NEWLINE.
low_statement: (assign | if_instr | jump_instr | label_instr | call_instr | macro_def | return_instr) NEWLINE;

literal: INTEGER_LITERAL | BOOL_LITERAL;

assign : location=IDENTIFIER KW_ASSIGN (operation | literal);

if_instr : KW_IF condition=operation KW_THEN NEWLINE (low_statement | NEWLINE)* ((KW_ELSE (low_statement | NEWLINE)* KW_BLOCK_END) | KW_BLOCK_END);

    
operation :  operand1=IDENTIFIER (arithmatic_operators | comparison_operators) operand2=IDENTIFIER;

label_instr : KW_LABEL id=IDENTIFIER;

jump_instr : (jumpz_op | jumpnz_op | call_op) id=IDENTIFIER;

call_instr : call_op name=IDENTIFIER args+=IDENTIFIER*;

macro_def: macro_instr name=IDENTIFIER args+=IDENTIFIER* NEWLINE (low_statement | NEWLINE)* block_end_instr;

return_instr : return_op (operation | INTEGER_LITERAL);

    
arithmatic_operators :
    add_op
    | sub_op
    | mult_op
    | div_op
    | mod_op
    ;
    
comparison_operators : 
    eq_comp |
    neq_comp |
    gt_comp |
    gte_comp |
    lt_comp |
    lte_comp
;


// Specific parser rules for each EduLang instruction.

// High Level Instructions


// Math Operators
add_op : KW_ADD;
sub_op : KW_SUB;
mult_op : KW_MULT;
div_op : KW_DIV;
mod_op : KW_MOD;

// Logic operators
eq_comp : KW_EQ;
neq_comp : KW_NEQ;
gt_comp : KW_GT;
gte_comp : KW_GTE;
lt_comp : KW_LT;
lte_comp : KW_LTE;





// Control Flow Instructions
jumpz_op : KW_JUMPZ;
jumpnz_op : KW_JUMPNZ;
call_op : KW_CALL;
return_op : KW_RETURN;


// Misc
print_instr : KW_PRINT;
input_instr : KW_INPUT;
halt_instr  : KW_HALT;

macro_instr : KW_MACRO;
block_end_instr : KW_BLOCK_END;


// Lexer Rules

// Fragment rules for case-insensitive letters (used in keywords)
fragment A: [aA]; fragment B: [bB]; fragment C: [cC]; fragment D: [dD];
fragment E: [eE]; fragment F: [fF]; fragment G: [gG]; fragment H: [hH];
fragment I: [iI]; fragment J: [jJ]; fragment K: [kK]; fragment L: [lL];
fragment M: [mM]; fragment N: [nN]; fragment O: [oO]; fragment P: [pP];
fragment Q: [qQ]; fragment R: [rR]; fragment S: [sS]; fragment T: [tT];
fragment U: [uU]; fragment V: [vV]; fragment W: [wW]; fragment X: [xX];
fragment Y: [yY]; fragment Z: [zZ]; 

fragment COLON: [:]; fragment AT: [@]; fragment EQUALS_SIGN: [=];


KW_ASSIGN : EQUALS_SIGN;
KW_LABEL : L A B E L;


// Math
KW_ADD  : A D D;
KW_SUB  : S U B;
KW_MULT : M U L T;
KW_DIV : D I V;
KW_MOD : M O D;
KW_NEG : N E G;


// Logic
KW_EQ : E Q;
KW_NEQ : N E Q;
KW_GT : G T;
KW_GTE : G T E;
KW_LT : L T;
KW_LTE : L T E;

// Flow
KW_JUMPZ : J U M P Z;
KW_JUMPNZ : J U M P N Z;
KW_CALL : C A L L;
KW_RETURN : R E T U R N;

// System
KW_PRINT: P R I N T;
KW_INPUT: I N P U T;
KW_HALT : H A L T;
KW_NOOP : N O O P;

// Sugar (doesn't necessarily follow TAC conventions)
KW_MACRO: M A C R O;

KW_IF: I F;
KW_THEN: T H E N;
KW_ELSE: E L S E;

KW_BLOCK_END: E N D; 

KW_OPEN: [(];
KW_CLOSE: [)];

// Token for integer literals
INTEGER_LITERAL : '-'? [0-9]+ ;

BOOL_LITERAL : (T R U E | F A L S E);

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