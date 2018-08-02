# This script runs in CI and bundles client-side assets

# This script requires two ENV variables to be set in the VSTS Build:
#  SEARCH_API_URL, and NODE_ENV

Write-Host "Bundling client side assets using:"
Write-Host "SEARCH_API_URL: $env:SEARCH_API_URL"
Write-Host "NODE_ENV: $env:NODE_ENV"

Set-Location web
npm install
npm run build
