param (
  [Parameter(Mandatory=$true)][string]$subscriptionId,
  [Parameter(Mandatory=$true)][string]$environmentName,
  [string]$region = "westeurope"
)

. 'common/environment-names.ps1'
$environment = parse-environment-name $environmentName

# Ensure we're using the correct subscription
az account set -s $subscriptionId

$resourceGroup = "theta-$environment"

# Set the default context to this environment's cluster
az aks get-credentials --resource-group $resourceGroup --name "$resourceGroup-cluster"
kubectl config use-context "$resourceGroup-cluster"

# (1) Deploy eventstore
kubectl --namespace=$environment create -f eventstore/deployment.yaml
kubectl --namespace=$environment create -f eventstore/service.yaml

# (2) Expose publicly for testing purposes

# Rewrite the nginx config as this references the namespace, which is different per environment
$nginxConfig = Get-Content eventstore/frontend.conf.template
$nginxConfig = $nginxConfig -Replace "{EnvironmentNamespace}", $environment

# Write the nginx config out to a frontend.conf file
Out-File "eventstore/frontend.conf" -InputObject $nginxConfig -Encoding ASCII -NoClobber

kubectl --namespace=$environment create configmap nginx-event-store-frontend-conf --from-file=eventstore/frontend.conf

# Remove the temporary nginx config
Remove-Item "eventstore/frontend.conf"

kubectl --namespace=$environment create -f eventstore/frontend-deployment.yaml
kubectl --namespace=$environment create -f eventstore/frontend-service.yaml