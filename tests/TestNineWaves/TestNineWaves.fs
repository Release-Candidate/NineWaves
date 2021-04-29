// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  TestNineWaves
// File:     Generic.fs
//
//==============================================================================


namespace TestNineWaves

open Expecto
open Swensen.Unquote
open System
open Expecto.Logging

open RC.Maya

[<AutoOpen>]
module TestNineWaves=

    // FSCheck configuration and logging ===========================================================

    let private logger = Log.create "NineWaves"

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
                        maxTest = 100000
                        endSize = 1000000 }

    let configGetWaveday = { FsCheckConfig.defaultConfig with
                                maxTest = 100000
                                endSize = 500 }

    let configFaster = { FsCheckConfig.defaultConfig with
                                    maxTest = 100
                                    endSize = 1000000 }


    // Helper functions ============================================================================

    let epsilon = NineWaves.Epsilon

    let maxValue = 2. * NineWaves.wavelength1

    let sanitizeFloat f =
        match f with
        | g when Double.IsNaN g -> 0.
        | g when Double.IsNegativeInfinity g -> -maxValue
        | g when Double.IsPositiveInfinity g -> maxValue
        | g when g > maxValue -> maxValue
        | g when -g > maxValue -> -maxValue
        | _ -> f

    let isSameEps a b =
        test <@ NineWaves.same a b @>

    // Generic functions ===========================================================================

    // Wavelengths =================================================================================

    let testWaveLength waveFunc waveLength t i =
        let tr = sanitizeFloat t
        isSameEps
            (waveFunc tr)
            (waveFunc <| (tr + float (i % 4)  * waveLength))

    let testHalfWaveLength waveFunc waveLength t i =
        let tr = sanitizeFloat t
        isSameEps
            -(waveFunc tr)
            (waveFunc <| (tr + float ((i % 4) * 2 + 1) * 0.5 * waveLength))

    let testWavelengthZero waveFunc waveLength i =
        isSameEps
            0.0
            (waveFunc <| (float (i % 200)  * waveLength))

    // getWaveDays =================================================================================

    let testGetWavedays getWaveday (waveLen: float) k j isN =
        let i = (abs k) * if Math.Sign (j: int) = 0 then 1 else Math.Sign (j: int)
        let waveNumber = i % 173 // arbitrary number
        let cycleLen = int64 waveLen / 2L
        let waveDay =
            int64 j % cycleLen
            |> (fun i -> if i < 1L then cycleLen + i else i)
        try
            let date: DateTime = NineWaves.referenceDate +
                                    TimeSpan.FromDays
                                        ((float waveNumber - 7.5) * waveLen +
                                        float waveDay +
                                        (if isN then float cycleLen else 0.))

            let (testResult: NineWaves.WaveDay) = getWaveday date
            test <@ testResult.DayNumber = waveDay @>
            test <@ testResult.WaveNumber = waveNumber  @>
            test <@ testResult.OfDays = cycleLen @>
            test <@ testResult.IsNight = isN @>
        with
        | :? OverflowException -> ()
        | :? ArgumentOutOfRangeException -> ()

    let referenceDates9 = [ (DateTime (2015, 5, 18),
                                { NineWaves.WaveDay.DayNumber = 2L;
                                  NineWaves.WaveDay.IsNight = true;
                                  NineWaves.WaveDay.WaveNumber = 43
                                  NineWaves.WaveDay.OfDays = 18L } )
                            (DateTime (2021, 4, 28),
                                { NineWaves.WaveDay.DayNumber = 14L;
                                  NineWaves.WaveDay.IsNight = true;
                                  NineWaves.WaveDay.WaveNumber = 103
                                  NineWaves.WaveDay.OfDays = 18L } )

                          ]

    let referenceDates8 = [ (DateTime (2015, 5, 18),
                                { NineWaves.WaveDay.DayNumber = 218L;
                                  NineWaves.WaveDay.IsNight = false;
                                  NineWaves.WaveDay.WaveNumber = 9
                                  NineWaves.WaveDay.OfDays = 360L } )
                          ]

    let referenceDates7 = [ (DateTime (2015, 5, 18),
                                { NineWaves.WaveDay.DayNumber = 1298L;
                                  NineWaves.WaveDay.IsNight = true;
                                  NineWaves.WaveDay.WaveNumber = 7
                                  NineWaves.WaveDay.OfDays = 7200L } )

                          ]

    let dayWalkList =
        [ for idx in -20 .. 20 do yield (idx, false); yield (idx, true)]
        |> List.map (fun (waveNum, isN) ->
                let cycleLen = NineWaves.wavelength9 * 0.5
                [ for i in 1 .. int (cycleLen) ->
                    ( NineWaves.referenceDate +
                      TimeSpan.FromDays
                                (float waveNum * NineWaves.wavelength9 +
                                (if isN then 0. else -cycleLen) +
                                float i),
                      { NineWaves.WaveDay.DayNumber = int64 i
                        NineWaves.WaveDay.IsNight = isN
                        NineWaves.WaveDay.WaveNumber = waveNum + 7
                        NineWaves.WaveDay.OfDays = int64 cycleLen } )
                 ] )
        |> List.collect id

    [<Tests>]
       let tests =
         testList
           "NineWaves"
            [
              testList
               "WaveLengths"
               [ // Test the wavelengths of all waves ==============================================

                testPropertyWithConfig config "wave1 has wavelength of wavelength1"
                <| fun t i -> testWaveLength NineWaves.wavefunc1 NineWaves.wavelength1 t i

                testPropertyWithConfig config "wave1 has a half wavelength of wavelength1 / 2"
                <| fun t i -> testHalfWaveLength NineWaves.wavefunc1 NineWaves.wavelength1 t i

                testPropertyWithConfig config "wave1 of wavelength1 is 0"
                <| fun i -> testWavelengthZero NineWaves.wavefunc1 NineWaves.wavelength1 i

                testPropertyWithConfig config "wave2 has wavelength of wavelength2"
                <| fun t i -> testWaveLength NineWaves.wavefunc2 NineWaves.wavelength2 t i

                testPropertyWithConfig config "wave2 has a half wavelength of wavelength2 / 2"
                <| fun t i -> testHalfWaveLength NineWaves.wavefunc2 NineWaves.wavelength2 t i

                testPropertyWithConfig config "wave2 of wavelength2 is 0"
                <| fun i -> testWavelengthZero NineWaves.wavefunc2 NineWaves.wavelength2 i

                testPropertyWithConfig config "wave3 has wavelength of wavelength3"
                <| fun t i -> testWaveLength NineWaves.wavefunc3 NineWaves.wavelength3 t i

                testPropertyWithConfig config "wave3 has a half wavelength of wavelength3 / 2"
                <| fun t i -> testHalfWaveLength NineWaves.wavefunc3 NineWaves.wavelength3 t i

                testPropertyWithConfig config "wave3 of wavelength3 is 0"
                <| fun i -> testWavelengthZero NineWaves.wavefunc3 NineWaves.wavelength3 i

                testPropertyWithConfig config "wave4 has wavelength of wavelength4"
                <| fun t i -> testWaveLength NineWaves.wavefunc4 NineWaves.wavelength4 t i

                testPropertyWithConfig config "wave4 has a half wavelength of wavelength4 / 2"
                <| fun t i -> testHalfWaveLength NineWaves.wavefunc4 NineWaves.wavelength4 t i

                testPropertyWithConfig config "wave4 of wavelength4 is 0"
                <| fun i -> testWavelengthZero NineWaves.wavefunc4 NineWaves.wavelength4 i

                testPropertyWithConfig config "wave5 has wavelength of wavelength5"
                <| fun t i -> testWaveLength NineWaves.wavefunc5 NineWaves.wavelength5 t i

                testPropertyWithConfig config "wave5 has a half wavelength of wavelength5 / 2"
                <| fun t i -> testHalfWaveLength NineWaves.wavefunc5 NineWaves.wavelength5 t i

                testPropertyWithConfig config "wave1 of wavelength5 is 0"
                <| fun i -> testWavelengthZero NineWaves.wavefunc5 NineWaves.wavelength5 i

                testPropertyWithConfig config "wave6 has wavelength of wavelength6"
                <| fun t i -> testWaveLength NineWaves.wavefunc6 NineWaves.wavelength6 t i

                testPropertyWithConfig config "wave6 has a half wavelength of wavelength6 / 2"
                <| fun t i -> testHalfWaveLength NineWaves.wavefunc6 NineWaves.wavelength6 t i

                testPropertyWithConfig config "wave6 of wavelength6 is 0"
                <| fun i -> testWavelengthZero NineWaves.wavefunc6 NineWaves.wavelength6 i

                testPropertyWithConfig config "wave7 has wavelength of wavelength7"
                <| fun t i -> testWaveLength NineWaves.wavefunc7 NineWaves.wavelength7 t i

                testPropertyWithConfig config "wave7 has a half wavelength of wavelength7 / 2"
                <| fun t i -> testHalfWaveLength NineWaves.wavefunc7 NineWaves.wavelength7 t i

                testPropertyWithConfig config "wave7 of wavelength7 is 0"
                <| fun i -> testWavelengthZero NineWaves.wavefunc7 NineWaves.wavelength7 i

                testPropertyWithConfig config "wave8 has wavelength of wavelength8"
                <| fun t i -> testWaveLength NineWaves.wavefunc8 NineWaves.wavelength8 t i

                testPropertyWithConfig config "wave8 has a half wavelength of wavelength8 / 2"
                <| fun t i -> testHalfWaveLength NineWaves.wavefunc1 NineWaves.wavelength1 t i

                testPropertyWithConfig config "wave8 of wavelength8 is 0"
                <| fun i -> testWavelengthZero NineWaves.wavefunc8 NineWaves.wavelength8 i

                testPropertyWithConfig config "wave9 has wavelength of wavelength9"
                <| fun t i -> testWaveLength NineWaves.wavefunc9 NineWaves.wavelength9 t i

                testPropertyWithConfig config "wave9 has a half wavelength of wavelength9 / 2"
                <| fun t i -> testHalfWaveLength NineWaves.wavefunc9 NineWaves.wavelength9 t i

                testPropertyWithConfig config "wave9 of wavelength9 is 0"
                <| fun i -> testWavelengthZero NineWaves.wavefunc9 NineWaves.wavelength9 i
               ]
              testList
                "GetWaveDay"
               [ // Test the `getWaveDay*` functions ===============================================

                testPropertyWithConfig configGetWaveday "Wave1 getWaveday"
                <| fun i j isN -> testGetWavedays NineWaves.getWaveday1 NineWaves.wavelength1 i j isN

                testPropertyWithConfig configGetWaveday "Wave2 getWaveday"
                <| fun i j isN -> testGetWavedays NineWaves.getWaveday2 NineWaves.wavelength2 i j isN

                testPropertyWithConfig configGetWaveday "Wave3 getWaveday"
                <| fun i j isN -> testGetWavedays NineWaves.getWaveday3 NineWaves.wavelength3 i j isN

                testPropertyWithConfig configGetWaveday "Wave4 getWaveday"
                <| fun i j isN -> testGetWavedays NineWaves.getWaveday4 NineWaves.wavelength4 i j isN

                testPropertyWithConfig configGetWaveday "Wave5 getWaveday"
                <| fun i j isN -> testGetWavedays NineWaves.getWaveday5 NineWaves.wavelength5 i j isN

                testPropertyWithConfig configGetWaveday "Wave6 getWaveday"
                <| fun i j isN -> testGetWavedays NineWaves.getWaveday6 NineWaves.wavelength6 i j isN

                testPropertyWithConfig configGetWaveday "Wave7 getWaveday"
                <| fun i j isN -> testGetWavedays NineWaves.getWaveday7 NineWaves.wavelength7 i j isN

                testPropertyWithConfig configGetWaveday "Wave8 getWaveday"
                <| fun i j isN -> testGetWavedays NineWaves.getWaveday8 NineWaves.wavelength8 i j isN

                testPropertyWithConfig configGetWaveday "Wave9 getWaveday"
                <| fun i j isN -> testGetWavedays NineWaves.getWaveday9 NineWaves.wavelength9 i j isN

                // Reference dates =================================================================

                testCase "Wave9 reference dates"
                <| fun () ->
                    referenceDates9
                    |> List.iter (fun elem ->
                                    let date, result = elem
                                    test <@ NineWaves.getWaveday9 date = result @>
                                 )

                testCase "Wave8 reference dates"
                <| fun () ->
                    referenceDates8
                    |> List.iter (fun elem ->
                                    let date, result = elem
                                    test <@ NineWaves.getWaveday8 date = result @>
                                 )
                testCase "Wave7 reference dates"
                <| fun () ->
                    referenceDates7
                    |> List.iter (fun elem ->
                                    let date, result = elem
                                    test <@ NineWaves.getWaveday7 date = result @>
                                 )

                // Walk through a list of days =====================================================
                testCase "Wave9 walk list of days and nights"
                <| fun () ->
                        dayWalkList
                        |> List.iter (fun elem ->
                                let date, result = elem
                                test <@ NineWaves.getWaveday9 date = result @>
                                )

               ]
            ]
