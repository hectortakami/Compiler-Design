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

namespace Chimera
{
    public class Lib
    {
        // Input/Output Operations
        public static void WrInt(int i)
        {
            Console.Write(i);
        }
        public static void WrStr(string s)
        {
            Console.Write(s);
        }
        public static void WrBool(bool b)
        {
            Console.Write(Convert.ToString(b).ToLower());
        }
        public static void WrLn()
        {
            Console.WriteLine();
        }
        public static int RdInt()
        {
            return Convert.ToInt32(Console.ReadLine());
        }
        public static string RdStr()
        {
            return Console.ReadLine();
        }

        // String Operations
        public static string AtStr(string s, int i)
        {
            return $"{s[i]}";
        }
        public static int LenStr(string s)
        {
            return s.Length;
        }
        public static int CmpStr(string s1, string s2)
        {
            return s1.CompareTo(s2);
        }
        public static string CatStr(string s1, string s2)
        {
            return s1 + s2;
        }

        // List Operations
        public static int LenLstInt(int[] loi)
        {
            return loi.Length;
        }
        public static int LenLstStr(string[] los)
        {
            return los.Length;
        }
        public static int LenLstBool(bool[] lob)
        {
            return lob.Length;
        }
        public static int[] NewLstInt(int size)
        {
            return new int[size];
        }
        public static string[] NewLstStr(int size)
        {
            return new string[size];
        }
        public static bool[] NewLstBool(int size)
        {
            return new bool[size];
        }

        // Conversion Operations
        public static string IntToStr(int i)
        {
            return $"{i}";
        }
        public static int StrToInt(string s)
        {
            return Convert.ToInt32(s);
        }
    }
}
