#!/bin/sh

#OUTPUT=output/csharp
OUTPUT=../../Assets/PAG
rm -rf $OUTPUT
java -cp ../antlr4-csharp-4.2.2-SNAPSHOT-complete.jar org.antlr.v4.Tool -o $OUTPUT -Dlanguage=CSharp_v3_5 SimplePAG.g4 
