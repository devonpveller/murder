# AGENTS.md

> Bootstrapped by little-coder. Update as the codebase changes
> (see "Keep this file in sync" below). Delete this file + add
> `.no-agents-md` to the repo root if you'd rather not have it.

## What this repo is

**Murder Engine** — a pixel art ECS game engine built on top of vendored
MonoGame source (project reference to `../MonoGame/MonoGame.Framework/`).
Written in C# / .NET 8. Uses [Bang](https://github.com/isadorasophia/bang)
for its ECS framework. The engine ships its own editor (no external editor.exe)
— editor code lives in a separate project so it never pollutes game code.

> **Architecture rule**: murder builds from vendored MonoGame source; never use
> the `Murder.FNA` NuGet package. This repo is a git submodule inside
> `monogame-engine` at `vendor/murder`, with sibling submodule `vendor/MonoGame`.

## How to work in it

- **Build**: `dotnet build src/Murder.Editor`
- **Test**: `dotnet test src/Murder.Tests`
- **CI**: GitHub Actions builds on Linux, Windows, macOS (see `.github/workflows/ci.yml`)
- **Submodules**: `bang/` and `gum/` are git submodules (fetched recursively in CI)
- **Vendored MonoGame**: Build requires `../MonoGame/` sibling (from `monogame-engine` parent repo)

## Layout

| Path | Description |
|------|-------------|
| `src/Murder/` | Core engine — Game class, ECS integration, assets, scenes, data management |
| `src/Murder.Editor/` | Editor project — ImGui-based editor, importers, architect |
| `src/Murder.Analyzers/` | Roslyn analyzers for Murder |
| `src/Murder.Analyzers.Tests/` | Analyzer unit tests |
| `src/Murder.Serializer/` | Source generator for serialization |
| `src/Murder.Tests/` | Engine unit tests |
| `bang/` | Git submodule — Bang ECS framework |
| `gum/` | Git submodule — Gum UI framework |
| `resources/` | Editor resources (fonts, shaders, lua scripts, aseprite assets) |
| `media/` | Logo and screenshots |

## Conventions noticed

- Partial classes split by domain (e.g., `Game.cs`, `Game_Scenes.cs`)
- Nullable enabled, unsafe blocks allowed, implicit usings on
- `.editorconfig` enforces coding style
- CI builds `Murder.Editor` as the integration target; tests are in `Murder.Tests`
- Game assets use custom `.murder` format; textures are aseprite assets

## Keep this file in sync

When you change the structure (add/remove a module, shift a boundary, change
build/test commands), update the affected sections of THIS file by hand
before declaring the task complete. Re-read the relevant pieces, re-summarize,
commit.
