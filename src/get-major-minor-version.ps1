$line = Get-Content -Path "version.txt" -TotalCount 1
echo "##vso[task.setvariable variable=majorMinor;isOutput=true]$line"