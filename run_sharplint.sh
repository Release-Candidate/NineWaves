#!/bin/sh
# SPDX-License-Identifier: MIT
# Copyright (C) 2021 Roland Csaszar
#
# Project:  Fabulous-TEMPLATE
# File:     run_sharplint.sh
#
################################################################################

# install dotnet tool install -g dotnet-fsharplint

dotnet fsharplint lint Fabulous-TEMPLATE.sln
