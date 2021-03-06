// <copyright file="CaesarTests.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography.Tests
{
    using System.Linq;
    using Useful.Security.Cryptography;
    using Xunit;

    public class CaesarTests
    {
        public static TheoryData<string, string, int> Data => new()
        {
            { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", 0 },
            { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "DEFGHIJKLMNOPQRSTUVWXYZABC", 3 },
            { ">?@ [\\]", ">?@ [\\]", 3 },
            { "�", "�", 3 },
        };

        [Fact]
        public void CtorSettings()
        {
            Caesar cipher = new(new CaesarSettings() { RightShift = 7 });
            Assert.Equal(7, cipher.Settings.RightShift);
        }

        [Theory]
        [MemberData(nameof(Data))]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "defghijklmnopqrstuvwxyzabc", 3)]
        public void Decrypt(string plaintext, string ciphertext, int rightShift)
        {
            Caesar cipher = new(new CaesarSettings());
            cipher.Settings.RightShift = rightShift;
            Assert.Equal(plaintext, cipher.Decrypt(ciphertext));
        }

        [Theory]
        [MemberData(nameof(Data))]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "DEFGHIJKLMNOPQRSTUVWXYZABC", 3)]
        public void Encrypt(string plaintext, string ciphertext, int rightShift)
        {
            Caesar cipher = new(new CaesarSettings());
            cipher.Settings.RightShift = rightShift;
            Assert.Equal(ciphertext, cipher.Encrypt(plaintext));
        }

        [Fact]
        public void GenerateSettings()
        {
            Caesar cipher = new(new CaesarSettings());

            const int testsCount = 5;
            int[] shifts = new int[testsCount];
            for (int i = 0; i < testsCount; i++)
            {
                cipher.GenerateSettings();
                shifts[i] = cipher.Settings.RightShift;
            }

            Assert.True(shifts.Distinct().Count() > 1);
        }

        [Fact]
        public void Name()
        {
            ICipher cipher = new Caesar(new CaesarSettings());
            Assert.Equal("Caesar", cipher.CipherName);
            Assert.Equal("Caesar", cipher.ToString());
        }

        [Fact]
        public void SinghCodeBook()
        {
            string ciphertext = "MHILY LZA ZBHL XBPZXBL MVYABUHL HWWPBZ JSHBKPBZ JHLJBZ KPJABT HYJHUBT LZA ULBAYVU";
            string plaintext = "FABER EST SUAE QUISQUE FORTUNAE APPIUS CLAUDIUS CAECUS DICTUM ARCANUM EST NEUTRON";

            ICipher cipher = new Caesar(new CaesarSettings()
            {
                RightShift = 7,
            });
            Assert.Equal(plaintext, cipher.Decrypt(ciphertext));
            Assert.Equal(ciphertext, cipher.Encrypt(plaintext));
        }
    }
}