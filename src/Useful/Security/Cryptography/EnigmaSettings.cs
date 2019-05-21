// <copyright file="EnigmaSettings.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The Enigma algorithm settings.
    /// </summary>
    public class EnigmaSettings : CipherSettings
    {
        /// <summary>
        /// The seperator between values in a key field.
        /// </summary>
        internal const char KeyDelimiter = ' ';

        /// <summary>
        /// Is the plugboard symmetric?.
        /// </summary>
        private const bool IsPlugboardSymmetric = true;

        /// <summary>
        /// The seperator between values in an IV field.
        /// </summary>
        private const char IVSeperator = ' ';

        /// <summary>
        /// the number of fields in the key.
        /// </summary>
        private const int KeyParts = 5;

        /// <summary>
        /// The seperator between key fields.
        /// </summary>
        private const char KeySeperator = '|';

        private EnigmaModel _model;

        ///// <summary>
        ///// Rotors, sorted by position.
        ///// </summary>
        // private SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings> rotorSettings;

        ///// <summary>
        ///// The setting for each rotor.
        ///// </summary>
        // private SortedDictionary<EnigmaRotorPosition, char> rotorSetting;

        ///// <summary>
        ///// Available rotors, sorted by position.
        ///// </summary>
        // private List<EnigmaRotorNumber> availableRotors;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnigmaSettings"/> class.
        /// </summary>
        /// <param name="byteKey">The encryption Key.</param>
        /// <param name="byteIV">The Initialization Vector.</param>
        public EnigmaSettings(byte[] byteKey, byte[] byteIV)
        {
            // Reset rotor settings
            // this.rotorSettings = new SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings>(new EnigmaRotorPositionSorter(SortOrder.Ascending));
            // this.Rotors = new EnigmaRotorSettings();
            // this.AllowedRotorPositions = new Collection<EnigmaRotorPosition>();
            // this.availableRotors = new List<EnigmaRotorNumber>();
            AllowedLetters = new Collection<char>();
            this.CipherName = "Enigma";
            this.Plugboard = new MonoAlphabeticSettings(AllowedLetters, new Dictionary<char, char>(), true);

            Key = byteKey;
            IV = byteIV;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnigmaSettings"/> class.
        /// </summary>
        public EnigmaSettings()
        {
        }

        /// <summary>
        /// Gets the allowed letters.
        /// </summary>
        public Collection<char> AllowedLetters { get; private set; }

        /// <summary>
        /// Gets the name of this cipher.
        /// </summary>
        public string CipherName { get; private set; }

        /// <inheritdoc/>
        public override IEnumerable<byte> IV
        {
            get
            {
                byte[] iv = EnigmaSettings.BuildIV(Rotors);

                return iv;
            }

            ////set
            ////{
            ////    // Example:
            ////    // G M Y
            ////    this.CheckNullArgument(() => value);

            ////    if (value.Length <= 0)
            ////    {
            ////        throw new CryptographicException("No IV specified.");
            ////    }

            ////    if (EnigmaSettings.BuildIV(Rotors) == value)
            ////    {
            ////        return;
            ////    }

            ////    char[] newChars = Encoding.Unicode.GetChars(value);

            ////    string newString = new string(newChars);

            ////    string[] parts = newString.Split(new char[] { IVSeperator }, StringSplitOptions.None);

            ////    if (parts.Length > Rotors.Count)
            ////    {
            ////        throw new ArgumentException("Too many IV parts specified.");
            ////    }

            ////    if (parts.Length < Rotors.Count)
            ////    {
            ////        throw new ArgumentException("Too few IV parts specified.");
            ////    }

            ////    parts = parts.Reverse().ToArray();

            ////    EnigmaRotorPosition rotorPosition;
            ////    EnigmaRotorNumber rotorNumber;

            ////    // Check that the rotor in the relevant position contains the specified letter
            ////    for (int i = 0; i < parts.Length; i++)
            ////    {
            ////        rotorPosition = (EnigmaRotorPosition)Enum.Parse(typeof(EnigmaRotorPosition), i.ToString(Culture.CurrentCulture));
            ////        rotorNumber = Rotors[rotorPosition].RotorNumber;

            ////        if (!AllowedLetters.Contains(parts[i][0]))
            ////        {
            ////            throw new ArgumentException("This setting is not allowed.");
            ////        }
            ////    }

            ////    for (int i = 0; i < parts.Length; i++)
            ////    {
            ////        rotorPosition = (EnigmaRotorPosition)Enum.Parse(typeof(EnigmaRotorPosition), i.ToString(Culture.CurrentCulture));

            ////        Rotors[rotorPosition].CurrentSetting = parts[i][0];

            ////        // this.SetRotorSetting_Private(rotorPosition, parts[i][0]);
            ////    }

            ////    // this.iv = EnigmaSettings.BuildIV(this.rotorSetting);
            ////    NotifyPropertyChanged();
            ////}
        }

        /// <summary>
        /// Gets or sets get the encryption Key.
        /// </summary>
        public byte[] Key
        {
            get
            {
                byte[] key = EnigmaSettings.BuildKey(this.Model, this.ReflectorNumber, Rotors, this.Plugboard);

                // Contract.Assert(key != null);
                return key;
            }

            set
            {
                // if (EnigmaSettings.BuildKey(this.Model, this.ReflectorNumber, this.Rotors, this.Plugboard) == keyValue)
                // {
                //    return;
                // }
                EnigmaSettings settings = ParseKey(value);

                // Model
                this.Model = settings.Model;

                // this.SetEnigmaModel();

                // Rotor Order
                Rotors = settings.Rotors;

                // foreach (KeyValuePair<EnigmaRotorPosition, EnigmaRotor> rotor in enigmaKey.RotorSettings)
                // {
                //    Contract.Assume(Enum.IsDefined(typeof(EnigmaRotorPosition), rotor.Key));
                //    Contract.Assume(Enum.IsDefined(typeof(EnigmaRotorNumber), rotor.Value.RotorNumber));
                //    this.Rotors[rotor.Key] = rotor.Value;
                // }

                // Plugboard
                // this.Plugboard.SetSubstitutions(enigmaKey.PlugboardPairs);
                this.Plugboard = settings.Plugboard;

                // this.key = EnigmaSettings.BuildKey(this.Model, this.rotorsByPosition, this.Plugboard);

                // No need to build IV?
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the type of Enigma machine.
        /// </summary>
        public EnigmaModel Model
        {
            get
            {
                return _model;
            }

            private set
            {
                // Contract.Requires(this.Rotors != null);
                _model = value;

                AllowedLetters = EnigmaSettings.GetKeyboardLetters(value);

                // this.AllowedRotorPositions = EnigmaSettings.GetAllowedRotorPositions(this.Model);
                // this.availableRotors = new List<EnigmaRotorNumber>(EnigmaSettings.GetAllowedRotors(this.Model));
                // this.rotorSettings.Clear();

                //// Fill in the allowed and available rotors
                // foreach (EnigmaRotorPosition rotorPosition in this.AllowedRotorPositions)
                // {
                //    // Set an empty rotor in each position
                //    this.rotorSettings.Add(rotorPosition, EnigmaRotorSettings.Empty);
                // }
                Rotors = EnigmaRotorSettings.Create(value);

                // Plugboard
                this.Plugboard = new MonoAlphabeticSettings(AllowedLetters, new Dictionary<char, char>(), IsPlugboardSymmetric);

                // if (this.Plugboard == null)
                // {
                //    this.Plugboard = new MonoAlphabeticSettings(plugboardKey, plugboardIV);
                //    this.Plugboard.SettingsChanged += new EventHandler<EventArgs>(this.Plugboard_SettingsChanged);
                // }
                // else
                // {

                // }

                // Reflector
                this.ReflectorNumber = GetDefaultReflector(this.Model).ReflectorNumber;
            }
        }

        /// <summary>
        /// Gets the number of plugboard pairs that have been swapped.
        /// </summary>
        public int PlugboardSubstitutionCount
        {
            get
            {
                return this.Plugboard.SubstitutionCount;
            }
        }

        ///// <summary>
        ///// Gets the allowed rotor positions.
        ///// </summary>
        // public Collection<EnigmaRotorPosition> AllowedRotorPositions { get; private set; }

        ///// <summary>
        ///// Gets the rotor count.
        ///// </summary>
        // public int RotorPositionCount
        // {
        //    get
        //    {
        //        return this.AllowedRotorPositions.Count;
        //    }
        // }

        /// <summary>
        /// Gets the reflector being used.
        /// </summary>
        public EnigmaReflectorNumber ReflectorNumber { get; private set; }

        public EnigmaRotorSettings Rotors { get; private set; }

        /// <summary>
        /// Gets or sets the plugboard settings.
        /// </summary>
        internal MonoAlphabeticSettings Plugboard { get; set; }

        // return key;
        // }

        /// <summary>
        /// Returns the allowed keyboard letters.
        /// </summary>
        /// <param name="model">The Enigma model to get the keyboard letters for.</param>
        /// <returns>The allowed keyboard letters for the specified model.</returns>
        public static Collection<char> GetKeyboardLetters(EnigmaModel model)
        {
            switch (model)
            {
                case EnigmaModel.Military:
                default:
                    {
                        return new Collection<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
                    }
            }
        }

        ///// <summary>
        ///// Clears out all the Plugboard settings and replaces them with new ones.
        ///// </summary>
        ///// <param name="pairs">The new plugboard pairs to replace the old ones with.</param>
        // public void SetPlugboardNew(Collection<SubstitutionPair> pairs)
        // {
        //    Contract.Requires(pairs != null);

        // SubstitutionPair.CheckPairs(EnigmaSettings.GetKeyboardLetters(this.Model), pairs, IsPlugboardSymmetric);

        // // TODO: Check new pairs aren't the same as existing pairs
        //    this.Plugboard.SetSubstitutions(pairs);

        // // this.key = EnigmaSettings.BuildKey(this.Model, this.rotorsByPosition, this.Plugboard);

        // // It's all good, raise an event
        //    this.NotifyPropertyChanged();
        // }

        ///// <summary>
        ///// Sets a new Plugboard setting.
        ///// </summary>
        ///// <param name="pair">The new plugboard setting.</param>
        // public void SetPlugboardPair(SubstitutionPair pair)
        // {
        //    SubstitutionPair.CheckPairs(EnigmaSettings.GetKeyboardLetters(this.Model), new Collection<SubstitutionPair>() { pair }, false);

        // this.Plugboard.SetSubstitution(pair);

        // // this.key = EnigmaSettings.BuildKey(this.Model, this.rotorsByPosition, this.Plugboard);

        // // It's all good, raise an event
        //    this.NotifyPropertyChanged();
        // }

        ///// <summary>
        ///// The rotors that can be used in this cipher.
        ///// </summary>
        ///// <param name="rotorPosition">The position to get the available rotors for.</param>
        ///// <returns>A collection of available rotors.</returns>
        // public IEnumerable<EnigmaRotorNumber> AvailableRotors()
        // {
        //    return this.availableRotors;

        // // this.availableRotors.Sort(new EnigmaRotorNumberSorter(SortOrder.Ascending));
        //    // return new Collection<EnigmaRotorNumber>(this.availableRotors[rotorPosition].ToArray());
        // }

        ///// <summary>
        ///// Set the rotor order for this machine.
        ///// </summary>
        ///// <param name="rotorPosition">The position to place this rotor in.</param>
        ///// <param name="rotorNumber">The rotor to put in this position.</param>
        // public void SetRotorOrder(EnigmaRotorPosition rotorPosition, EnigmaRotorSettings rotor)
        // {
        //    if (!this.AllowedRotorPositions.Contains(rotorPosition))
        //    {
        //        throw new ArgumentException("This position is not available.");
        //    }

        // // Is the rotor being set to the existing one?
        //    if (this.rotorSettings[rotorPosition].RotorNumber == rotor.RotorNumber)
        //    {
        //        return;
        //    }

        // if (!this.availableRotors.Contains(rotor.RotorNumber))
        //    {
        //        throw new ArgumentException("This rotor in this position is not available.");
        //    }

        // this.SetRotorOrder_Private(rotorPosition, rotor);

        // // this.key = EnigmaSettings.BuildKey(this.Model, this.rotorsByPosition, this.Plugboard);

        // // It's all good, raise an event
        //    this.OnSettingsChanged();
        // }

        ///// <summary>
        ///// Set the rotor order for this machine.
        ///// </summary>
        ///// <param name="rotorPosition">The position to place this rotor in.</param>
        ///// <returns>The rotor to put in the position.</returns>
        // public EnigmaRotorNumber GetRotorOrder(EnigmaRotorPosition rotorPosition)
        // {
        //    if (!this.AllowedRotorPositions.Contains(rotorPosition))
        //    {
        //        throw new ArgumentException("Invalid rotor position.");
        //    }

        // return this.rotorSettings[rotorPosition].RotorNumber;
        // }

        ///// <summary>
        ///// Gets the rotor's ring setting.
        ///// </summary>
        ///// <param name="rotorPosition">The rotor position for which to get the ring setting.</param>
        ///// <returns>The ring setting for the specified position.</returns>
        // public char GetRingSetting(EnigmaRotorPosition rotorPosition)
        // {
        //    if (!this.AllowedRotorPositions.Contains(rotorPosition))
        //    {
        //        throw new ArgumentException("This position is not allowed.");
        //    }

        // if (this.rotorSettings[rotorPosition].IsEmpty)
        //    {
        //        throw new ArgumentException("No rotor currently in this position.");
        //    }

        // if (!this.rotorSettings.ContainsKey(rotorPosition))
        //    {
        //        throw new ArgumentException("No rotor currently in this position.");
        //    }

        // return this.rotorSettings[rotorPosition].RingPosition;
        // }

        ///// <summary>
        ///// Gets the rotor settings.
        ///// </summary>
        ///// <param name="rotorPosition">The rotor position for which to get the setting.</param>
        ///// <returns>The setting for the specified position.</returns>
        // public char GetRotorSetting(EnigmaRotorPosition rotorPosition)
        // {
        //    if (!this.AllowedRotorPositions.Contains(rotorPosition))
        //    {
        //        throw new ArgumentException("This position is not allowed.");
        //    }

        // if (this.Rotors[rotorPosition].IsNone)
        //    {
        //        throw new ArgumentException("No rotor currently in this position.");
        //    }

        // if (!this.Rotors.ContainsKey(rotorPosition))
        //    {
        //        throw new ArgumentException("No rotor currently in this position.");
        //    }

        // return this.rotorSettings[rotorPosition].CurrentSetting;
        // }

        ///// <summary>
        ///// Sets the rotor settings.
        ///// </summary>
        ///// <param name="rotorPosition">The rotor position to set.</param>
        ///// <param name="letter">The letter to set this position to.</param>
        // public void SetRotorSetting(EnigmaRotorPosition rotorPosition, char letter)
        // {
        //    if (!this.AllowedRotorPositions.Contains(rotorPosition))
        //    {
        //        throw new ArgumentException("This position is not allowed.");
        //    }

        // if (this.rotorSettings[rotorPosition].RotorNumber == EnigmaRotorNumber.None)
        //    {
        //        throw new ArgumentException("This setting is not allowed.");
        //    }

        // if (!EnigmaRotor.GetAllowedLetters(this.rotorSettings[rotorPosition].RotorNumber).Contains(letter))
        //    {
        //        throw new ArgumentException("This setting is not allowed.");
        //    }

        // this.SetRotorSetting_Private(rotorPosition, letter);

        // // this.iv = EnigmaSettings.BuildIV(this.rotorSetting);
        //    this.OnSettingsChanged();
        // }

        ///// <summary>
        ///// Set the encryption Key.
        ///// </summary>
        ///// <param name="keyValue">The key for this cipher.</param>
        // public void SetKey(byte[] keyValue)
        // {
        //    //if (EnigmaSettings.BuildKey(this.Model, this.ReflectorNumber, this.Rotors, this.Plugboard) == keyValue)
        //    //{
        //    //    return;
        //    //}

        // EnigmaSettings enigmaKey = ParseEnigmaKey(keyValue);

        // // Model
        //    this.Model = enigmaKey.Model;
        //    // this.SetEnigmaModel();

        // // Rotor Order
        //    this.Rotors = enigmaKey.Rotors;

        // //foreach (KeyValuePair<EnigmaRotorPosition, EnigmaRotor> rotor in enigmaKey.RotorSettings)
        //    //{
        //    //    Contract.Assume(Enum.IsDefined(typeof(EnigmaRotorPosition), rotor.Key));
        //    //    Contract.Assume(Enum.IsDefined(typeof(EnigmaRotorNumber), rotor.Value.RotorNumber));
        //    //    this.Rotors[rotor.Key] = rotor.Value;
        //    //}

        // // Plugboard
        //    // this.Plugboard.SetSubstitutions(enigmaKey.PlugboardPairs);
        //    this.Plugboard = enigmaKey.Plugboard;

        // // this.key = EnigmaSettings.BuildKey(this.Model, this.rotorsByPosition, this.Plugboard);

        // // No need to build IV?
        //    this.NotifyPropertyChanged();
        // }

        ///// <summary>
        ///// Get the encryption Key.
        ///// </summary>
        ///// <returns>The encryption key.</returns>
        // public byte[] GetKey()
        // {
        //    Contract.Ensures(Contract.Result<byte[]>() != null);

        // byte[] key = EnigmaSettings.BuildKey(this.Model, this.ReflectorNumber, this.Rotors, this.Plugboard);

        // Contract.Assert(key != null);
        public static EnigmaSettings GetRandom()
        {
            byte[] key = EnigmaSettings.GetRandomKey();
            byte[] iv = EnigmaSettings.GetRandomIV(key);
            EnigmaSettings settings = new EnigmaSettings(key, iv);
            return settings;
        }

        // // this.SetRotorSetting_Private(rotorPosition, allowedLetters[0]);
        //        }
        //    }
        // }
        internal static int GetIvLength(EnigmaModel model)
        {
            switch (model)
            {
                case EnigmaModel.Military:
                    return 5;
                case EnigmaModel.M3:
                case EnigmaModel.M4:
                    return 7;
                default:
                    throw new CryptographicException("Unknown Enigma model.");
            }
        }

        internal static EnigmaSettings ParseKey(byte[] key)
        {
            // Example:
            // model|reflector|rotors|ring|plugboard
            EnigmaSettings settings = new EnigmaSettings();

            char[] tempKey = Encoding.Unicode.GetChars(key);
            string keyString = new string(tempKey);

            string[] parts = keyString.Split(new char[] { KeySeperator }, StringSplitOptions.None);

            if (parts.Length != KeyParts)
            {
                throw new ArgumentException("Incorrect number of key parts.");
            }

            // Model
            if (string.IsNullOrEmpty(parts[0]))
            {
                throw new ArgumentException("Model has not been specified.");
            }

            settings._model = (EnigmaModel)Enum.Parse(typeof(EnigmaModel), parts[0]);

            // Reflector
            settings.ReflectorNumber = (EnigmaReflectorNumber)Enum.Parse(typeof(EnigmaReflectorNumber), parts[1]);
            if (!GetAllowed(settings._model).Contains(settings.ReflectorNumber))
            {
                throw new ArgumentException("This reflector is not available.");
            }

            // Rotor Order
            string[] rotors = parts[2].Split(new char[] { KeyDelimiter });
            if (rotors.Length <= 0)
            {
                throw new ArgumentException("No rotors specified.");
            }

            int rotorPositionsCount = EnigmaRotorSettings.GetAllowedRotorPositions(settings._model).Count;

            if (rotors.Length > rotorPositionsCount)
            {
                throw new ArgumentException("Too many rotors specified.");
            }

            if (rotors.Length < rotorPositionsCount)
            {
                throw new ArgumentException("Too few rotors specified.");
            }

            if (rotors[0].Length == 0)
            {
                throw new ArgumentException("No rotors specified.");
            }

            rotors = rotors.Reverse().ToArray();

            // Ring
            string[] rings = parts[3].Split(new char[] { KeyDelimiter });

            if (rings.Length <= 0)
            {
                throw new ArgumentException("No rings specified.");
            }

            if (rings.Length > rotorPositionsCount)
            {
                throw new ArgumentException("Too many rings specified.");
            }

            if (rings.Length < rotorPositionsCount)
            {
                throw new ArgumentException("Too few rings specified.");
            }

            if (rings[0].Length == 0)
            {
                throw new ArgumentException("No rings specified.");
            }

            rings = rings.Reverse().ToArray();

            EnigmaRotorSettings rotorSettings = EnigmaRotorSettings.Create(settings._model);

            for (int i = 0; i < rotors.Length; i++)
            {
                if (string.IsNullOrEmpty(rotors[i]) || rotors[i].Contains("\0"))
                {
                    throw new ArgumentException("Null or empty rotor specified.");
                }

                EnigmaRotorPosition rotorPosition = (EnigmaRotorPosition)Enum.Parse(typeof(EnigmaRotorPosition), i.ToString(Culture.CurrentCulture));
                EnigmaRotorNumber rotorNumber = EnigmaUINameConverter.Convert(rotors[i]);

                if (!rotorSettings.GetAvailableRotors(rotorPosition).Contains(rotorNumber))
                {
                    throw new ArgumentException("This rotor in this position is not available.");
                }

                EnigmaRotor enigmaRotor = new EnigmaRotor(rotorNumber);
                if (!enigmaRotor.Letters.Contains(rings[i][0]))
                {
                    throw new ArgumentException("This ring position is invalid.");
                }

                enigmaRotor.RingPosition = rings[i][0];

                // enigmaRotor.CurrentSetting = 'A';
                rotorSettings[rotorPosition] = enigmaRotor;
            }

            settings.Rotors = rotorSettings;

            // Plugboard
            string plugs = parts[4];

            // if (string.IsNullOrEmpty(plugs) || plugs.Contains("\0"))
            // {
            //    throw new ArgumentException("Null or empty plugs specified.");
            // }
            Dictionary<char, char> pairs = MonoAlphabeticSettings.GetPairs(EnigmaSettings.GetKeyboardLetters(settings._model), plugs, KeyDelimiter, true);

            byte[] plugboardKey = MonoAlphabeticSettings.BuildKey(EnigmaSettings.GetKeyboardLetters(settings._model), pairs, true);
            byte[] plugboardIv = MonoAlphabeticSettings.BuildIV();

            settings.Plugboard = new MonoAlphabeticSettings(plugboardKey, plugboardIv);

            return settings;
        }

        internal void AdvanceRotor(EnigmaRotorPosition rotorPosition, char currentSetting)
        {
            // Contract.Assert(this.AllowedRotorPositions.Contains(rotorPosition));
            Contract.Assert(AllowedLetters.Contains(currentSetting));

            Rotors[rotorPosition].CurrentSetting = currentSetting;

            // this.SetRotorSetting(rotorPosition, currentSetting);
        }

        private static EnigmaSettings GetDefault()
        {
            byte[] key = EnigmaSettings.GetDefaultKey();
            byte[] iv = EnigmaSettings.GetDefaultIV(key);
            EnigmaSettings settings = new EnigmaSettings(key, iv);
            return settings;
        }

        private static EnigmaReflector GetDefaultReflector(EnigmaModel model)
        {
            switch (model)
            {
                case EnigmaModel.Military:
                    return new EnigmaReflector(EnigmaReflectorNumber.B);
                case EnigmaModel.M3:
                case EnigmaModel.M4:
                    return new EnigmaReflector(EnigmaReflectorNumber.BThin);
                default:
                    throw new CryptographicException("Unknown Enigma model.");
            }
        }

        private static EnigmaRotor GetRandom(EnigmaRotorNumber rotorNumber)
        {
            Random rnd = new Random();
            int index1 = rnd.Next(0, allowedletters.Count);
            int index2 = rnd.Next(0, letters.Count);

            EnigmaRotor rotor = new EnigmaRotor(rotorNumber);
            rotor.RingPosition = letters[index1];
            rotor.CurrentSetting = letters[index2];

            return rotor;
        }

        private static byte[] BuildIV(EnigmaRotorSettings rotorSettings)
        {
            // Example:
            // G M Y
            byte[] result = Encoding.Unicode.GetBytes(rotorSettings.GetSettingKey());

            return result;
        }

        private static byte[] BuildKey(
            EnigmaModel model,
            EnigmaReflectorNumber reflector,
            EnigmaRotorSettings rotors,
            MonoAlphabeticSettings plugboard)
        {
            // Example:
            // "model|reflector|rotors|ring|plugboard"
            // "Military|B|III II I|C B A|DN GR IS KC QX TM PV HY FW BJ"
            StringBuilder key = new StringBuilder();

            // Model
            key.Append(model.ToString());
            key.Append(KeySeperator);

            // Reflector
            key.Append(reflector.ToString());
            key.Append(KeySeperator);

            // Rotor order
            key.Append(rotors.GetOrderKey());
            key.Append(KeySeperator);

            // Ring setting
            key.Append(rotors.GetRingKey());
            key.Append(KeySeperator);

            // Plugboard
            key.Append(plugboard.GetSettingKey());

            // Contract.Assume(Encoding.Unicode.GetBytes(key.ToString()) != null);
            return Encoding.Unicode.GetBytes(key.ToString());
        }

        /// <summary>
        /// Returns the default initialization vector.
        /// </summary>
        /// <param name="key">The key to base the initialization vector on.</param>
        /// <returns>The default initialization vector.</returns>
        private static byte[] GetDefaultIV(byte[] key)
        {
            EnigmaSettings enigmaKey = EnigmaSettings.ParseKey(key);
            EnigmaRotorSettings rotorSettings = new EnigmaRotorSettings(enigmaKey.Model);
            byte[] iv = EnigmaSettings.BuildIV(rotorSettings);
            return iv;
        }

        /// <summary>
        /// Returns the default initialization key.
        /// </summary>
        /// <returns>The default initialization key.</returns>
        private static byte[] GetDefaultKey()
        {
            // Model
            EnigmaModel model = EnigmaModel.Military;

            // Rotor Settings
            EnigmaRotorSettings rotorSettings = new EnigmaRotorSettings(model);

            // Plugboard
            MonoAlphabeticSettings plugboard = new MonoAlphabeticSettings(EnigmaSettings.GetKeyboardLetters(model));

            byte[] key = EnigmaSettings.BuildKey(model, EnigmaReflectorNumber.B, rotorSettings, plugboard);
            return key;
        }

        //// return allowedRotorPositions;
        //// }

        private static EnigmaModel GetDefaultModel()
        {
            return EnigmaModel.Military;
        }

        /// <summary>
        /// Returns a randomly generated initialization vector.
        /// </summary>
        /// <param name="key">The key to base the initialization vector on.</param>
        /// <returns>A randomly generated initialization vector.</returns>
        private static byte[] GetRandomIV(byte[] key)
        {
            EnigmaSettings enigmaKey = EnigmaSettings.ParseKey(key);
            EnigmaRotorSettings rotorSettings = EnigmaRotorSettings.GetRandom(enigmaKey.Model);
            byte[] iv = EnigmaSettings.BuildIV(rotorSettings);
            return iv;
        }

        /// <summary>
        /// Returns a randomly generated initialization key.
        /// </summary>
        /// <returns>A randomly generated initialization key.</returns>
        private static byte[] GetRandomKey()
        {
            // Model
            EnigmaModel model = EnigmaSettings.GetRandomModel();

            // Reflector
            EnigmaReflectorNumber reflector = GetRandomReflector(model).ReflectorNumber;

            // Rotor Settings
            EnigmaRotorSettings rotorSettings = EnigmaRotorSettings.GetRandom(model);

            // Plugboard
            Collection<char> allowedLetters = EnigmaSettings.GetKeyboardLetters(model);
            MonoAlphabeticSettings plugboard = MonoAlphabeticSettings.GetRandom(allowedLetters, true);

            byte[] key = EnigmaSettings.BuildKey(model, reflector, rotorSettings, plugboard);
            return key;
        }

        private static EnigmaReflector GetRandomReflector(EnigmaModel model)
        {
            Random rnd = new Random();

            List<EnigmaReflectorNumber> reflectors = GetAllowed(model);

            int nextRandomNumber = rnd.Next(0, reflectors.Count);

            return new EnigmaReflector(reflectors[nextRandomNumber]);
        }

        // private static List<EnigmaReflectorNumber> GetAllowedReflectors(EnigmaModel model)
        // {
        //    switch (model)
        //    {
        //        case (EnigmaModel.Military):
        //        case (EnigmaModel.M3):
        //            {
        //                return new List<EnigmaReflectorNumber>(2) {
        //                    EnigmaReflectorNumber.B,
        //                    EnigmaReflectorNumber.C
        //                    };
        //            }

        // case (EnigmaModel.M4):
        //            {
        //                return new List<EnigmaReflectorNumber>(2) {
        //                    EnigmaReflectorNumber.BThin,
        //                    EnigmaReflectorNumber.CThin
        //                    };
        //            }
        //        default:
        //            {
        //                throw new CryptographicException("Unknown Enigma model.");
        //            }
        //    }
        // }

        ///// <summary>
        ///// Gets all the allowed rotors for a given Enigma machine type.
        ///// </summary>
        ///// <param name="model">The specified Enigma machine type.</param>
        ///// <returns>All the allowed rotors for the machine type and position.</returns>
        // private static List<EnigmaRotorNumber> GetAllowedRotors(EnigmaModel model)
        // {
        //    Contract.Requires(Enum.IsDefined(typeof(EnigmaModel), model));

        // switch (model)
        //    {
        //        case EnigmaModel.Military:
        //            {
        //                return new List<EnigmaRotorNumber>(6) {
        //                    EnigmaRotorNumber.None,
        //                    EnigmaRotorNumber.One,
        //                    EnigmaRotorNumber.Two,
        //                    EnigmaRotorNumber.Three,
        //                    EnigmaRotorNumber.Four,
        //                    EnigmaRotorNumber.Five
        //                };
        //            }

        // case EnigmaModel.M3:
        //            {
        //                return new List<EnigmaRotorNumber>(9) {
        //                    EnigmaRotorNumber.None,
        //                    EnigmaRotorNumber.One,
        //                    EnigmaRotorNumber.Two,
        //                    EnigmaRotorNumber.Three,
        //                    EnigmaRotorNumber.Four,
        //                    EnigmaRotorNumber.Five,
        //                    EnigmaRotorNumber.Six,
        //                    EnigmaRotorNumber.Seven,
        //                    EnigmaRotorNumber.Eight
        //                };
        //            }

        // case EnigmaModel.M4:
        //            {
        //                return new List<EnigmaRotorNumber>(11) {
        //                    EnigmaRotorNumber.None,
        //                    EnigmaRotorNumber.One,
        //                    EnigmaRotorNumber.Two,
        //                    EnigmaRotorNumber.Three,
        //                    EnigmaRotorNumber.Four,
        //                    EnigmaRotorNumber.Five,
        //                    EnigmaRotorNumber.Six,
        //                    EnigmaRotorNumber.Seven,
        //                    EnigmaRotorNumber.Eight,
        //                    EnigmaRotorNumber.Beta,
        //                    EnigmaRotorNumber.Gamma
        //                };
        //            }

        // default:
        //            {
        //                throw new ArgumentException("Unknown Enigma model.");
        //            }
        //    }
        // }

        ///// <summary>
        ///// Gets the allowed rotors for a given rotor position in a given Enigma machine type.
        ///// </summary>
        ///// <param name="model">The specified Enigma machine type.</param>
        ///// <param name="rotorPosition">The specified rotor position.</param>
        ///// <returns>The allowed rotors for the machine type and position.</returns>
        // internal static List<EnigmaRotorNumber> GetAllowedRotors(EnigmaModel model, EnigmaRotorPosition rotorPosition)
        // {
        //    Contract.Requires(Enum.IsDefined(typeof(EnigmaModel), model));
        //    Contract.Requires(Enum.IsDefined(typeof(EnigmaRotorPosition), rotorPosition));

        // Contract.Requires(EnigmaSettings.GetAllowedRotorPositions(model).Contains(rotorPosition));

        // List<EnigmaRotorNumber> allowedRotors = new List<EnigmaRotorNumber>();

        // allowedRotors.Add(EnigmaRotorNumber.None);

        // switch (model)
        //    {
        //        case EnigmaModel.Military:
        //            {
        //                allowedRotors.Add(EnigmaRotorNumber.One);
        //                allowedRotors.Add(EnigmaRotorNumber.Two);
        //                allowedRotors.Add(EnigmaRotorNumber.Three);
        //                allowedRotors.Add(EnigmaRotorNumber.Four);
        //                allowedRotors.Add(EnigmaRotorNumber.Five);

        // return allowedRotors;
        //            }

        // case EnigmaModel.M3:
        //            {
        //                allowedRotors.Add(EnigmaRotorNumber.One);
        //                allowedRotors.Add(EnigmaRotorNumber.Two);
        //                allowedRotors.Add(EnigmaRotorNumber.Three);
        //                allowedRotors.Add(EnigmaRotorNumber.Four);
        //                allowedRotors.Add(EnigmaRotorNumber.Five);
        //                allowedRotors.Add(EnigmaRotorNumber.Six);
        //                allowedRotors.Add(EnigmaRotorNumber.Seven);
        //                allowedRotors.Add(EnigmaRotorNumber.Eight);

        // return allowedRotors;
        //            }

        // case EnigmaModel.M4:
        //            {
        //                if (rotorPosition == EnigmaRotorPosition.Fastest
        //                    || rotorPosition == EnigmaRotorPosition.Second
        //                    || rotorPosition == EnigmaRotorPosition.Third)
        //                {
        //                    allowedRotors.Add(EnigmaRotorNumber.One);
        //                    allowedRotors.Add(EnigmaRotorNumber.Two);
        //                    allowedRotors.Add(EnigmaRotorNumber.Three);
        //                    allowedRotors.Add(EnigmaRotorNumber.Four);
        //                    allowedRotors.Add(EnigmaRotorNumber.Five);
        //                    allowedRotors.Add(EnigmaRotorNumber.Six);
        //                    allowedRotors.Add(EnigmaRotorNumber.Seven);
        //                    allowedRotors.Add(EnigmaRotorNumber.Eight);
        //                }
        //                else if (rotorPosition == EnigmaRotorPosition.Forth)
        //                {
        //                    allowedRotors.Add(EnigmaRotorNumber.Beta);
        //                    allowedRotors.Add(EnigmaRotorNumber.Gamma);
        //                }
        //                return allowedRotors;
        //            }

        // default:
        //            {
        //                throw new ArgumentException("Unknown Enigma model.");
        //            }
        //    }
        // }

        ///// <summary>
        ///// Returns the available rotors for a given Enigma model and given position.
        ///// </summary>
        ///// <param name="model">The specified Enigma model.</param>
        ///// <param name="rotorsByPosition">The rotors currently in the positions.</param>
        ///// <param name="rotorPosition">Which position to get the availablity for.</param>
        ///// <returns>The available rotors for a given Enigma model and given position.</returns>
        // private static List<EnigmaRotorNumber> GetAvailableRotors(EnigmaModel model, SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings> rotorSettings, EnigmaRotorPosition rotorPosition)
        // {
        //    Contract.Requires(rotorSettings != null);
        //    Contract.Assert(EnigmaSettings.GetAllowedRotorPositions(model).Contains(rotorPosition));

        // List<EnigmaRotorNumber> allowedRotors = GetAllowedRotors(model, rotorPosition);

        // foreach (KeyValuePair<EnigmaRotorPosition, EnigmaRotorSettings> rotorSetting in rotorSettings)
        //    {
        //        Contract.Assert(EnigmaSettings.GetAllowedRotorPositions(model).Contains(rotorSetting.Key));
        //        //Contract.Assert(EnigmaSettings.GetAllowedRotors(model, rotorSettings[rotorSettings].RingPosition).Contains(rotorByPosition.Value));

        // if (rotorSetting.Value.RotorNumber != EnigmaRotorNumber.None)
        //        {
        //            if (allowedRotors.Contains(rotorSetting.Value.RotorNumber))
        //            {
        //                allowedRotors.Remove(rotorSetting.Value.RotorNumber);
        //            }
        //        }
        //    }

        // return allowedRotors;
        // }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        // internal static Collection<EnigmaRotorPosition> GetAllowedRotorPositions(EnigmaModel model)
        // {
        //    Collection<EnigmaRotorPosition> allowedRotorPositions = new Collection<EnigmaRotorPosition>();

        // switch (model)
        //    {
        //        case EnigmaModel.Military:
        //            {
        //                allowedRotorPositions.Add(EnigmaRotorPosition.Fastest);
        //                allowedRotorPositions.Add(EnigmaRotorPosition.Second);
        //                allowedRotorPositions.Add(EnigmaRotorPosition.Third);
        //                break;
        //            }
        //        case EnigmaModel.M3:
        //        case EnigmaModel.M4:
        //            {
        //                allowedRotorPositions.Add(EnigmaRotorPosition.Fastest);
        //                allowedRotorPositions.Add(EnigmaRotorPosition.Second);
        //                allowedRotorPositions.Add(EnigmaRotorPosition.Third);
        //                allowedRotorPositions.Add(EnigmaRotorPosition.Forth);
        //                break;
        //            }

        // default:
        //            {
        //                throw new ArgumentException("Unknown Enigma model.");
        //            }
        //    }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static EnigmaModel GetRandomModel()
        {
            Random rnd = new Random();

            int i = rnd.Next(0, Enum.GetValues(typeof(EnigmaModel)).Length - 1);
            EnigmaModel model = (EnigmaModel)Enum.Parse(typeof(EnigmaModel), i.ToString(CultureInfo.CurrentCulture), true);
            return model;
        }

        private static List<EnigmaReflectorNumber> GetAllowed(EnigmaModel model)
        {
            switch (model)
            {
                case EnigmaModel.Military:
                case EnigmaModel.M3:
                    {
                        return new List<EnigmaReflectorNumber>(2)
                    {
                        EnigmaReflectorNumber.B,
                        EnigmaReflectorNumber.C,
                    };
                    }

                case EnigmaModel.M4:
                    {
                        return new List<EnigmaReflectorNumber>(2)
                    {
                        EnigmaReflectorNumber.BThin,
                        EnigmaReflectorNumber.CThin,
                    };
                    }

                default:
                    {
                        throw new CryptographicException("Unknown Enigma model.");
                    }
            }
        }

        // private static EnigmaReflectorNumber GetDefaultReflector(EnigmaModel model)
        // {
        //    switch (model)
        //    {
        //        case EnigmaModel.Military:
        //            return EnigmaReflectorNumber.B;
        //        case EnigmaModel.M3:
        //        case EnigmaModel.M4:
        //            return EnigmaReflectorNumber.BThin;
        //        default:
        //            throw new CryptographicException("Unknown Enigma model.");
        //    }
        // }

        // private static EnigmaReflectorNumber GetRandomReflector(EnigmaModel model)
        // {
        //    Random rnd = new Random();

        // List<EnigmaReflectorNumber> reflectors = GetAllowedReflectors(model);

        // int nextRandomNumber = rnd.Next(0, reflectors.Count);

        // return reflectors[nextRandomNumber];
        // }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        // private static SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings> GetRandomRotorSettings(EnigmaModel model)
        // {
        //    Random rnd = new Random();
        //    int nextRandomNumber;
        //    SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings> rotorSettings = new SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings>();

        // Collection<EnigmaRotorPosition> allowedRotorPositions = EnigmaSettings.GetAllowedRotorPositions(model);

        // List<EnigmaRotorNumber> availableRotorNumbers;
        //    foreach (EnigmaRotorPosition rotorPosition in allowedRotorPositions)
        //    {
        //        availableRotorNumbers = EnigmaSettings.GetAvailableRotors(model, rotorSettings, rotorPosition);
        //        if (availableRotorNumbers.Contains(EnigmaRotorNumber.None))
        //        {
        //            availableRotorNumbers.Remove(EnigmaRotorNumber.None);
        //        }

        // nextRandomNumber = rnd.Next(0, availableRotorNumbers.Count);

        // rotorSettings[rotorPosition] = GetRandomRotorSettings(availableRotorNumbers[nextRandomNumber]);
        //    }

        // return rotorSettings;
        // }
        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="rotorOrder"></param>
        ///// <returns></returns>
        // private static EnigmaRotorSettings GetDefaultRotorSettings(EnigmaRotorSettings rotorOrder)
        // {
        //    Contract.Requires(rotorOrder != null);

        // SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings> rotorSetting = new SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings>();

        // foreach (KeyValuePair<EnigmaRotorPosition, EnigmaRotorSettings> rotor in rotorOrder)
        //    {
        //        Collection<char> letters = EnigmaRotor.GetAllowedLetters(rotor.Value.RotorNumber);
        //        EnigmaRotorSettings setting = new EnigmaRotorSettings(rotor.Value.RotorNumber, letters[0], letters[0]);
        //        rotorSetting.Add(rotor.Key, setting);
        //    }

        // return rotorSetting;
        // }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="rotorOrder"></param>
        ///// <returns></returns>
        // private static EnigmaRotorSettings GetRandomRotorSettings(EnigmaRotorNumber rotorNumber)
        // {
        //    Random rnd = new Random();
        //    Collection<char> letters = EnigmaRotor.GetAllowedLetters(rotorNumber);

        // int index1 = rnd.Next(0, letters.Count);
        //    int index2 = rnd.Next(0, letters.Count);
        //    return new EnigmaRotorSettings(rotorNumber, letters[index1], letters[index2]);
        // }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="rotorPosition"></param>
        ///// <param name="letter"></param>
        // private void SetRotorSetting_Private(EnigmaRotorPosition rotorPosition, char letter)
        // {
        //    Contract.Requires(Enum.IsDefined(typeof(EnigmaRotorPosition), rotorPosition));
        //    Contract.Requires(this.rotorSettings != null);
        //    Contract.Requires(this.AllowedRotorPositions.Contains(rotorPosition));

        // // Contract.Assert(this.rotorsByPosition[rotorPosition] != EnigmaRotorNumber.None);
        //    // Contract.Assert(EnigmaRotor.GetAllowedLetters(this.rotorsByPosition[rotorPosition]).Contains(letter));

        // if (this.rotorSettings.ContainsKey(rotorPosition))
        //    {
        //        if (this.rotorSettings[rotorPosition].CurrentSetting == letter)
        //        {
        //            // Nothing to set, so return
        //            return;
        //        }

        // this.rotorSettings[rotorPosition].CurrentSetting = letter;
        //    }
        //    else
        //    {
        //        throw new Exception();
        //        // this.rotorSetting.Add(rotorPosition, letter);
        //    }
        // }
        ///// <summary>
        /////
        ///// </summary>
        // private void SetEnigmaModel()
        // {
        //    Contract.Requires(this.Rotors != null);

        // this.AllowedLetters = EnigmaSettings.GetKeyboardLetters(this.Model);
        //    //this.AllowedRotorPositions = EnigmaSettings.GetAllowedRotorPositions(this.Model);
        //    //this.availableRotors = new List<EnigmaRotorNumber>(EnigmaSettings.GetAllowedRotors(this.Model));
        //    //this.rotorSettings.Clear();

        // //// Fill in the allowed and available rotors
        //    //foreach (EnigmaRotorPosition rotorPosition in this.AllowedRotorPositions)
        //    //{
        //    //    // Set an empty rotor in each position
        //    //    this.rotorSettings.Add(rotorPosition, EnigmaRotorSettings.Empty);
        //    //}

        // // Plugboard
        //    byte[] plugboardKey = MonoAlphabeticSettings.BuildKey(this.AllowedLetters, new Collection<SubstitutionPair>(), IsPlugboardSymmetric);
        //    byte[] plugboardIV = MonoAlphabeticSettings.BuildIV();
        //    if (this.Plugboard == null)
        //    {
        //        this.Plugboard = new MonoAlphabeticSettings(plugboardKey, plugboardIV);
        //        this.Plugboard.SettingsChanged += new EventHandler<EventArgs>(this.Plugboard_SettingsChanged);
        //    }
        //    else
        //    {
        //        this.Plugboard.SetKey(plugboardKey);
        //        this.Plugboard.SetIV(plugboardIV);
        //    }

        // // Reflector
        //    this.ReflectorNumber = EnigmaSettings.GetDefaultReflector(this.Model);
        // }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        // private void Plugboard_SettingsChanged(object sender, EventArgs e)
        // {
        //    this.NotifyPropertyChanged();
        // }

        ///// <summary>
        ///// Set the rotor order for this machine.
        ///// </summary>
        ///// <param name="rotorPosition">The position to place this rotor in.</param>
        ///// <param name="rotorNumber">The rotor to put in this position.</param>
        // private void SetRotorOrder_Private(EnigmaRotorPosition rotorPosition, EnigmaRotorSettings rotor)
        // {
        //    Contract.Requires(Enum.IsDefined(typeof(EnigmaRotorPosition), rotorPosition));
        //    Contract.Requires(rotor != null);
        //    // Contract.Requires(this.AllowedRotorPositions.Contains(rotorPosition));
        //    // Contract.Requires(this.availableRotorsByPosition[rotorPosition].Contains(rotorNumber));

        // // Contract.Assert(Enum.IsDefined(typeof(EnigmaRotorNumber), rotorNumber));

        // if (rotor.RotorNumber == this.rotorSettings[rotorPosition].RotorNumber)
        //    {
        //        return;
        //    }

        // EnigmaRotorNumber currentRotor = this.rotorSettings[rotorPosition].RotorNumber;

        // if (!this.availableRotors.Contains(currentRotor))
        //    {
        //        this.availableRotors.Add(currentRotor);
        //    }

        // if (!rotor.IsEmpty)
        //    {
        //        this.availableRotors.Remove(rotor.RotorNumber);
        //    }

        // this.availableRotors.Sort(new EnigmaRotorNumberSorter(SortOrder.Ascending));

        // // Set the rotor order
        //    if (this.rotorSettings.ContainsKey(rotorPosition))
        //    {
        //        this.rotorSettings[rotorPosition] = rotor;
        //    }
        //    else
        //    {
        //        this.rotorSettings.Add(rotorPosition, rotor);
        //    }

        // if (!this.rotorSettings.ContainsKey(rotorPosition))
        //    {
        //        Contract.Assume(Enum.IsDefined(typeof(EnigmaRotorNumber), rotor.RotorNumber));
        //        Collection<char> allowedLetters = EnigmaRotor.GetAllowedLetters(rotor.RotorNumber);
        //        if (allowedLetters.Count > 0)
        //        {
        //            this.Rotors[rotorPosition].CurrentSetting = allowedLetters[0];
    }
}