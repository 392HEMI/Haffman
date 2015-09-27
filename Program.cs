using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haffman
{
    class Program
    {
        class Node
        {
            private char[] chars;
            private int value;
            private Node a;
            private Node b;


            public char[] Chars
            {
                get { return chars; }
                set { chars = value; }
            }

            public int Value
            {
                get { return value; }
                set { this.value = value; }
            }

            public Node A
            {
                get { return a; }
                set { a = value; }
            }

            public Node B
            {
                get { return b; }
                set { b = value; }
            }

            public bool IsEnd
            {
                get { return chars.Length == 1; }
            }


            public Node(char chr, int value)
            {
                chars = new char[1] { chr };
                this.value = value;
            }

            public Node(Node a, Node b)
            {
                this.a = a;
                this.b = b;
                this.value = a.Value + b.Value;
                this.Chars = new char[a.Chars.Length + b.Chars.Length];
                Array.Copy(a.Chars, 0, this.chars, 0, a.Chars.Length);
                Array.Copy(b.Chars, 0, this.chars, a.Chars.Length, b.Chars.Length);
            }
        }

        static int min_node(Node[] nodes, int ignore)
        {
            Node node = null;
            int index = 0;
            for (int i = 0; i < nodes.Length; i++)
                if (nodes[i] != null && i != ignore)
                {
                    node = nodes[i];
                    index = i;
                    break;
                }
            for (int i = 0; i < nodes.Length; i++)
                if (nodes[i] != null && i != ignore && nodes[i].Value < node.Value)
                {
                    node = nodes[i];
                    index = i;
                }
            return index;
        }

        static void calc(string text, out char[] chars, out int[] counts)
        {
            HashSet<char> chars_set = new HashSet<char>();

            foreach (char c in text)
                chars_set.Add(c);

            chars = chars_set.ToArray();
            counts = new int[chars.Length];

            foreach (char c in text)
                for (int i = 0; i < chars.Length; i++)
                    if (chars[i] == c)
                        counts[i]++;
        }

        static void sort(char[] chars, int[] counts)
        {
            for (int i = chars.Length; i > 0; i--)
                for (int j = 0; j < i - 1; j++)
                    if (counts[j] > counts[j + 1])
                    {
                        char chr = chars[j];
                        chars[j] = chars[j + 1];
                        chars[j + 1] = chr;
                        int n = counts[j];
                        counts[j] = counts[j + 1];
                        counts[j + 1] = n;
                    }
        }

        static Node tree(char[] chars, int[] counts)
        {
            Node[] nodes = new Node[chars.Length];
            for (int i = 0; i < nodes.Length; i++)
                nodes[i] = new Node(chars[i], counts[i]);
            
            do
            {
                int a_index = min_node(nodes, -1);
                int b_index = min_node(nodes, a_index);
                Node c = new Node(nodes[a_index], nodes[b_index]);
                nodes[a_index] = c;
                nodes[b_index] = null;

                int count = 0;
                for (int i = 0; i < nodes.Length; i++)
                    if (nodes[i] != null)
                        count++;
                if (count == 1)
                    break;
            }
            while (true);

            Node node = null;
            for (int i = 0; i < nodes.Length; i++)
                if (nodes[i] != null)
                {
                    node = nodes[i];
                    break;
                }
            return node;
        }

        static bool[] calc_code(Node node, char c)
        {
            bool[] code = new bool[0];

            while (true)
            {
                if (node.IsEnd)
                    break;
                Array.Resize<bool>(ref code, code.Length + 1);
                if (node.A.Chars.Contains(c))
                {
                    node = node.A;
                    code[code.Length - 1] = false;
                }
                else
                {
                    node = node.B;
                    code[code.Length - 1] = true;
                }
            }
            return code;
        }
        
        static bool[] encode(string text, out Node node)
        {
            char[] chars;
            int[] counts;
            calc(text, out chars, out counts);
            sort(chars, counts);
            node = tree(chars, counts);

            bool[] encoded_text = new bool[0];
            for (int i = 0; i < text.Length; i++)
            {
                bool[] code = calc_code(node, text[i]);
                int length = encoded_text.Length;
                Array.Resize<bool>(ref encoded_text, length + code.Length);
                Array.Copy(code, 0, encoded_text, length, code.Length);
            }

            return encoded_text;
        }

        static string decode(bool[] encoded_text, Node node)
        {
            Node node_a = node;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < encoded_text.Length;)
            {
                while (!node.IsEnd)
                {
                    if (encoded_text[i++])
                        node = node.B;
                    else
                        node = node.A;
                }
                builder.Append(node.Chars[0]);
                node = node_a;
            }
            return builder.ToString();
        }

        static void Main(string[] args)
        {
            Console.Write("Enter text: ");
            string text = Console.ReadLine();
            Node node;
            bool[] encoded_text = encode(text, out node);
            foreach (bool b in encoded_text)
                Console.Write(b ? "1" : "0");
            Console.WriteLine();
            string decoded_text = decode(encoded_text, node);
            Console.WriteLine(decoded_text);
            Console.ReadKey();
        }
    }
}