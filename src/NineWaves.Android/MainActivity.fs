// SPDX-License-Identifier: MIT
// Copyright 2018 Fabulous contributors.
// Copyright 2021 Roland Csaszar
//
// Project:  NineWaves.Android
// File:     MainActivity.fs
//
//==============================================================================

namespace NineWaves.Android

open System

open Android.App
open Android.Content
open Android.Content.PM
open Android.Runtime
open Android.Views
open Android.Widget
open Android.OS
open Xamarin.Forms.Platform.Android
open NineWaves.Android

open NineWavesApp

[<Activity (Label = "NineWaves.Android", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation))>]
type MainActivity() =
    inherit FormsAppCompatActivity()
    override this.OnCreate(bundle: Bundle) =
        FormsAppCompatActivity.TabLayoutResource <- NineWaves.Android.Resources.Layout.Tabbar
        FormsAppCompatActivity.ToolbarResource <- NineWaves.Android.Resources.Layout.Toolbar

        base.OnCreate (bundle)
        Xamarin.Essentials.Platform.Init(this, bundle)
        Xamarin.Forms.Forms.Init(this, bundle)
        this.LoadApplication(NineWavesApp.App ())

    override this.OnRequestPermissionsResult(requestCode: int, permissions: string[], [<GeneratedEnum>] grantResults: Android.Content.PM.Permission[]) =
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults)
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults)
