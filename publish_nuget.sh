#!/bin/sh
# SPDX-License-Identifier: MIT
# Copyright (C) 2021 Roland Csaszar
#
# Project:  NineWaves
# File:     run_sharplint.sh
#
################################################################################

# The Nuget token must be saved using `nuget setapikey` to not need to input it.


dotnet nuget push ./src/NineWaves/bin/Release/NineWaves.*.nupkg --source https://api.nuget.org/v3/index.json
