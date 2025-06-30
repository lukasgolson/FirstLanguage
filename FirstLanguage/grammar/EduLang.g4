grammar EduLang;

/*
 * ==============================================================================
 * Parser Rules
 * ==============================================================================
 */

program: statement* EOF;

statement:
      label_def
    | macro_def
    | instruction NEWLINE
    | NEWLINE
    ;

label_def: name=IDENTIFIER ':';

// A macro block containing TAC-style statements
macro_def: KW_MACRO name=IDENTIFIER args+=IDENTIFIER* NEWLINE body=block KW_ENDMACRO;

block: statement*;

// All executable instructions
instruction:
      assign_instr
    | if_instr
    | jump_instr
    | call_instr
    | return_instr
    | print_instr
    | input_instr
    | halt_instr
    | noop_instr
    ;

// An operand is a simple variable or constant.
operand: IDENTIFIER | literal;
literal: INTEGER_LITERAL | BOOL_LITERAL;


assign_instr:
    dest=IDENTIFIER '=' (
          src1=operand op=operator src2=operand // e.g., x = y ADD z
        | src=operand                          // e.g., x = y
    );

if_instr:
    KW_IF condition=operand NEWLINE
        then_block=block
    (KW_ELSE NEWLINE
        else_block=block
    )?
    KW_ENDIF;

jump_instr:
      KW_JUMP target=IDENTIFIER
    | (KW_JUMPZ | KW_JUMPNZ) condition=operand target=IDENTIFIER // condition is now an operand
    ;

call_instr: KW_CALL target=IDENTIFIER (args+=operand)*;
return_instr: KW_RETURN value=operand;
print_instr: KW_PRINT value=operand;
input_instr: KW_INPUT target=IDENTIFIER;
halt_instr:  KW_HALT;
noop_instr:  KW_NOOP;

operator:
      KW_ADD | KW_SUB | KW_MULT | KW_DIV | KW_MOD
    | KW_EQ | KW_NEQ | KW_GT | KW_GTE | KW_LT | KW_LTE
    ;

/*
 * ==============================================================================
 * Lexer Rules
 * ==============================================================================
 */

COLON : ':';

// --- Keywords (Case-Insensitive via Fragments) ---
// Math
KW_ADD  : A D D;
KW_SUB  : S U B;
KW_MULT : M U L T;
KW_DIV  : D I V;
KW_MOD  : M O D;

// Comparison Logic
KW_EQ   : E Q;
KW_NEQ  : N E Q;
KW_GT   : G T;
KW_GTE  : G T E;
KW_LT   : L T;
KW_LTE  : L T E;

// Flow Control & Procedures
KW_IF        : I F;
KW_ELSE      : E L S E;
KW_JUMP      : J U M P;
KW_JUMPZ     : J U M P Z;
KW_JUMPNZ    : J U M P N Z;
KW_CALL      : C A L L;
KW_RETURN    : R E T U R N;
KW_MACRO     : M A C R O;
KW_ENDIF     : E N D I F;
KW_ENDMACRO  : E N D M A C R O;

// System Instructions
KW_PRINT : P R I N T;
KW_INPUT : I N P U T;
KW_HALT  : H A L T;
KW_NOOP  : N O O P;

// --- Literals and Identifiers ---
INTEGER_LITERAL : '-'? [0-9]+;
BOOL_LITERAL    : T R U E | F A L S E;
IDENTIFIER      : [a-zA-Z_] [a-zA-Z_0-9]*;

// --- Whitespace and Comments ---
COMMENT : '#' ~[\r\n]* -> channel(HIDDEN);
WS      : [ \t]+      -> channel(HIDDEN);
NEWLINE : ( '\r' '\n'? | '\n' );

// --- Case-Insensitive Letter Fragments (Complete Set) ---
fragment A: [aA]; fragment B: [bB]; fragment C: [cC]; fragment D: [dD];
fragment E: [eE]; fragment F: [fF]; fragment G: [gG]; fragment H: [hH];
fragment I: [iI]; fragment J: [jJ]; fragment K: [kK]; fragment L: [lL];
fragment M: [mM]; fragment N: [nN]; fragment O: [oO]; fragment P: [pP];
fragment Q: [qQ]; fragment R: [rR]; fragment S: [sS]; fragment T: [tT];
fragment U: [uU]; fragment V: [vV]; fragment W: [wW]; fragment X: [xX];
fragment Y: [yY]; fragment Z: [zZ];