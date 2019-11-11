/*
  Chimera compiler - Token class for the scanner.
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

using System;
using System.Collections.Generic;

namespace Chimera {

    class SemanticAnalyzer {

        //-----------------------------------------------------------
        static readonly IDictionary<TokenCategory, Type> typeMapper =
            new Dictionary<TokenCategory, Type>() {
                { TokenCategory.BOOL, Type.BOOLEAN },
                { TokenCategory.INT, Type.INTEGER }, 
                { TokenCategory.STR, Type.STRING}                
            };

        //-----------------------------------------------------------
        public SymbolTable Table {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticAnalyzer() {
            Table = new SymbolTable();
        }

        //-----------------------------------------------------------
        public Type Visit(Program_Node node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
            return Type.VOID;
        }

        //-----------------------------------------------------------
       
        public Type Visit(Literal_List node) {
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Declaration_List node) {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        
        public Type Visit(Constant_Declaration node) {
            return Type.VOID;
        }

        public Type Visit(Variable_Declaration node) {
            return Type.VOID;
        }

        public Type Visit(Procedure_Declaration node) {
            return Type.VOID;
        }

        public Type Visit(Parameter_Declaration node) {
            return Type.VOID;
        }

        //-----------------------------------------------------------
       
        public Type Visit(Statement_List node) {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------

        public Type Visit(Call_Statement node) {
            return Type.VOID;
        }

        public Type Visit(Assignment_Statement node) {
            return Type.VOID;
        }

        public Type Visit(If_Statement node) {
            return Type.VOID;
        }

        public Type Visit(ElseIf_Statement node) {
            return Type.VOID;
        }

        public Type Visit(Else_Statement node) {
            return Type.VOID;
        }

        public Type Visit(Loop_Statement node) {
            return Type.VOID;
        }
        
        public Type Visit(For_Statement node) {
            return Type.VOID;
        }

        public Type Visit(IN_Statement node) {
            return Type.VOID;
        }

        public Type Visit(DO_Statement node) {
            return Type.VOID;
        }

        public Type Visit(Return_Statement node) {
            return Type.VOID;
        }

        public Type Visit(Exit_Statement node) {
            return Type.VOID;
        }


        //-----------------------------------------------------------
        public Type Visit(Identifier node) {

            var variableName = node.AnchorToken.Lexeme;

            if (Table.Contains(variableName)) {
                return Table[variableName];
            }

            throw new SemanticError(
                "Undeclared variable: " + variableName,
                node.AnchorToken);
        }

        public Type Visit(Call_Expression node) {
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(IntLiteralNode node) {

            var intStr = node.AnchorToken.Lexeme;

            try {
                Convert.ToInt32(intStr);

            } catch (OverflowException) {
                throw new SemanticError(
                    "Integer literal too large: " + intStr, 
                    node.AnchorToken);
            }

            return Type.INTEGER;
        }

        public Type Visit(StrLiteralNode node) {
            return Type.STRING;
        }

        public Type Visit(TrueNode node) {
            return Type.BOOLEAN;
        }

        public Type Visit(FalseNode node) {
            return Type.BOOLEAN;
        }

        public Type Visit(ListLiteralNode node) {
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(AndNode node) {
            VisitBinaryOperator("and", node, Type.BOOLEAN);
            return Type.BOOLEAN;
        }

        public Type Visit(OrNode node) {
            VisitBinaryOperator("or", node, Type.BOOLEAN);
            return Type.BOOLEAN;
        }

        public Type Visit(XORNode node) {
            VisitBinaryOperator("xor", node, Type.BOOLEAN);
            return Type.BOOLEAN;
        }

        public Type Visit(EqualNode node) {
            VisitBinaryOperator("=", node, Type.INTEGER);
            return Type.BOOLEAN;
        }

        public Type Visit(InequalNode node) {
            Type type = Visit((dynamic)node[0]);
            switch (type) {
                case Type.BOOLEAN:
                    VisitBinaryOperator("<>", node, Type.BOOLEAN);
                    return Type.BOOLEAN;
                case Type.INTEGER:
                    VisitBinaryOperator("<>", node, Type.INTEGER);
                    return Type.BOOLEAN;
                default:
                    throw new SemanticError($"Operator {node.AnchorToken.Lexeme} requires one "
                                            + $"of {Type.BOOLEAN} or {Type.INTEGER}", node.AnchorToken);
                
            }
        }

        public Type Visit(LessEqualNode node) {
            VisitBinaryOperator("<=", node, Type.INTEGER);
            return Type.BOOLEAN;
        }

        public Type Visit(GreaterEqualNode node) {
            VisitBinaryOperator(">=", node, Type.INTEGER);
            return Type.BOOLEAN;
        }

        public Type Visit(LessNode node) {
            VisitBinaryOperator("<", node, Type.INTEGER);
            return Type.BOOLEAN;
        }

        public Type Visit(GreaterNode node) {
            VisitBinaryOperator(">", node, Type.INTEGER);
            return Type.BOOLEAN;
        }

        public Type Visit(AddNode node) {
            VisitBinaryOperator("+", node, Type.INTEGER);
            return Type.INTEGER;
        }

        public Type Visit(SubNode node) {
            VisitBinaryOperator("-", node, Type.INTEGER);
            return Type.INTEGER;
        }

        public Type Visit(MulNode node) {
            VisitBinaryOperator("*", node, Type.INTEGER);
            return Type.INTEGER;
        }

        public Type Visit(QuoNode node) {
            VisitBinaryOperator("div", node, Type.INTEGER);
            return Type.INTEGER;
        }

        public Type Visit(RemNode node) {
            VisitBinaryOperator("rem", node, Type.INTEGER);
            return Type.INTEGER;
        }

        public Type Visit(CompNode node) {
            if (Visit((dynamic) node[0]) != Type.BOOLEAN) {
                throw new SemanticError(
                    "Operator - requires an operand of type " + Type.BOOLEAN,
                    node.AnchorToken);
            }
            return Type.BOOLEAN;
        }

        public Type Visit(NegNode node) {
            if (Visit((dynamic) node[0]) != Type.INTEGER) {
                throw new SemanticError(
                    "Operator - requires an operand of type " + Type.INTEGER,
                    node.AnchorToken);
            }
            return Type.INTEGER;
        }

        //-----------------------------------------------------------

        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }

        //-----------------------------------------------------------
        void VisitBinaryOperator(string op, Node node, Type type) {
            if (Visit((dynamic) node[0]) != type || 
                Visit((dynamic) node[1]) != type) {
                throw new SemanticError(
                    String.Format(
                        "Operator {0} requires two operands of type {1}",
                        op, 
                        type),
                    node.AnchorToken);
            }
        }
    }
}
