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

namespace Chimera
{

    enum TokenCategory
    {
        IDENTIFIER,
        ILLEGAL_CHAR,
        EOF,
        PROGRAM,
        CONST,
        VAR,
        COLON,
        COMMA,
        SEMICOLON,
        COLON_EQUAL,
        END,
        INTEGER,
        INT_LITERAL,
        BOOLEAN,
        STRING,
        STRING_LITERAL,
        LIST,
        OF,
        CURLY_OPEN,
        CURLY_CLOSE,
        PARENTHESIS_OPEN,
        PARENTHESIS_CLOSE,
        PROCEDURE,
        BEGIN,
        BRACKET_OPEN,
        BRACKET_CLOSE,
        IF,
        THEN,
        ELSEIF,
        ELSE,
        LOOP,
        FOR,
        IN,
        DO,
        RETURN,
        EXIT,
        AND,
        OR,
        XOR,
        EQUAL,
        UNEQUAL,
        LESS_THAN,
        MORE_THAN,
        LESS_THAN_EQUAL,
        MORE_THAN_EQUAL,
        PLUS,
        MINUS,
        TIMES,
        DIV,
        REM,
        NOT,
        TRUE,
        FALSE,
    }
}

