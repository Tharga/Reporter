@echo off

..\.nuget\NuGet.exe pack ..\Tharga.Reporter.Engine\Tharga.Reporter.Engine.csproj -Prop Configuration=Release

xcopy "*.nupkg" "C:\Dev\Nuget\" /Y