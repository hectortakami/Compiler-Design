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

namespace Chimera
{

    class Parser
    {

        static readonly ISet<TokenCategory> firstOfDeclaration =
            new HashSet<TokenCategory>() {
                //Type ‹simple-type› | ‹list-type› | <constant-declaration> | <var-declaration> | <program-declaration> | <procedure-declaration>
                TokenCategory.CONST,
                TokenCategory.VAR,
                TokenCategory.PROCEDURE
            };

        static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                //Statement  ‹call-statement› | <assign-statement> | ‹if-statement› | ‹loop-statement› | ‹for-statement› | ‹return-statement› | ‹exit-statement›
                TokenCategory.IDENTIFIER,
                TokenCategory.IF,
                TokenCategory.LOOP,
                TokenCategory.FOR,
                TokenCategory.RETURN,
                TokenCategory.EXIT
            };

        static readonly ISet<TokenCategory> firstOfLogicOperator =
            new HashSet<TokenCategory>() {
                TokenCategory.AND,
                TokenCategory.OR,
                TokenCategory.XOR
            };

        static readonly ISet<TokenCategory> firstOfRelationalOperator =
            new HashSet<TokenCategory>() {
                TokenCategory.EQUAL,
                TokenCategory.INEQUAL,
                TokenCategory.LESSEQUAL,
                TokenCategory.GREATEREQUAL,
                TokenCategory.LESS,
                TokenCategory.GREATER
            };

        static readonly ISet<TokenCategory> firstOfSumOperator =
            new HashSet<TokenCategory>() {
                TokenCategory.ADD,
                TokenCategory.SUB
            };

        static readonly ISet<TokenCategory> firstOfMulOperator =
            new HashSet<TokenCategory>() {
                TokenCategory.MUL,
                TokenCategory.QUO,
                TokenCategory.REM
            };

        static readonly ISet<TokenCategory> firstOfSimpleExpression =
            new HashSet<TokenCategory>() {
                TokenCategory.PARENTHESIS_OPEN,
                TokenCategory.IDENTIFIER,
                TokenCategory.INT_LITERAL,
                TokenCategory.STR_LITERAL,
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.BRACKETS_OPEN
            };

        IEnumerator<Token> tokenStream;

        public Parser(IEnumerator<Token> tokenStream)
        {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }

        public TokenCategory CurrentToken
        {
            get { return tokenStream.Current.Category; }

        }

        public Token Expect(TokenCategory category)
        {
            if (CurrentToken == category)
            {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            }
            else
            {
                throw new SyntaxError(category, tokenStream.Current);
            }
        }

        public Node Program()
        {
            var declList = new Declaration_List();
            var stmtList = new Statement_List();

            while (firstOfDeclaration.Contains(CurrentToken))
            {
                declList.Add(Declaration());
            }
            Expect(TokenCategory.PROGRAM);
            while (firstOfStatement.Contains(CurrentToken))
            {
                stmtList.Add(Statement());
            }
            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);
            Expect(TokenCategory.EOF);

            return new Program_Node() {
                declList,
                stmtList
            };

            
        }

        public Node Declaration()
        {
            switch (CurrentToken)
            {
                case TokenCategory.CONST:
                    return ConstantDeclaration();

                case TokenCategory.VAR:
                    return VariableDeclaration();

                case TokenCategory.PROCEDURE:
                    return ProcedureDeclaration();

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);

            }

        }

        public Node ProcedureDeclaration()
        {
            var procedureNode = new Procedure_Declaration(){
                AnchorToken = Expect(TokenCategory.PROCEDURE)
            };

            var procedureName = new Identifier(){
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };

            var declList = new Declaration_List();
            var stmtList = new Statement_List();
            
            Expect(TokenCategory.PARENTHESIS_OPEN);
            while (CurrentToken == TokenCategory.IDENTIFIER)
            {
                declList.Add(ParameterDeclaration());
            }
            Expect(TokenCategory.PARENTHESIS_CLOSE);

            if (CurrentToken == TokenCategory.DECL)
            {
                var declNode = new Identifier() {
                    AnchorToken = Expect(TokenCategory.DECL)
                };
                declNode.Add(procedureName);
                declNode.Add(Type());
                
                procedureNode.Add(declNode);
            }else{
                procedureNode.Add(procedureName);
            }

            Expect(TokenCategory.EOL);
            

            while (CurrentToken == TokenCategory.CONST)
            {
                declList.Add(ConstantDeclaration());
            }
            while (CurrentToken == TokenCategory.VAR)
            {
                declList.Add(VariableDeclaration());
            }


            stmtList.AnchorToken = (Expect(TokenCategory.BEGIN));
            while (firstOfStatement.Contains(CurrentToken))
            {
                stmtList.Add(Statement());
            }
            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);

            procedureName.Add(declList);
            procedureName.Add(stmtList);

            return procedureNode;
        }

        public Node ConstantDeclaration()
        {
            var constNode = new Constant_Declaration() {
                AnchorToken = Expect(TokenCategory.CONST)
            };
            while (CurrentToken == TokenCategory.IDENTIFIER)
            {                
                var identifierNode = new Identifier() {
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                };
                var assignmentNode = new Identifier() {
                    AnchorToken = Expect(TokenCategory.ASSIGN)
                };
                
                assignmentNode.Add(identifierNode);
                assignmentNode.Add( Literal() );
                Expect(TokenCategory.EOL);

                constNode.Add(assignmentNode);
            }

            
            return constNode;

        }

        public Node VariableDeclaration()
        {
            var varNode = new Variable_Declaration();
            varNode.AnchorToken = Expect(TokenCategory.VAR);

            while (CurrentToken == TokenCategory.IDENTIFIER)
            {
                var id1Node = new Identifier(){
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                };
                while (CurrentToken == TokenCategory.COMMA)
                {                    
                    var commaNode = new Identifier(){
                        AnchorToken = Expect(TokenCategory.COMMA)
                    };
                    var id2Node = new Identifier(){
                        AnchorToken = Expect(TokenCategory.IDENTIFIER)
                    };

                    commaNode.Add(id1Node);
                    commaNode.Add(id2Node);

                    id1Node = commaNode;

                }
                var declNode = new Identifier(){
                    AnchorToken = Expect(TokenCategory.DECL)
                };
                declNode.Add(id1Node);
                declNode.Add(Type());

                Expect(TokenCategory.EOL);
                varNode.Add(declNode);

            }
            
            return varNode;
        }

        public Node ParameterDeclaration()
        {
            var identifierNode = new Identifier() {
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            while (CurrentToken == TokenCategory.COMMA)
            {               
                var commaNode = new Identifier() {
                    AnchorToken = Expect(TokenCategory.COMMA)
                };
                var identifier2 = new Identifier() {
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                };
                commaNode.Add(identifierNode);
                commaNode.Add(identifier2);

                identifierNode = commaNode;
            }

            var paramNode = new Parameter_Declaration()
            {
                AnchorToken = Expect(TokenCategory.DECL)
            };
            paramNode.Add(identifierNode);
            paramNode.Add(Type());

            Expect(TokenCategory.EOL);
            return paramNode;
        }


        public Node Type()
        {
            switch (CurrentToken)
            {

                case TokenCategory.INT:
                case TokenCategory.STR:
                case TokenCategory.BOOL:
                    return SimpleType();

                case TokenCategory.LIST:
                    return ListType();

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);
            }
        }
        public Node SimpleType()
        {
            switch (CurrentToken)
            {

                case TokenCategory.INT:
                    var intType = new IntNode()
                    {
                        AnchorToken = Expect(TokenCategory.INT)
                    };
                    return intType;


                case TokenCategory.STR:
                    var strType = new StrNode()
                    {
                        AnchorToken = Expect(TokenCategory.STR)
                    };
                    return strType;

                case TokenCategory.BOOL:
                    var boolType = new BoolNode()
                    {
                        AnchorToken = Expect(TokenCategory.BOOL)
                    };
                    return boolType;

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);
            }

        }
        public Node ListType()
        {
            var listType = new ListTypeNode(){
                AnchorToken = Expect(TokenCategory.LIST)
            };            
            Expect(TokenCategory.OF);
            listType.Add(SimpleType());
            return listType;
        }


        public Node Literal()
        {
            switch (CurrentToken)
            {
                case TokenCategory.INT_LITERAL:
                case TokenCategory.STR_LITERAL:
                case TokenCategory.TRUE:
                case TokenCategory.FALSE:
                    return SimpleLiteral();

                case TokenCategory.KEY_OPEN:
                    return List();

                case TokenCategory.BRACKETS_OPEN:
                    return List();

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);
            }
        }
        public Node SimpleLiteral()
        {
            switch (CurrentToken)
            {

                case TokenCategory.INT_LITERAL:
                    var intLiteral = new IntLiteralNode()
                    {
                        AnchorToken = Expect(TokenCategory.INT_LITERAL)
                    };
                    return intLiteral;


                case TokenCategory.STR_LITERAL:
                    var str_literal = Expect(TokenCategory.STR_LITERAL);
                    var strLiteral = new StrLiteralNode() {};
                    strLiteral.AnchorToken = str_literal;
                    return strLiteral;

                case TokenCategory.TRUE:
                    var trueLiteral = new TrueNode()
                    {
                        AnchorToken = Expect(TokenCategory.TRUE)
                    };
                    return trueLiteral;

                case TokenCategory.FALSE:
                    var falseLiteral = new FalseNode()
                    {
                        AnchorToken = Expect(TokenCategory.FALSE)
                    };
                    return falseLiteral;

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);
            }
        }
        public Node List()
        {
            var listLiteral = new ListLiteralNode();

            if (CurrentToken == TokenCategory.KEY_OPEN)
            {
                Expect(TokenCategory.KEY_OPEN);
                var litList = SimpleLiteral();
                while (CurrentToken == TokenCategory.COMMA)
                {
                    var commaNode = new Identifier(){
                        AnchorToken = Expect(TokenCategory.COMMA)
                    };
                    commaNode.Add(litList);
                    commaNode.Add(SimpleLiteral());
                    
                    litList = commaNode;
                }
                listLiteral.Add(litList);
                Expect(TokenCategory.KEY_CLOSE);
            }
            if (CurrentToken == TokenCategory.BRACKETS_OPEN)
            {
                Expect(TokenCategory.BRACKETS_OPEN);
                if (CurrentToken != TokenCategory.IDENTIFIER)
                {
                    var litList = SimpleLiteral();
                    while (CurrentToken == TokenCategory.COMMA)
                    {
                        var commaNode = new Identifier(){
                            AnchorToken = Expect(TokenCategory.COMMA)
                        };
                        commaNode.Add(litList);
                        commaNode.Add(SimpleLiteral());
                    
                        litList = commaNode;
                    }
                    listLiteral.Add(litList);
                }
                else
                {
                    var expList = Expression();
                    while (CurrentToken == TokenCategory.COMMA)
                    {
                        var commaNode = new Identifier(){
                            AnchorToken = Expect(TokenCategory.COMMA)
                        };
                        commaNode.Add(expList);
                        commaNode.Add(Expression());
                    
                        expList = commaNode;
                    }
                    listLiteral.Add(expList);
                }

                Expect(TokenCategory.BRACKETS_CLOSE);
            }

            return listLiteral;

        }

        public Node Statement()
        {

            switch (CurrentToken)
            {
                case TokenCategory.IDENTIFIER:
                    var identifier = Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.PARENTHESIS_OPEN)
                    {
                        return CallStatement(identifier);                        
                    }
                    else
                    {
                        return AssignmentStatement(identifier);
                    }

                case TokenCategory.IF:
                    return IfStatement();

                case TokenCategory.LOOP:
                    return LoopStatement();

                case TokenCategory.FOR:
                    return ForStatement();

                case TokenCategory.RETURN:
                    return ReturnStatement();

                case TokenCategory.EXIT:
                    return ExitStatement();

                default:
                    throw new SyntaxError(firstOfStatement,
                                          tokenStream.Current);
            }
        }

        
        
        public Node CallStatement(Token identifier)
        {
            var callStatement = new Call_Statement(){
                AnchorToken = identifier
            };

            Expect(TokenCategory.PARENTHESIS_OPEN);            
            if (CurrentToken != TokenCategory.PARENTHESIS_CLOSE)
            {                
                var expr1 = Expression();
                while (CurrentToken == TokenCategory.COMMA)
                {
                    var expr2 = new Identifier(){
                        AnchorToken = Expect(TokenCategory.COMMA)
                    };
                    expr2.Add(expr1);
                    expr2.Add(Expression());
                    expr1 = expr2;
                }
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                Expect(TokenCategory.EOL);

                callStatement.Add(expr1);

            }else{
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                Expect(TokenCategory.EOL);
            }     
            
            return callStatement;       
        }

        public Node ExitStatement()
        {
            var exitNode = new Exit_Statement()
            {
                AnchorToken = Expect(TokenCategory.EXIT)
            };
            Expect(TokenCategory.EOL);
            return exitNode;
            
        }
        public Node ReturnStatement()
        {
            var returnNode = new Return_Statement() {
                AnchorToken = Expect(TokenCategory.RETURN)
            };
            returnNode.Add(Expression());

            Expect(TokenCategory.EOL);
            return returnNode;
        }
        public Node ForStatement()
        {
            var forNode = new For_Statement() {
                AnchorToken = Expect(TokenCategory.FOR)
            };
            var forID = new Identifier() {
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            var inNode = new IN_Statement() {
                AnchorToken = Expect(TokenCategory.IN)     
            };
            inNode.Add(forID);
            inNode.Add(Expression());

            var doNode = new DO_Statement() {
                AnchorToken = Expect(TokenCategory.DO)
            };
            
            var stmtList = new Statement_List();
            while (firstOfStatement.Contains(CurrentToken))
            {
                stmtList.Add(Statement());
            }

            doNode.Add(stmtList);

            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);

            forNode.Add(inNode);
            forNode.Add(doNode);
            return forNode;
        }
        public Node LoopStatement()
        {
            var loopNode = new Loop_Statement() {
                AnchorToken = Expect(TokenCategory.LOOP)
            };
            var stmtList = new Statement_List();
            while (firstOfStatement.Contains(CurrentToken))
            {
                stmtList.Add(Statement());
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);

            loopNode.Add(stmtList);
            return loopNode;
        }

        public Node IfStatement()
        {
            var ifNode = new If_Statement(){
                AnchorToken = Expect(TokenCategory.IF)
            };
            ifNode.Add(Expression());
            Expect(TokenCategory.THEN);

            var stmtList = new Statement_List();
            while (firstOfStatement.Contains(CurrentToken))
            {
                stmtList.Add(Statement());
            }
            ifNode.Add(stmtList);

            while (CurrentToken == TokenCategory.ELSEIF)
            {
                var elseIfNode = new ElseIf_Statement(){
                    AnchorToken = Expect(TokenCategory.ELSEIF)
                };
                
                elseIfNode.Add(Expression());
                Expect(TokenCategory.THEN);

                var stmtListElseIf = new Statement_List();
                while (firstOfStatement.Contains(CurrentToken))
                {
                    stmtListElseIf.Add(Statement());
                }

                elseIfNode.Add(stmtListElseIf);
                ifNode.Add(elseIfNode);
            }
            while (CurrentToken == TokenCategory.ELSE)
            {
                var elseNode = new Else_Statement(){
                    AnchorToken = Expect(TokenCategory.ELSE)
                };
                var stmtListElse = new Statement_List();
                while (firstOfStatement.Contains(CurrentToken))
                {
                   stmtListElse.Add(Statement());
                }

                elseNode.Add(stmtListElse);
                ifNode.Add(elseNode);
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);

            return ifNode;
        }
        

        public Node AssignmentStatement(Token identifier)
        {
            var identifierNode = new Identifier(){
                AnchorToken = identifier
            };

            if (CurrentToken == TokenCategory.BRACKETS_OPEN)
            {
                Expect(TokenCategory.BRACKETS_OPEN);
                identifierNode.Add(Expression());
                Expect(TokenCategory.BRACKETS_CLOSE);
            }

            var assignmentNode = new Assignment_Statement(){
                AnchorToken = Expect(TokenCategory.ASSIGN)
            };

            assignmentNode.Add(identifierNode);
            assignmentNode.Add(Expression());


            Expect(TokenCategory.EOL);
            
        
            return assignmentNode;
        }

        public Node Expression()
        {
            return LogicExpression();
        }
        public Node LogicExpression()
        {
            var expr1 = RelationalExpression();
            while (firstOfLogicOperator.Contains(CurrentToken))
            {
                var expr2 = LogicOperator();
                expr2.Add(expr1);
                expr2.Add(RelationalExpression());
                expr1 = expr2;
            }

            return expr1;
        }
        public Node RelationalExpression()
        {
            var expr1 = SumExpression();
            while (firstOfRelationalOperator.Contains(CurrentToken))
            {
                var expr2 = RelationalOperator();
                expr2.Add(expr1);
                expr2.Add(SumExpression());
                expr1 = expr2;
            }
            return expr1;
        }
        public Node SumExpression()
        {
            var expr1 = MulExpression();
            while (firstOfSumOperator.Contains(CurrentToken))
            {
                var expr2 = SumOperator();
                expr2.Add(expr1);
                expr2.Add(MulExpression());
                expr1 = expr2;
            }

            return expr1;
        }

        public Node MulExpression()
        {
            var expr1 = UnaryExpression();
            while (firstOfMulOperator.Contains(CurrentToken))
            {
                var expr2 = MulOperator();
                expr2.Add(UnaryExpression());
                expr1 = expr2;
            }

            return expr1;
        }

        public Node UnaryExpression()
        {
            switch (CurrentToken)
            {
                case TokenCategory.COMP:
                    Expect(TokenCategory.COMP);
                    return UnaryExpression();

                case TokenCategory.SUB:
                    Expect(TokenCategory.SUB);
                    return UnaryExpression();

                default:
                    return SimpleExpression();
            }
        }


        public Node SimpleExpression()
        {
            switch (CurrentToken)
            {
                case TokenCategory.PARENTHESIS_OPEN:
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    var expressionParenthesisNode = Expression();
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    return expressionParenthesisNode;

                case TokenCategory.IDENTIFIER:
                    var identifier = Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.PARENTHESIS_OPEN)
                    {
                       return Call(identifier);
                    }
                    if (CurrentToken == TokenCategory.BRACKETS_OPEN)
                    {
                       return List();
                    }
                    else{
                        var expressionIdentifier = new Identifier() {
                            AnchorToken = identifier
                        };
                        return expressionIdentifier;
                    }
                    

                case TokenCategory.INT_LITERAL:
                case TokenCategory.STR_LITERAL:
                case TokenCategory.TRUE:
                case TokenCategory.FALSE:
                case TokenCategory.KEY_OPEN:
                    return Literal();

                case TokenCategory.BRACKETS_OPEN:
                    Expect(TokenCategory.BRACKETS_OPEN);
                    var expressionBracketsNode = Expression();
                    Expect(TokenCategory.BRACKETS_CLOSE);
                    return expressionBracketsNode;


                default:
                    throw new SyntaxError(firstOfSimpleExpression,
                                          tokenStream.Current);
            }
        }
        public Node Call(Token identifier)
        {
            var callNode = new Call_Expression(){
                AnchorToken = identifier
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            if (CurrentToken != TokenCategory.PARENTHESIS_CLOSE)
            {
                var expr1 = Expression();
                while (CurrentToken == TokenCategory.COMMA)
                {
                    var commaNode = new Identifier(){
                        AnchorToken = Expect(TokenCategory.COMMA)
                    };
                    commaNode.Add(expr1);
                    commaNode.Add(Expression());

                    expr1 = commaNode;
                }

                callNode.Add(expr1);
            }
            Expect(TokenCategory.PARENTHESIS_CLOSE);

            
            return callNode;
        }


        public Node LogicOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.AND:
                    return new AndNode()
                    {
                        AnchorToken = Expect(TokenCategory.AND)
                    };

                case TokenCategory.OR:
                    return new OrNode()
                    {
                        AnchorToken = Expect(TokenCategory.OR)
                    };

                case TokenCategory.XOR:
                    return new XORNode()
                    {
                        AnchorToken = Expect(TokenCategory.XOR)
                    };

                default:
                    throw new SyntaxError(firstOfLogicOperator,
                                          tokenStream.Current);
            }
        }
        public Node RelationalOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.EQUAL:
                    return new EqualNode()
                    {
                        AnchorToken = Expect(TokenCategory.EQUAL)
                    };

                case TokenCategory.INEQUAL:
                    return new InequalNode()
                    {
                        AnchorToken = Expect(TokenCategory.INEQUAL)
                    };

                case TokenCategory.LESSEQUAL:
                    return new LessEqualNode()
                    {
                        AnchorToken = Expect(TokenCategory.LESSEQUAL)
                    };

                case TokenCategory.GREATEREQUAL:
                    return new GreaterEqualNode()
                    {
                        AnchorToken = Expect(TokenCategory.GREATEREQUAL)
                    };
                case TokenCategory.LESS:
                    return new LessNode()
                    {
                        AnchorToken = Expect(TokenCategory.LESS)
                    };

                case TokenCategory.GREATER:
                    return new GreaterNode()
                    {
                        AnchorToken = Expect(TokenCategory.GREATER)
                    };

                default:
                    throw new SyntaxError(firstOfRelationalOperator,
                                          tokenStream.Current);
            }
        }

        public Node SumOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.ADD:
                    return new AddNode()
                    {
                        AnchorToken = Expect(TokenCategory.ADD)
                    }; ;

                case TokenCategory.SUB:
                    return new SubNode()
                    {
                        AnchorToken = Expect(TokenCategory.SUB)
                    }; ;

                default:
                    throw new SyntaxError(firstOfSumOperator,
                                          tokenStream.Current);
            }
        }

        public Node MulOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.MUL:
                    return new MulNode()
                    {
                        AnchorToken = Expect(TokenCategory.MUL)
                    };

                case TokenCategory.QUO:
                    return new QuoNode()
                    {
                        AnchorToken = Expect(TokenCategory.QUO)
                    };

                case TokenCategory.REM:
                    return new RemNode()
                    {
                        AnchorToken = Expect(TokenCategory.REM)
                    };

                default:
                    throw new SyntaxError(firstOfMulOperator,
                                          tokenStream.Current);
            }
        }
    }
}