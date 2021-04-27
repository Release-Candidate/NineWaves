// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Fabulous-TEMPLATE
// File:     View.fs
// Date:     4/10/2021 9:03:21 PM
//==============================================================================

/// The namespace of the IOS and Android Tzolkin app.
namespace Fabulous-TEMPLATE

open Fabulous.XamarinForms
open Xamarin.Forms
open Xamarin.Essentials


/// Holds the view `view` of MVU. The app's pages.
[<AutoOpen>]
module View =

    /// <summary>
    /// The first page of the app.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function.</param>
    /// <returns>The page instance of the first page.</returns>
    let homePage model dispatch =
        View
            .ContentPage(title = "Calendario",
                         backgroundColor = Style.backgroundColor model.IsDarkMode,
                         appearing = (fun () -> dispatch <| SetCurrentPage Home),
                         content = View.StackLayout (
                             backgroundColor = Style.backgroundBrownDark,
                             children =
                         )


            )
            .HasNavigationBar(true)
            .HasBackButton (false)

    /// <summary>
    /// The view of MVU.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function.</param>
    /// <returns>The app's main navigation page instance.</returns>
    let view model dispatch =
        match model.ShowSystemAppInfo with
        | true -> AppInfo.ShowSettingsUI ()
        | false -> ()

        View.NavigationPage (
            sizeChanged = (fun (width, height) -> dispatch (SetOrientation (width, height))),
            useSafeArea = true,
            barBackgroundColor = tabBackgroundColor,
            barTextColor = tabForegroundColor,
            pages =
                match model.CurrentPage with
                | Home -> [ homePage model dispatch ]
        )
