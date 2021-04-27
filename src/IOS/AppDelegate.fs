// SPDX-License-Identifier: Apache-2.0
// Copyright 2018 Fabulous contributors.
// Copyright 2021 Roland Csaszar
//
// Project:  iOS
// File:     AppDelegate.fs
//
//==============================================================================

namespace NineWaves.iOS

open System
open UIKit
open Foundation
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

open NineWavesApp



[<Register("AppDelegate")>]
type AppDelegate () =
    inherit FormsApplicationDelegate ()

    override this.FinishedLaunching(app, options) =
        Forms.Init ()

        let appcore = new NineWavesApp.App.App ()
        this.LoadApplication (appcore)
        base.FinishedLaunching (app, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main (args, null, "AppDelegate")
        0
