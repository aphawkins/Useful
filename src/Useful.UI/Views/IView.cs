﻿// <copyright file="IView.cs" company="APH Software">
// Copyright (c) Andrew Hawkins. All rights reserved.
// </copyright>

namespace Useful.UI.Views
{
    using Useful.UI.Controllers;

    /// <summary>
    /// An interface that all views should implement.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Initializes the view.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Sets the controller.
        /// </summary>
        /// <param name="controller">Teh controller.</param>
        void SetController(IController controller);
    }
}