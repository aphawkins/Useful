﻿// <copyright file="ROT13SymmetricTests.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography.Tests
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Useful.Security.Cryptography;
    using Xunit;

    public class ROT13SymmetricTests
    {
        public static TheoryData<string, string> Data => new()
        {
            { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "NOPQRSTUVWXYZABCDEFGHIJKLM" },
            { "abcdefghijklmnopqrstuvwxyz", "nopqrstuvwxyzabcdefghijklm" },
            { ">?@ [\\]", ">?@ [\\]" },
            { "Å", "Å" },
        };

        [Fact]
        public void Ctor()
        {
            using SymmetricAlgorithm cipher = new ROT13Symmetric();
            Assert.Equal(Array.Empty<byte>(), cipher.Key);
            Assert.Equal(Array.Empty<byte>(), cipher.IV);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Decrypt(string plaintext, string ciphertext)
        {
            using SymmetricAlgorithm cipher = new ROT13Symmetric();
            Assert.Equal(ciphertext, CipherMethods.SymmetricTransform(cipher, CipherTransformMode.Decrypt, plaintext));
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Encrypt(string plaintext, string ciphertext)
        {
            using SymmetricAlgorithm cipher = new ROT13Symmetric();
            Assert.Equal(ciphertext, CipherMethods.SymmetricTransform(cipher, CipherTransformMode.Encrypt, plaintext));
        }

        [Fact]
        public void IvGenerateCorrectness()
        {
            using SymmetricAlgorithm cipher = new ROT13Symmetric();
            cipher.GenerateIV();
            Assert.Equal(Array.Empty<byte>(), cipher.IV);
        }

        [Fact]
        public void IvSet()
        {
            using SymmetricAlgorithm cipher = new ROT13Symmetric
            {
                IV = Encoding.Unicode.GetBytes("A"),
            };
            Assert.Equal(Array.Empty<byte>(), cipher.IV);
        }

        [Fact]
        public void KeyGenerateCorrectness()
        {
            using SymmetricAlgorithm cipher = new ROT13Symmetric();
            cipher.GenerateKey();
            Assert.Equal(Array.Empty<byte>(), cipher.Key);
        }

        [Fact]
        public void KeySet()
        {
            using SymmetricAlgorithm cipher = new ROT13Symmetric
            {
                Key = Encoding.Unicode.GetBytes("A"),
            };
            Assert.Equal(Array.Empty<byte>(), cipher.Key);
        }

        [Fact]
        public void Name()
        {
            using SymmetricAlgorithm cipher = new ROT13Symmetric();
            Assert.Equal("ROT13", cipher.ToString());
        }
    }
}