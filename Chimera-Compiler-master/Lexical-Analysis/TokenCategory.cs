/*
  Chimera compiler - Token categories for the scanner.
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

    enum TokenCategory {
      IDENTIFIER,
      EOF,
      ILLEGAL_CHAR,
      INT,
      INT_LITERAL,
      STR,
      STR_LITERAL,
      BOOL,
      TRUE,
      FALSE,
      CONST,
      VAR,
      PROGRAM,
      END,
      EOL,
      ASSIGN,
      DECL,
      PARENTHESIS_OPEN,
      PARENTHESIS_CLOSE,
      KEY_OPEN,
      KEY_CLOSE,
      BRACKETS_OPEN,
      BRACKETS_CLOSE,
      IF,
      THEN,
      ELSEIF,
      ELSE,
      FOR,
      IN,
      DO,
      EXIT,
      LOOP,
      RETURN,
      AND,
      OR,
      XOR,
      QUO,
      REM,
      COMP,
      NEG,
      PROCEDURE,
      BEGIN,
      LIST,
      OF,
      LIST_LITERAL,
      LIST_INX,
      EQUAL,
      INEQUAL,
      LESS,
      GREATER,
      LESSEQUAL,
      GREATEREQUAL,
      ADD,
      SUB,
      MUL,
      COMMA,
      WRINT,
      WRSTR,
      WRBOOL,
      WRLN,
      RDINT,
      RDSTR,
      ATSTR,
      LENSTR,
      CMPSTR,
      CATSTR,
      LENLSTINT,
      LENLSTSTR,
      LENLSTBOOL,
      NEWLSTINT,
      NEWLSTSTR,
      NEWLSTBOOL,
      INTTOSTR,
      STRTOINT




    }
}

