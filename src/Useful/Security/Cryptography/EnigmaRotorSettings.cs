﻿// <copyright file="EnigmaRotorSettings.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Enigma rotor settings.
    /// </summary>
    public class EnigmaRotorSettings : INotifyPropertyChanged
    {
        private List<EnigmaRotorNumber> _availableRotors;
        private IDictionary<EnigmaRotorPosition, EnigmaRotor> _list = new Dictionary<EnigmaRotorPosition, EnigmaRotor>();
        private EnigmaModel _model;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnigmaRotorSettings"/> class.
        /// </summary>
        /// <param name="model">The Enigma model.</param>
        public EnigmaRotorSettings(EnigmaModel model)
        {
            _model = model;

            _availableRotors = GetAllowedRotors(model);

            _list = new Dictionary<EnigmaRotorPosition, EnigmaRotor>();
            foreach (EnigmaRotorPosition position in GetAllowedRotorPositions(model))
            {
                _list[position] = new EnigmaRotor(EnigmaRotorNumber.None);
            }
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the available rotors.
        /// </summary>
        public IEnumerable<EnigmaRotorNumber> AvailableRotors123
        {
            get
            {
                return _availableRotors;
            }
        }

        /// <summary>
        /// Gets the number of rotors.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Sets the rotor settings.
        /// </summary>
        /// <param name="position">The rotor position to set.</param>
        /// <returns>The rotor to set in this position.</returns>
        public EnigmaRotor this[EnigmaRotorPosition position]
        {
            get
            {
                return _list[position];
            }

            set
            {
                _list[position] = value;

                NotifyPropertyChanged();

                // Correct the available rotors list
                // Never remove the None rotor from the list
                if (value.RotorNumber != EnigmaRotorNumber.None)
                {
                    if (_availableRotors.Contains(value.RotorNumber))
                    {
                        _availableRotors.Remove(value.RotorNumber);
                        NotifyPropertyChanged(nameof(AvailableRotors123));
                    }
                }
            }
        }

        /// <summary>
        /// Get the allowed rotor positions.
        /// </summary>
        /// <param name="model">The Enigma model.</param>
        /// <returns>The allowed rotor positions.</returns>
        public static Collection<EnigmaRotorPosition> GetAllowedRotorPositions(EnigmaModel model)
        {
            Collection<EnigmaRotorPosition> allowedRotorPositions = new Collection<EnigmaRotorPosition>();

            switch (model)
            {
                case EnigmaModel.Military:
                    {
                        allowedRotorPositions.Add(EnigmaRotorPosition.Fastest);
                        allowedRotorPositions.Add(EnigmaRotorPosition.Second);
                        allowedRotorPositions.Add(EnigmaRotorPosition.Third);
                        break;
                    }

                case EnigmaModel.M3:
                case EnigmaModel.M4:
                    {
                        allowedRotorPositions.Add(EnigmaRotorPosition.Fastest);
                        allowedRotorPositions.Add(EnigmaRotorPosition.Second);
                        allowedRotorPositions.Add(EnigmaRotorPosition.Third);
                        allowedRotorPositions.Add(EnigmaRotorPosition.Forth);
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("Unknown Enigma model.");
                    }
            }

            return allowedRotorPositions;
        }

        /// <summary>
        /// Gets the allowed rotors for a given rotor position in a given Enigma machine type.
        /// </summary>
        /// <param name="model">The specified Enigma machine type.</param>
        /// <param name="rotorPosition">The specified rotor position.</param>
        /// <returns>The allowed rotors for the machine type and position.</returns>
        public static IList<EnigmaRotorNumber> GetAllowedRotors(EnigmaModel model, EnigmaRotorPosition rotorPosition)
        {
            List<EnigmaRotorNumber> allowedRotors = new List<EnigmaRotorNumber>
            {
                EnigmaRotorNumber.None,
            };

            switch (model)
            {
                case EnigmaModel.Military:
                    {
                        allowedRotors.Add(EnigmaRotorNumber.I);
                        allowedRotors.Add(EnigmaRotorNumber.II);
                        allowedRotors.Add(EnigmaRotorNumber.III);
                        allowedRotors.Add(EnigmaRotorNumber.IV);
                        allowedRotors.Add(EnigmaRotorNumber.V);

                        return allowedRotors;
                    }

                case EnigmaModel.M3:
                    {
                        allowedRotors.Add(EnigmaRotorNumber.I);
                        allowedRotors.Add(EnigmaRotorNumber.II);
                        allowedRotors.Add(EnigmaRotorNumber.III);
                        allowedRotors.Add(EnigmaRotorNumber.IV);
                        allowedRotors.Add(EnigmaRotorNumber.V);
                        allowedRotors.Add(EnigmaRotorNumber.VI);
                        allowedRotors.Add(EnigmaRotorNumber.VII);
                        allowedRotors.Add(EnigmaRotorNumber.VIII);

                        return allowedRotors;
                    }

                case EnigmaModel.M4:
                    {
                        if (rotorPosition == EnigmaRotorPosition.Fastest
                            || rotorPosition == EnigmaRotorPosition.Second
                            || rotorPosition == EnigmaRotorPosition.Third)
                        {
                            allowedRotors.Add(EnigmaRotorNumber.I);
                            allowedRotors.Add(EnigmaRotorNumber.II);
                            allowedRotors.Add(EnigmaRotorNumber.III);
                            allowedRotors.Add(EnigmaRotorNumber.IV);
                            allowedRotors.Add(EnigmaRotorNumber.V);
                            allowedRotors.Add(EnigmaRotorNumber.VI);
                            allowedRotors.Add(EnigmaRotorNumber.VII);
                            allowedRotors.Add(EnigmaRotorNumber.VIII);
                        }
                        else if (rotorPosition == EnigmaRotorPosition.Forth)
                        {
                            allowedRotors.Add(EnigmaRotorNumber.Beta);
                            allowedRotors.Add(EnigmaRotorNumber.Gamma);
                        }

                        return allowedRotors;
                    }

                default:
                    {
                        throw new ArgumentException("Unknown Enigma model.");
                    }
            }
        }

        /// <summary>
        /// Returns the available rotors for a given Enigma model and given position.
        /// </summary>
        /// <param name="rotorPosition">Which position to get the availablity for.</param>
        /// <returns>The available rotors for a given Enigma model and given position.</returns>
        public IList<EnigmaRotorNumber> GetAvailableRotors(EnigmaRotorPosition rotorPosition)
        {
            // Contract.Assert(AllowedRotorPositions.Contains(rotorPosition));
            IList<EnigmaRotorNumber> availableRotors = GetAllowedRotors(_model, rotorPosition);

            foreach (EnigmaRotorPosition position in GetAllowedRotorPositions(_model))
            {
                // Contract.Assert(EnigmaSettings.GetAllowedRotorPositions(model).Contains(rotorSetting.Key));
                // Contract.Assert(EnigmaSettings.GetAllowedRotors(model, rotorSettings[rotorSettings].RingPosition).Contains(rotorByPosition.Value));
                if (_list[position].RotorNumber != EnigmaRotorNumber.None)
                {
                    if (availableRotors.Contains(_list[position].RotorNumber))
                    {
                        availableRotors.Remove(_list[position].RotorNumber);
                    }
                }
            }

            return availableRotors;
        }

        /// <summary>
        /// Gets the key order.
        /// </summary>
        /// <returns>The key order.</returns>
        public string GetOrderKey()
        {
            StringBuilder key = new StringBuilder();

            foreach (KeyValuePair<EnigmaRotorPosition, EnigmaRotor> position in _list.Reverse().ToArray())
            {
                key.Append(position.Value.RotorNumber);
                key.Append(EnigmaSettings.KeyDelimiter);
            }

            if (_list.Count > 0)
            {
                key.Remove(key.Length - 1, 1);
            }

            return key.ToString();
        }

        /// <summary>
        /// Gets the ring key.
        /// </summary>
        /// <returns>The ring key.</returns>
        public string GetRingKey()
        {
            StringBuilder key = new StringBuilder();

            foreach (KeyValuePair<EnigmaRotorPosition, EnigmaRotor> position in _list.Reverse().ToArray())
            {
                key.Append(position.Value.RingPosition);
                key.Append(EnigmaSettings.KeyDelimiter);
            }

            if (_list.Count > 0)
            {
                key.Remove(key.Length - 1, 1);
            }

            return key.ToString();
        }

        /// <summary>
        /// Gets the settings key.
        /// </summary>
        /// <returns>The settings key.</returns>
        public string GetSettingKey()
        {
            StringBuilder key = new StringBuilder();

            foreach (KeyValuePair<EnigmaRotorPosition, EnigmaRotor> position in _list.Reverse().ToArray())
            {
                key.Append(position.Value.CurrentSetting);
                key.Append(EnigmaSettings.KeyDelimiter);
            }

            if (_list.Count > 0)
            {
                key.Remove(key.Length - 1, 1);
            }

            return key.ToString();
        }

        /// <summary>
        /// Gets all the allowed rotors for a given Enigma machine type.
        /// </summary>
        /// <param name="model">The specified Enigma machine type.</param>
        /// <returns>All the allowed rotors for the machine type and position.</returns>
        private static List<EnigmaRotorNumber> GetAllowedRotors(EnigmaModel model)
        {
            switch (model)
            {
                case EnigmaModel.Military:
                    {
                        return new List<EnigmaRotorNumber>(6)
                        {
                            EnigmaRotorNumber.None,
                            EnigmaRotorNumber.I,
                            EnigmaRotorNumber.II,
                            EnigmaRotorNumber.III,
                            EnigmaRotorNumber.IV,
                            EnigmaRotorNumber.V,
                        };
                    }

                case EnigmaModel.M3:
                    {
                        return new List<EnigmaRotorNumber>(9)
                        {
                            EnigmaRotorNumber.None,
                            EnigmaRotorNumber.I,
                            EnigmaRotorNumber.II,
                            EnigmaRotorNumber.III,
                            EnigmaRotorNumber.IV,
                            EnigmaRotorNumber.V,
                            EnigmaRotorNumber.VI,
                            EnigmaRotorNumber.VII,
                            EnigmaRotorNumber.VIII,
                        };
                    }

                case EnigmaModel.M4:
                    {
                        return new List<EnigmaRotorNumber>(11)
                        {
                            EnigmaRotorNumber.None,
                            EnigmaRotorNumber.I,
                            EnigmaRotorNumber.II,
                            EnigmaRotorNumber.III,
                            EnigmaRotorNumber.IV,
                            EnigmaRotorNumber.V,
                            EnigmaRotorNumber.VI,
                            EnigmaRotorNumber.VII,
                            EnigmaRotorNumber.VIII,
                            EnigmaRotorNumber.Beta,
                            EnigmaRotorNumber.Gamma,
                        };
                    }

                default:
                    {
                        throw new ArgumentException("Unknown Enigma model.");
                    }
            }
        }
        /////// <summary>
        ///////
        /////// </summary>
        /////// <param name="rotorOrder"></param>
        /////// <returns></returns>
        //// private static EnigmaRotorSettings GetDefaultRotorSettings(EnigmaRotorSettings rotorOrder)
        //// {
        ////    Contract.Requires(rotorOrder != null);

        //// SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings> rotorSetting = new SortedDictionary<EnigmaRotorPosition, EnigmaRotorSettings>();

        //// foreach (KeyValuePair<EnigmaRotorPosition, EnigmaRotorSettings> rotor in rotorOrder)
        ////    {
        ////        Collection<char> letters = EnigmaRotor.GetAllowedLetters(rotor.Value.RotorNumber);
        ////        EnigmaRotorSettings setting = new EnigmaRotorSettings(rotor.Value.RotorNumber, letters[0], letters[0]);
        ////        rotorSetting.Add(rotor.Key, setting);
        ////    }

        //// return rotorSetting;
        //// }

        /////// <summary>
        ///////
        /////// </summary>
        /////// <param name="model"></param>
        /////// <returns></returns>
        ////public static EnigmaRotorSettings GetDefault(EnigmaModel model)
        ////{
        ////    EnigmaRotorSettings rotorSettings = EnigmaRotorSettings.Create(model);
        ////    rotorSettings[EnigmaRotorPosition.Fastest] = new EnigmaRotor(EnigmaRotorNumber.Three);
        ////    rotorSettings[EnigmaRotorPosition.Second] = new EnigmaRotor(EnigmaRotorNumber.Two);
        ////    rotorSettings[EnigmaRotorPosition.Third] = new EnigmaRotor(EnigmaRotorNumber.One);
        ////    switch (model)
        ////    {
        ////        case EnigmaModel.Military:
        ////            break;
        ////        case EnigmaModel.M3:
        ////        case EnigmaModel.M4:
        ////            rotorSettings[EnigmaRotorPosition.Forth] = EnigmaRotor.Create(EnigmaRotorNumber.Beta);
        ////            break;
        ////        default:
        ////            throw new ArgumentException("Unknown Enigma model.");
        ////    }

        ////    return rotorSettings;
        ////}
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null)
            {
                return;
            }

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}