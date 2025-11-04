#!/bin/sh

# dotnet publish -c release -r osx-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishTrimmed=true
# dotnet publish -c release -r osx-x64 --self-contained false /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
dotnet publish -c release -r osx-arm64 --self-contained false /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true
cp bin/release/net9.0/osx-arm64/publish/guash_doquery ~/bin
