﻿// <copyright file="MonoAlphabeticKeyGenerator.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Useful.Interfaces.Security.Cryptography;

    /// <summary>
    /// MonoAlphabetic key generator.
    /// </summary>
    internal class MonoAlphabeticKeyGenerator : IKeyGenerator
    {
        /// <inheritdoc />
        public byte[] RandomIv() => Array.Empty<byte>();

        /// <inheritdoc />
        public byte[] RandomKey()
        {
            MonoAlphabeticSettings mono = new MonoAlphabeticSettings();
            List<char> allowedLettersCloneFrom = new List<char>(mono.CharacterSet);
            List<char> allowedLettersCloneTo = new List<char>(mono.CharacterSet);

            Random rnd = new Random();
            int indexFrom;
            int indexTo;

            char from;
            char to;

            while (allowedLettersCloneFrom.Count > 0)
            {
                indexFrom = rnd.Next(0, allowedLettersCloneFrom.Count);

                //// Extensions.IndexOutOfRange(indexFrom, 0, allowedLettersCloneFrom.Count - 1);

                from = allowedLettersCloneFrom[indexFrom];
                allowedLettersCloneFrom.RemoveAt(indexFrom);
                if (mono.IsSymmetric
                    && allowedLettersCloneTo.Contains(from))
                {
                    allowedLettersCloneTo.Remove(from);
                }

                indexTo = rnd.Next(0, allowedLettersCloneTo.Count);
                to = allowedLettersCloneTo[indexTo];

                allowedLettersCloneTo.RemoveAt(indexTo);
                if (mono.IsSymmetric
                    && allowedLettersCloneFrom.Contains(to))
                {
                    allowedLettersCloneFrom.Remove(to);
                }

                ////if (from == to)
                ////{
                ////    continue;
                ////}

                mono[from] = to;
            }

            return mono.Key.ToArray();
        }
    }
}