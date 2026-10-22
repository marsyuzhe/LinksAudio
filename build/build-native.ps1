param([Parameter(Mandatory=$true)][string]$ZigPath)
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
$sdkRoot=Join-Path $projectRoot 'vendor/silk-v3-decoder/silk'
$env:ZIG_GLOBAL_CACHE_DIR=Join-Path $projectRoot '.tools/zig-cache'
$env:ZIG_LOCAL_CACHE_DIR=Join-Path $projectRoot '.tools/zig-local'
$nativeSources=(Get-ChildItem -LiteralPath (Join-Path $sdkRoot 'src') -Filter '*.c' -File).FullName
$nativeOutput=Join-Path $projectRoot 'src/LinksAudio.Codecs/native/LinksSilk.dll'
& $ZigPath cc -shared -O2 -s -fwrapv -fno-strict-aliasing -Wno-everything -I (Join-Path $sdkRoot 'interface') -I (Join-Path $sdkRoot 'src') (Join-Path $projectRoot 'src/LinksAudio.Codecs/native/links_silk.c') @nativeSources -o $nativeOutput
if($LASTEXITCODE -ne 0){throw 'Native SDK build failed'}
Get-FileHash -LiteralPath $nativeOutput -Algorithm SHA256
