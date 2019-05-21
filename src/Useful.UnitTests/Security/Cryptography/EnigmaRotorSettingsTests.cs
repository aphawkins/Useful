﻿// <copyright file="EnigmaRotorSettings.UnitTests.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.Security.Cryptography.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Useful.Security.Cryptography;
    using Xunit;

    public class EnigmaRotorSettingsTest
    {
        private string _propertiesChanged;

        /// <summary>
        /// A test for EnigmaSettings Constructor.
        /// </summary>
        [Fact]
        public void EnigmaRotorSettings_ctor()
        {
            EnigmaRotorSettings settings = EnigmaRotorSettings.Create(EnigmaModel.Military);
        }

        [Fact]
        public void EnigmaSettings_SetRotorOrder()
        {
            EnigmaRotorSettings target = EnigmaRotorSettings.Create(EnigmaModel.Military);
            List<EnigmaRotorNumber> availableRotors;
            availableRotors = target.AvailableRotors.ToList();
            Assert.IsTrue(availableRotors.Count == 6);
            Assert.IsTrue(availableRotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(availableRotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(availableRotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(availableRotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(availableRotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(availableRotors[5] == EnigmaRotorNumber.Five);
            target[EnigmaRotorPosition.Fastest] = EnigmaRotor.Create(EnigmaRotorNumber.One);

            availableRotors = target.AvailableRotors.ToList();
            Assert.IsTrue(availableRotors.Count == 5);
            Assert.IsTrue(availableRotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(availableRotors[1] == EnigmaRotorNumber.Two);
            Assert.IsTrue(availableRotors[2] == EnigmaRotorNumber.Three);
            Assert.IsTrue(availableRotors[3] == EnigmaRotorNumber.Four);
            Assert.IsTrue(availableRotors[4] == EnigmaRotorNumber.Five);
            _propertiesChanged = string.Empty;
            target[EnigmaRotorPosition.Second] = EnigmaRotor.Create(EnigmaRotorNumber.Two);

            availableRotors = target.AvailableRotors.ToList();
            Assert.IsTrue(availableRotors.Count == 4);
            Assert.IsTrue(availableRotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(availableRotors[1] == EnigmaRotorNumber.Three);
            Assert.IsTrue(availableRotors[2] == EnigmaRotorNumber.Four);
            Assert.IsTrue(availableRotors[3] == EnigmaRotorNumber.Five);
            target[EnigmaRotorPosition.Third] = EnigmaRotor.Create(EnigmaRotorNumber.Three);

            availableRotors = target.AvailableRotors.ToList();
            Assert.IsTrue(availableRotors.Count == 3);
            Assert.IsTrue(availableRotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(availableRotors[1] == EnigmaRotorNumber.Four);
            Assert.IsTrue(availableRotors[2] == EnigmaRotorNumber.Five);
        }

        [Fact]
        public void EnigmaSettings_SetRotorOrderPropertyChanged()
        {
            EnigmaRotorSettings settings = EnigmaRotorSettings.Create(EnigmaModel.Military);
            settings.PropertyChanged += targetPropertyChanged;
            _propertiesChanged = string.Empty;

            settings[EnigmaRotorPosition.Fastest] = EnigmaRotor.Create(EnigmaRotorNumber.One);
            Assert.IsTrue(_propertiesChanged == "Item;AvailableRotors;");
        }

        [Fact]
        public void EnigmaRotorSettings_SetRotor()
        {
            EnigmaRotorSettings settings = EnigmaRotorSettings.Create(EnigmaModel.Military);
            Assert.IsTrue(settings[EnigmaRotorPosition.Fastest].RotorNumber == EnigmaRotorNumber.None);
            settings[EnigmaRotorPosition.Fastest] = EnigmaRotor.Create(EnigmaRotorNumber.One);
            Assert.IsTrue(settings[EnigmaRotorPosition.Fastest].RotorNumber == EnigmaRotorNumber.One);
        }

        [Fact]
        public void EnigmaRotorSettings_GetAllowedRotorPositions()
        {
            Collection<EnigmaRotorPosition> positions = EnigmaRotorSettings.GetAllowedRotorPositions(EnigmaModel.Military);
            Assert.IsTrue(positions.Count == 3);
            Assert.IsTrue(positions[0] == EnigmaRotorPosition.Fastest);
            Assert.IsTrue(positions[1] == EnigmaRotorPosition.Second);
            Assert.IsTrue(positions[2] == EnigmaRotorPosition.Third);

            positions = EnigmaRotorSettings.GetAllowedRotorPositions(EnigmaModel.M3);
            Assert.IsTrue(positions.Count == 4);
            Assert.IsTrue(positions[0] == EnigmaRotorPosition.Fastest);
            Assert.IsTrue(positions[1] == EnigmaRotorPosition.Second);
            Assert.IsTrue(positions[2] == EnigmaRotorPosition.Third);
            Assert.IsTrue(positions[3] == EnigmaRotorPosition.Forth);

            positions = EnigmaRotorSettings.GetAllowedRotorPositions(EnigmaModel.M4);
            Assert.IsTrue(positions.Count == 4);
            Assert.IsTrue(positions[0] == EnigmaRotorPosition.Fastest);
            Assert.IsTrue(positions[1] == EnigmaRotorPosition.Second);
            Assert.IsTrue(positions[2] == EnigmaRotorPosition.Third);
            Assert.IsTrue(positions[3] == EnigmaRotorPosition.Forth);
        }

        [Fact]
        public void EnigmaRotorSettings_GetAllowedRotorPositions1()
        {
            List<EnigmaRotorNumber> rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.Military, EnigmaRotorPosition.Fastest);
            Assert.IsTrue(rotors.Count == 6);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(rotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(rotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(rotors[5] == EnigmaRotorNumber.Five);

            rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.Military, EnigmaRotorPosition.Second);
            Assert.IsTrue(rotors.Count == 6);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(rotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(rotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(rotors[5] == EnigmaRotorNumber.Five);

            rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.Military, EnigmaRotorPosition.Third);
            Assert.IsTrue(rotors.Count == 6);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(rotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(rotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(rotors[5] == EnigmaRotorNumber.Five);

            rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.M3, EnigmaRotorPosition.Fastest);
            Assert.IsTrue(rotors.Count == 9);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(rotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(rotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(rotors[5] == EnigmaRotorNumber.Five);
            Assert.IsTrue(rotors[6] == EnigmaRotorNumber.Six);
            Assert.IsTrue(rotors[7] == EnigmaRotorNumber.Seven);
            Assert.IsTrue(rotors[8] == EnigmaRotorNumber.Eight);

            rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.M3, EnigmaRotorPosition.Second);
            Assert.IsTrue(rotors.Count == 9);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(rotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(rotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(rotors[5] == EnigmaRotorNumber.Five);
            Assert.IsTrue(rotors[6] == EnigmaRotorNumber.Six);
            Assert.IsTrue(rotors[7] == EnigmaRotorNumber.Seven);
            Assert.IsTrue(rotors[8] == EnigmaRotorNumber.Eight);

            rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.M3, EnigmaRotorPosition.Third);
            Assert.IsTrue(rotors.Count == 9);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(rotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(rotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(rotors[5] == EnigmaRotorNumber.Five);
            Assert.IsTrue(rotors[6] == EnigmaRotorNumber.Six);
            Assert.IsTrue(rotors[7] == EnigmaRotorNumber.Seven);
            Assert.IsTrue(rotors[8] == EnigmaRotorNumber.Eight);

            rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.M4, EnigmaRotorPosition.Fastest);
            Assert.IsTrue(rotors.Count == 9);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(rotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(rotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(rotors[5] == EnigmaRotorNumber.Five);
            Assert.IsTrue(rotors[6] == EnigmaRotorNumber.Six);
            Assert.IsTrue(rotors[7] == EnigmaRotorNumber.Seven);
            Assert.IsTrue(rotors[8] == EnigmaRotorNumber.Eight);

            rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.M4, EnigmaRotorPosition.Second);
            Assert.IsTrue(rotors.Count == 9);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(rotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(rotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(rotors[5] == EnigmaRotorNumber.Five);
            Assert.IsTrue(rotors[6] == EnigmaRotorNumber.Six);
            Assert.IsTrue(rotors[7] == EnigmaRotorNumber.Seven);
            Assert.IsTrue(rotors[8] == EnigmaRotorNumber.Eight);

            rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.M4, EnigmaRotorPosition.Third);
            Assert.IsTrue(rotors.Count == 9);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.One);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Two);
            Assert.IsTrue(rotors[3] == EnigmaRotorNumber.Three);
            Assert.IsTrue(rotors[4] == EnigmaRotorNumber.Four);
            Assert.IsTrue(rotors[5] == EnigmaRotorNumber.Five);
            Assert.IsTrue(rotors[6] == EnigmaRotorNumber.Six);
            Assert.IsTrue(rotors[7] == EnigmaRotorNumber.Seven);
            Assert.IsTrue(rotors[8] == EnigmaRotorNumber.Eight);

            rotors = EnigmaRotorSettings.GetAllowedRotors(EnigmaModel.M4, EnigmaRotorPosition.Forth);
            Assert.IsTrue(rotors.Count == 3);
            Assert.IsTrue(rotors[0] == EnigmaRotorNumber.None);
            Assert.IsTrue(rotors[1] == EnigmaRotorNumber.Beta);
            Assert.IsTrue(rotors[2] == EnigmaRotorNumber.Gamma);
        }

        [Fact]
        public void EnigmaRotorSettings_GetOrderKeys()
        {
            EnigmaRotorSettings settings = EnigmaRotorSettings.Create(EnigmaModel.M4);
            settings[EnigmaRotorPosition.Fastest] = EnigmaRotor.Create(EnigmaRotorNumber.One);
            settings[EnigmaRotorPosition.Second] = EnigmaRotor.Create(EnigmaRotorNumber.Three);
            settings[EnigmaRotorPosition.Third] = EnigmaRotor.Create(EnigmaRotorNumber.Five);
            settings[EnigmaRotorPosition.Forth] = EnigmaRotor.Create(EnigmaRotorNumber.Beta);
            Assert.AreEqual(settings.GetOrderKey(), "Beta V III I");
        }

        [Fact]
        public void EnigmaRotorSettings_GetSettingKeys()
        {
            EnigmaRotorSettings settings = EnigmaRotorSettings.Create(EnigmaModel.M4);
            settings[EnigmaRotorPosition.Fastest] = EnigmaRotor.Create(EnigmaRotorNumber.One);
            settings[EnigmaRotorPosition.Fastest].CurrentSetting = 'B';
            settings[EnigmaRotorPosition.Second] = EnigmaRotor.Create(EnigmaRotorNumber.Three);
            settings[EnigmaRotorPosition.Second].CurrentSetting = 'D';
            settings[EnigmaRotorPosition.Third] = EnigmaRotor.Create(EnigmaRotorNumber.Five);
            settings[EnigmaRotorPosition.Third].CurrentSetting = 'E';
            settings[EnigmaRotorPosition.Forth] = EnigmaRotor.Create(EnigmaRotorNumber.Beta);
            settings[EnigmaRotorPosition.Forth].CurrentSetting = 'G';
            Assert.AreEqual(settings.GetSettingKey(), "G E D B");
        }

        [Fact]
        public void GetRingKeys()
        {
            EnigmaRotorSettings settings = EnigmaRotorSettings.Create(EnigmaModel.M4);
            settings[EnigmaRotorPosition.Fastest] = new EnigmaRotor(EnigmaRotorNumber.One);
            settings[EnigmaRotorPosition.Fastest].RingPosition = 'B';
            settings[EnigmaRotorPosition.Second] = new EnigmaRotor(EnigmaRotorNumber.Three);
            settings[EnigmaRotorPosition.Second].RingPosition = 'D';
            settings[EnigmaRotorPosition.Third] = new EnigmaRotor(EnigmaRotorNumber.Five);
            settings[EnigmaRotorPosition.Third].RingPosition = 'E';
            settings[EnigmaRotorPosition.Forth] = new EnigmaRotor(EnigmaRotorNumber.Beta);
            settings[EnigmaRotorPosition.Forth].RingPosition = 'G';
            Assert.Equal("G E D B", settings.GetRingKey());
        }

        private void targetPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _propertiesChanged += e.PropertyName;
            _propertiesChanged += ";";
        }
    }
}