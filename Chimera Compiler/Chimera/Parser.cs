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

        public void Program()
        {

            while (firstOfDeclaration.Contains(CurrentToken))
            {
                Declaration();
            }
            Expect(TokenCategory.PROGRAM);
            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }
            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);

            Expect(TokenCategory.EOF);


        }

        public void Declaration()
        {
            switch (CurrentToken)
            {
                case TokenCategory.CONST:
                    ConstantDeclaration();
                    break;

                case TokenCategory.VAR:
                    VariableDeclaration();
                    break;

                case TokenCategory.PROCEDURE:
                    ProcedureDeclaration();
                    break;

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);

            }

        }
        public void ProcedureDeclaration()
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
        }
        public void ConstantDeclaration()
        {
            Expect(TokenCategory.CONST);
            while (CurrentToken == TokenCategory.IDENTIFIER)
            {
                Expect(TokenCategory.IDENTIFIER);
                Expect(TokenCategory.ASSIGN);
                Literal();
                Expect(TokenCategory.EOL);
            }

        }
        public void VariableDeclaration()
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
        }
        public void ParameterDeclaration()
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


        public void Type()
        {
            switch (CurrentToken)
            {

                case TokenCategory.INT:
                case TokenCategory.STR:
                case TokenCategory.BOOL:
                    SimpleType();
                    break;

                case TokenCategory.LIST:
                    ListType();
                    break;

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);
            }
        }
        public void SimpleType()
        {
            switch (CurrentToken)
            {

                case TokenCategory.INT:
                    Expect(TokenCategory.INT);
                    break;

                case TokenCategory.STR:
                    Expect(TokenCategory.STR);
                    break;

                case TokenCategory.BOOL:
                    Expect(TokenCategory.BOOL);
                    break;

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);
            }

        }
        public void ListType()
        {
            Expect(TokenCategory.LIST);
            Expect(TokenCategory.OF);
            SimpleType();
        }


        public void Literal()
        {
            switch (CurrentToken)
            {

                case TokenCategory.INT_LITERAL:
                case TokenCategory.STR_LITERAL:
                case TokenCategory.TRUE:
                case TokenCategory.FALSE:
                    SimpleLiteral();
                    break;

                case TokenCategory.KEY_OPEN:
                    List();
                    break;

                case TokenCategory.BRACKETS_OPEN:
                    List();
                    break;

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);
            }
        }
        public void SimpleLiteral()
        {
            switch (CurrentToken)
            {

                case TokenCategory.INT_LITERAL:
                    Expect(TokenCategory.INT_LITERAL);
                    break;

                case TokenCategory.STR_LITERAL:
                    Expect(TokenCategory.STR_LITERAL);
                    break;

                case TokenCategory.TRUE:
                    Expect(TokenCategory.TRUE);
                    break;

                case TokenCategory.FALSE:
                    Expect(TokenCategory.FALSE);
                    break;

                default:
                    throw new SyntaxError(firstOfDeclaration,
                                          tokenStream.Current);
            }
        }
        public void List()
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

        }
        public void Statement()
        {

            switch (CurrentToken)
            {
                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.PARENTHESIS_OPEN)
                    {
                        CallStatement();
                    }
                    else
                    {
                        AssignmentStatement();
                    }
                    break;

                case TokenCategory.IF:
                    IfStatement();
                    break;

                case TokenCategory.LOOP:
                    LoopStatement();
                    break;

                case TokenCategory.FOR:
                    ForStatement();
                    break;

                case TokenCategory.RETURN:
                    ReturnStatement();
                    break;

                case TokenCategory.EXIT:
                    ExitStatement();
                    break;

                default:
                    throw new SyntaxError(firstOfStatement,
                                          tokenStream.Current);
            }
        }

        public void ExitStatement()
        {
            Expect(TokenCategory.EXIT);
            Expect(TokenCategory.EOL);
        }
        public void ReturnStatement()
        {
            Expect(TokenCategory.RETURN);
            Expression();
            Expect(TokenCategory.EOL);
        }
        public void ForStatement()
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
        }
        public void LoopStatement()
        {
            Expect(TokenCategory.LOOP);
            while (firstOfStatement.Contains(CurrentToken))
            {
                Statement();
            }
            Expect(TokenCategory.END);
            Expect(TokenCategory.EOL);
        }
        public void IfStatement()
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
        }
        public void CallStatement()
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

        }
        public void AssignmentStatement()
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

        }

        public void Expression()
        {
            LogicExpression();
        }
        public void LogicExpression()
        {
            RelationalExpression();
            while (firstOfLogicOperator.Contains(CurrentToken))
            {
                LogicOperator();
                RelationalExpression();
            }
        }
        public void RelationalExpression()
        {
            SumExpression();
            while (firstOfRelationalOperator.Contains(CurrentToken))
            {
                RelationalOperator();
                SumExpression();
            }
        }
        public void SumExpression()
        {
            MulExpression();
            while (firstOfSumOperator.Contains(CurrentToken))
            {
                SumOperator();
                MulExpression();
            }
        }

        public void MulExpression()
        {
            UnaryExpression();
            while (firstOfMulOperator.Contains(CurrentToken))
            {
                MulOperator();
                UnaryExpression();
            }
        }

        public void UnaryExpression()
        {
            switch (CurrentToken)
            {
                case TokenCategory.COMP:
                    Expect(TokenCategory.COMP);
                    UnaryExpression();
                    break;

                case TokenCategory.SUB:
                    Expect(TokenCategory.SUB);
                    UnaryExpression();
                    break;

                default:
                    SimpleExpression();
                    break;
            }
        }


        public void SimpleExpression()
        {
            switch (CurrentToken)
            {
                case TokenCategory.PARENTHESIS_OPEN:
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    Expression();
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    break;

                case TokenCategory.IDENTIFIER:
                    Expect(TokenCategory.IDENTIFIER);
                    if (CurrentToken == TokenCategory.PARENTHESIS_OPEN)
                    {
                        Call();
                    }
                    if (CurrentToken == TokenCategory.BRACKETS_OPEN)
                    {
                        List();
                    }
                    break;

                case TokenCategory.INT_LITERAL:
                case TokenCategory.STR_LITERAL:
                case TokenCategory.TRUE:
                case TokenCategory.FALSE:
                case TokenCategory.KEY_OPEN:
                    Literal();
                    break;

                case TokenCategory.BRACKETS_OPEN:
                    Expect(TokenCategory.BRACKETS_OPEN);
                    Expression();
                    Expect(TokenCategory.BRACKETS_CLOSE);
                    break;

                default:
                    throw new SyntaxError(firstOfSimpleExpression,
                                          tokenStream.Current);
            }
        }
        public void Call()
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
        }

        public void LogicOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.AND:
                    Expect(TokenCategory.AND);
                    break;

                case TokenCategory.OR:
                    Expect(TokenCategory.OR);
                    break;

                case TokenCategory.XOR:
                    Expect(TokenCategory.XOR);
                    break;

                default:
                    throw new SyntaxError(firstOfLogicOperator,
                                          tokenStream.Current);
            }
        }
        public void RelationalOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.EQUAL:
                    Expect(TokenCategory.EQUAL);
                    break;

                case TokenCategory.INEQUAL:
                    Expect(TokenCategory.INEQUAL);
                    break;

                case TokenCategory.LESSEQUAL:
                    Expect(TokenCategory.LESSEQUAL);
                    break;

                case TokenCategory.GREATEREQUAL:
                    Expect(TokenCategory.GREATEREQUAL);
                    break;

                case TokenCategory.LESS:
                    Expect(TokenCategory.LESS);
                    /*if (CurrentToken == TokenCategory.EQUAL)
                    {
                        Expect(TokenCategory.EQUAL);
                    }*/
                    break;

                case TokenCategory.GREATER:
                    Expect(TokenCategory.GREATER);
                    /*if (CurrentToken == TokenCategory.EQUAL)
                    {
                        Expect(TokenCategory.EQUAL);
                    }*/
                    break;



                default:
                    throw new SyntaxError(firstOfRelationalOperator,
                                          tokenStream.Current);
            }
        }

        public void SumOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.ADD:
                    Expect(TokenCategory.ADD);
                    break;

                case TokenCategory.SUB:
                    Expect(TokenCategory.SUB);
                    break;

                default:
                    throw new SyntaxError(firstOfSumOperator,
                                          tokenStream.Current);
            }
        }

        public void MulOperator()
        {
            switch (CurrentToken)
            {
                case TokenCategory.MUL:
                    Expect(TokenCategory.MUL);
                    break;

                case TokenCategory.QUO:
                    Expect(TokenCategory.QUO);
                    break;

                case TokenCategory.REM:
                    Expect(TokenCategory.REM);
                    break;

                default:
                    throw new SyntaxError(firstOfMulOperator,
                                          tokenStream.Current);
            }
        }
    }
}
