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
using System.Text;
using System.Collections.Generic;

namespace Chimera
{

    public class ProcedureTable : IEnumerable<KeyValuePair<string, ProcedureTable.Row>>
    {

        public class Row
        {
            public Row(Type type, bool isPredefined)
            {
                this.type = type;
                this.isPredefined = isPredefined;
                symbols = new SymbolTable();
            }
            public Type type { get; private set; }
            public bool isPredefined { get; private set; }
            public SymbolTable symbols { get; private set; }

            public override string ToString()
            {
                var symbolsSting = "";
                if (symbols.Count() > 0)
                {
                    symbolsSting = "\t" + symbols.ToString().Replace("\n", "\n\t");
                }
                return $"return_type [ {type} ]\tpredefined_proc [ {isPredefined} ] \n{symbolsSting}\n*************************************************************************\n";
            }
        }

        IDictionary<string, ProcedureTable.Row> data = new SortedDictionary<string, ProcedureTable.Row>();

        //-----------------------------------------------------------
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("\n=========================================================================\n");
            sb.Append("|\t\t\t    PROCEDURE TABLE \t\t\t\t|\n");
            sb.Append("=========================================================================\n\n");
            foreach (var entry in data)
            {
                if (entry.Value.isPredefined)
                {
                    continue;
                }
                sb.Append(String.Format(" proc_name [ {0} ]\t{1}\n",
                                        entry.Key,
                                        entry.Value));
            }
            sb.Append("=========================================================================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public ProcedureTable.Row this[string key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value;
            }
        }

        //-----------------------------------------------------------
        public bool Contains(string key)
        {
            return data.ContainsKey(key);
        }

        //-----------------------------------------------------------
        public IEnumerator<KeyValuePair<string, ProcedureTable.Row>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
