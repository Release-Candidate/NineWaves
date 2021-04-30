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
    /// Show the Tzolk’in date on the left and a date picker to select the date
    /// to display on the right.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <param name="date">The gregorian date to display as a Tzolk’in date.</param>
    /// <returns>A list of `Frame` to show the Tzolk’in date of the date
    /// selected in the second Frame.</returns>
    let dateSelector model dispatch date =
        [ View.Frame (
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
            ) ]

    let formatWaveDay (waveDay: NineWaves.WaveDay) waveNum =
       [ View.Span ( text = (sprintf "%d. Wave %d:" waveDay.WaveNumber waveNum),
                    fontAttributes = FontAttributes.Bold,
                    fontSize = FontSize.fromValue 18.,
                    lineHeight = 1.8,
                    textColor = Color.Black
         )
         View.Span ( text = (sprintf
                                "%s %d / %d\n"
                                (if waveDay.IsNight then "Night" else "Day")
                                waveDay.DayNumber
                                waveDay.OfDays
                                ),
                    fontAttributes = FontAttributes.Bold,
                    fontSize = FontSize.fromValue 18.,
                    lineHeight = 1.8,
                    textColor = accentDarkRed
        ) ]

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
    /// Display a `Frame` containing the Tzolk’in date, the gregorian date and
    /// the Tzolk’in day glyph description of the gregorian date selected with
    /// the date picker.
    /// </summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <param name="date">The gregorian date to display as Tzolk’in date.</param>
    /// <returns>A `StackLayout` holding the Tzolk’in date, the gregorian date and
    /// the Tzolk’in day glyph description of the gregorian date selected with
    /// the date picker.</returns>
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
                                      padding = Thickness (5.0, 10.0, 5.0, 10.0),
                                      orientation = setVerticalIfLandscape model.IsLandscape,
                                      backgroundColor = Style.backgroundBrown,
                                      horizontalOptions = LayoutOptions.Center,
                                      children = dateSelector model dispatch date
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

    /// Display the date view page, the Tzolk’in day information in a carousel
    /// view of consecutive days.
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

          View.Label (
                text = versionInfo,
                fontSize = FontSize.fromNamedSize NamedSize.Micro,
                textColor = Style.foregroundColor model.IsDarkMode,
                backgroundColor = Style.backgroundBrownDark,//Style.backgroundColor model.IsDarkMode,
                verticalTextAlignment = TextAlignment.End,
                horizontalTextAlignment = TextAlignment.End,
                horizontalOptions = LayoutOptions.Fill,
                verticalOptions = LayoutOptions.Fill
            ) ]
