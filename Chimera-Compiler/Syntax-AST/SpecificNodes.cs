/*
  Chimera compiler - Specific node subclasses for the AST (Abstract 
  Syntax Tree).
  Created by Hector Takami & Ernesto Cervantes
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace Chimera {    

    class Program: Node {}

    class DeclarationList: Node {}

    class Declaration: Node {}

    class StatementList: Node {}

    class ExpressionList: Node {}

    class Assignment: Node {}

    class If: Node {}

    class Identifier: Node {}

/* Simple Literal Nodes */
    class IntLiteralNode: Node {}

    class StrLiteralNode: Node {}

    class TrueNode: Node {}

    class FalseNode: Node {}

/* Operator Nodes */

/* Logic Operator Nodes */
    class AndNode: Node {}

    class OrNode: Node {}

    class XORNode: Node {}

/* Relational Operators Nodes */

    class EqualNode: Node {}

    class InequalNode: Node {}
    
    class LessEqualNode: Node {}
    
    class GreaterEqualNode: Node {}
    
    class LessNode: Node {}
    
    class GreaterNode: Node {}

/* Sum Operators Nodes */
    
    class AddNode: Node {}
    
    class SubNode: Node {}
    
/* Mul Operators Nodes */

    class MulNode: Node {}
    
    class QuoNode: Node {}
    
    class RemNode: Node {}
    
    
}