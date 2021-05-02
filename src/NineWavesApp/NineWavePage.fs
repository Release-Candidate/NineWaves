// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  NineWavesApp
// File:     NineWavePage.fs
// Date:     4/29/2021 8:40:08 PM
//==============================================================================
/// The namespace of the IOS and Android NineWaves app.
namespace NineWavesApp

open Fabulous.XamarinForms
open Xamarin.Forms
open System

open RC.Maya

/// The page of the app showing the date cards with information about all 9 waves.
[<AutoOpen>]
module NineWavePage=

    /// <summary>
    /// Show a date picker to select the date to display.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <param name="date">The date to display information about</param>
    /// <returns>The `Frame` containing the `DatePicker`.</returns>
    let dateSelector model dispatch date =
        View.Frame (
            backgroundColor = backgroundBrownLight,
            borderColor = backgroundBrownDark,
            hasShadow = true,
            padding = Thickness (5., 5.),
            content =
                View.DatePicker (
                    minimumDate = DateTime.MinValue,
                    maximumDate = DateTime.MaxValue,
                    date = date,
                    format = localeFormat,
                    dateSelected = (fun args -> SetDate args.NewDate |> dispatch),
                    width = 105.0,
                    verticalOptions = LayoutOptions.Fill,
                    textColor = Style.foregroundLight,
                    backgroundColor = Style.backgroundNone,
                    fontSize = Style.normalFontSize,
                    horizontalOptions = LayoutOptions.EndAndExpand
                )
            )

    /// <summary>Convert days to years.</summary>
    /// <param name="days">The number of days to convert to years</param>
    /// <returns>The days as years.</returns>
    let daysToYears (days: int64) =
        match days with
        | i when i < 365L -> sprintf "%dd" i
        | _ ->  sprintf "%da" (int64 <| float days / 365.25)

    /// <summary>
    /// Format the information of one wave for use in a label.
    /// </summary>
    /// <param name="waveDay">The `WaveDay` instance to display.</param>
    /// <param name="waveNum">The number of the wave, one of 1 to 9.</param>
    /// <returns></returns>
    let formatWaveDay (waveDay: NineWaves.WaveDay) waveNum =
       [ View.Span ( text = (sprintf "onda %d: " (*waveDay.WaveNumber*) waveNum),
                    fontAttributes = FontAttributes.Bold,
                    fontSize = Style.waveInfoTextSize,
                    lineHeight = Style.waveInfoLineHeigt,
                    textColor = Style.foregroundLight
         )
         View.Span ( text = (sprintf
                                "%s, %s / %s\n"
                                (if waveDay.IsNight then "noche" else "día")
                                (daysToYears waveDay.DayNumber)
                                (daysToYears waveDay.OfDays)
                                ),
                    fontAttributes = FontAttributes.Bold,
                    fontSize = Style.waveInfoTextSize,
                    lineHeight = Style.waveInfoLineHeigt,
                    textColor = Style.accentDarkRed
        ) ]

    /// <summary>
    /// Format the information of all 9 waves of the current date.
    /// </summary>
    /// <param name="date">The date to display the waves information of.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A list of `Span`, usable to display in a label.</returns>
    let formatWaveDayDescriptions date dispatch =
        View.FormattedString (
               formatWaveDay (NineWaves.getWaveday9 date) 9 @
               formatWaveDay (NineWaves.getWaveday8 date) 8 @
               formatWaveDay (NineWaves.getWaveday7 date) 7 @
               formatWaveDay (NineWaves.getWaveday6 date) 6 @
               formatWaveDay (NineWaves.getWaveday5 date) 5 @
               formatWaveDay (NineWaves.getWaveday4 date) 4 @
               formatWaveDay (NineWaves.getWaveday3 date) 3 @
               formatWaveDay (NineWaves.getWaveday2 date) 2 @
               formatWaveDay (NineWaves.getWaveday1 date) 1 @
               [ View.Span "\n  " ]

        )


    /// <summary>
    /// Display a `Frame` containing a button to switch to the graph page, a
    /// date picker to sect the day to display and information about all 9 waves.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <param name="date">The date to display information about.</param>
    /// <returns>A `StackLayout` holding a button to switch to the graph page,
    /// the `DatePicker` to select the date to show information of and
    /// a `Label` with information about all nine waves.</returns>
    let tzolkinCard model dispatch date =
        View.StackLayout (
            horizontalOptions = LayoutOptions.Center,
            padding = Thickness 5.,
            children =
                [ View.Frame (
                      backgroundColor = backgroundBrown,
                      cornerRadius = 20.,
                      padding = Thickness (0., 10., 0., 10.),
                      hasShadow = true,
                      content =
                          View.StackLayout (
                              padding = Thickness 10.,
                              orientation = setHorizontalIfLandscape model.IsLandscape,
                              horizontalOptions = LayoutOptions.Center,
                              verticalOptions = LayoutOptions.Center,
                              backgroundColor = Style.backgroundBrown,
                              children =
                                  [ View.StackLayout (
                                      orientation = setVerticalIfLandscape model.IsLandscape,
                                      backgroundColor = backgroundBrown,
                                      padding = Thickness (5., 10., 10., 0.),
                                      children = [
                                          View.Button (
                                              text = "Onda",
                                              backgroundColor = backgroundBrownLight,
                                              borderWidth = 1.,
                                              fontSize = normalFontSize,
                                              padding = Thickness (10., 10.),
                                              borderColor = backgroundBrownDark,
                                              command = (fun _ -> SetCurrentPage Home |> dispatch)
                                              )
                                          dateSelector model dispatch model.Date ]
                                    )

                                    //  separator model.IsLandscape model.IsDarkMode

                                    View.Label (
                                        formattedText =
                                            formatWaveDayDescriptions model.Date dispatch,
                                        lineBreakMode = LineBreakMode.WordWrap,
                                        horizontalOptions = LayoutOptions.Center) ]
                          )
                  ) ]
        )

    /// Display the date view page, the information about the nine waves at a
    /// date in a carousel view of consecutive days.
    ///
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>A list containing the date view page.</returns>
    let nineWavePage model dispatch =
        [ View.CarouselView (
            peekAreaInsets = Thickness 20.,
            loop = true,
            position = 1,
            backgroundColor = Style.backgroundBrownDark,
            verticalOptions = LayoutOptions.Center,
            horizontalOptions = LayoutOptions.Center,
            positionChanged = (fun args -> dispatch <| CarouselChanged args),
            items =
                [ tzolkinCard model dispatch model.Date
                  tzolkinCard model dispatch model.Date
                  tzolkinCard model dispatch model.Date ]
            )
         ]
