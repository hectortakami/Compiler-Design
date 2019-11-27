/*
  Chimera compiler - Program driver.
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

using System.Collections.Generic;

namespace Chimera
{
    /************ Auxiliary Nodes **************/
    class Program_Node : Node { }
    class Constant_List : Node { }
    class Procedure_List : Node { }
    class Statement_List : Node { }


    /************ Declaration Nodes ************/
    class Constant_Declaration: Node {}
    class Variable_Declaration: Node {}
    class Procedure_Declaration: Node {}
    class Parameter_Declaration: Node {}


    /************* Expression Nodes *************/
    class Call_Expression: Node {}
    class IdentifierNode : Node { public bool isAssignment { get; set; } = false; }


    /************* Statement Nodes *************/
    class Call_Statement: Node {}
    class Assignment_Statement: Node {}   
    class If_Statement: Node {}
    class ElseIf_Statement: Node {}
    class Elif_Statement: Node {}
    class Else_Statement: Node {}
    class Loop_Statement: Node {}
    class For_Statement: Node {}
    class Return_Statement: Node {}
    class Exit_Statement: Node {}  
    class Condition_List : Node { } 



    /**************** Type Nodes ****************/
    class IntNode: Node {}
    class StrNode: Node {}
    class BoolNode: Node {}
    class ListTypeNode: Node {}
    class VoidTypeNode : Node { }
    class ListIndexNode : Node { public bool isAssignment { get; set; } = false; }



    /************** Literal Nodes **************/

    class IntLiteralNode: Node {}
    class StrLiteralNode: Node {}
    class TrueNode: Node {}
    class FalseNode: Node {}
    class ListLiteralNode: Node {}


    
    /************ Operator Nodes ************/

    /* Logic Operator Nodes */  
    class AndNode : Node { }
    class OrNode : Node { }
    class XORNode : Node { }

    /* Relational Operators Nodes */
    class EqualNode : Node { }
    class InequalNode: Node {}
    class LessEqualNode: Node {}
    class GreaterEqualNode: Node {}
    class LessNode: Node {}
    class GreaterNode: Node {}

    /* Sum Operators Nodes */
    class AddNode : Node { }
    class SubNode : Node { }

    /* Mul Operators Nodes */
    class MulNode : Node { }
    class QuoNode : Node { }
    class RemNode : Node { }

    /* Unary Operators Nodes */
    class CompNode : Node { }
}
