// <copyright file="EnigmaSymmetric.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Simulates the Enigma encoding machine.
    /// </summary>
    public sealed class EnigmaSymmetric : SymmetricAlgorithm
    {
        /// <summary>
        /// The seperator between values in a key field.
        /// </summary>
        internal const char KeyDelimiter = ' ';

        /// <summary>
        /// The number of fields in the key.
        /// </summary>
        private const int KeyParts = 4;

        /// <summary>
        /// The seperator between key fields.
        /// </summary>
        private const char KeySeperator = '|';

        private readonly Enigma _algorithm;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnigmaSymmetric"/> class.
        /// </summary>
        public EnigmaSymmetric()
        {
            _algorithm = new Enigma(new EnigmaSettings());
            Reset();
        }

        /// <inheritdoc />
        public override byte[] Key
        {
            // Example:
            // "reflector|rotors|ring|plugboard"
            // "B|III II I|03 02 01|DN GR IS KC QX TM PV HY FW BJ"
            get
            {
                StringBuilder key = new();

                // Reflector
                key.Append(_algorithm.Settings.Reflector.ReflectorNumber.ToString());
                key.Append(KeySeperator);

                // Rotor order
                key.Append(RotorOrderString(_algorithm.Settings.Rotors));
                key.Append(KeySeperator);

                // Ring setting
                key.Append(RotorRingString(_algorithm.Settings.Rotors));
                key.Append(KeySeperator);

                // Plugboard
                key.Append(PlugboardString(_algorithm.Settings.Plugboard));

                return Encoding.Unicode.GetBytes(key.ToString());
            }

            set
            {
                try
                {
                    _algorithm.Settings = GetSettingsKey(value);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Error parsing Key.", nameof(Key), ex);
                }

                base.Key = value;
            }
        }

        /// <inheritdoc />
        public override byte[] IV
        {
            get
            {
                // Example:
                // G M Y
                byte[] result = Encoding.Unicode.GetBytes(RotorSettingString(_algorithm.Settings.Rotors));
                return result;
            }

            set
            {
                try
                {
                    _algorithm.Settings = GetSettingsIv(_algorithm.Settings, value);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Error parsing IV.", nameof(IV), ex);
                }

                base.IV = value;
            }
        }

        /// <inheritdoc />
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV)
        {
            Key = rgbKey;
            IV = rgbIV ?? Array.Empty<byte>();
            return new ClassicalSymmetricTransform(_algorithm, CipherTransformMode.Decrypt);
        }

        /// <inheritdoc />
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV)
        {
            Key = rgbKey;
            IV = rgbIV ?? Array.Empty<byte>();
            return new ClassicalSymmetricTransform(_algorithm, CipherTransformMode.Encrypt);
        }

        /// <inheritdoc />
        public override void GenerateIV()
        {
            _algorithm.Settings.Rotors[EnigmaRotorPosition.Fastest].CurrentSetting = GetRandomRotorCurrentSetting();
            _algorithm.Settings.Rotors[EnigmaRotorPosition.Second].CurrentSetting = GetRandomRotorCurrentSetting();
            _algorithm.Settings.Rotors[EnigmaRotorPosition.Third].CurrentSetting = GetRandomRotorCurrentSetting();
            IVValue = IV;
        }

        /// <inheritdoc />
        public override void GenerateKey()
        {
            _algorithm.GenerateSettings();
            KeyValue = Key;
        }

        /// <inheritdoc/>
        public override string ToString() => _algorithm.CipherName;

        private static IEnigmaSettings GetSettingsKey(byte[] key)
        {
            string keyString = Encoding.Unicode.GetString(key);
            string[] parts = keyString.Split(new char[] { KeySeperator }, StringSplitOptions.None);

            if (parts.Length != KeyParts)
            {
                throw new ArgumentException("Incorrect number of key parts.", nameof(key));
            }

            IEnigmaReflector reflector = ParseEnigmaReflectorNumber(parts[0]);
            IDictionary<EnigmaRotorPosition, EnigmaRotorNumber> rotorNumbers = ParseEnigmaRotorNumbers(parts[1]);
            IDictionary<EnigmaRotorPosition, int> rings = ParseEnigmaRings(parts[2]);

            IReadOnlyDictionary<EnigmaRotorPosition, IEnigmaRotor> list = new Dictionary<EnigmaRotorPosition, IEnigmaRotor>
            {
                { EnigmaRotorPosition.Fastest, new EnigmaRotor() { RotorNumber = rotorNumbers[EnigmaRotorPosition.Fastest], RingPosition = rings[EnigmaRotorPosition.Fastest] } },
                { EnigmaRotorPosition.Second, new EnigmaRotor() { RotorNumber = rotorNumbers[EnigmaRotorPosition.Second], RingPosition = rings[EnigmaRotorPosition.Second] } },
                { EnigmaRotorPosition.Third, new EnigmaRotor() { RotorNumber = rotorNumbers[EnigmaRotorPosition.Third], RingPosition = rings[EnigmaRotorPosition.Third] } },
            };

            EnigmaRotors rotors = new(list);

            EnigmaPlugboard plugboard = ParsePlugboard(parts[3]);

            return new EnigmaSettings() { Reflector = reflector, Rotors = rotors, Plugboard = plugboard };
        }

        private static IEnigmaSettings GetSettingsIv(IEnigmaSettings settings, byte[] iv)
        {
            string ivString = iv != null ? Encoding.Unicode.GetString(iv) : string.Empty;

            IDictionary<EnigmaRotorPosition, char> rotorSettings = ParseEnigmaRotorSettings(ivString);

            settings.Rotors[EnigmaRotorPosition.Fastest].CurrentSetting = rotorSettings[EnigmaRotorPosition.Fastest];
            settings.Rotors[EnigmaRotorPosition.Second].CurrentSetting = rotorSettings[EnigmaRotorPosition.Second];
            settings.Rotors[EnigmaRotorPosition.Third].CurrentSetting = rotorSettings[EnigmaRotorPosition.Third];

            return settings;
        }

        private static IEnigmaReflector ParseEnigmaReflectorNumber(string reflector)
        {
            if (reflector.Length > 1 ||
                !char.IsLetter(reflector[0]) ||
                !Enum.TryParse(reflector, out EnigmaReflectorNumber reflectorNumber))
            {
                throw new ArgumentException("Incorrect reflector.", nameof(reflector));
            }

            return new EnigmaReflector() { ReflectorNumber = reflectorNumber };
        }

        private static IDictionary<EnigmaRotorPosition, EnigmaRotorNumber> ParseEnigmaRotorNumbers(string rotorNumbers)
        {
            int rotorPositionsCount = 3;
            string[] rotors = rotorNumbers.Split(new char[] { ' ' });
            Dictionary<EnigmaRotorPosition, EnigmaRotorNumber> newRotors = new();

            if (rotors.Length <= 0)
            {
                throw new ArgumentException("No rotors specified.", nameof(rotorNumbers));
            }

            if (rotors.Length > rotorPositionsCount)
            {
                throw new ArgumentException("Too many rotors specified.", nameof(rotorNumbers));
            }

            if (rotors.Length < rotorPositionsCount)
            {
                throw new ArgumentException("Too few rotors specified.", nameof(rotorNumbers));
            }

            for (int i = 0; i < rotors.Length; i++)
            {
                string rotor = rotors.Reverse().ToList()[i];
                if (string.IsNullOrEmpty(rotor) || rotor.Contains("\0"))
                {
                    throw new ArgumentException("Null or empty rotor specified.", nameof(rotorNumbers));
                }

                if (!Enum.TryParse(rotor, out EnigmaRotorNumber rotorNumber)
                    || rotorNumber.ToString() != rotor)
                {
                    throw new ArgumentException($"Invalid rotor number {rotor}.", nameof(rotorNumbers));
                }

                newRotors.Add((EnigmaRotorPosition)i, rotorNumber);
            }

            return newRotors;
        }

        private static IDictionary<EnigmaRotorPosition, int> ParseEnigmaRings(string ringSettings)
        {
            int rotorPositionsCount = 3;
            string[] rings = ringSettings.Split(new char[] { ' ' });

            if (rings.Length <= 0)
            {
                throw new ArgumentException("No rings specified.", nameof(ringSettings));
            }

            if (rings.Length > rotorPositionsCount)
            {
                throw new ArgumentException("Too many rings specified.", nameof(ringSettings));
            }

            if (rings.Length < rotorPositionsCount)
            {
                throw new ArgumentException("Too few rings specified.", nameof(ringSettings));
            }

            if (rings[0].Length == 0)
            {
                throw new ArgumentException("No rings specified.", nameof(ringSettings));
            }

            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i].Length != 2)
                {
                    throw new ArgumentException("Ring number format incorrect.", nameof(ringSettings));
                }

                if (!int.TryParse(rings[i], out _))
                {
                    throw new ArgumentException("Ring number is not a number.", nameof(ringSettings));
                }
            }

            return new Dictionary<EnigmaRotorPosition, int>
            {
                { EnigmaRotorPosition.Fastest, int.Parse(rings[2]) },
                { EnigmaRotorPosition.Second, int.Parse(rings[1]) },
                { EnigmaRotorPosition.Third, int.Parse(rings[0]) },
            };
        }

        private static IDictionary<EnigmaRotorPosition, char> ParseEnigmaRotorSettings(string rotorSettings)
        {
            int rotorPositionsCount = 3;
            string[] rotorSetting = rotorSettings.Split(new char[] { ' ' });

            if (rotorSetting.Length <= 0)
            {
                throw new ArgumentException("No rotor settings specified.", nameof(rotorSettings));
            }

            if (rotorSetting.Length > rotorPositionsCount)
            {
                throw new ArgumentException("Too many rotor settings specified.", nameof(rotorSettings));
            }

            if (rotorSetting.Length < rotorPositionsCount)
            {
                throw new ArgumentException("Too few rotor settings specified.", nameof(rotorSettings));
            }

            if (rotorSetting[0].Length == 0)
            {
                throw new ArgumentException("No rotor settings specified.", nameof(rotorSettings));
            }

            return new Dictionary<EnigmaRotorPosition, char>
            {
                { EnigmaRotorPosition.Fastest, rotorSetting[2][0] },
                { EnigmaRotorPosition.Second, rotorSetting[1][0] },
                { EnigmaRotorPosition.Third, rotorSetting[0][0] },
            };
        }

        private static EnigmaPlugboard ParsePlugboard(string plugboard)
        {
            IList<EnigmaPlugboardPair> pairs = new List<EnigmaPlugboardPair>();
            string[] rawPairs = plugboard.Split(new char[] { KeyDelimiter });

            // No plugs specified
            if (rawPairs.Length == 1 && rawPairs[0].Length == 0)
            {
                return new EnigmaPlugboard(pairs);
            }

            // Check for plugs made up of pairs
            foreach (string rawPair in rawPairs)
            {
                if (rawPair.Length != 2)
                {
                    throw new ArgumentException("Setting must be a pair.", nameof(plugboard));
                }

                if (pairs.Any(pair => pair.From == rawPair[0]))
                {
                    throw new ArgumentException("Setting already set.", nameof(plugboard));
                }

                pairs.Add(new() { From = rawPair[0], To = rawPair[1] });
            }

            return new EnigmaPlugboard(pairs);
        }

        private static string RotorSettingString(IEnigmaRotors settings)
        {
            StringBuilder key = new();

            foreach (KeyValuePair<EnigmaRotorPosition, IEnigmaRotor> position in settings.Rotors.Reverse().ToArray())
            {
                key.Append(position.Value.CurrentSetting);
                key.Append(KeyDelimiter);
            }

            if (settings.Rotors.Count > 0)
            {
                key.Remove(key.Length - 1, 1);
            }

            return key.ToString();
        }

        private static string RotorOrderString(IEnigmaRotors settings)
        {
            StringBuilder key = new();

            foreach (KeyValuePair<EnigmaRotorPosition, IEnigmaRotor> position in settings.Rotors.Reverse().ToArray())
            {
                key.Append(position.Value.RotorNumber);
                key.Append(KeyDelimiter);
            }

            if (settings.Rotors.Count > 0)
            {
                key.Remove(key.Length - 1, 1);
            }

            return key.ToString();
        }

        private static string RotorRingString(IEnigmaRotors settings)
        {
            StringBuilder key = new();

            foreach (KeyValuePair<EnigmaRotorPosition, IEnigmaRotor> position in settings.Rotors.Reverse().ToArray())
            {
                key.Append($"{position.Value.RingPosition:00}");
                key.Append(KeyDelimiter);
            }

            if (settings.Rotors.Count > 0)
            {
                key.Remove(key.Length - 1, 1);
            }

            return key.ToString();
        }

        private static string PlugboardString(IEnigmaPlugboard plugboard)
        {
            StringBuilder key = new();
            IReadOnlyDictionary<char, char> substitutions = plugboard.Substitutions();

            foreach (KeyValuePair<char, char> pair in substitutions)
            {
                key.Append(pair.Key);
                key.Append(pair.Value);
                key.Append(KeyDelimiter);
            }

            if (substitutions.Count > 0
                && key.Length > 0)
            {
                key.Remove(key.Length - 1, 1);
            }

            return key.ToString();
        }

        private static char GetRandomRotorCurrentSetting() => "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[new Random().Next(0, 25)];

        private void Reset()
        {
            ModeValue = CipherMode.ECB;
            PaddingValue = PaddingMode.None;
            KeySizeValue = 16;
            BlockSizeValue = 16 * 5;
            FeedbackSizeValue = 16;
            LegalBlockSizesValue = new KeySizes[1];
            LegalBlockSizesValue[0] = new KeySizes(0, int.MaxValue, 16);
            LegalKeySizesValue = new KeySizes[1];
            LegalKeySizesValue[0] = new KeySizes(0, int.MaxValue, 16);
            KeyValue = Array.Empty<byte>();
            IVValue = Array.Empty<byte>();
        }
    }
}