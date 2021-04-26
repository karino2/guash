#!/bin/sh

dotnet publish -c release -r osx.11.0-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishTrimmed=true
cp bin/release/net5.0/osx.11.0-x64/publish/guash ~/bin