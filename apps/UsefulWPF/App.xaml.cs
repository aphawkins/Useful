﻿// <copyright file="App.xaml.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace UsefulWPF
{
    using System.Windows;
    using Useful;
    using Useful.Security.Cryptography;
    using Useful.Security.Cryptography.UI.Services;
    using Useful.Security.Cryptography.UI.ViewModels;

    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CryptographyWindow app = new CryptographyWindow();
            IRepository<ICipher> repository = new CipherRepository();

#pragma warning disable IDISP004 // Don't ignore return value of type IDisposable.
            repository.Create(new Atbash());
            repository.Create(new ROT13());
#pragma warning restore CA2000 // Dispose objects before losing scope

            CipherService service = new CipherService(repository);
            CipherViewModel context = new CipherViewModel(service);
            app.DataContext = context;
            app.Show();
        }
    }
}