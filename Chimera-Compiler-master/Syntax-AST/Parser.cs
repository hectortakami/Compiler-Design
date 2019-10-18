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
            var declList = new DeclarationList();
            var stmtList = new StatementList();

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

            return new ProgramNode() {
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
        public Node Statement()
        {

            switch (CurrentToken)
            {
                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.PARENTHESIS_OPEN)
                    {
                        return CallStatement();
                    }
                    else
                    {
                        return AssignmentStatement();
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

        public Node ProcedureDeclaration()
        {

            Expect(TokenCategory.PROCEDURE);
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            while (CurrentToken == TokenCategory.IDENTIFIER)
            {
                ParameterDeclaration();
            }
            Expect(TokenCategory.PARENTHESIS_CLOSE);

            if (CurrentToken == TokenCategory.DECL)
            {
                Expect(TokenCategory.DECL);
                Type();
            }

            Expect(TokenCategory.EOL);
            while (CurrentToken == TokenCategory.CONST)
            {
                ConstantDeclaration();
            }
            while (CurrentToken == TokenCategory.VAR)
            {
                VariableDeclaration();
            }
            Expect(TokenCategory.BEGIN);
            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }
            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);

            var result = new Declaration()
            {

            };
            return result;
        }
        public Node ConstantDeclaration()
        {
            while (CurrentToken == TokenCategory.IDENTIFIER)
            {
                Expect(TokenCategory.IDENTIFIER);
                Expect(TokenCategory.ASSIGN);
                Literal();
                Expect(TokenCategory.EOL);
            }
            var result = new Declaration()
            {

            };
            return result;

        }
        public Node VariableDeclaration()
        {
            Expect(TokenCategory.VAR);
            while (CurrentToken == TokenCategory.IDENTIFIER)
            {
                Expect(TokenCategory.IDENTIFIER);
                while (CurrentToken == TokenCategory.COMMA)
                {
                    Expect(TokenCategory.COMMA);
                    Expect(TokenCategory.IDENTIFIER);
                }
                Expect(TokenCategory.DECL);
                Type();
                Expect(TokenCategory.EOL);
            }

            var result = new Declaration()
            {

            };
            return result;
        }
        public Node ParameterDeclaration()
        {
            Expect(TokenCategory.IDENTIFIER);
            while (CurrentToken == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                Expect(TokenCategory.IDENTIFIER);
            }
            Expect(TokenCategory.DECL);
            Type();
            Expect(TokenCategory.EOL);

            var result = new Declaration()
            {

            };
            return result;
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
            Expect(TokenCategory.LIST);
            Expect(TokenCategory.OF);
            var simpleType = SimpleType();
            return simpleType;
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
                    var strLiteral = new StrLiteralNode()
                    {
                        AnchorToken = Expect(TokenCategory.STR_LITERAL)
                    };
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
            if (CurrentToken == TokenCategory.KEY_OPEN)
            {
                Expect(TokenCategory.KEY_OPEN);
                SimpleLiteral();
                while (CurrentToken == TokenCategory.COMMA)
                {
                    Expect(TokenCategory.COMMA);
                    SimpleLiteral();
                }
                Expect(TokenCategory.KEY_CLOSE);
            }
            if (CurrentToken == TokenCategory.BRACKETS_OPEN)
            {
                Expect(TokenCategory.BRACKETS_OPEN);
                if (CurrentToken != TokenCategory.IDENTIFIER)
                {
                    SimpleLiteral();
                    while (CurrentToken == TokenCategory.COMMA)
                    {
                        Expect(TokenCategory.COMMA);
                        SimpleLiteral();
                    }
                }
                else
                {
                    Expression();
                    while (CurrentToken == TokenCategory.COMMA)
                    {
                        Expect(TokenCategory.COMMA);
                        Expression();
                    }
                }

                Expect(TokenCategory.BRACKETS_CLOSE);
            }

            var listLiteral = new IntLiteralNode()
            {
            };
            return listLiteral;

        }

        public Node ExitStatement()
        {
            var exitLiteral = new StatementNode()
            {
                AnchorToken = Expect(TokenCategory.EXIT)
            };
            Expect(TokenCategory.EOL);
            return exitLiteral;
            
        }
        public Node ReturnStatement()
        {
            Expect(TokenCategory.RETURN);
            Expression();
            Expect(TokenCategory.EOL);

            var statement = new StatementNode() {

            };
            return statement;
        }
        public Node ForStatement()
        {
            Expect(TokenCategory.FOR);
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.IN);
            Expression();
            Expect(TokenCategory.DO);
            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }
            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);

            var statement = new StatementNode() {

            };
            return statement;
        }
        public Node LoopStatement()
        {
            Expect(TokenCategory.LOOP);
            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }
            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);

            var statement = new StatementNode() {

            };
            return statement;
        }
        public Node IfStatement()
        {
            Expect(TokenCategory.IF);
            Expression();
            Expect(TokenCategory.THEN);
            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }
            while (CurrentToken == TokenCategory.ELSEIF)
            {
                Expect(TokenCategory.ELSEIF);
                Expression();
                Expect(TokenCategory.THEN);
                while (firstOfStatement.Contains(CurrentToken))
                {
                    Statement();
                }
            }
            while (CurrentToken == TokenCategory.ELSE)
            {
                Expect(TokenCategory.ELSE);
                while (firstOfStatement.Contains(CurrentToken))
                {
                    Statement();
                }
            }

            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);

            var statement = new StatementNode() {

            };
            return statement;
        }
        public Node CallStatement()
        {
            Expect(TokenCategory.PARENTHESIS_OPEN);
            if (CurrentToken != TokenCategory.PARENTHESIS_CLOSE)
            {
                Expression();
                while (CurrentToken == TokenCategory.COMMA)
                {
                    Expect(TokenCategory.COMMA);
                    Expression();
                }
            }
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.EOL);

            var statement = new StatementNode() {

            };
            return statement;

        }
        public Node AssignmentStatement()
        {
            if (CurrentToken == TokenCategory.BRACKETS_OPEN)
            {
                Expect(TokenCategory.BRACKETS_OPEN);
                Expression();
                Expect(TokenCategory.BRACKETS_CLOSE);
            }
            Expect(TokenCategory.ASSIGN);
            Expression();
            Expect(TokenCategory.EOL);

            var statement = new StatementNode() {

            };
            return statement;

        }

        public Node Expression()
        {
            return LogicExpression();
        }
        public Node LogicExpression()
        {
            RelationalExpression();
            while (firstOfLogicOperator.Contains(CurrentToken))
            {
                LogicOperator();
                RelationalExpression();
            }

            var expressionNode = new ExpressionNode() {

            };
            return expressionNode;
        }
        public Node RelationalExpression()
        {
            SumExpression();
            while (firstOfRelationalOperator.Contains(CurrentToken))
            {
                RelationalOperator();
                SumExpression();
            }

            var expressionNode = new ExpressionNode() {

            };
            return expressionNode;
        }
        public Node SumExpression()
        {
            MulExpression();
            while (firstOfSumOperator.Contains(CurrentToken))
            {
                SumOperator();
                MulExpression();
            }

            var expressionNode = new ExpressionNode() {

            };
            return expressionNode;
        }

        public Node MulExpression()
        {
            UnaryExpression();
            while (firstOfMulOperator.Contains(CurrentToken))
            {
                MulOperator();
                UnaryExpression();
            }

            var expressionNode = new ExpressionNode() {

            };
            return expressionNode;
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
                    var identifierToken = Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.PARENTHESIS_OPEN)
                    {
                       return Call();
                    }
                    if (CurrentToken == TokenCategory.BRACKETS_OPEN)
                    {
                       return List();
                    }
                    else{
                        var expressionIdentifierNode = new ExpressionNode() {
                            AnchorToken = identifierToken
                        };
                        return expressionIdentifierNode;
                    }
                    

                case TokenCategory.INT_LITERAL:
                case TokenCategory.STR_LITERAL:
                case TokenCategory.TRUE:
                case TokenCategory.FALSE:
                case TokenCategory.KEY_OPEN:
                    return Literal();

                case TokenCategory.BRACKETS_OPEN:
                    Expect(TokenCategory.BRACKETS_OPEN);
                    var expressionBracketsNode =Expression();
                    Expect(TokenCategory.BRACKETS_CLOSE);
                    return expressionBracketsNode;


                default:
                    throw new SyntaxError(firstOfSimpleExpression,
                                          tokenStream.Current);
            }
        }
        public Node Call()
        {
            Expect(TokenCategory.PARENTHESIS_OPEN);
            if (CurrentToken != TokenCategory.PARENTHESIS_CLOSE)
            {
                Expression();
                while (CurrentToken == TokenCategory.COMMA)
                {
                    Expect(TokenCategory.COMMA);
                    Expression();
                }
            }
            Expect(TokenCategory.PARENTHESIS_CLOSE);

            var expressionCallNode = new ExpressionNode() {

            };
            return expressionCallNode;
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