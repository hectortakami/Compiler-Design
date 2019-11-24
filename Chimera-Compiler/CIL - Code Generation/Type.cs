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

namespace Chimera
{
    public enum Type
    {
        BOOL,
        INT,
        VOID,
        STRING,
        LIST,
        INT_LIST,
        BOOL_LIST,
        STRING_LIST,
    }

    static class TypeMethods
    {

        public static Type ToListType(this Type t)
        {
            switch (t)
            {
                case Type.INT:
                    return Type.INT_LIST;
                case Type.STRING:
                    return Type.STRING_LIST;
                case Type.BOOL:
                    return Type.BOOL_LIST;
                default:
                    throw new Exception($"Type {t} has no equivalent list type");
            }
        }

        public static Type FromListType(this Type t)
        {
            switch (t)
            {
                case Type.INT_LIST:
                    return Type.INT;
                case Type.STRING_LIST:
                    return Type.STRING;
                case Type.BOOL_LIST:
                    return Type.BOOL;
                default:
                    throw new Exception($"List type {t} has no equivalent type");
            }
        }

        public static bool CompatibleWith(this Type t, Type other)
        {
            if (t == Type.LIST || other == Type.LIST)
            {
                Type otherType = t == Type.LIST ? other : t;
                var valid = new Type[] { Type.LIST, Type.BOOL_LIST, Type.INT_LIST, Type.STRING_LIST };
                return valid.Contains(otherType);
            }
            return t == other;
        }

        public static string ToCilType(this Type type)
        {
            switch (type)
            {
                case Type.BOOL:
                    return "bool";
                case Type.INT:
                    return "int32";
                case Type.STRING:
                    return "string";
                case Type.BOOL_LIST:
                case Type.INT_LIST:
                    return "int32[]";
                case Type.STRING_LIST:
                    return "string[]";
                case Type.VOID:
                    return "void";
            }
            throw new Exception($"Could not find CIL type for: {type}");
        }

        public static dynamic DefaultValue(this Type t)
        {
            switch (t)
            {
                case Type.BOOL:
                    return false;
                case Type.INT:
                    return 0;
                case Type.STRING:
                    return "";
                case Type.INT_LIST:
                    return new int[] { 0 };
                case Type.BOOL_LIST:
                    return new bool[] { false };
                case Type.STRING_LIST:
                    return new string[] { "" };
            }
            return null;
        }
    }
}
