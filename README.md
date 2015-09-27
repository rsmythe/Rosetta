# Rosetta
C# to TypeScript via [Roslyn](https://github.com/dotnet/roslyn)

## Overview
**Status:** Ongoing (under development)

_Rosetta_ is a project for converting C# code into [TypeScript](http://www.typescriptlang.org/). We do this by means of project [Roslyn](https://github.com/dotnet/roslyn) by Microsoft.

## How it works
_Rosetta_ is written in C# and performs syntax analysis of C# code in order to convert it into _TypeScript_. There are many applications, however _Rosetta_ is developed with those specific ones as target:

- Providing a tool for converting c#-to-javascript (like [ScriptSharp](https://github.com/nikhilk/scriptsharp)) codebases into _TypeScript_.
- Providing a tool for converting C# odebases into _TypeScript_.

## Requirements
_Rosetta_ relies can be executed on the following platforms:

- Windows

_Rosetta_ depends on:

- The .NET Framework 4.0+.
- Project [Roslyn](https://github.com/dotnet/roslyn).
