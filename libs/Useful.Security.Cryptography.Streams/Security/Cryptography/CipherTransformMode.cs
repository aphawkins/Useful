﻿// <copyright file="CipherTransformMode.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography
{
    /// <summary>
    /// Define the direction of encryption.
    /// </summary>
    public enum CipherTransformMode
    {
        /// <summary>
        /// Encryption transformer.
        /// </summary>
        Encrypt,

        /// <summary>
        /// Decryption transformer.
        /// </summary>
        Decrypt,
    }
}