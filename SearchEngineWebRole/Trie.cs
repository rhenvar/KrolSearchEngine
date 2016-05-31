using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SearchEngineWebRole
{
    /// <summary>
    /// Trie stores reference to 
    /// </summary>
    public class Trie
    {
        public TrieNode Root { get; set; }
        public int Size { get; private set; }
        public string LastTitle { get; private set; }

        public Trie(Stream stream)
        {
            Size = 0;
            Root = new TrieNode();
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    try
                    {
                        string word = reader.ReadLine();
                        string[] wordArray = word.Split('|');
                        word = wordArray[0].ToLower();
                        int viewCount = Convert.ToInt32(wordArray[1]);

                        // filter out unpopular pages
                        Add(word, viewCount);
                        LastTitle = word;
                        Size++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Made it to " + reader.ReadLine());
                        Console.Read();
                    }
                }
            }
        }

        // WIP
        public void Add(string word, int viewCount)
        {
            int charIndex = 0;
            TrieNode currentNode = Root;

            while (charIndex < word.Length && currentNode != null)
            {
                if (currentNode.Suffixes != null)
                {
                    string partialSuffix = word.Substring(charIndex);
                    currentNode.AddSuffix(partialSuffix);
                    break;
                }

                currentNode.TotalWords++;
                currentNode.AverageViewCount += Convert.ToDouble(viewCount) / currentNode.TotalWords;

                char choice = word[charIndex];
                if (!currentNode.Nodes.ContainsKey(choice))
                {
                    currentNode.Nodes[choice] = new TrieNode();
                }
                currentNode = currentNode.Nodes[choice];
                charIndex++;
            }
        }

        public string[] GetSuggestions(string partial)
        {
            List<string> suggestions;
            dynamic testNode = GetNode(partial);
            if (testNode is List<string>)
            {
                return testNode.ToArray();
            }
            else
            {
                suggestions = GetSuggestions(new List<string>(), testNode, partial);
                return suggestions.ToArray();
            }
        }

        public List<string> GetSuggestions(List<string> words, TrieNode currentNode, string build)
        {
            if (words.Count == 10 || currentNode == null)
            {
                return words;
            }
            if (currentNode.Suffixes != null)
            {
                for (int i = 0; i < currentNode.Suffixes.Count && words.Count < 10; i++)
                {
                    string potentialSuffix = currentNode.Suffixes[i];
                    words.Add(build + potentialSuffix);
                }
            }
            else
            {
                var children = currentNode.Nodes.Select(x => new Tuple<TrieNode, char>(x.Value, x.Key)).OrderByDescending(x => x.Item1);
                foreach (Tuple<TrieNode, char> child in children)
                {
                    words = GetSuggestions(words, child.Item1, build + child.Item2);
                }
            }
            return words;
        }

        // returns null if no such combo
        // For retrieving results
        private dynamic GetNode(string partial)
        {
            return GetNode(partial, 0, Root);
        }

        private dynamic GetNode(string partial, int partialIndex, TrieNode currentNode)
        {
            if (currentNode == null || partialIndex == partial.Length)
            {
                return currentNode;
            }
            char choice = partial[partialIndex];
            if (currentNode.Suffixes != null)
            {
                string partialSuffix = partial.Substring(partialIndex);

                foreach (string possibleSuffix in currentNode.Suffixes)
                {
                    if (possibleSuffix.StartsWith(partialSuffix))
                    {
                        // return suggestions since the user nagivated to a List hybrid
                        List<string> results = new List<string>();
                        for (int i = 0; i < currentNode.Suffixes.Count && results.Count < 10; i++)
                        {
                            if (currentNode.Suffixes[i].StartsWith(partialSuffix))
                            {
                                int lastIndex = partial.LastIndexOf(partialSuffix);
                                results.Add(partial.Substring(0, lastIndex) + currentNode.Suffixes[i]);
                            }
                        }
                        return results;
                    }
                }
                return null;
            }
            if (currentNode.Nodes.ContainsKey(choice))
                return GetNode(partial, partialIndex + 1, currentNode.Nodes[choice]);
            return null;
        }
    }
}
