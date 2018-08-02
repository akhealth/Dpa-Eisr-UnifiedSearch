# This script runs the test suite and computes code coverage for both `web-test` and `api-test` projects.
#    It also publishes results to VSTS and generates an HTML coverage report.

# In VSTS, follow this script with build steps to:
#  1. Publish test results: They are in `VSTest` format. Result files are in `test-output/api-test-results.trx` and `test-output/web-test-results.trx`
#     Check "Merge test results"
#  2. Publish coverage results: Coverage results are in `test-output/cobertura.xml`. Report results are in `test-output/coverage-report`

# Install some tools
# Need to do this from _one_ of the projects, which is sorta weird
Set-Location api-tests
Write-Host "* Installing code coverage tool OpenCover"
dotnet add package opencover --version 4.6.519 --package-directory ../tools
Write-Host "* Installing OpenCoverToCoberturaConverter"
dotnet add package OpenCoverToCoberturaConverter --version 0.2.6 --package-directory ../tools
# Generates HTML coverage reports
Write-Host "* Installing ReportGenerator"
dotnet add package ReportGenerator --version 3.1.2 --package-directory ../tools
Set-Location ..

# Generate test-output directory structure
Write-Host "* Creating output directories"
mkdir test-output
mkdir test-output/coverage-report

# Load ENV variables for tests
Invoke-Expression -Command "build/test.env.ps1"

# Run the API Tests. OpenCover runs `dotnet test`
# a TRX (VSTest results xml) file for VSTS; and also a code coverage report are generated.
Write-Host "* Running api tests and computing coverage"
$arglist = "-target:""C:\Program Files\dotnet\dotnet.exe"" " `
  + "-targetargs:""test api-tests/api-tests.csproj --logger:trx;LogFileName=api-test-results.trx --results-directory:../test-output"" " `
  + "-register:user -output:test-output/opencover.xml -oldStyle -filter:""+[API]SearchApi.* -[API]SearchApi.SearchApi -[API]SearchApi.Startup -[API]SearchApi.Models.*"""
Start-Process "./tools/opencover/4.6.519/tools/OpenCover.Console.exe" -Wait -NoNewWindow -ArgumentList $arglist

# Run the Web Tests. Same as above.
# VSTS only allows us to publish _one_ coverage file, so we need to `-mergeoutput` this with our previous results.
Write-Host "* Running web tests and computing coverage"
$arglist = "-target:""C:\Program Files\dotnet\dotnet.exe"" " `
  + "-targetargs:""test web-tests/web-tests.csproj --logger:trx;LogFileName=web-test-results.trx --results-directory:../test-output"" " `
  + "-register:user -output:test-output/opencover.xml -oldStyle -filter:""+[Web]SearchWeb.Controllers*"" -mergeoutput"
Start-Process "./tools/opencover/4.6.519/tools/OpenCover.Console.exe" -Wait -NoNewWindow -ArgumentList $arglist

Write-Host "* Converting coverage report to VSTS format"
Start-Process "./tools/opencovertocoberturaconverter/0.2.6/tools/OpenCoverToCoberturaConverter.exe" -Wait -NoNewWindow `
-ArgumentList "-input:test-output/opencover.xml -output:test-output/cobertura.xml -sources:."

# Generate HTML report
Write-Host "* Generating HTML coverage report"
Start-Process "./tools/reportgenerator/3.1.2/tools/ReportGenerator.exe" -Wait -NoNewWindow `
-ArgumentList "-reports:test-output/opencover.xml -targetdir:test-output/coverage-report"