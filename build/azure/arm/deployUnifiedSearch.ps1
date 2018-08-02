# Deploy UnifiedSearch AppService Infrastructure

# This should be >= 5.6, this is still changing pretty fast.
Import-Module AzureRm
Get-Module -Name AzureRm

# Login
if ([string]::IsNullOrEmpty($(Get-AzureRmContext).Account)) {
  Login-AzureRmAccount -EnvironmentName AzureUSGovernment
}

# Variables
$baseName = "Test-DpaEisrUnifiedSearch"
$location = "USGov Arizona"

# Create Resource Group container
New-AzureRmResourceGroup -Name $baseName -Location $location

# Deployment dry-run
Test-AzureRmResourceGroupDeployment -ResourceGroupName $baseName -TemplateFile unified-search-template.json

# Create new deployment
New-AzureRmResourceGroupDeployment -ResourceGroupName $baseName -TemplateFile unified-search-template.json

# Destroy infra
# Remove-AzureRmResourceGroup -Name $baseName