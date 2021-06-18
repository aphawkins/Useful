﻿// <copyright file="Rot13.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography
{
    using System;
    using System.Text;

    /// <summary>
    /// The ROT13 cipher.
    /// </summary>
    public sealed class Rot13 : ICipher
    {
        /// <inheritdoc />
        public string CipherName => "ROT13";

        /// <inheritdoc />
        public string Decrypt(string ciphertext) => Encrypt(ciphertext);

        /// <inheritdoc />
        public string Encrypt(string plaintext)
        {
            if (plaintext == null)
            {
                throw new ArgumentNullException(nameof(plaintext));
            }

            StringBuilder sb = new(plaintext.Length);

            for (int i = 0; i < plaintext.Length; i++)
            {
                int c = plaintext[i];

                // Uppercase
                if (c is >= 'A' and <= 'Z')
                {
                    sb.Append((char)(((c - 'A' + 13) % 26) + 'A'));
                }

                // Lowercase
                else if (c is >= 'a' and <= 'z')
                {
                    sb.Append((char)(((c - 'a' + 13) % 26) + 'A'));
                }
                else
                {
                    sb.Append((char)c);
                }
            }

            return sb.ToString();
        }

        /// <inheritdoc />
        public override string ToString() => CipherName;
    }
}