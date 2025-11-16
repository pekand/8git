########################################

function Copy-WithFullPath {
    param (
        [string]$Source,
        [string]$Destination
    )

    # Ensure the destination directory exists
    $destinationDir = Split-Path -Path $Destination
    if (!(Test-Path -Path $destinationDir)) {
        New-Item -ItemType Directory -Path $destinationDir -Force | Out-Null
    }

    # Copy the file and overwrite if necessary
    Copy-Item -Path $Source -Destination $Destination -Force
}

########################################

$CERT_STRONG_NAME = $env:CERT_STRONG_NAME
Write-Host "CERT_STRONG_NAME=>$CERT_STRONG_NAME<"

$snPath = 'sn.exe'

########################################

$signtoolPath = 'signtool.exe'
$innoInstallPath = 'iscc'

$CERT_CODE = $env:CERT_CODE
Write-Host "CERT_CODE=>$CERT_CODE<"

$CERT_PWD = $env:CERT_PWD
Write-Host "CERT_PWD=>$CERT_PWD<"

$CERT_PUBLISHER = $env:CERT_PUBLISHER
Write-Host "CERT_PUBLISHER=>$CERT_PUBLISHER<"

$tag = git describe --tags --abbrev=0
Write-Output "TAG=>$tag<"

$paths = @(
    "8Git\bin\x64\Release\net9.0-windows\8Git.dll"
    "8Git\bin\x64\Release\net9.0-windows\8Git.exe"
)

foreach ($path in $paths) {
    Write-Output "################## Processing: $path"
    if (Test-Path $path) {
        Write-Output "################## STRONG NAME"
        #& $snPath -R "$path" $CERT_STRONG_NAME
        Write-Output "################## STRONG NAME VERIFY"
        #& $snPath -v "$path"
        Write-Output "################## SUBSCRIBE"
        & $signtoolPath sign /fd SHA256 /f "$CERT_CODE" /p $CERT_PWD /tr http://timestamp.digicert.com /td sha256 /v "$path"
        Write-Output "################## SUBSCRIBE VERIFY"
		& $signtoolPath verify /pa /v "$path"
    } else {
        Write-Output "Path does not exist: $path"
    }
}

########################################
Write-Output "################## BUILD PACKAGE"
$version = Get-Content -Path ".\version.txt" -TotalCount 1
Write-Output "################## VERSION $version" 
& $innoInstallPath /q 8Git.iss /DMyAppVersion=$version /DMyAppPublisher="$CERT_PUBLISHER" /DMyAppURL="https://pekand.com/page/MtExpSolver"

########################################


$exeFiles = Get-ChildItem -Path Output -Filter *.exe -Recurse -File
foreach ($file in $exeFiles) {        
    $TARGET=$file.FullName
    Write-Output "################## SUBSCRIBE PACKAGE $TARGET"
    & $signtoolPath sign /fd SHA256 /f "$CERT_CODE" /p $CERT_PWD /tr http://timestamp.digicert.com /td sha256 /v "$TARGET"
    Write-Output "################## SUBSCRIBE VERIFY"
    & $signtoolPath verify /pa /v "$TARGET"
    Write-Output "################## CREATE PACKAGE HASH"
    $hash = Get-FileHash -Path $TARGET -Algorithm SHA256
    $hash.Hash | Out-File -FilePath "$TARGET.SHA256"
}

Read-Host "Press Enter to continue"
