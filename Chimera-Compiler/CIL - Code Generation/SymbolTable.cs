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
using System.Text;
using System.Collections.Generic;

namespace Chimera
{

    public class SymbolTable : IEnumerable<KeyValuePair<string, SymbolTable.Row>>
    {

        public class Row
        {
            public Row(Type type, ProcedureType kind, int pos = -1, dynamic value = null)
            {
                this.type = type;
                this.kind = kind;
                this.pos = pos;
                this.value = value;
            }
            public Type type { get; private set; }
            public ProcedureType kind { get; private set; }
            public int pos { get; private set; }
            public dynamic value { get; private set; }

            public override string ToString()
            {
                var posString = pos == -1 ? "-" : $"{pos}";
                return $"{type}, {kind}, {posString}";
            }
        }

        IDictionary<string, SymbolTable.Row> data = new SortedDictionary<string, SymbolTable.Row>();

        //-----------------------------------------------------------
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Symbol Table\n");
            sb.Append("====================\n");
            foreach (var entry in data)
            {
                sb.Append(String.Format("{0}: {1}\n",
                                        entry.Key,
                                        entry.Value));
            }
            sb.Append("====================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public SymbolTable.Row this[string key]
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
        public IEnumerator<KeyValuePair<string, SymbolTable.Row>> GetEnumerator()
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
