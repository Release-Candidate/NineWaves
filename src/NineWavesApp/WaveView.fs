// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  NineWavesApp
// File:     WaveView.fs
// Date:     4/30/2021 7:09:47 PM
//==============================================================================

/// The namespace of the IOS and Android NineWaves app.
namespace NineWavesApp

open System.Diagnostics
open Xamarin.Forms
open Fabulous.XamarinForms
open Fabulous.XamarinForms.SkiaSharp

open RC.Maya

/// The page showing the graph of the waves.
[<AutoOpen>]
module WaveView=

    let drawWave
        (canvas: SkiaSharp.SKCanvas)
        paint
        unitWidth
        unitHeight
        getWaveDay
        (waveFunc : float -> float)
        model
        starty
        =
        let graph = new SkiaSharp.SKPath ()

        let { NineWaves.WaveDay.DayNumber = currDayTemp
              NineWaves.WaveDay.OfDays = numDays
              NineWaves.WaveDay.IsNight = isNight } = getWaveDay model.Date

        Trace.TraceInformation <| sprintf "day: %d ofdays: %d isn: %b" currDayTemp numDays isNight

        let currDay = if isNight then currDayTemp + numDays else currDayTemp

        let xScaleFac = float numDays / 18.

        let start = SkiaSharp.SKPoint ( (float32 (-20. + float currDay)/float32 xScaleFac +
                                            20.f - float32 currDay/float32 xScaleFac) * unitWidth,
                                        (starty +
                                                float32 (waveFunc ((-20. + float currDay))))
                                                * unitHeight)

        graph.MoveTo start

        [ for idx in (-19. + float currDay) .. (20. + float currDay) ->
                    (float32 idx/float32 xScaleFac + 20.f - float32 currDay/float32 xScaleFac,
                        starty + float32 (waveFunc (idx))) ]

        |> List.map (fun (x, y) -> SkiaSharp.SKPoint (x * unitWidth, y * unitHeight) )
        |> List.iter graph.LineTo

        canvas.DrawPath (graph, paint)

    let drawAllWaves
        (args: SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs)
        (canvas: SkiaSharp.SKCanvas)
        paint
        model
        dispatch
        =
        let unitHeight = float32 args.Info.Height / 9.f
        let unitWidth = float32 args.Info.Width / 40.F

        use paintDarkBrown = new SkiaSharp.SKPaint (
                                Style = SkiaSharp.SKPaintStyle.Stroke,
                                Color = backgroundBrownDarkSK,
                                StrokeWidth = 10.0f )

        let xAxis = new SkiaSharp.SKPath ()

        xAxis.MoveTo (SkiaSharp.SKPoint ( 0.f, 1.f * unitHeight))

        xAxis.LineTo (SkiaSharp.SKPoint ( 40.f * unitWidth, 1.f * unitHeight))

        canvas.DrawPath (xAxis, paintDarkBrown)

        let xAxis2 = new SkiaSharp.SKPath ()

        xAxis2.MoveTo (SkiaSharp.SKPoint ( 0.f, 4.f * unitHeight))

        xAxis2.LineTo (SkiaSharp.SKPoint ( 40.f * unitWidth, 4.f * unitHeight))

        canvas.DrawPath (xAxis2, paintDarkBrown)

        let xAxis3 = new SkiaSharp.SKPath ()

        xAxis3.MoveTo (SkiaSharp.SKPoint ( 0.f, 7.f * unitHeight))

        xAxis3.LineTo (SkiaSharp.SKPoint ( 40.f * unitWidth, 7.f * unitHeight))

        canvas.DrawPath (xAxis3, paintDarkBrown)

        let yAxis = new SkiaSharp.SKPath ()

        yAxis.MoveTo (SkiaSharp.SKPoint ( 20.f * unitWidth, 0.f))

        yAxis.LineTo (SkiaSharp.SKPoint ( 20.f * unitWidth, 9.f * unitHeight))

        canvas.DrawPath (yAxis, paintDarkBrown)

        drawWave
            canvas
            paint
            unitWidth
            unitHeight
            NineWaves.getWaveday9
            NineWaves.wavefunc9
            model
            1.f

        drawWave
            canvas
            paint
            unitWidth
            unitHeight
            NineWaves.getWaveday8
            NineWaves.wavefunc8
            model
            4.f

        drawWave
            canvas
            paint
            unitWidth
            unitHeight
            NineWaves.getWaveday7
            NineWaves.wavefunc7
            model
            7.f

    let waveView model dispatch =
        [ View.Button (
            text = "Calendario",
            backgroundColor = backgroundBrownLight,
            command = (fun _ -> SetCurrentPage Waves |> dispatch)
          )
          View.SKCanvasView (
            enableTouchEvents = false,
            invalidate = false,
            backgroundColor = backgroundBrownLight,
            horizontalOptions = LayoutOptions.FillAndExpand,
            verticalOptions = LayoutOptions.FillAndExpand,
            paintSurface = (fun args ->
                let info = args.Info
                let surface = args.Surface
                let canvas = surface.Canvas

                canvas.Clear()
                use paint = new SkiaSharp.SKPaint (
                                Style = SkiaSharp.SKPaintStyle.Stroke,
                                Color = accentDarkRedSK,
                                StrokeWidth = 15.0f )
                drawAllWaves args canvas paint model dispatch
            ),
            touch = (fun args -> (args.Location.X, args.Location.Y) |> ignore)
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
            )
        ]

