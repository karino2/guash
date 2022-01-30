## Install

Binary drop is only available for Mac OSX.

### homebrew tap

```
$ brew tap karino2/tap
$ brew install karino2/tap/guash
```

### Troubleshooting on homebrew tap

If you see the error saying
```
A fatal error occurred. The required library libhostfxr.dylib could not be found.
```

You need to set DOTNET_ROOT like following (in .zshrc or .bashrc, etc.)

```
$ export DOTNET_ROOT="$(brew --prefix)/opt/dotnet/libexec"
```

### Manual installation

Download zip from releases and copy following files to directory in PATH.

- `bin/release/net5.0/osx-x64/publish/guash_doquery`
- `scripts/guash`
- `scripts/guash_filter`
- `scripts/guash_readtext`

### Manual build

You can build it in any environment supported in dotnet core sdk.

Here is the example of how I build on OS X.

```
$ dotnet publish -c release -r osx-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishTrimmed=true
```

The result binary is at `bin/release/net5.0/osx-x64/publish/guash_doquery`.


## Open source libraries

Guash use following libraries.

- [bluma.css](https://bulma.io/)
- [Font awesome free 5 web](https://fontawesome.com/) 
- [Argu](https://www.nuget.org/packages/Argu/)
- [Photino.NET](https://www.nuget.org/packages/Photino.NET/)