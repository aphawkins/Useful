//-----------------------------------------------------------------------
// <copyright file="Enigma.cs" company="APH Software">
//     Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>
// <summary>Simulates the Enigma encoding machine.</summary>
//-----------------------------------------------------------------------

namespace Useful.Security.Cryptography
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Simulates the Enigma encoding machine.
    /// </summary>
    public sealed class Enigma : SymmetricAlgorithm
    {
        /// <summary>
        /// The size of a byte.
        /// </summary>
        private const int SizeOfByte = 8;

        /// <summary>
        /// The length of the key.
        /// </summary>
        private readonly int _LengthOfKey = 5;

        /// <summary>
        /// Initializes a new instance of the Enigma class.
        /// </summary>
        public Enigma() // EnigmaModel model)
        {
            ModeValue = CipherMode.ECB;
            PaddingValue = PaddingMode.None;
            KeySizeValue = int.MaxValue;

            // switch (model)
            // {
            //    case EnigmaModel.Military:
            //        {
            //            LengthOfKey = 5;
            //            break;
            //        }
            //    case EnigmaModel.Navy:
            //    case EnigmaModel.M4:
            //        {
            //            LengthOfKey = 5;
            //            break;
            //        }
            //    default:
            //        throw new Exception();
            // }
            BlockSizeValue = _LengthOfKey * sizeof(char) * SizeOfByte;

            // FeedbackSizeValue = 2;
            LegalBlockSizesValue = new KeySizes[1];
            LegalBlockSizesValue[0] = new KeySizes(0, int.MaxValue, 1);
            LegalKeySizesValue = new KeySizes[1];
            LegalKeySizesValue[0] = new KeySizes(0, int.MaxValue, 1);

            EnigmaSettings defaultSettings = EnigmaSettings.GetDefault();
            KeyValue = defaultSettings.Key;
            IVValue = defaultSettings.IV;

            // BlockSizeValue = this.m_settings.GetIV().Length * 8;
        }

        public override byte[] Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                EnigmaSettings enigmaKey = EnigmaSettings.ParseKey(value);
                BlockSizeValue = EnigmaSettings.GetIvLength(enigmaKey.Model) * sizeof(char) * SizeOfByte;
                base.Key = value;
            }
        }

        /// <summary>
        /// Creates a symmetric decryptor object.
        /// </summary>
        /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
        /// <param name="rgbIV">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>The symmetric decryptor object.</returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            this.CheckNullArgument(() => rgbKey);
            this.CheckNullArgument(() => rgbIV);

            try
            {
                return new EnigmaTransform(rgbKey, rgbIV, CipherTransformMode.Decrypt);
            }
            catch (ArgumentException ex)
            {
                throw new CryptographicException("Error with Key or IV.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override ICryptoTransform CreateDecryptor()
        {
            if (Key == null)
            {
                throw new CryptographicException("Key is null.");
            }

            if (IV == null)
            {
                throw new CryptographicException("IV is null.");
            }

            try
            {
                return new EnigmaTransform(Key, IV, CipherTransformMode.Decrypt);
            }
            catch (ArgumentException ex)
            {
                throw new CryptographicException("Error with Key or IV.", ex);
            }
        }

        /// <summary>
        /// Creates a symmetric encryptor object.
        /// </summary>
        /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
        /// <param name="rgbIV">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>The symmetric encryptor object.</returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            this.CheckNullArgument(() => rgbKey);
            this.CheckNullArgument(() => rgbIV);

            try
            {
                return new EnigmaTransform(rgbKey, rgbIV, CipherTransformMode.Encrypt);
            }
            catch (ArgumentException ex)
            {
                throw new CryptographicException("Error with Key or IV.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override ICryptoTransform CreateEncryptor()
        {
            if (Key == null)
            {
                throw new CryptographicException("Key is null.");
            }

            if (IV == null)
            {
                throw new CryptographicException("IV is null.");
            }

            try
            {
                return new EnigmaTransform(Key, IV, CipherTransformMode.Encrypt);
            }
            catch (ArgumentException ex)
            {
                throw new CryptographicException("Error with Key or IV.", ex);
            }
        }

        /// <summary>
        /// Generates a random initialization vector (IV) to use for the algorithm.
        /// </summary>
        public override void GenerateIV()
        {
            IVValue = EnigmaSettings.GetRandom().IV;
        }

        /// <summary>
        /// Generates a random key to be used for the algorithm.
        /// </summary>
        public override void GenerateKey()
        {
            KeyValue = EnigmaSettings.GetRandom().Key;// Key();
        }
    }
}