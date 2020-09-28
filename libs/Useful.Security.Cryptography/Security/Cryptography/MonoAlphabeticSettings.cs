﻿// <copyright file="MonoAlphabeticSettings.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// The monoalphabetic algorithm settings.
    /// </summary>
    public sealed class MonoAlphabeticSettings : IMonoAlphabeticSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoAlphabeticSettings"/> class.
        /// </summary>
        public MonoAlphabeticSettings()
            : this("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoAlphabeticSettings"/> class.
        /// </summary>
        /// <param name="characterSet">The valid character set.</param>
        /// <param name="substitutions">A substitution for each character.</param>
        public MonoAlphabeticSettings(string characterSet, string substitutions)
        {
            if (characterSet == null)
            {
                throw new ArgumentNullException(nameof(characterSet));
            }

            if (substitutions == null)
            {
                throw new ArgumentNullException(nameof(substitutions));
            }

            ParseCharacterSet(characterSet);
            ParseSubstitutions(substitutions);
        }

        /// <inheritdoc />
        public string Substitutions { get; private set; } = string.Empty;

        /// <inheritdoc />
        public string CharacterSet { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <inheritdoc />
        public int SubstitutionCount
        {
            get
            {
                int count = 0;

                for (int i = 0; i < CharacterSet.Length; i++)
                {
                    if (CharacterSet[i] != Substitutions[i])
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <inheritdoc />
        public char this[char substitution]
        {
            get
            {
                int subsIndex = CharacterSet.IndexOf(substitution);
                if (subsIndex < 0)
                {
                    return substitution;
                }

                return Substitutions[subsIndex];
            }

            set
            {
                Debug.Print($"[{substitution},{value}]");

                char from = substitution;
                int fromIndex = CharacterSet.IndexOf(from);

                if (fromIndex < 0)
                {
                    throw new ArgumentException("Substitution must be an valid character.", nameof(value));
                }

                char to = value;
                int toIndex = CharacterSet.IndexOf(to);

                if (toIndex < 0)
                {
                    throw new ArgumentException("Substitution must be an valid character.", nameof(value));
                }

                if (Substitutions[fromIndex] == to)
                {
                    // Trying to set the same as already set
                    return;
                }

                char fromSubs = Substitutions[fromIndex];
                int toInvIndex = Substitutions.IndexOf(to);
                //// char toInv = CharacterSet[toInvIndex];

                if (Substitutions[fromIndex] == to)
                {
                    return;
                }

                char[] temp = Substitutions.ToArray();
                temp[fromIndex] = to;
                Substitutions = new string(temp);
                temp[toInvIndex] = fromSubs;
                Substitutions = new string(temp);

                Debug.Print($"{string.Join(string.Empty, Substitutions)}");
            }
        }

        /// <inheritdoc />
        public char Reverse(char letter)
        {
            if (CharacterSet.IndexOf(letter) < 0)
            {
                return letter;
            }

            return Substitutions.First(x => this[x] == letter);
        }

        private void ParseCharacterSet(string characterSet)
        {
            if (string.IsNullOrWhiteSpace(characterSet))
            {
                throw new ArgumentException("Invalid number of characters.", nameof(characterSet));
            }

            foreach (char character in characterSet)
            {
                if (!char.IsLetter(character))
                {
                    throw new ArgumentException("All characters must be letters.", nameof(characterSet));
                }
            }

            if (characterSet.Length != characterSet.Distinct().Count())
            {
                throw new ArgumentException("Characters must not be duplicated.", nameof(characterSet));
            }

            CharacterSet = characterSet;
        }

        private void ParseSubstitutions(string substitutions)
        {
            if (string.IsNullOrWhiteSpace(substitutions)
                || substitutions.Length != CharacterSet.Length)
            {
                throw new ArgumentException("Incorrect number of substitutions.", nameof(substitutions));
            }

            foreach (char character in substitutions)
            {
                if (!char.IsLetter(character))
                {
                    throw new ArgumentException("All substitutions must be letters.", nameof(substitutions));
                }
            }

            if (substitutions.Length != substitutions.Distinct().Count())
            {
                throw new ArgumentException("Substitutions must not be duplicated.", nameof(substitutions));
            }

            if (!substitutions.All(x => CharacterSet.Contains(x)))
            {
                throw new ArgumentException("Substitutions must be in the character set.", nameof(substitutions));
            }

            Substitutions = substitutions;
        }
    }
}