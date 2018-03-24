﻿// <copyright file="ConsoleView.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UsefulConsole.UI.Views
{
    using System;
    using System.Collections.Generic;
    using Useful.Security.Cryptography;
    using Useful.UI.Controllers;
    using Useful.UI.Views;

    internal class ConsoleView : ICipherView
    {
        private CipherController controller;
        private CipherTransformMode modeSelected;

        public void DisplayCipherSelect(IList<ICipher> ciphers)
        {
            ICipher cipherSelected = null;
            ICipherSettingsView settingsView = null;

            while (cipherSelected == null)
            {
                Console.WriteLine("Select a cipher:");

                for (int i = 0; i < ciphers.Count; i++)
                {
                    Console.WriteLine($"{i}: {ciphers[i].CipherName}");
                }

                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.D0:
                        {
                            // Caesar
                            cipherSelected = ciphers[0];
                            settingsView = new CaesarSettingsView();
                            break;
                        }

                    case ConsoleKey.D1:
                        {
                            // Reverse
                            cipherSelected = ciphers[1];
                            break;
                        }

                    case ConsoleKey.D2:
                        {
                            // ROT13
                            cipherSelected = ciphers[2];
                            break;
                        }
                }

                Console.WriteLine();
            }

            controller.SelectCipher(cipherSelected, settingsView);

            Console.WriteLine($"Cipher selected: {cipherSelected.CipherName}");
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        /// <param name="cipherNames">The names of all the available ciphers.</param>
        public void Initialize()
        {
            string text;
            IList<ICipher> ciphers = controller.GetCiphers();
            DisplayCipherSelect(ciphers);
            DisplayEncryptionMode();
            DisplayEnterText();

            while (!string.IsNullOrEmpty(text = Console.ReadLine()))
            {
                if (modeSelected == CipherTransformMode.Encrypt)
                {
                    controller.Encrypt(text);
                }
                else
                {
                    controller.Decrypt(text);
                }

                DisplayEnterText();
            }
        }

        public void SetController(IController controller)
        {
            this.controller = (CipherController)controller;
        }

        public void ShowCiphertext(string ciphertext)
        {
            Console.WriteLine($"Ciphertext = {ciphertext}");
        }

        public void ShowPlaintext(string plaintext)
        {
            Console.WriteLine($"Plaintext = {plaintext}");
        }

        public void ShowSettings(ICipherSettingsView settingsView)
        {
        }

        private void DisplayEncryptionMode()
        {
            bool isGood = false;
            modeSelected = CipherTransformMode.Encrypt;

            while (!isGood)
            {
                Console.WriteLine("Select a mode:");

                Console.WriteLine($"E: {CipherTransformMode.Encrypt}");
                Console.WriteLine($"D: {CipherTransformMode.Decrypt}");

                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.D:
                        {
                            modeSelected = CipherTransformMode.Decrypt;
                            isGood = true;
                            break;
                        }

                    case ConsoleKey.E:
                        {
                            modeSelected = CipherTransformMode.Encrypt;
                            isGood = true;
                            break;
                        }
                }

                Console.WriteLine();
            }

            Console.WriteLine($"Encryption mode selected: {modeSelected}");
        }

        private void DisplayEnterText()
        {
            Console.Write($"Enter text to {modeSelected}: ");
        }
    }
}