﻿// <copyright file="EnigmaReflectorGenerator.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Enigma Reflector settings generator.
    /// </summary>
    internal class EnigmaReflectorGenerator
    {
        public static EnigmaReflector Generate()
        {
            Random rnd = new();

            List<EnigmaReflectorNumber> reflectors = new()
            {
                EnigmaReflectorNumber.B,
                EnigmaReflectorNumber.C,
            };

            int nextRandomNumber = rnd.Next(0, reflectors.Count);

            return new EnigmaReflector(reflectors[nextRandomNumber]);
        }
    }
}