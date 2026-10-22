param([Parameter(Mandatory=$true)][string]$ZigPath)
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
$sdk=Join-Path $projectRoot 'vendor/silk-v3-decoder/silk'
$env:ZIG_GLOBAL_CACHE_DIR=Join-Path $projectRoot '.tools/zig-cache'
$env:ZIG_LOCAL_CACHE_DIR=Join-Path $projectRoot '.tools/zig-local'
New-Item -ItemType Directory -Force (Join-Path $projectRoot '.tools') | Out-Null
$sources=(Get-ChildItem -LiteralPath (Join-Path $sdk 'src') -Filter '*.c' -File).FullName
$generator=Join-Path $projectRoot '.tools/generate-fixture.exe'
& $ZigPath cc -O2 -s -fwrapv -fno-strict-aliasing -Wno-everything -I (Join-Path $sdk 'interface') -I (Join-Path $sdk 'src') (Join-Path $projectRoot 'tests/generate-fixture.c') @sources -o $generator
if($LASTEXITCODE -ne 0){throw 'Fixture generator build failed'}
& $generator (Join-Path $projectRoot 'tests/Fixtures/synthetic-tone.silk') (Join-Path $projectRoot 'tests/Fixtures/reference.pcm')
if($LASTEXITCODE -ne 0){throw 'Fixture generation failed'}
