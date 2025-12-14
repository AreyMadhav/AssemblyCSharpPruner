# asmc-pruner

asmc-pruner is a command-line tool for pruning methods from compiled
Assembly-CSharp (.NET) binaries, commonly found in Unity-based applications.

It is designed to help reverse engineers, modders, and security researchers
reduce noise in Unity assemblies by identifying and removing unused or
irrelevant code paths.

The tool operates directly on compiled assemblies and does not require access
to the original Unity project or source code.

---

## Features

- Analyze Unity `Assembly-CSharp.dll` binaries
- Identify unused or unreachable code
- Prune assemblies to reduce size and complexity
- Designed for reverse engineering and analysis workflows
- Cross-platform (.NET)

---

## Use Cases

- Reverse engineering Unity games
- Malware and code analysis
- Modding and patching Unity-based applications
- Reducing analysis surface during static inspection

---

## Requirements

- .NET runtime (system-provided)
- Linux, Windows, or macOS

---

## Status

This project is under active development.
Interfaces and output formats may change until a stable release is published.

---

## License

This project is licensed under the MIT License.
See the LICENSE file for details.
