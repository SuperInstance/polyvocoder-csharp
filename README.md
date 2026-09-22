# polyvocoder-csharp

> **C#/.NET 9 port of the Polyvocoder universal head.**
> Schema-parity with the Python, TypeScript, and Rust implementations.
> Polyformalism port 6 of 7.

## Quick Start

```bash
dotnet run --project Program.csproj
```

Should output:
```
fnv1a-64('café Δ 日本語') = 0x024a555471370b18d
verify: PASS
```

## Fleet Canary

The C# port pins to the Quilt fleet canary:

**`fnv1a-64('café Δ 日本語') = 0x024a555471370b18d`**

This is verified by the `Program.cs` `Main()` method.

## Schema Parity

JSON serialization uses both `canon_worthy` (snake_case) and `canonWorthy` (camelCase)
property names. Result JSON is interoperable with the TypeScript binding and Python
implementation.

## Why C#/.NET?

The C# port provides:
- **Cross-platform** — Windows, Linux, macOS via .NET 9
- **Enterprise tooling** — Visual Studio, Rider, NuGet integration
- **Performance** — Comparable to Java for compute-heavy work
- **Type safety** — Nullable reference types + records + pattern matching

## Use Cases

- **Native desktop apps** — UWP, WinUI, MAUI
- **Enterprise backend** — ASP.NET Core, Azure Functions
- **Game dev** — Unity, Godot C# bindings
- **Windows-native** — PowerShell modules, WPF apps

## Polyformalism Fleet

This is port 6 of the polyformalism fleet:
1. Python
2. TypeScript
3. Rust
4. Bash
5. JavaScript ESM
6. **C#/.NET 9** ← you are here
7. (reserved)

All ports share the canary `0x024a555471370b18d`.

## License

MIT — Casey / SuperInstance, Sept 22, 2026
