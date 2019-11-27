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
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Chimera
{

    class Node : IEnumerable<Node>
    {

        private static readonly Dictionary<TokenCategory, System.Type> nodeForToken =
            new Dictionary<TokenCategory, System.Type>() {
                { TokenCategory.PLUS, typeof(AddNode) },
                { TokenCategory.MINUS, typeof(SubNode) },

                { TokenCategory.AND, typeof(AndNode) },
                { TokenCategory.OR, typeof(OrNode) },
                { TokenCategory.XOR, typeof(XORNode) },
                { TokenCategory.NOT, typeof(CompNode) },

                { TokenCategory.TIMES, typeof(MulNode) },
                { TokenCategory.DIV, typeof(QuoNode) },
                { TokenCategory.REM, typeof(RemNode) },

                { TokenCategory.EQUAL, typeof(EqualNode) },
                { TokenCategory.UNEQUAL, typeof(InequalNode) },
                { TokenCategory.LESS_THAN, typeof(LessNode) },
                { TokenCategory.MORE_THAN, typeof(GreaterNode) },
                { TokenCategory.LESS_THAN_EQUAL, typeof(LessEqualNode) },
                { TokenCategory.MORE_THAN_EQUAL, typeof(GreaterEqualNode) },

                { TokenCategory.INT_LITERAL, typeof(IntLiteralNode) },
                { TokenCategory.STRING_LITERAL, typeof(StrLiteralNode) },
                { TokenCategory.TRUE, typeof(TrueNode) },
                { TokenCategory.FALSE, typeof(FalseNode) },

                { TokenCategory.INTEGER, typeof(IntNode) },
                { TokenCategory.STRING, typeof(StrNode) },
                { TokenCategory.BOOLEAN, typeof(BoolNode) },

                { TokenCategory.IDENTIFIER, typeof(IdentifierNode) }
            };

        public static Node fromToken(Token token)
        {
            var node = (Node)Activator.CreateInstance(nodeForToken[token.Category]);
            node.AnchorToken = token;
            return node;
        }

        List<Node> children = new List<Node>();

        public dynamic extra { get; set; }

        public Node this[int index]
        {
            get
            {
                return children[index];
            }
        }

        public Token AnchorToken { get; set; }

        private static int lastId = 0;
        private int id { get; set; }

        public Node()
        {
            id = lastId++;
        }

        public void Add(Node node)
        {
            if (node == null)
            {
                return;
            }
            children.Add(node);
        }

        public void Add(List<Node> nodes)
        {
            children.AddRange(nodes.Where(n => n != null));
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", GetType().Name, AnchorToken);
        }

        public string ToStringTree()
        {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }

        static void TreeTraversal(Node node, string indent, StringBuilder sb)
        {
            sb.Append(indent);
            sb.Append(node);
            sb.Append('\n');
            foreach (var child in node.children)
            {
                TreeTraversal(child, indent + "  ", sb);
            }
        }

        public string ToGraphStringTree()
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph AST {");
            GraphTreeTraversal(this, "", sb);
            sb.AppendLine("}");
            return sb.ToString();
        }

        static void GraphTreeTraversal(Node node, string indent, StringBuilder sb)
        {
            var nodeName = node.GetType().Name;
            sb.AppendLine($"\t{node.id} [label=\"{nodeName.Remove(nodeName.Length - 4)}\\n{node.AnchorToken?.ToEscapedString()}\"];");
            foreach (var child in node.children)
            {
                sb.AppendLine($"\t{node.id}->{child.id};");
                GraphTreeTraversal(child, indent + "  ", sb);
            }
        }

    }
}
