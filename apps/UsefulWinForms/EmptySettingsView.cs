﻿// <copyright file="EmptySettingsView.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace UsefulWinForms
{
    using System.Windows.Forms;
    using Useful.Security.Cryptography.UI.Controllers;
    using Useful.Security.Cryptography.UI.Views;

    public partial class EmptySettingsView : UserControl, ICipherSettingsView
    {
        public EmptySettingsView()
        {
            InitializeComponent();
        }

        public void SetController(IController controller)
        {
        }

        public void Initialize()
        {
        }
    }
}