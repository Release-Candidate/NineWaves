// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  Fabulous-TEMPLATE
// File:     Update.fs
// Date:     4/22/2021 12:00:09 PM
//==============================================================================

/// The namespace of the IOS and Android Fabulous-TEMPLATE app.
namespace Fabulous-TEMPLATE

open System
open Fabulous
open Xamarin.Forms

open RC.Maya


/// The MVU update function.
[<AutoOpen>]
module Update=

    /// <summary>
    /// Message `SetCurrentPage`. Sets the current page to the first page `Home`
    /// or the filter view page `CalendarFilter`.
    /// </summary>
    /// <param name="model">The MVU model to update.</param>
    /// <param name="page">The new page to display.</param>
    /// <returns>The updated model and `cmdDateListViewHeight`</returns>
    let setCurrPage model page =
        { model with CurrentPage = page },
        cmdDateListViewHeight

    /// <summary>
    /// The update function of MVU.
    /// </summary>
    /// <param name="msg">The message to process.</param>
    /// <param name="model">The MVU model.</param>
    /// <returns>The updated model and a command to execute.</returns>
    let update msg model =
        match msg with

        | SetCurrentPage page -> setCurrPage model page

        | SetAppTheme (theme: OSAppTheme) ->
            match theme with
            | OSAppTheme.Dark -> { model with IsDarkMode = true }, Cmd.none
            | _ -> { model with IsDarkMode = false }, Cmd.none

        | SetOrientation (x, y) ->
            match x, y with
            | width, height when width > height -> { model with IsLandscape = true }, Cmd.none
            | _, _ -> { model with IsLandscape = false }, Cmd.none

        | ShowSystemAppInfo doShow ->
            match doShow with
            | true -> { model with ShowSystemAppInfo = true }, Cmd.none
            | false -> { model with ShowSystemAppInfo = false }, Cmd.none

        | OpenURL url -> model, cmdOpenUrl url
