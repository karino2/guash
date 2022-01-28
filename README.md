## Build

I build on OS X.

```
$ dotnet publish -c release -r osx-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishTrimmed=true
```

The result binary is at `bin/release/net5.0/osx-x64/publish/guash_doquery`.

## Install

Copy following files to directory in PATH.

- `bin/release/net5.0/osx-x64/publish/guash_doquery`
- `scripts/guash`
- `scripts/guash_filter`
- `scripts/guash_readtext`

## Open source libraries

Guash use following libraries.

- [bluma.css](https://bulma.io/)
- [Font awesome free 5 web](https://fontawesome.com/) 
- [Argu](https://www.nuget.org/packages/Argu/)
- [Photino.NET](https://www.nuget.org/packages/Photino.NET/)