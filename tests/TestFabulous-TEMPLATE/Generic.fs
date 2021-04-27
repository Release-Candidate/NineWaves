// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TestFabulous-TEMPLATE
// File:     Generic.fs
// Date:     4/24/2021 1:11:17 PM
//==============================================================================


namespace TestFabulous-TEMPLATE

open Expecto
open Swensen.Unquote
open System
open FsCheck
open Expecto.Logging

open RC.Maya

[<AutoOpen>]
module Generic=

    let private logger = Log.create "Fabulous-Template"

    let private loggerFunc logFunc moduleName name no args =
         logFunc (
            Message.eventX "{module} '{test}' #{no}, generated '{args}'"
            >> Message.setField "module" moduleName
            >> Message.setField "test" name
            >> Message.setField "no" no
            >> Message.setField "args" args )

    let loggerFuncDeb moduleName name no args =
        loggerFunc logger.debugWithBP moduleName name no args

    let loggerFuncInfo moduleName name no args =
        loggerFunc logger.infoWithBP moduleName name no args

    let config = { FsCheckConfig.defaultConfig with
                        maxTest = 10000
                        endSize = 1000000 }

    let configList = { FsCheckConfig.defaultConfig with
                            maxTest = 15
                            endSize = 500 }

    let configFasterThan = { FsCheckConfig.defaultConfig with
                                    maxTest = 100
                                    endSize = 1000000 }
