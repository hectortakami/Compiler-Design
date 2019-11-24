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
    class AndNode : Node { }
    class OrNode : Node { }
    class XorNode : Node { }
    class NotNode : Node { }
    class EqualNode : Node { }
    class UnequalNode : Node { }
    class LessThanNode : Node { }
    class MoreThanNode : Node { }
    class LessThanEqualNode : Node { }
    class MoreThanEqualNode : Node { }
    class MinusNode : Node { }
    class PlusNode : Node { }
    class TimesNode : Node { }
    class DivNode : Node { }
    class RemNode : Node { }
    class ExitNode : Node { }
    class IntegerNode : Node { }
    class StringNode : Node { }
    class BooleanNode : Node { }
    class VoidTypeNode : Node { }
    class ListTypeNode : Node { }
    class IntLiteralNode : Node { }
    class StringLiteralNode : Node { }
    class BoolLiteralNode : Node { }
    class ListLiteralNode : Node { }
    class ConstantDeclarationNode : Node { }
    class ConstantListNode : Node { }
    class IdentifierNode : Node
    {
        public bool isAssignment { get; set; } = false;
    }
    class ReturnStatementNode : Node { }
    class LoopStatementNode : Node { }
    class VariableDeclarationNode : Node { }
    class ForStatementNode : Node { }
    class StatementListNode : Node { }
    class IfStatementNode : Node { }
    class ElifStatementNode : Node { }
    class ElseStatementNode : Node { }
    class ProgramNode : Node { }
    class ProcedureDeclarationNode : Node { }
    class ProcedureListNode : Node { }
    class ParameterDeclarationNode : Node { }
    class AssignmentNode : Node { }
    class CallStatementNode : Node { }
    class CallNode : Node { }
    class ListIndexNode : Node
    {
        public bool isAssignment { get; set; } = false;
    }
    class ElseIfListNode : Node { }
}
