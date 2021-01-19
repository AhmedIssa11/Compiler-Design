# Compiler-Design

Module
1. Lexical Analyzer
Lexical Analysis is the first phase of compiler also known as scanner. It converts the input program into a sequence of Tokens.It can be implemented with the Deterministic finite Automata.
2. Syntax Analyzer
Syntax Analysis or Parsing is the second phase,i.e. after lexical analysis. It checks the syntactical structure of the given input,i.e. whether the given input is in the correct syntax (of the language in which the input has been written) or not.It does so by building a data structure, called a Parse tree or Syntax tree.The parse tree is constructed by using the pre-defined Grammar of the language and the input string.If the given input string can be produced with the help of the syntax tree (in the derivation process),the input string is found to be in the correct syntax.
3. Semantic Analyzer
Semantic analysis is the task of ensuring that the declarations and statements of a program are semantically correct, i.e,that their meaning is clear and consistent with the way in which control structures and data types are supposed to be used.
