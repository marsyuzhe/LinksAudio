param([Parameter(Mandatory=$true)][string]$ZigPath)
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
$archive=Join-Path $projectRoot 'docs/lame-3.100.tar.gz'
if((Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash -ne 'A76BE4B76C158A7C2C52B0912856BA1BC35EE62F768B16BBE89FA23DC27E3487'){throw 'LAME source archive checksum mismatch'}
$work=Join-Path $projectRoot '.tools/lame-source'
New-Item -ItemType Directory -Force $work | Out-Null
& tar.exe -xf $archive -C $work
if($LASTEXITCODE -ne 0){throw 'Source extraction failed'}
$source=Join-Path $work 'lame-3.100'
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'lame/config.h') -Destination (Join-Path $source 'config.h') -Force
$env:ZIG_GLOBAL_CACHE_DIR=Join-Path $projectRoot '.tools/zig-cache'
$env:ZIG_LOCAL_CACHE_DIR=Join-Path $projectRoot '.tools/zig-local'
$files=(Get-ChildItem -LiteralPath (Join-Path $source 'libmp3lame') -Filter '*.c' -File).FullName
$output=Join-Path $projectRoot 'src/LinksAudio.Codecs/native/libmp3lame.64.dll'
& $ZigPath cc -shared -O2 -s -fwrapv -Wno-everything -DHAVE_CONFIG_H -I $source -I (Join-Path $source 'include') -I (Join-Path $source 'libmp3lame') @files (Join-Path $source 'libmp3lame/vector/xmm_quantize_sub.c') '-Wl,--export-all-symbols' -o $output
if($LASTEXITCODE -ne 0){throw 'LAME build failed'}
Get-FileHash -LiteralPath $output -Algorithm SHA256
