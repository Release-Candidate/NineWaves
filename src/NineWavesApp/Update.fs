// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  NineWavesApp
// File:     Update.fs
// Date:     4/22/2021 12:00:09 PM
//==============================================================================

/// The namespace of the IOS and Android NineWaves app.
namespace NineWavesApp

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
        Cmd.none

    /// <summary>
    /// Message `CarouselChanged`. Called, if the current item in the date view
    /// carousel has changed.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="args">The `PositionChangedEventArgs`.</param>
    /// <returns>The unchanged model and `Cmd.none`.</returns>
    let carChanged model (args: PositionChangedEventArgs) =
        let direction = args.CurrentPosition - args.PreviousPosition

        match args.PreviousPosition, args.CurrentPosition with
        | 0, 2 ->
            { model with
                    Date = model.Date + System.TimeSpan.FromDays -1. },
            Cmd.none
        | 2, 0 ->
            { model with
                    Date = model.Date + System.TimeSpan.FromDays 1. },
            Cmd.none
        | _, _ ->
            { model with
                    Date = model.Date + System.TimeSpan.FromDays (float direction) },
            Cmd.none

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

        | SetDate date -> { model with Date = date }, Cmd.none

        | CarouselChanged args -> carChanged model args
