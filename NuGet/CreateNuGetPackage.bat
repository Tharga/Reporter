@echo off

C:\Dev\Utils\NuGet.exe pack ..\Tharga.Reporter.Engine\Tharga.Reporter.Engine.csproj -Prop Configuration=Release

xcopy "*.nupkg" "C:\Dev\Nuget\"