grammar SimplePAG;

// The org.terasology.logic.grammar file consists of a header and a rules section
pag :   '#rules'    rules
    ;

// The rules sections consists of one or more producton rules
rules   :   (rule)+
    ;

/* Each rule is of the form 
 *      predecessor : condition ::- successor : probability;
 * <conditon> and <probability> are optional for each rule.
 */
rule    :   ID '::-' successor (':' FLOAT)? (successor (':' FLOAT)? )*  ';'
    ;

/* A successor is anything can occur on the right side of a production rule. Namely that are the defined methods such
 * as Set, Subdivide, Repeat and the the Component Split. Furthermore, a successor can be another non terminal shapeSymbol.
 * Therefore it can also be an identifier.
 */
successor
    :   cmdDefinition
    |   ID
    ;

cmdDefinition
    :   ID argumentsDefinition
    |   pushPopScope
    ;

pushPopScope
    :   '[' | ']'
    ;

argumentsDefinition
    :   '(' argumentDefinition (',' argumentDefinition)* ')'
    |   '()'
    ;

// Simple value arguments
argumentDefinition
    : unary
    | shape_name
    ; 

// An <AssetURI> is a ':' seperated list of strings, framed by double quotes
// e.g.     "engine:CobbleStone", "mod:myMod:Block"
asset_uri
    :   '"' ID (':' ID)* '"'
    ;

shape_name
    : '"' ID '"'
    ;

unary
  :  '-' atom
  |  '+' atom
  |  atom
  ;

atom
  :  NUMBER
  |  ID
  ;

NUMBER 
  :  FLOAT
  |  INTEGER
  ;

// Integer with no leading zero
INTEGER
    :   DIGIT+
    ;

FLOAT
    :   DIGIT+ '.' DIGIT+
    |   '.' DIGIT+
    ;

fragment
DIGIT
    :  '0'..'9'
    ;

// Identifiers have to start with a letter and can continue with letters, numbers or '_'
ID  :   ('a'..'z'|'A'..'Z') ('a'..'z'|'A'..'Z'|'0'..'9'|'_')*
    ;

// Whitespace characters such as tabs and linefeeds
WS  :   ( ' '
    | '\t'
    | '\r'
    | '\n'
        ) -> channel(HIDDEN)
    ;
