// SPDX-License-Identifier: MIT
// Copyright (C) 2021 Roland Csaszar
//
// Project:  NineWaves
// File:     NineWaves.fs
//
//==============================================================================

/// Namespace containing all Maya calendar and 'nine waves' libraries.
namespace RC.Maya

open System

/// Module holding everything needed for calculations with the nine waves.
module NineWaves=

    /// The number of significant
    let RoundPrecision = 12

    /// The value to use instead of `0` for comparisons.
    let Epsilon = 0.0001

    /// Round a float to `RoundPrecision` number of fractional digits.
    ///
    /// Params:
    ///         `t` The float to round.
    ///
    /// Returns:
    ///         The float rounded to `RoundPrecision` number of fractional
    ///         digits.
    let round (t: float) =
        Math.Round (t, RoundPrecision)

    /// Compare two floats, checks if `|a - b| < Epsilon`.
    ///
    /// Params:
    ///         `a` and `b`, the floats to compare for equality.
    ///
    /// Returns:
    ///             `true`, if the two numbers are 'same enough', `false` else.
    let same a b =
       (round a - round b) |> Math.Abs < Epsilon

    /// Constant, `2π`.
    let TwoPi = 2. * Math.PI

    /// The date all 9 waves have finished their first 13 cycles (half wavelengths).
    /// 28th of October, 2011.
    let referenceDate = DateTime (2011, 10, 28)

    // Cycle times / wavelengths ===============================================

    /// Wavelength of the 9th wave, 36 days.
    let wavelength9 = 36.

    /// Wavelength of the 8th wave, 36 * 20 days, 720 days.
    let wavelength8 = 36. * 20.

    /// Wavelength of the 7th wave, 36 * 20 * 20 days, 14,400 days, ~40 years.
    let wavelength7 = 36. * 20. * 20.

    /// Wavelength of the 6th wave, 36 * 20 * 20 * 20 days, 288,000 days,
    /// ~789 years.
    let wavelength6 = 36. * 20. * 20. * 20.

    /// Wavelength of the 5th wave, 36 * 20 * 20 * 20 * 20 days, 5,760,000 days,
    /// ~15,770 years.
    let wavelength5 = 36. * 20. * 20. * 20. * 20.

    /// Wavelength of the 4th wave, 36 * 20 * 20 * 20 * 20 * 20 days,
    /// 115,200,000 days, ~315,407 years.
    let wavelength4 = 36. * 20. * 20. * 20. * 20. * 20.

    /// Wavelength of the 3rd wave, 36 * 20 * 20 * 20 * 20 * 20 * 20 days,
    /// 2,304,000,000 days, ~6,308,142 years.
    let wavelength3 = 36. * 20. * 20. * 20. * 20. * 20. * 20.

    /// Wavelength of the 2nd wave, 36 * 20 * 20 * 20 * 20 * 20 * 20 * 20 days,
    /// 46,080,000,000 days, ~126,162,859 years.
    let wavelength2 = 36. * 20. * 20. * 20. * 20. * 20. * 20. * 20.

    /// Wavelength of the 1st wave, 36 * 20 * 20 * 20 * 20 * 20 * 20 * 20 * 20 days,
    /// 921,600,000,000 days, ~2,523,257,180 years.
    let wavelength1 = 36. * 20. * 20. * 20. * 20. * 20. * 20. * 20. * 20.

    /// A sine wave with a wave length of `wavelength`, that is 0 when `t = 0`.
    /// Has an amplitude of 1.
    /// Because of the factor of `1 / wavelength` this causes numerical problems
    /// with large wavelengths (like `wavelength1`).
    ///
    /// Params:
    ///         `wavelength` The wavelength of the sine wave.
    ///         `t` The parameter, time.
    ///
    /// Returns:
    ///          A point on the sine wave at the parameter `t`. Rounded to
    ///          ``RoundPrecision` number of fractional digits.
    let wavefunc wavelength t =
        (TwoPi * t) / wavelength
        |> sin
        |> round
        |> ( * ) -1.

    /// Sine wave describing the 9th wave, with a wave length of `wavelength9`.
    let wavefunc9 = wavefunc wavelength9

    /// Sine wave describing the 8th wave, with a wave length of `wavelength8`.
    let wavefunc8 = wavefunc wavelength8

    /// Sine wave describing the 7th wave, with a wave length of `wavelength7`.
    let wavefunc7 = wavefunc wavelength7

    /// Sine wave describing the 6th wave, with a wave length of `wavelength6`.
    let wavefunc6 = wavefunc wavelength6

    /// Sine wave describing the 5th wave, with a wave length of `wavelength5`.
    let wavefunc5 = wavefunc wavelength5

    /// Sine wave describing the 4th wave, with a wave length of `wavelength4`.
    let wavefunc4 = wavefunc wavelength4

    /// Sine wave describing the 3rd wave, with a wave length of `wavelength3`.
    let wavefunc3 = wavefunc wavelength3

    /// Sine wave describing the 2nd wave, with a wave length of `wavelength2`.
    let wavefunc2 = wavefunc wavelength2

    /// Sine wave describing the 1st wave, with a wave length of `wavelength1`.
    let wavefunc1 = wavefunc wavelength1

    /// Type describing a date relative to a wave.
    ///
    /// Fields:
    ///         `DayNumber` The day since the start of the last wave's cycle.
    ///         `OfDays` The number of days in the wave's cycle.
    ///         `WaveNumber` The number of cycles since the reference date.
    ///         `IsNight` Is this a 'night' (< 0) or a 'day' (> 0)?
    type WaveDay = {
        DayNumber: int64;
        OfDays: int64;
        WaveNumber: int;
        IsNight: bool
        }

    /// Return the `WaveDay`, the location of a date in a given wave.
    ///
    /// Params:
    ///         `waveLength` The wavelength of the wave to calculate.
    ///         `date` The date to calculate the location in the wave of.
    ///
    /// Returns:
    ///          A filled `WaveDay` instance.
    let getWaveday (waveLength: float) date =
        let cycleLen = int64 (waveLength * 0.5)
        let dayDiff = date - referenceDate |> (fun ts -> ts.Days)

        let waveNum =
            int64 (dayDiff + (if dayDiff > 0 then -1 else 0) ) /
            cycleLen

        let dayNum =
            int64 dayDiff % cycleLen
            |> (fun i -> if i > 0L then i else cycleLen + i)

        let waveNumCalc num  =
            match num with
            | i when i % 2 = 0 -> i
            | _ -> num + Math.Sign num
            |> (fun i -> i / 2)
            |> ( + ) 7

        { DayNumber = dayNum
          OfDays = cycleLen
          WaveNumber = waveNumCalc (int waveNum)
          IsNight = if dayDiff - 1 < 0 then
                        abs (waveNum) % 2L = 1L
                    else
                        waveNum % 2L = 0L }

    /// Return the `WaveDay` instance for a date relative to the 9th wave.
    let getWaveday9 = getWaveday wavelength9

    /// Return the `WaveDay` instance for a date relative to the 8th wave.
    let getWaveday8 = getWaveday wavelength8

    /// Return the `WaveDay` instance for a date relative to the 7th wave.
    let getWaveday7 = getWaveday wavelength7

    /// Return the `WaveDay` instance for a date relative to the 6th wave.
    let getWaveday6 = getWaveday wavelength6

    /// Return the `WaveDay` instance for a date relative to the 5th wave.
    let getWaveday5 = getWaveday wavelength5

    /// Return the `WaveDay` instance for a date relative to the 4th wave.
    let getWaveday4 = getWaveday wavelength4

    /// Return the `WaveDay` instance for a date relative to the 3th wave.
    let getWaveday3 = getWaveday wavelength3

    /// Return the `WaveDay` instance for a date relative to the 2th wave.
    let getWaveday2 = getWaveday wavelength2

    /// Return the `WaveDay` instance for a date relative to the 1th wave.
    let getWaveday1 = getWaveday wavelength1
