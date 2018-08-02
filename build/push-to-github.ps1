# This script pushes code to an open-source repository on Github
# 1. Pass these _hidden/encrypted_ parameters in from VSTS build step:
#   the "Arguments" look like `-user "$(gh-user)" -token "$(gh-access-token)" -repo "$(gh-repo)"`
#   you can generate an access token here: https://github.com/settings/tokens
# 2. Uncheck "Fail on Standard Error" for the VSTS build step; git will write to STDERR

Param(
  [string]$user,
  [string]$token,
  [string]$repo
)

# Force push to github using the access-token
$command = "git push https://${user}:${token}@github.com/$repo HEAD:master --force"
Invoke-Expression($command)