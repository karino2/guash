### Build

I build on OS X.

```
$ dotnet publish -c release -r osx.11.0-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishTrimmed=true
```

The result binary is at `bin/release/net5.0/osx.11.0-x64/publish/guash_doquery`.

### Install

Copy following files to directory in PATH.

- `bin/release/net5.0/osx.11.0-x64/publish/guash_doquery`
- `scripts/guash`
- `scripts/guash_filter`
- `scripts/guash_readtext`
