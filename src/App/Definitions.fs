// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  app
// File:     Definitions.fs
// Date:     4/10/2021 7:50:49 PM
//==============================================================================

/// The namespace of the IOS and Android Fabulous-TEMPLATE app.
namespace app


open Xamarin.Essentials
open Fabulous.XamarinForms
open Xamarin.Forms
open Fabulous
open System.Globalization
open System
open System.Reflection
open Svg.Skia

open RC.Maya



/// Holds the most basic definitions, the MVU model type `Model`, the MVU message type `Msg`,
/// the MVU `init` and `update` functions.
[<AutoOpen>]
module Definitions =

    // App-wide constants =========================================================================

    /// <summary>
    /// App name and package name, from the Android / IOS manifest.
    /// </summary>
    let appNameInfo = sprintf "%s (Package %s)" AppInfo.Name AppInfo.PackageName

    /// <summary>
    /// Version and build number.
    /// </summary>
    let version = sprintf "%s (Build %s)" AppInfo.VersionString AppInfo.BuildString

    /// <summary>
    /// App name and version, all in one string.
    /// </summary>
    let versionInfo = sprintf "%s %s" appNameInfo version

    /// <summary>
    /// Density of the device's screen, pixels are density x units.
    /// </summary>
    let screenDensity = DeviceDisplay.MainDisplayInfo.Density

    /// <summary>
    /// Width of the device's screen, changes in landscape and portrait mode.
    /// </summary>
    let screenWidth () = DeviceDisplay.MainDisplayInfo.Width

    /// <summary>
    /// Height of the device's screen, changes in landscape and portrait mode.
    /// </summary>
    let screenHeight () = DeviceDisplay.MainDisplayInfo.Height


    /// <summary>
    /// The current locale's date separator (like `/`, `.`, `-` ...).
    /// </summary>
    let localeSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator

    /// <summary>
    /// A string describing the current locale's short date format, like "MM/dd/yyyy".
    /// </summary>
    let localeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern

    // The model ===================================================================================

    /// The pages of the App.
    type Pages =
        | Home

    /// The MVU model.
    type Model =
        { CurrentPage: Pages
          IsDarkMode: bool
          IsLandscape: bool
          ShowSystemAppInfo: bool }

    // The messages ================================================================================

    /// MVU messages.
    type Msg =
        | SetCurrentPage of Pages
        | SetAppTheme of OSAppTheme
        | SetOrientation of float * float
        | ShowSystemAppInfo of bool
        | OpenURL of string

    // Widget references ===========================================================================
    // Instances of widgets needed to interact with.

   

    // Commands ====================================================================================
    /// <summary>
    /// Opens a URL using the system's default browser.
    /// </summary>
    /// <param name="url">The URL to open as a string.</param>
    /// <returns>`Cmd.none`</returns>
    let cmdOpenUrl (url) =
              Launcher.OpenAsync (new Uri (url))
              |> Async.AwaitTask
              |> Async.StartImmediate
              Cmd.none

    // Widget related ==============================================================================

    /// <summary>
    /// Return a stream of a resource's data.
    /// </summary>
    /// <param name="path">The path of the resource, like "app.images.number-02.svg".</param>
    /// <returns>A stream of the resource's data.</returns>
    let getResourceStream path =
         let assembly = IntrospectionExtensions.GetTypeInfo(typedefof<Model>).Assembly
         assembly.GetManifestResourceStream (path)

    /// <summary>
    /// Return a stream of a resource's data, given as filename in
    /// `app/images/`.
    /// </summary>
    /// <param name="filename">The filename of the resource, without the
    /// directory `app/images/`</param>
    /// <returns>A stream of the resource's data.</returns>
    let getImageStream filename =
        sprintf "app.images.%s" filename |> getResourceStream

    /// <summary>
    /// Return a stream of a SVG image's data.
    /// </summary>
    /// <param name="name">The name of the SVG to load, without extension ".svg".</param>
    /// <returns>A stream of the resource's data.</returns>
    let getSVGStream name =
        sprintf "app.images.%s.svg" name |> getResourceStream

    /// <summary>
    /// Convert a SVG image to a PNG image with the given height.
    /// </summary>
    /// <param name="spaceHeight">The height the PNG should have, in units
    /// (not pixels).</param>
    /// <param name="name">The name of the SVG to convert, without the ".svg"
    /// suffix</param>
    /// <returns>The image data of a SVG converted to a PNG</returns>
    let getPNGFromSVG spaceHeight name =
        let svg = new SKSvg ()
        let svgPicture = svg.Load (getSVGStream name)
        let height = float32 <| spaceHeight * screenDensity
        let scaleFac = height / svgPicture.CullRect.Height
        let bitmap1 = svgPicture.ToBitmap (SkiaSharp.SKColor.Empty,
                                    scaleFac,
                                    scaleFac,
                                    SkiaSharp.SKColorType.Rgba8888,
                                    SkiaSharp.SKAlphaType.Premul,
                                    SkiaSharp.SKColorSpace.CreateSrgb () )
        let image =  SkiaSharp.SKImage.FromBitmap (bitmap1)
        let data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100)
        let stream = data.AsStream (true)
        let data = Array.zeroCreate <| int stream.Length
        stream.Read (data, 0, data.Length) |> ignore
        data

    // Init ========================================================================================

    /// <summary>
    /// Initial state of the MVU model.
    /// </summary>
    let initModel =
        { CurrentPage = Home
          IsDarkMode =
              if Application.Current.RequestedTheme = OSAppTheme.Dark then
                  true
              else
                  false
          IsLandscape = false
          ShowSystemAppInfo = false }

    /// <summary>
    /// Initialize the model and commands.
    /// </summary>
    /// <returns>The initialized model and `Cmd.none` as tuple.</returns>
    let init () = initModel, Cmd.none

    // Functions needed by `update` ================================================================
