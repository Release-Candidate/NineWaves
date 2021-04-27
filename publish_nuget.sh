#!/bin/sh
# SPDX-License-Identifier: MIT
# Copyright (C) 2021 Roland Csaszar
#
# Project:  Fabulous-TEMPLATE
# File:     run_sharplint.sh
#
################################################################################

# The Nuget token must be saved using `nuget setapikey` to not need to input it.

dotnet nuget push ./src/Fabulous-TEMPLATE/bin/Release/Fabulous-TEMPLATE.*.nupkg --source https://api.nuget.org/v3/index.json
