// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  NineWavesApp
// File:     WaveView.fs
// Date:     4/30/2021 7:09:47 PM
//==============================================================================

/// The namespace of the IOS and Android NineWaves app.
namespace NineWavesApp

open System
open System.Diagnostics
open Xamarin.Forms
open Fabulous.XamarinForms
open Fabulous.XamarinForms.SkiaSharp

open RC.Maya


/// The page showing the graph of the waves.
[<AutoOpen>]
module WaveView=

    let mutable panDate = DateTime.Today

    let drawText
        (args: SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs)
        (x: float32)
        (y: float32)
        (text: string)
        (canvas: SkiaSharp.SKCanvas)
        (painter: SkiaSharp.SKPaint)
        (painterBackground: SkiaSharp.SKPaint)
        =
        let maxW = float32 args.Info.Width
        let maxH = float32 args.Info.Height
        let mutable boundingRect = SkiaSharp.SKRect ()
        painter.MeasureText (text, &boundingRect) |> ignore
        let w, h = boundingRect.Width, boundingRect.Height
        match x with
        | r when r + 23.f < maxW / 2.f && r + 23.f + w > maxW / 2.f ->
                                              boundingRect.Left <- r + 23.f - w
        | r when r + 23.f + w > maxW -> boundingRect.Left <- maxW - w - 10.f
        | r when r + 23.f < 0.f -> boundingRect.Left <- 0.f + 10.f
        | _ -> boundingRect.Left <- x + 23.f
        boundingRect.Right <- boundingRect.Left + w
        match y with
        | s when s - 15.f - h < 0.f -> boundingRect.Top <- 0.f + 15.f
        | s when s - 15.f > maxH -> boundingRect.Top <- maxH - h - 15.f
        | _ -> boundingRect.Top <- y - 15.f - h
        boundingRect.Bottom <- boundingRect.Top + h
        canvas.DrawRect (boundingRect, painterBackground)
        canvas.DrawText (text, boundingRect.Left, boundingRect.Bottom, painter)

    let drawWave
        args
        (canvas: SkiaSharp.SKCanvas)
        paint
        unitWidth
        unitHeight
        (getWaveDay : DateTime -> NineWaves.WaveDay)
        (waveFunc : float -> float)
        model
        starty
        =
        let graph = new SkiaSharp.SKPath ()
        let day = getWaveDay model.Date
        let isNight = day.IsNight
        let numDays = day.OfDays
        let currDay =
            day.DayNumber
            |> (fun d -> if isNight then d + numDays else d)

        let canvasX idx =
            unitWidth * (idx + 20.f)

        let getSpecialPoints idx =
               18.f * (idx - float32 currDay / float32 numDays)

        let getSpecialDate (idx: float32) =
               float (idx * float32 numDays - float32 currDay)

        let getWaveY canvasX =
            float canvasX
            |> ( * ) (float numDays / 18.)
            |> ( + ) (float currDay)
            |> waveFunc
            |> float32
            |> ( + ) starty
            |> ( * ) unitHeight

        let start = SkiaSharp.SKPoint ( 0.f, getWaveY -20.f )

        graph.MoveTo start

        [ for idx in -19.f .. 20.f ->  canvasX idx, getWaveY idx ]
        |> List.iter graph.LineTo

        canvas.DrawPath (graph, paint)

        use paintBlack = new SkiaSharp.SKPaint (
                                        Style = SkiaSharp.SKPaintStyle.Stroke,
                                        Color = blackSK,
                                        StrokeWidth = 7.5f )

        use paintBlackText = new SkiaSharp.SKPaint (
                                        Style = SkiaSharp.SKPaintStyle.StrokeAndFill,
                                        Color = blackSK )
        paintBlackText.TextSize <- 45.f

        use paintDot = new SkiaSharp.SKPaint (
                                        Style = SkiaSharp.SKPaintStyle.StrokeAndFill,
                                        Color = accentDarkRedSK,
                                        StrokeWidth = 7.5f )
        use paintDotInner = new SkiaSharp.SKPaint (
                                        Style = SkiaSharp.SKPaintStyle.StrokeAndFill,
                                        Color = backgroundBrownSK,
                                        StrokeWidth = 7.5f )
        let drawWaveDot (x, idx) =
            canvas.DrawCircle ( (x + 20.f) * unitWidth, getWaveY x, 15.f, paintDot)
            canvas.DrawCircle ( (x + 20.f) * unitWidth, getWaveY x, 7.5f, paintDotInner)
            let date = model.Date + TimeSpan.FromDays (getSpecialDate idx)
            drawText
                args
                (canvasX x)
                (getWaveY x)
                (sprintf "%s" (date.ToShortDateString ()))
                canvas
                paintBlackText
                paintDotInner

        let dayQuotient = float32 (Math.Floor (float currDay / float numDays))

        [ for idx in dayQuotient - 1.5f .. 0.5f .. dayQuotient + 1.5f -> getSpecialPoints idx, idx ]
        |> List.filter (fun (e, _) -> -20.f <= e && e <= 20.f)
        |> List.iter drawWaveDot

        canvas.DrawCircle ( 20.f * unitWidth, getWaveY 0.f, 15.f, paintBlack)

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

        xAxis.MoveTo (SkiaSharp.SKPoint ( 0.f, 1.5f * unitHeight))

        xAxis.LineTo (SkiaSharp.SKPoint ( 40.f * unitWidth, 1.5f * unitHeight))

        canvas.DrawPath (xAxis, paintDarkBrown)

        let xAxis2 = new SkiaSharp.SKPath ()

        xAxis2.MoveTo (SkiaSharp.SKPoint ( 0.f, 4.5f * unitHeight))

        xAxis2.LineTo (SkiaSharp.SKPoint ( 40.f * unitWidth, 4.5f * unitHeight))

        canvas.DrawPath (xAxis2, paintDarkBrown)

        let xAxis3 = new SkiaSharp.SKPath ()

        xAxis3.MoveTo (SkiaSharp.SKPoint ( 0.f, 7.5f * unitHeight))

        xAxis3.LineTo (SkiaSharp.SKPoint ( 40.f * unitWidth, 7.5f * unitHeight))

        canvas.DrawPath (xAxis3, paintDarkBrown)

        let yAxis = new SkiaSharp.SKPath ()

        yAxis.MoveTo (SkiaSharp.SKPoint ( 20.f * unitWidth, 0.f))

        yAxis.LineTo (SkiaSharp.SKPoint ( 20.f * unitWidth, 9.f * unitHeight))

        canvas.DrawPath (yAxis, paintDarkBrown)

        drawWave
            args
            canvas
            paint
            unitWidth
            unitHeight
            NineWaves.getWaveday9
            NineWaves.wavefunc9
            model
            1.5f

        drawWave
            args
            canvas
            paint
            unitWidth
            unitHeight
            NineWaves.getWaveday8
            NineWaves.wavefunc8
            model
            4.5f

        drawWave
            args
            canvas
            paint
            unitWidth
            unitHeight
            NineWaves.getWaveday7
            NineWaves.wavefunc7
            model
            7.5f

    let waveView model dispatch =
        [ View.StackLayout (
                orientation = StackOrientation.Horizontal,
                backgroundColor = backgroundBrown,
                padding = Thickness (5., 10., 10., 0.),
                children = [
                    View.Button (
                        text = "Calendario",
                        backgroundColor = backgroundBrownLight,
                        borderWidth = 1.,
                        fontSize = normalFontSize,
                        padding = Thickness (10., 10.),
                        borderColor = backgroundBrownDark,
                        command = (fun _ -> SetCurrentPage Waves |> dispatch)
                        )
                    dateSelector model dispatch model.Date ]
          )
          View.SKCanvasView (
                enableTouchEvents = false,
                invalidate = true,
                backgroundColor = backgroundBrown,
                horizontalOptions = LayoutOptions.FillAndExpand,
                verticalOptions = LayoutOptions.FillAndExpand,
                gestureRecognizers =
                    [ View.PanGestureRecognizer (touchPoints=1,
                        panUpdated=(fun panArgs ->
                            if panArgs.StatusType = GestureStatus.Running then
                                dispatch (SetDate (panDate - TimeSpan.FromDays (panArgs.TotalX/10.)))
                            elif panArgs.StatusType = GestureStatus.Started then
                                panDate <- model.Date
                                )
                      ) ],
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

