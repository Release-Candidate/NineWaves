// SPDX-License-Identifier: MIT
// Copyright 2018 Fabulous contributors.
// Copyright 2021 Roland Csaszar
//
// Project:  Fabulous-TEMPLATE.Android
// File:     AssemblyInfo.fs
//
//==============================================================================

namespace Fabulous-TEMPLATE.Android

open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Android.App

// the name of the type here needs to match the name inside the ResourceDesigner attribute
type Resources = Fabulous-TEMPLATE.Android.Resource

[<assembly: Android.Runtime.ResourceDesigner("Fabulous-TEMPLATE.Android.Resources", IsApplication = true)>]

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[<assembly: AssemblyTitle("Fabulous-TEMPLATE")>]
[<assembly: AssemblyDescription("")>]
[<assembly: AssemblyConfiguration("")>]
[<assembly: AssemblyCompany("")>]
[<assembly: AssemblyProduct("Fabulous-TEMPLATE")>]
[<assembly: AssemblyCopyright("Copyright © 2021 Roland Csaszar")>]
[<assembly: AssemblyTrademark("")>]
[<assembly: AssemblyCulture("")>]
[<assembly: ComVisible(false)>]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [<assembly: AssemblyVersion("1.0.*")>]
[<assembly: AssemblyVersion("1.0.0.0")>]
[<assembly: AssemblyFileVersion("1.0.0.0")>]

#if DEBUG
[<assembly: Application(Debuggable = true)>]
#else
[<assembly: Application(Debuggable = false)>]
#endif

// Add some common permissions, these can be removed if not needed
do ()
