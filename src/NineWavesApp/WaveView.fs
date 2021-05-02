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

open SkiaSharp
open SkiaSharp.Views.Forms
open Xamarin.Forms
open Fabulous.XamarinForms
open Fabulous.XamarinForms.SkiaSharp

open RC.Maya


/// The page showing the graph of the 9th, 8th and 7th waves.
[<AutoOpen>]
module WaveView=

    /// <summary>The date to save the model's date in while panning the graph
    /// view.</summary>
    let mutable panDate = DateTime.Today

    /// <summary>Draw text with a brown (transparent ;) background, try to keep
    ///  the whole text on screen.</summary>
    /// <param name="args">`SKPaintSurfaceEventArgs` used for the available
    /// width and height.</param>
    /// <param name="x">X coordinate to draw the text to (lower left corner).</param>
    /// <param name="y">Y coordinate to draw the text to (lower left corner).</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="canvas">`SKCanvas` to use to paint.</param>
    /// <param name="painter">`SKPaint`to use to paint the text with.</param>
    /// <param name="painterBackground">`SKPaint`to use to paint the text's
    /// background with.</param>
    let drawText
        (args: SKPaintSurfaceEventArgs)
        x
        y
        text
        (canvas: SKCanvas)
        (painter: SKPaint)
        painterBackground
        =
        let maxW = float32 args.Info.Width
        let maxH = float32 args.Info.Height
        let mutable boundingRect = SKRect ()
        painter.MeasureText ((text: string), &boundingRect) |> ignore
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

    /// <summary>Draw the graph of a wave, including dots at the changes of night and day
    /// and the highest and lowest points of the days and nights.</summary>
    /// <param name="args">`SKPaintSurfaceEventArgs` used for the available
    /// width and height.</param>
    /// <param name="canvas">`SKCanvas` to use to paint.</param>
    /// <param name="paint">`SKPaint`to use to paint the text with.</param>
    /// <param name="unitWidth">Factor to scale all x values with.</param>
    /// <param name="unitHeight">Factor to scale all y values with.</param>
    /// <param name="getWaveDay">The `WaveDay` function to use to get data about
    /// the wave</param>
    /// <param name="waveFunc">The sine function of the wave to use.</param>
    /// <param name="model">The MVU model.</param>
    /// <param name="starty">The y offset of the x axis to use.</param>
    let drawWave
        args
        (canvas: SKCanvas)
        paint
        unitWidth
        unitHeight
        (getWaveDay : DateTime -> NineWaves.WaveDay)
        (waveFunc : float -> float)
        model
        starty
        =
        let graph = new SKPath ()
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

        let start = SKPoint ( 0.f, getWaveY -20.f )
        graph.MoveTo start
        [ for idx in -19.f .. 20.f ->  canvasX idx, getWaveY idx ]
        |> List.iter graph.LineTo
        canvas.DrawPath (graph, paint)

        use paintBlack = new SKPaint (  Style = SKPaintStyle.Stroke,
                                        Color = blackSK,
                                        StrokeWidth = pointStrokeWidth )

        use paintBlackText = new SKPaint (Style = SKPaintStyle.StrokeAndFill,
                                            Color = blackSK )
        paintBlackText.TextSize <- graphTextSize

        use paintDot = new SKPaint ( Style = SKPaintStyle.StrokeAndFill,
                                    Color = accentDarkRedSK,
                                    StrokeWidth = pointStrokeWidth )
        use paintDotInner = new SKPaint ( Style = SKPaintStyle.StrokeAndFill,
                                        Color = backgroundBrownSK,
                                        StrokeWidth = pointStrokeWidth )
        let drawWaveDot (x, idx) =
            canvas.DrawCircle ( (x + 20.f) * unitWidth,
                                getWaveY x,
                                dotCirceRadius,
                                paintDot )
            canvas.DrawCircle ( (x + 20.f) * unitWidth,
                                getWaveY x,
                                innerDotCircleRadius,
                                paintDotInner )

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

        canvas.DrawCircle ( 20.f * unitWidth, getWaveY 0.f, dotCirceRadius, paintBlack)

    /// <summary>Draw the graphs of all 3 waves, including axis.</summary>
    /// <param name="args">The `SKPaintSurfaceEventArgs` to use to draw.</param>
    /// <param name="model">The MVU model.</param>
    let drawAllWaves (args: SKPaintSurfaceEventArgs) model =
        let unitHeight = float32 args.Info.Height / 9.f
        let unitWidth = float32 args.Info.Width / 40.F
        let surface = args.Surface
        let canvas = surface.Canvas
        canvas.Clear()
        use paint = new SKPaint ( Style = SKPaintStyle.Stroke,
                                Color = accentDarkRedSK,
                                StrokeWidth = graphStrokeWidth )

        use paintDarkBrown = new SKPaint ( Style = SKPaintStyle.Stroke,
                                        Color = backgroundBrownDarkSK,
                                        StrokeWidth = axisStrokeWidth )

        let drawXAxis y =
            let xAxis = new SKPath ()
            xAxis.MoveTo (SKPoint ( 0.f, y * unitHeight))
            xAxis.LineTo (SKPoint ( 40.f * unitWidth, y * unitHeight))
            canvas.DrawPath (xAxis, paintDarkBrown)

        drawXAxis 1.5f
        drawXAxis 4.5f
        drawXAxis 7.5f

        let yAxis = new SKPath ()
        yAxis.MoveTo (SKPoint ( 20.f * unitWidth, 0.f))
        yAxis.LineTo (SKPoint ( 20.f * unitWidth, 9.f * unitHeight))
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

    /// <summary>The pan-able view showing the graphs of the 9th, the 8th and the 7th
    /// wave.</summary>
    /// <param name="model">The MVU model.</param>
    /// <param name="dispatch">The message dispatch function</param>
    /// <returns>The list of `Views` of the graph page.</returns>
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
                    [ View.PanGestureRecognizer (touchPoints = 1,
                        panUpdated = (fun args ->
                            if args.StatusType = GestureStatus.Running then
                                dispatch (SetDate (panDate -
                                            TimeSpan.FromDays (args.TotalX/panSpeedInverse)))
                            elif args.StatusType = GestureStatus.Started then
                                panDate <- model.Date
                                )
                      ) ],
                paintSurface = (fun args -> drawAllWaves args model)
          )
          View.Label (
                text = versionInfo,
                fontSize = FontSize.fromNamedSize NamedSize.Micro,
                textColor = Style.foregroundColor model.IsDarkMode,
                backgroundColor = Style.backgroundBrownDark,
                verticalTextAlignment = TextAlignment.End,
                horizontalTextAlignment = TextAlignment.End,
                horizontalOptions = LayoutOptions.Fill,
                verticalOptions = LayoutOptions.Fill
                )
        ]
