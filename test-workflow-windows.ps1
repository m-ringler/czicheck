# Simulate the Windows build-native job for czicheck-sharp workflow

$ErrorActionPreference = "Stop"

Write-Host "=== CziCheckSharp Workflow Simulation (Windows) ===" -ForegroundColor Cyan

# Step 0: Initialize Visual Studio Developer Environment
Write-Host "`n[0/6] Initializing Visual Studio Developer Environment..." -ForegroundColor Yellow
Import-Module "C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\Tools\Microsoft.VisualStudio.DevShell.dll"
Enter-VsDevShell 8a781a1b -SkipAutomaticLocation -DevCmdArguments "-arch=x64 -host_arch=x64"
Write-Host "Visual Studio Developer Environment loaded" -ForegroundColor Green

# Step 1: Prep vcpkg (simplified - assumes you have vcpkg)
Write-Host "`n[1/6] Checking vcpkg..." -ForegroundColor Yellow
$VCPKG_DIR = "V:\Repos\github\m-ringler\czicheck\external\vcpkg"
if (-not (Test-Path "$VCPKG_DIR\vcpkg.exe")) {
    Write-Host "ERROR: vcpkg not found at $VCPKG_DIR" -ForegroundColor Red
    Write-Host "Please bootstrap vcpkg first" -ForegroundColor Red
    exit 1
}
Write-Host "vcpkg found at: $VCPKG_DIR" -ForegroundColor Green

# Step 2: Configure CMake
Write-Host "`n[2/6] Configuring CMake with static triplet..." -ForegroundColor Yellow
cmake -B "build-test" -DCMAKE_BUILD_TYPE=Release `
      -DCMAKE_TOOLCHAIN_FILE="$VCPKG_DIR/scripts/buildsystems/vcpkg.cmake" `
      -DVCPKG_TARGET_TRIPLET=x64-windows-static

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: CMake configuration failed" -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "CMake configuration successful" -ForegroundColor Green

# Step 3: Build
Write-Host "`n[3/6] Building native library..." -ForegroundColor Yellow
cmake --build build-test --config Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "Build successful" -ForegroundColor Green

# Step 4: Package native libraries
Write-Host "`n[4/6] Packaging native libraries..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path "native-artifacts" | Out-Null

if (-not (Test-Path "build-test/CZICheck/capi/Release/libczicheckc.dll")) {
    Write-Host "ERROR: libczicheckc.dll not found" -ForegroundColor Red
    exit 1
}

Copy-Item "build-test/CZICheck/capi/Release/libczicheckc.dll" "native-artifacts/"
Write-Host "Copied: libczicheckc.dll" -ForegroundColor Green

if (Test-Path "build-test/CZICheck/capi/Release/libczicheckc.pdb") {
    Copy-Item "build-test/CZICheck/capi/Release/libczicheckc.pdb" "native-artifacts/"
    Write-Host "Copied: libczicheckc.pdb" -ForegroundColor Green
}

Write-Host "`nNative artifacts:" -ForegroundColor Cyan
Get-ChildItem "native-artifacts/" | Format-Table Name, Length, LastWriteTime

# Step 5: Run tests
Write-Host "`n[5/6] Running tests..." -ForegroundColor Yellow
dotnet test CziCheckSharp.Tests/CziCheckSharp.Tests.csproj `
    --configuration Release `
    -p:NativeLibraryPath="$PWD/native-artifacts"

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Tests failed" -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "Tests passed" -ForegroundColor Green

# Step 6: Create NuGet package
Write-Host "`n[6/6] Creating NuGet package..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path "nuget-output" | Out-Null

dotnet pack CziCheckSharp/CziCheckSharp.csproj `
    --configuration Release `
    --output ./nuget-output `
    -p:Winx64LibLocation="$PWD/native-artifacts/*" `
    -p:Linuxx64LibLocation=""

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: NuGet pack failed" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nGenerated NuGet packages:" -ForegroundColor Cyan
Get-ChildItem "nuget-output/" | Format-Table Name, Length, LastWriteTime

# Verify package contents
Write-Host "`n=== Package Contents Verification ===" -ForegroundColor Cyan
$nupkg = Get-ChildItem "nuget-output/*.nupkg" | Select-Object -First 1
if ($nupkg) {
    Write-Host "Inspecting: $($nupkg.Name)" -ForegroundColor Yellow
    
    # Extract and check for native libraries
    $tempDir = "temp-nupkg-extract"
    if (Test-Path $tempDir) {
        Remove-Item -Recurse -Force $tempDir
    }
    
    Expand-Archive -Path $nupkg.FullName -DestinationPath $tempDir
    
    Write-Host "`nChecking for native libraries in package..." -ForegroundColor Yellow
    $winLibs = Get-ChildItem "$tempDir/runtimes/win-x64/native/*" -ErrorAction SilentlyContinue
    if ($winLibs) {
        Write-Host "✓ Found Windows native libraries:" -ForegroundColor Green
        $winLibs | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor Green }
    } else {
        Write-Host "✗ No Windows native libraries found in package" -ForegroundColor Red
    }
    
    Remove-Item -Recurse -Force $tempDir
}

Write-Host "`n=== Workflow Simulation Complete! ===" -ForegroundColor Cyan
Write-Host "All steps completed successfully" -ForegroundColor Green
