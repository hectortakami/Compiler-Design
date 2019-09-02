/*
  Chimera compiler - This class performs the lexical analysis, 
  (a.k.a. scanning).
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
using System.Text;
using System.Text.RegularExpressions;

namespace Chimera {

    class Scanner {

        readonly string input;
                                        
             /* 
                | (?<Negation>            [-]                             )

             */
        

        static readonly Regex regex = new Regex(
            @"  
                (?<Identifier>          [a-zA-Z]+?[_a-zA-Z0-9]*         )
              | (?<MultiLineComment>    [/][*](.|\n)*[*][/]             )
              | (?<SingleLineComment>   [/][/](.)*                      )
              | (?<IntLiteral>          \d+                             )
              | (?<StrLiteral>          [\""]([^\""]|[\""])*?[\""]      )
              | (?<Assignment>          [:][=]                          )
              | (?<Declaration>         [:]                             )
              | (?<ParLeft>             [(]                             )
              | (?<KeyLeft>             [{]                             )
              | (?<BrackLeft>           [[]                             )
              | (?<ParRight>            [)]                             )
              | (?<KeyRight>            [}]                             )
              | (?<BrackRight>          []]                             )
              | (?<Equal>               [=]                             )
              | (?<Inequal>             [<][>]                          )
              | (?<Less>                [<]                             )
              | (?<Greater>             [>]                             )
              | (?<LessEqual>           [<][=]                          )
              | (?<GreaterEqual>        [>][=]                          )
              | (?<Add>                 [+]                             )
              | (?<Substract>           [-]                             )
              | (?<Multiply>            [*]                             )
              | (?<Separator>           [,]                             )          
              | (?<Newline>             \n                              )    
              | (?<EndOfLine>           [;]                             )              
              | (?<WhiteSpace>          \s        )     # Must go anywhere after Newline.
              | (?<Other>               .         )     # Must be last: match any other character.
            ", 
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );
            
            /*     
      NEG
      LIST_LITERAL
      LIST_INX

      */
        static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"integer", TokenCategory.INT},
                {"string", TokenCategory.STR},
                {"boolean", TokenCategory.BOOL},
                {"true", TokenCategory.TRUE},
                {"false", TokenCategory.FALSE},
                {"const", TokenCategory.CONST},
                {"var", TokenCategory.VAR},
                {"end", TokenCategory.END},
                {"program", TokenCategory.PROGRAM},
                {"if", TokenCategory.IF},
                {"then", TokenCategory.THEN},
                {"elseif", TokenCategory.ELSEIF},
                {"else", TokenCategory.ELSE},
                {"for", TokenCategory.FOR},
                {"in", TokenCategory.IN},
                {"do", TokenCategory.DO},
                {"exit", TokenCategory.EXIT},
                {"loop", TokenCategory.LOOP},
                {"return", TokenCategory.RETURN},
                {"and", TokenCategory.AND},
                {"or", TokenCategory.OR},
                {"xor", TokenCategory.XOR},
                {"div", TokenCategory.QUO},
                {"rem", TokenCategory.REM},
                {"not", TokenCategory.COMP},
                {"procedure", TokenCategory.PROCEDURE},
                {"begin", TokenCategory.BEGIN},
                {"list", TokenCategory.LIST},
                {"of", TokenCategory.OF},
                {"WrInt", TokenCategory.WRINT},
                {"WrStr", TokenCategory.WRSTR},
                {"WrBool", TokenCategory.WRBOOL},
                {"WrLn", TokenCategory.WRLN},
                {"RdInt", TokenCategory.RDINT},
                {"RdStr", TokenCategory.RDSTR},
                {"AtStr", TokenCategory.ATSTR},
                {"LenStr", TokenCategory.LENSTR},
                {"CmpStr", TokenCategory.CMPSTR},
                {"CatStr", TokenCategory.CATSTR},
                {"LenLstInt", TokenCategory.LENLSTINT},
                {"LenLstStr", TokenCategory.LENLSTSTR},
                {"LenLstBool", TokenCategory.LENLSTBOOL},
                {"NewLstInt", TokenCategory.NEWLSTINT},
                {"NewLstStr", TokenCategory.NEWLSTSTR},
                {"NewLstBool", TokenCategory.NEWLSTBOOL},
                {"IntToStr", TokenCategory.INTTOSTR},
                {"StrToInt", TokenCategory.STRTOINT},

            };

        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"IntLiteral", TokenCategory.INT_LITERAL},
                {"StrLiteral", TokenCategory.STR_LITERAL},
                {"EndOfLine", TokenCategory.EOL},
                {"Assignment", TokenCategory.ASSIGN},
                {"Declaration", TokenCategory.DECL},
                {"ParLeft", TokenCategory.PARENTHESIS_OPEN},
                {"ParRight", TokenCategory.PARENTHESIS_CLOSE},
                {"KeyLeft", TokenCategory.KEY_OPEN},
                {"KeyRight", TokenCategory.KEY_CLOSE},
                {"BrackLeft", TokenCategory.BRACKETS_OPEN},
                {"BrackRight", TokenCategory.BRACKETS_CLOSE},
                {"Equal", TokenCategory.EQUAL},
                {"Inequal", TokenCategory.INEQUAL},
                {"Less", TokenCategory.LESS},
                {"Greater", TokenCategory.GREATER},
                {"LessEqual", TokenCategory.LESSEQUAL},
                {"GreaterEqual", TokenCategory.GREATEREQUAL},
                {"Add", TokenCategory.ADD},
                {"Substract", TokenCategory.SUB},
                {"Multiply", TokenCategory.MUL},         
                {"Separator", TokenCategory.PARAM_SEPARATOR},                         

            };

        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Start() {

            var row = 1;
            var columnStart = 0;

            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["Newline"].Success) {

                    // Found a new line.
                    row++;
                    columnStart = m.Index + m.Length;

                } else if (m.Groups["WhiteSpace"].Success 
                || m.Groups["SingleLineComment"].Success ) {

                    // Skip white space and comments.

                } else if (m.Groups["MultiLineComment"].Success) {
                    string s = m.Value;
                    var arr = s.Split('\n');
                    foreach (var item in arr) {row++; }
                    row--;


                } else if (m.Groups["Identifier"].Success) {

                    if (keywords.ContainsKey(m.Value)) {

                        // Matched string is a Chimera keyword.
                        yield return newTok(m, keywords[m.Value]);                                               

                    } else { 

                        // Otherwise it's just a plain identifier.
                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }

                } else if (m.Groups["Other"].Success) {

                    // Found an illegal character.
                    yield return newTok(m, TokenCategory.ILLEGAL_CHAR);

                } else {

                    // Match must be one of the non keywords.
                    foreach (var name in nonKeywords.Keys) {
                        if (m.Groups[name].Success) {
                            yield return newTok(m, nonKeywords[name]);
                            break;
                        }
                    }
                }
            }

            yield return new Token(null, 
                                   TokenCategory.EOF, 
                                   row, 
                                   input.Length - columnStart + 1);
        }
    }
}
