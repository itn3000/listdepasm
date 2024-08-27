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

## Limitation

**Cannot read deps from single binary**

## Example

```
> listdepasm.exe --asm listdepasm.dll
{
  "Name": "listdepasm",
  "Version": "0.0.1.0",
  "Location": "C:\\Users\\skito\\Documents\\repos\\listdepasm\\listdepasm\\bin\\Release\\net8.0",
  "Dependencies": [
    {
      "Name": "System.Runtime",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    },
    {
      "Name": "System.Collections",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    },
    {
      "Name": "System.Text.Json",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    },
    {
      "Name": "System.ComponentModel",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    },
    {
      "Name": "System.ComponentModel.Annotations",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    },
    {
      "Name": "System.Runtime.InteropServices",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    },
    {
      "Name": "System.Linq",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    },
    {
      "Name": "System.Text.Encodings.Web",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    },
    {
      "Name": "System.Console",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    },
    {
      "Name": "System.Memory",
      "Version": "8.0.0.0",
      "Location": "",
      "Dependencies": []
    }
  ]
}
```

# ChangeLog

## 0.0.1

initial release