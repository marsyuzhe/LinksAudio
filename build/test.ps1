param([string]$DotnetPath="")
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
if(!$DotnetPath){$bundled=Join-Path $projectRoot '.tools/dotnet/dotnet.exe';$DotnetPath=if(Test-Path -LiteralPath $bundled){$bundled}else{'dotnet'}}
& $DotnetPath run --project (Join-Path $projectRoot 'tests/LinksAudio.Tests') -c Release -- $projectRoot
if($LASTEXITCODE -ne 0){throw 'Audio tests failed'}
$app=Join-Path $projectRoot 'dist/LinksAudio-Portable-win-x64/LinksAudio.exe'
if(Test-Path -LiteralPath $app){foreach($name in @('result.txt','fallback.txt','playback.txt')){$oldResult=Join-Path $projectRoot ('tests/output/ui/'+$name);if(Test-Path -LiteralPath $oldResult){Remove-Item -LiteralPath $oldResult}};$testProcess=Start-Process -FilePath $app -ArgumentList @('--ui-test',('"'+$projectRoot+'"')) -WindowStyle Hidden -PassThru;$testProcess.WaitForExit();if($testProcess.ExitCode -ne 0){throw 'GUI process failed'};foreach($name in @('result.txt','fallback.txt','playback.txt')){$result=Get-Content (Join-Path $projectRoot ('tests/output/ui/'+$name)) -Raw;if(!$result.StartsWith('PASS:')){throw $result};Write-Host $result}}
