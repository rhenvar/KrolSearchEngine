using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchEngineWebRole
{
    /// <summary>
    /// TrieNode stores character, average word viewcount
    /// of child words, and reference to children nodes
    /// </summary>
    public class TrieNode : IComparable
    {
        private const int SUFFIX_SIZE = 30;

        public double AverageViewCount { get; set; }
        public int TotalWords { get; set; }

        public bool IsWord { get; set; }
        public List<string> Suffixes;
        public Dictionary<char, TrieNode> Nodes;

        public TrieNode()
        {
            IsWord = false;
            Suffixes = new List<string>();
        }

        public TrieNode(char character)
        {
            IsWord = false;
        }

        public void AddSuffix(string partialSuffix)
        {
            if (Suffixes != null)
            {
                Suffixes.Add(partialSuffix);
                if (Suffixes.Count == SUFFIX_SIZE)
                {
                    // reorder this
                    ProcessSuffixes();
                }
            }
        }

        private void ProcessSuffixes()
        {
            Nodes = new Dictionary<char, TrieNode>();
            foreach (string suffix in Suffixes)
            {
                //Nodes[suffix[0]] = new TrieNode();
                char firstSuffixChar = suffix[0];
                if (!Nodes.ContainsKey(firstSuffixChar))
                {
                    Nodes[firstSuffixChar] = new TrieNode();
                }
                if (suffix.Length > 1)
                {
                    Nodes[firstSuffixChar].AddSuffix(suffix.Substring(1));
                }

            }
            Suffixes = null;
        }

        public int CompareTo(object obj)
        {
            TrieNode other = obj as TrieNode;
            if (AverageViewCount == other.AverageViewCount)
            {
                return 1;
            }
            return Convert.ToInt32(AverageViewCount - other.AverageViewCount);
        }
    }
}