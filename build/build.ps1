param([string]$DotnetPath='dotnet',[switch]$SkipRestore)
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
$output=Join-Path $projectRoot 'dist/LinksAudio-Portable-win-x64'
foreach($name in @('LinksSilk.dll','libmp3lame.64.dll')){if(!(Test-Path -LiteralPath (Join-Path $projectRoot ('src/LinksAudio.Codecs/native/'+$name)))){throw "Missing native dependency: $name"}}
$argsForPublish=@('publish',(Join-Path $projectRoot 'src/LinksAudio.App/LinksAudio.App.csproj'),'-c','Release','-o',$output)
if($SkipRestore){$argsForPublish+='--no-restore'}
& $DotnetPath @argsForPublish
if($LASTEXITCODE -ne 0){throw 'Release publish failed; do not distribute an older output as this build'}
foreach($name in @('TEST_REPORT.md','THIRD_PARTY_NOTICES.md','LICENSE','CHANGELOG.md')){Copy-Item -LiteralPath (Join-Path $projectRoot $name) -Destination $output -Force}
Copy-Item -LiteralPath (Join-Path $projectRoot 'docs/PORTABLE_README.md') -Destination (Join-Path $output 'README.md') -Force
Copy-Item -LiteralPath (Join-Path $projectRoot 'docs/PORTABLE_README_EN.md') -Destination (Join-Path $output 'README_EN.md') -Force
Copy-Item -LiteralPath (Join-Path $projectRoot 'licenses') -Destination $output -Recurse -Force
$sources=Join-Path $output 'licenses/sources';New-Item -ItemType Directory -Force $sources | Out-Null
Copy-Item -LiteralPath (Join-Path $projectRoot 'docs/lame-3.100.tar.gz') -Destination $sources -Force
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'lame/config.h') -Destination (Join-Path $sources 'lame-config.h') -Force
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'build-lame.ps1') -Destination $sources -Force
Copy-Item -LiteralPath (Join-Path $projectRoot 'docs/LAME_SOURCE_PROVENANCE.md') -Destination $sources -Force
$hashes=Get-ChildItem -LiteralPath $output -File | Where-Object Extension -in '.exe','.dll' | Get-FileHash -Algorithm SHA256 | Select-Object @{Name='File';Expression={Split-Path $_.Path -Leaf}},Hash
$runtimeConfig=Get-Content (Join-Path $projectRoot 'src/LinksAudio.App/bin/Release/net10.0-windows/win-x64/LinksAudio.runtimeconfig.json') -Raw | ConvertFrom-Json
@{App='Links Audio';Version='1.0.0';Author='Links Tam';Sdk=(& $DotnetPath --version);Runtime=$runtimeConfig.runtimeOptions.includedFrameworks;Target='win-x64';SelfContained=$true;Hashes=$hashes} | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $output 'build-info.json') -Encoding utf8
Write-Host 'Release publish and dependency copy complete.'
