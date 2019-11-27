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

using System;
using System.Linq;
using System.Collections.Generic;

namespace Chimera
{

    class SemanticAnalyzer
    {
        //-----------------------------------------------------------
        static readonly IDictionary<TokenCategory, Type> typeMapper =
            new Dictionary<TokenCategory, Type>() {
                { TokenCategory.BOOLEAN, Type.BOOL },
                { TokenCategory.STRING, Type.STR },
                { TokenCategory.INTEGER, Type.INT }
            };
        //-----------------------------------------------------------
        public SymbolTable symbolTable
        {
            get;
            private set;
        }
        //-----------------------------------------------------------
        public ProcedureTable procedureTable
        {
            get;
            private set;
        }
        //-----------------------------------------------------------
        public SemanticAnalyzer()
        {
            symbolTable = new SymbolTable();
            procedureTable = new ProcedureTable();
            LoadPredefinedFunctions();
        }


        //-----------------------------------------------------------
        // AUXILIARY SCOPE FLAG VARIABLES
        //-----------------------------------------------------------
        private bool insideLoop = false;
        private string insideProcedure = "";
        //-----------------------------------------------------------


        //-----------------------------------------------------------
        // SEMANTIC CONTAINERS
        //-----------------------------------------------------------
        public Type Visit(Program_Node node)
        {
            VisitChildren(node);
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(Statement_List node)
        {
            VisitChildren(node);
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(Constant_List node)
        {
            VisitChildren(node);
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(Procedure_List node)
        {
            VisitChildren(node);
            return Type.VOID;
        }
        //-----------------------------------------------------------       


        //-----------------------------------------------------------
        // DECLARATIONS
        //-----------------------------------------------------------
        public Type Visit(Constant_Declaration node)
        {
            var varName = node.AnchorToken.Lexeme;
            SymbolTable table = SelectSymbolTable();
            if (table.Contains(varName))
            {
                throw new SemanticError($"Duplicated constant: {varName}",
                                        node.AnchorToken);
            }
            Type type = Visit((dynamic)node[0]);
            if (type == Type.LIST)
            {
                throw new SemanticError($"List constants should have at least one element",
                            node.AnchorToken);
            }
            table = SelectSymbolTable();
            table[varName] = new SymbolTable.Register(type, ProcedureType.CONST, -1, ObtainLiteralValue(node[0]));
            return type;
        }
        //-----------------------------------------------------------
        public Type Visit(Variable_Declaration node)
        {
            foreach (var typeNode in node)
            {
                Type type = Visit((dynamic)typeNode);
                foreach (var idNode in typeNode)
                {
                    var varName = idNode.AnchorToken.Lexeme;
                    SymbolTable table = SelectSymbolTable();

                    if (table.Contains(varName))
                    {
                        throw new SemanticError($"Duplicated variable: {varName}",
                                                idNode.AnchorToken);
                    }
                    else
                    {
                        dynamic variableDefaultValue = null;
                        switch (type)
                        {
                            case Type.BOOL:
                                variableDefaultValue = false;
                                break;
                            case Type.INT:
                                variableDefaultValue = 0;
                                break;
                            case Type.STR:
                                variableDefaultValue = "";
                                break;
                            case Type.INT_LIST:
                                variableDefaultValue = new int[] { 0 };
                                break;
                            case Type.BOOL_LIST:
                                variableDefaultValue = new bool[] { false };
                                break;
                            case Type.STR_LIST:
                                variableDefaultValue = new string[] { "" };
                                break;
                        }
                        table[varName] = new SymbolTable.Register(type, ProcedureType.VAR, -1, variableDefaultValue);
                    }
                }
            }
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(Procedure_Declaration node)
        {
            var procedureName = node.AnchorToken.Lexeme;
            if (procedureTable.Contains(procedureName))
            {
                throw new SemanticError($"Duplicate procedure {procedureName}", node.AnchorToken);
            }
            Type procedureType = Visit((dynamic)node[1]);
            procedureTable[procedureName] = new ProcedureTable.ProcedureData
            (procedureType, false);
            insideProcedure = procedureName;

            Visit((dynamic)node[0]);
            VisitChildren(node, 2);

            insideProcedure = "";
            return procedureType;
        }
        //-----------------------------------------------------------
        public Type Visit(Parameter_Declaration node)
        {
            foreach (var typeNode in node)
            {
                Type type = Visit((dynamic)typeNode);
                int pos = 0;
                foreach (var idNode in typeNode)
                {
                    var varName = idNode.AnchorToken.Lexeme;
                    SymbolTable table = SelectSymbolTable();

                    if (table.Contains(varName))
                    {
                        throw new SemanticError($"Duplicated parameter: {varName}",
                                                idNode.AnchorToken);
                    }
                    else
                    {
                        dynamic parameterDefaultValue = null;
                        switch (type)
                        {
                            case Type.BOOL:
                                parameterDefaultValue = false;
                                break;
                            case Type.INT:
                                parameterDefaultValue = 0;
                                break;
                            case Type.STR:
                                parameterDefaultValue = "";
                                break;
                            case Type.INT_LIST:
                                parameterDefaultValue = new int[] { 0 };
                                break;
                            case Type.BOOL_LIST:
                                parameterDefaultValue = new bool[] { false };
                                break;
                            case Type.STR_LIST:
                                parameterDefaultValue = new string[] { "" };
                                break;
                        }
                        table[varName] = new SymbolTable.Register(type, ProcedureType.PARAM, pos++, parameterDefaultValue);
                    }
                }
            }
            return Type.VOID;
        }
        //-----------------------------------------------------------


        //-----------------------------------------------------------
        // STATEMENTS
        //-----------------------------------------------------------
        public Type Visit(Assignment_Statement node)
        {
            Type type1 = Visit((dynamic)node[0]);
            if (node[0] is IdentifierNode && ObtainSymbolByKey(node[0].AnchorToken.Lexeme).procType == ProcedureType.CONST)
            {
                throw new SemanticError($"Cannot assign to constant '{node[0].AnchorToken.Lexeme}'", node[0].AnchorToken);
            }
            Type type2 = Visit((dynamic)node[1]);

            bool typesCompatible;
            if (type1 == Type.LIST || type2 == Type.LIST)
            {
                Type otherType = type1 == Type.LIST ? type2 : type1;
                var valid = new Type[] { Type.LIST, Type.BOOL_LIST, Type.INT_LIST, Type.STR_LIST };
                typesCompatible = valid.Contains(otherType);
            }
            else
            {
                typesCompatible = type1 == type2;
            }

            if (typesCompatible)
            {
                return Type.VOID;
            }
            else
            {
                throw new SemanticError($"Cannot assign a value of type {type2} to a variable of type {type1}",
                    node.AnchorToken);
            }

        }
        //-----------------------------------------------------------
        public Type Visit(Loop_Statement node)
        {
            var lastInLoopOrFor = insideLoop;
            insideLoop = true;

            VisitChildren(node);

            insideLoop = lastInLoopOrFor;
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(For_Statement node)
        {
            Type varType = Visit((dynamic)node[0]);
            Type listType = Visit((dynamic)node[1]);
            dynamic varListType;
            switch (varType)
            {
                case Type.INT:
                    varListType = Type.INT_LIST;
                    break;
                case Type.STR:
                    varListType = Type.STR_LIST;
                    break;
                case Type.BOOL:
                    varListType = Type.BOOL_LIST;
                    break;
                default:
                    throw new Exception($"Type {varType} has no equivalent list type");
            }

            if (varListType != listType)
            {
                throw new SemanticError($"Incompatible types {varType} and {listType}",
                    node[0].AnchorToken);
            }

            string key = $"__{node[0].AnchorToken.Lexeme}_index";
            SymbolTable table = SelectSymbolTable();
            table[key] = new SymbolTable.Register(Type.INT, ProcedureType.VAR, -1, 0);

            var lastInLoopOrFor = insideLoop;
            insideLoop = true;

            Visit((dynamic)node[2]);

            insideLoop = lastInLoopOrFor;
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(Exit_Statement node)
        {
            if (!insideLoop)
            {
                throw new SemanticError("Unexpected exit statement", node.AnchorToken);
            }
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(If_Statement node)
        {
            Type conditionType = Visit((dynamic)node[0]);
            if (conditionType != Type.BOOL)
            {
                throw new SemanticError($"Condition has to be of type {Type.BOOL} but got {conditionType}",
                    node.AnchorToken);
            }
            VisitChildren(node, 1);
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(Condition_List node)
        {
            VisitChildren(node);
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(Elif_Statement node)
        {
            Type conditionType = Visit((dynamic)node[0]);
            if (conditionType != Type.BOOL)
            {
                throw new SemanticError($"Condition has to be of type {Type.BOOL} but got {conditionType}",
                    node.AnchorToken);
            }
            Visit((dynamic)node[1]);
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(Else_Statement node)
        {
            VisitChildren(node);
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(Return_Statement node)
        {
            if (insideProcedure == "")
            {
                throw new SemanticError("Unexpected return statement",
                    node.AnchorToken);
            }
            Type type = node.Count() == 0 ? Type.VOID : Visit((dynamic)node[0]);
            var procedureType = procedureTable[insideProcedure].type;

            bool typesCompatible;
            if (procedureType == Type.LIST || type== Type.LIST)
            {
                Type otherType = procedureType == Type.LIST ? type: procedureType;
                var valid = new Type[] { Type.LIST, Type.BOOL_LIST, Type.INT_LIST, Type.STR_LIST };
                typesCompatible = valid.Contains(otherType);
            }
            else
            {
                typesCompatible = procedureType == type;
            }

            if (typesCompatible)
            {
                return type;
            }else{
                throw new SemanticError($"Invalid return type {type} for procedure of type {procedureType}",
                    node.AnchorToken);
            }
            
        }
        //-----------------------------------------------------------
        public Type Visit(Call_Statement node)
        {
            var name = node.AnchorToken.Lexeme;
            if (procedureTable.Contains(name))
            {
                var procedure = procedureTable[name];
                var _params = procedure.symbols.Where(kv => kv.Value.procType == ProcedureType.PARAM)
                                            .OrderBy(kv => kv.Value.pos)
                                            .ToList();
                if (node.Count() != _params.Count())
                {
                    throw new SemanticError($"Wrong number of params to procedure call: "
                        + $"expected {_params.Count()} but got {node.Count()}", node.AnchorToken);
                }
                for (int i = 0; i < _params.Count; ++i)
                {
                    var _node = node[i];
                    var _param = _params[i];
                    Type nodeType = Visit((dynamic)_node);

                    bool typesCompatible;
                    if (nodeType == Type.LIST || _param.Value.type == Type.LIST)
                    {
                        Type otherType = nodeType == Type.LIST ? _param.Value.type : nodeType;
                        var valid = new Type[] { Type.LIST, Type.BOOL_LIST, Type.INT_LIST, Type.STR_LIST };
                        typesCompatible = valid.Contains(otherType);
                    }
                    else
                    {
                        typesCompatible = nodeType == _param.Value.type;
                    }

                    if (typesCompatible == false)
                    {
                        throw new SemanticError($"Incompatible types {nodeType} and {_param.Value.type} for parameter {_param.Key}",
                            _node.AnchorToken);
                    }
                }
                return Type.VOID;
            }
            else
            {
                throw new SemanticError($"Undeclared procedure: {name}", node.AnchorToken);
            }

        }
        //-----------------------------------------------------------


        //-----------------------------------------------------------
        // VARIABLES TYPES
        //-----------------------------------------------------------
        public Type Visit(IntNode node)
        {
            return Type.INT;
        }
        //----------------------------------------------------------- 
        public Type Visit(StrNode node)
        {
            return Type.STR;
        }
        //-----------------------------------------------------------       
        public Type Visit(BoolNode node)
        {
            return Type.BOOL;
        }
        //-----------------------------------------------------------        
        public Type Visit(VoidTypeNode node)
        {
            return Type.VOID;
        }
        //-----------------------------------------------------------
        public Type Visit(ListTypeNode node)
        {
            dynamic listType;
            switch (typeMapper[node.AnchorToken.Category])
            {
                case Type.INT:
                    listType = Type.INT_LIST;
                    break;
                case Type.STR:
                    listType = Type.STR_LIST;
                    break;
                case Type.BOOL:
                    listType = Type.BOOL_LIST;
                    break;
                default:
                    throw new Exception($"Type {typeMapper[node.AnchorToken.Category]} has no equivalent list type");
            }

            return listType;
        }
        //-----------------------------------------------------------


        //-----------------------------------------------------------
        // LITERAL TYPES
        //-----------------------------------------------------------
        public Type Visit(IntLiteralNode node)
        {
            var intStr = node.AnchorToken.Lexeme;
            try
            {
                Convert.ToInt32(intStr);

            }
            catch (OverflowException)
            {
                throw new SemanticError(
                    $"Integer literal too large: {intStr}",
                    node.AnchorToken);
            }

            return Type.INT;
        }
        //-----------------------------------------------------------   
        public Type Visit(StrLiteralNode node)
        {
            return Type.STR;
        }
        //-----------------------------------------------------------      
        public Type Visit(TrueNode node)
        {
            return Type.BOOL;
        }
        //-----------------------------------------------------------       
        public Type Visit(FalseNode node)
        {
            return Type.BOOL;
        }
        //-----------------------------------------------------------       
        public Type Visit(ListLiteralNode node)
        {
            if (node.Count() == 0)
            {
                return Type.LIST;
            }
            Type first = Visit((dynamic)node[0]);
            foreach (var n in node)
            {
                Type t = Visit((dynamic)n);
                if (t != first)
                {
                    throw new SemanticError("All elements of a list should be the same tipe, "
                                            + $"expected {first} but got {t}", n.AnchorToken);
                }
            }

            dynamic listLiteralType;
            switch (first)
            {
                case Type.INT:
                    listLiteralType = Type.INT_LIST;
                    break;
                case Type.STR:
                    listLiteralType = Type.STR_LIST;
                    break;
                case Type.BOOL:
                    listLiteralType = Type.BOOL_LIST;
                    break;
                default:
                    throw new Exception($"Type {first} has no equivalent list type");
            }
            return listLiteralType;
        }
        //-----------------------------------------------------------
        public Type Visit(ListIndexNode node)
        {
            Type type = Visit((dynamic)node[0]);
            Type indexType = Visit((dynamic)node[1]);
            dynamic listLiteralType;

            node.extra = type;
            if (indexType != Type.INT)
            {
                throw new SemanticError($"List indexes should be {Type.INT}, got {indexType}", node[1].AnchorToken);
            }

            switch (type)
            {
                case Type.INT_LIST:
                    listLiteralType = Type.INT;
                    break;
                case Type.STR_LIST:
                    listLiteralType = Type.STR;
                    break;
                case Type.BOOL_LIST:
                    listLiteralType = Type.BOOL;
                    break;
                default:
                    throw new Exception($"List type {type} has no equivalent primitive literal type");
            }
            return listLiteralType;
        }
        //-----------------------------------------------------------


        //-----------------------------------------------------------
        // EXPRESSION NODES
        //-----------------------------------------------------------
        public Type Visit(IdentifierNode node)
        {
            var variableName = node.AnchorToken.Lexeme;
            var symbol = ObtainSymbolByKey(variableName);
            if (symbol != null)
            {
                return symbol.type;
            }

            throw new SemanticError(
                $"Undeclared variable or constant: {variableName}",
                node.AnchorToken);
        }
        //-----------------------------------------------------------
        public Type Visit(Call_Expression node)
        {
            var name = node.AnchorToken.Lexeme;
            if (procedureTable.Contains(name))
            {
                var procedure = procedureTable[name];
                var _params = procedure.symbols.Where(kv => kv.Value.procType == ProcedureType.PARAM)
                                            .OrderBy(kv => kv.Value.pos)
                                            .ToList();
                if (node.Count() != _params.Count())
                {
                    throw new SemanticError($"Wrong number of params to procedure call: "
                        + $"expected {_params.Count()} but got {node.Count()}", node.AnchorToken);
                }
                for (int i = 0; i < _params.Count; ++i)
                {
                    var _node = node[i];
                    var _param = _params[i];
                    Type nodeType = Visit((dynamic)_node);

                    bool typesCompatible;
                    if (nodeType == Type.LIST || _param.Value.type == Type.LIST)
                    {
                        Type otherType = nodeType == Type.LIST ? _param.Value.type : nodeType;
                        var valid = new Type[] { Type.LIST, Type.BOOL_LIST, Type.INT_LIST, Type.STR_LIST };
                        typesCompatible = valid.Contains(otherType);
                    }
                    else
                    {
                        typesCompatible = nodeType == _param.Value.type;
                    }

                    if (!typesCompatible)
                    {
                        throw new SemanticError($"Incompatible types {nodeType} and {_param.Value.type} for parameter {_param.Key}",
                            _node.AnchorToken);
                    }
                }
                return procedure.type;
            }
            else
            {
                throw new SemanticError($"Undeclared procedure: {name}", node.AnchorToken);
            }
        }
        //-----------------------------------------------------------


        //-----------------------------------------------------------
        // OPERATORS
        //-----------------------------------------------------------
        public Type Visit(AndNode node)
        {
            VisitBinaryOperator(node, Type.BOOL);
            return Type.BOOL;
        }
        //-----------------------------------------------------------
        public Type Visit(OrNode node)
        {
            VisitBinaryOperator(node, Type.BOOL);
            return Type.BOOL;
        }
        //-----------------------------------------------------------
        public Type Visit(XORNode node)
        {
            VisitBinaryOperator(node, Type.BOOL);
            return Type.BOOL;
        }
        //-----------------------------------------------------------
        public Type Visit(CompNode node)
        {
            if (Visit((dynamic)node[0]) != Type.BOOL)
            {
                throw new SemanticError(
                    $"Operator {node.AnchorToken.Lexeme} requires an operand of type {Type.BOOL}",
                    node.AnchorToken);
            }
            return Type.BOOL;
        }
        //-----------------------------------------------------------
        public Type Visit(EqualNode node)
        {
            Type type = Visit((dynamic)node[0]);
            VisitBinaryOperator(node, type);
            return Type.BOOL;
        }
        //-----------------------------------------------------------       
        public Type Visit(InequalNode node)
        {
            Type type = Visit((dynamic)node[0]);
            VisitBinaryOperator(node, type);
            return Type.BOOL;
        }
        //-----------------------------------------------------------
        public Type Visit(LessNode node)
        {
            VisitBinaryOperator(node, Type.INT);
            return Type.BOOL;
        }
        //-----------------------------------------------------------
        public Type Visit(GreaterNode node)
        {
            VisitBinaryOperator(node, Type.INT);
            return Type.BOOL;
        }
        //-----------------------------------------------------------       
        public Type Visit(LessEqualNode node)
        {
            VisitBinaryOperator(node, Type.INT);
            return Type.BOOL;
        }
        //-----------------------------------------------------------       
        public Type Visit(GreaterEqualNode node)
        {
            VisitBinaryOperator(node, Type.INT);
            return Type.BOOL;
        }
        //-----------------------------------------------------------
        public Type Visit(SubNode node)
        {
            VisitBinaryOperator(node, Type.INT);
            return Type.INT;
        }
        //-----------------------------------------------------------       
        public Type Visit(AddNode node)
        {
            VisitBinaryOperator(node, Type.INT);
            return Type.INT;
        }
        //-----------------------------------------------------------
        public Type Visit(MulNode node)
        {
            VisitBinaryOperator(node, Type.INT);
            return Type.INT;
        }
        //-----------------------------------------------------------    
        public Type Visit(QuoNode node)
        {
            VisitBinaryOperator(node, Type.INT);
            return Type.INT;
        }
        //-----------------------------------------------------------
        public Type Visit(RemNode node)
        {
            VisitBinaryOperator(node, Type.INT);
            return Type.INT;
        }
        //-----------------------------------------------------------


        //-----------------------------------------------------------
        // MISCELLANEOUS FUNCTIONS
        //-----------------------------------------------------------
        public void LoadPredefinedFunctions()
        {
            procedureTable["WrInt"] = new ProcedureTable.ProcedureData
            (Type.VOID, true);
            procedureTable["WrInt"].symbols["i"] = new SymbolTable.Register(Type.INT, ProcedureType.PARAM, 0);

            procedureTable["WrStr"] = new ProcedureTable.ProcedureData
            (Type.VOID, true);
            procedureTable["WrStr"].symbols["s"] = new SymbolTable.Register(Type.STR, ProcedureType.PARAM, 0);

            procedureTable["WrBool"] = new ProcedureTable.ProcedureData
            (Type.VOID, true);
            procedureTable["WrBool"].symbols["b"] = new SymbolTable.Register(Type.BOOL, ProcedureType.PARAM, 0);

            procedureTable["WrLn"] = new ProcedureTable.ProcedureData
            (Type.VOID, true);

            procedureTable["RdInt"] = new ProcedureTable.ProcedureData
            (Type.INT, true);
            procedureTable["RdStr"] = new ProcedureTable.ProcedureData
            (Type.STR, true);

            procedureTable["AtStr"] = new ProcedureTable.ProcedureData
            (Type.STR, true);
            procedureTable["AtStr"].symbols["s"] = new SymbolTable.Register(Type.STR, ProcedureType.PARAM, pos: 0);
            procedureTable["AtStr"].symbols["i"] = new SymbolTable.Register(Type.INT, ProcedureType.PARAM, pos: 1);

            procedureTable["LenStr"] = new ProcedureTable.ProcedureData
            (Type.INT, true);
            procedureTable["LenStr"].symbols["s"] = new SymbolTable.Register(Type.STR, ProcedureType.PARAM, pos: 0);

            procedureTable["CmpStr"] = new ProcedureTable.ProcedureData
            (Type.INT, true);
            procedureTable["CmpStr"].symbols["s1"] = new SymbolTable.Register(Type.STR, ProcedureType.PARAM, pos: 0);
            procedureTable["CmpStr"].symbols["s2"] = new SymbolTable.Register(Type.STR, ProcedureType.PARAM, pos: 1);

            procedureTable["CatStr"] = new ProcedureTable.ProcedureData
            (Type.STR, true);
            procedureTable["CatStr"].symbols["s1"] = new SymbolTable.Register(Type.STR, ProcedureType.PARAM, pos: 0);
            procedureTable["CatStr"].symbols["s2"] = new SymbolTable.Register(Type.STR, ProcedureType.PARAM, pos: 1);

            procedureTable["LenLstInt"] = new ProcedureTable.ProcedureData
            (Type.INT, true);
            procedureTable["LenLstInt"].symbols["loi"] = new SymbolTable.Register(Type.INT_LIST, ProcedureType.PARAM, pos: 0);

            procedureTable["LenLstStr"] = new ProcedureTable.ProcedureData
            (Type.INT, true);
            procedureTable["LenLstStr"].symbols["los"] = new SymbolTable.Register(Type.STR_LIST, ProcedureType.PARAM, pos: 0);

            procedureTable["LenLstBool"] = new ProcedureTable.ProcedureData
            (Type.INT, true);
            procedureTable["LenLstBool"].symbols["lob"] = new SymbolTable.Register(Type.BOOL_LIST, ProcedureType.PARAM, pos: 0);

            procedureTable["NewLstInt"] = new ProcedureTable.ProcedureData
            (Type.INT_LIST, true);
            procedureTable["NewLstInt"].symbols["size"] = new SymbolTable.Register(Type.INT, ProcedureType.PARAM, pos: 0);

            procedureTable["NewLstStr"] = new ProcedureTable.ProcedureData
            (Type.STR_LIST, true);
            procedureTable["NewLstStr"].symbols["size"] = new SymbolTable.Register(Type.INT, ProcedureType.PARAM, pos: 0);

            procedureTable["NewLstBool"] = new ProcedureTable.ProcedureData
            (Type.BOOL_LIST, true);
            procedureTable["NewLstBool"].symbols["size"] = new SymbolTable.Register(Type.INT, ProcedureType.PARAM, pos: 0);

            procedureTable["IntToStr"] = new ProcedureTable.ProcedureData
            (Type.STR, true);
            procedureTable["IntToStr"].symbols["i"] = new SymbolTable.Register(Type.INT, ProcedureType.PARAM, pos: 0);

            procedureTable["StrToInt"] = new ProcedureTable.ProcedureData
            (Type.INT, true);
            procedureTable["StrToInt"].symbols["s"] = new SymbolTable.Register(Type.STR, ProcedureType.PARAM, pos: 0);
        }
        //-----------------------------------------------------------
        void VisitChildren(Node node, int skip = 0, int take = 0)
        {
            skip = Math.Min(skip, node.Count());
            if (take == 0)
            {
                take = node.Count() - skip;
            }
            foreach (var n in node.Skip(skip).Take(take))
            {
                Visit((dynamic)n);
            }
        }
        void VisitBinaryOperator(Node node, Type type)
        {
            if (Visit((dynamic)node[0]) != type ||
                Visit((dynamic)node[1]) != type)
            {
                throw new SemanticError(
                    System.String.Format(
                        "Operator {0} requires two operands of type {1}",
                        node.AnchorToken.Lexeme,
                        type),
                    node.AnchorToken);
            }
        }
        //-----------------------------------------------------------
        SymbolTable SelectSymbolTable()
        {
            SymbolTable table;
            if (insideProcedure.Length == 0)
            {
                table = symbolTable;
            }
            else
            {
                table = procedureTable[insideProcedure].symbols;
            }
            return table;
        }
        SymbolTable.Register ObtainSymbolByKey(string key)
        {
            if (symbolTable.Contains(key))
            {
                return symbolTable[key];
            }
            else if (insideProcedure.Length > 0 && procedureTable[insideProcedure].symbols.Contains(key))
            {
                return procedureTable[insideProcedure].symbols[key];
            }else{
                return null;
            }            
        }
        public dynamic ObtainLiteralValue(Node node)
        {
            if (node is IntLiteralNode)
            {
                return Convert.ToInt32(node.AnchorToken.Lexeme);
            }
            else if (node is StrLiteralNode)
            {
                return node.AnchorToken.Lexeme;
            }
            else if (node is TrueNode)
            {
                return Convert.ToBoolean(node.AnchorToken.Lexeme);
            }
            else if (node is FalseNode)
            {
                return Convert.ToBoolean(node.AnchorToken.Lexeme);
            }
            else if (node is ListLiteralNode)
            {
                if (node.Count() == 0)
                {
                    throw new SemanticError("Cannot get the value of an empty list", node.AnchorToken);
                }
                Type t = Visit((dynamic)node[0]);
                switch (t)
                {
                    case Type.INT:
                        return node.Select(n => (int)ObtainLiteralValue(n)).ToArray();
                    case Type.BOOL:
                        return node.Select(n => (bool)ObtainLiteralValue(n)).ToArray();
                    case Type.STR:
                        return node.Select(n => (string)ObtainLiteralValue(n)).ToArray();
                }
            }
            return null;
        }
        //-----------------------------------------------------------
    }
}