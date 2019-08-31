$version = $args[0]
Write-Host "Set version: $version"

$FullPath = Resolve-Path $PSScriptRoot\..\UnobtrusiveCode\source.extension.vsixmanifest
Write-Host $FullPath
[xml]$content = Get-Content $FullPath
$content.PackageManifest.Metadata.Identity.Version = $version
$content.Save($FullPath)
