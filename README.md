# listdepasm

This is the tool for getting dependent assemblies of specified managed assembly.

# Usage

## Get Binary

you can download binaries from release page, extract archive, then you will get executable file(listdepasm)

## Install from dotnet global tool

1. install .NET SDK
2. run `dotnet tool install -g listdepasm`
3. add `$HOME/.dotnet/tools` to PATH environment variable

## Run

run `listdepasm --help` and you will get help.
Here is the options

* `--asm`: examined assembly(Required)
* `--search-path`: additional search path for assembly(default: target assembly directory)
* `--max-depth`: maximum examining depth(default: 1)
