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
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Chimera
{

    public class Driver
    {

        const string VERSION = "0.4";

        //-----------------------------------------------------------
        static readonly string[] ReleaseIncludes = {
            "Lexical analysis",
            "Syntactic analysis",
            "AST construction",
            "Semantic analysis"
        };

        //-----------------------------------------------------------
        void PrintAppHeader() {
            Console.WriteLine("Chimera compiler, version " + VERSION);
            Console.WriteLine("Copyright \u00A9 2019 by Hector Takami & Ernesto Cervantes, ITESM CEM.");           
            Console.WriteLine("Inspired by \u00A9 Ariel Ortiz Buttercup Compiler ITESM CEM.");
            
            Console.WriteLine("This program is free software; you may "
                + "redistribute it under the terms of");
            Console.WriteLine("the GNU General Public License version 3 or "
                + "later.");
            Console.WriteLine("This program has absolutely no warranty.");
        }

        //-----------------------------------------------------------
        void PrintReleaseIncludes()
        {
            Console.WriteLine("Included in this release:");
            foreach (var phase in ReleaseIncludes)
            {
                Console.WriteLine("   * " + phase);
            }
        }

        //-----------------------------------------------------------
 void Run(string[] args)
        {

            PrintAppHeader();
            Console.WriteLine();
            PrintReleaseIncludes();
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.Error.WriteLine(
                    "Please specify the name of at least one input file.");
                Environment.Exit(1);
            }

            foreach (string inputPath in args)
            {
                try
                {
                    var input = File.ReadAllText(inputPath);
                    var parser = new Parser(new Scanner(input).Start().GetEnumerator());

                    var ast = parser.Program();
                    
                    var semantic = new SemanticAnalyzer();
                    semantic.Visit((dynamic)ast);
                    Console.WriteLine("Semantics OK.");
                    Console.WriteLine();
                    Console.WriteLine(semantic.symbolTable);
                    Console.WriteLine();
                    Console.WriteLine(semantic.procedureTable);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Exception on file: '{inputPath}'");
                    if (e is FileNotFoundException
                        || e is SyntaxError
                        || e is SemanticError)
                    {
                        Console.Error.WriteLine(e.Message);
                        Console.WriteLine("-----------");
                        Console.WriteLine(e.StackTrace);
                        Environment.Exit(1);
                    }

                    throw;
                }
            }
        }

        //-----------------------------------------------------------
        public static void Main(string[] args)
        {
            new Driver().Run(args);
        }
    }
}
