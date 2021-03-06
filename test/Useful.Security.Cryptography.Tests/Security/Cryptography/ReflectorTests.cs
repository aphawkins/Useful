﻿// <copyright file="ReflectorTests.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography.Tests
{
    using Useful.Security.Cryptography;
    using Xunit;

    public class ReflectorTests
    {
        public static TheoryData<string, string, string, string> Data => new()
        {
            { "ABC", "ABC", "ABC", "ABC" }, // Default settings
            { "ABC", "BAC", "ABC", "BAC" }, // Simple substitution
            { "ABCD", "BADC", "ABCD", "BADC" }, // All characters subtituted
            { "ABC", "ABC", "Å", "Å" }, // Invalid character
            { "ABC", "BAC", "AB C", "BA C" }, // Space
        };

        [Theory]
        [MemberData(nameof(Data))]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BADCFEHGIJKLMNOPQRSTUVWXYZ", "HELLOWORLD", "GfLlOwOrLc")]
        public void Decrypt(string characterSet, string substitutions, string plaintext, string ciphertext)
        {
            IReflectorSettings settings = new ReflectorSettings()
            { CharacterSet = characterSet.ToCharArray(), Substitutions = substitutions.ToCharArray() };
            Reflector cipher = new(settings);
            Assert.Equal(plaintext, cipher.Decrypt(ciphertext));
        }

        [Theory]
        [MemberData(nameof(Data))]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "BADCFEHGIJKLMNOPQRSTUVWXYZ", "HeLlOwOrLd", "GFLLOWORLC")]
        public void EncryptCipher(string characterSet, string substitutions, string plaintext, string ciphertext)
        {
            IReflectorSettings settings = new ReflectorSettings()
            { CharacterSet = characterSet.ToCharArray(), Substitutions = substitutions.ToCharArray() };
            Reflector cipher = new(settings);
            Assert.Equal(ciphertext, cipher.Encrypt(plaintext));
        }

        [Fact]
        public void GenerateSettings()
        {
            Reflector cipher = new(new ReflectorSettings());

            const int testsCount = 5;
            for (int i = 0; i < testsCount; i++)
            {
                cipher.GenerateSettings();
                Assert.NotEqual(cipher.Settings.CharacterSet, cipher.Settings.Substitutions);
                foreach (char c in cipher.Settings.CharacterSet)
                {
                    Assert.NotEqual(cipher.Settings[c], c);
                }
            }
        }

        [Fact]
        public void Name()
        {
            IReflectorSettings settings = new ReflectorSettings();
            Reflector cipher = new(settings);
            Assert.Equal("Reflector", cipher.CipherName);
            Assert.Equal("Reflector", cipher.ToString());
        }
    }
}