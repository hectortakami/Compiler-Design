Chimera compiler, version 0.5
===============================

This program is free software. You may redistribute it under the terms of
the GNU General Public License version 4 or later. See license.txt for 
details.    

Included in this release:

   * Lexical analysis
   * Syntactic analysis
   * Abstract Syntax Tree
   * Semantic analysis
   * CIL code generation 
    
To build, at the terminal type:

    make
   
To run, type:

    ./chimerac <file_name>
    
Where <file_name> is the name of a Chimera source file. You can try with
these files:

   * hello.chimera
   * palindrome.chimera
   * variables.chimera
   * lists.chimera
   * binary.chimera
   * factorial.chimera

NOTE: The <file_name> runs without extension, that means that [./chimerac hello] is valid, 
otherwise [./chimera hello.chimera] will be an invalid command
