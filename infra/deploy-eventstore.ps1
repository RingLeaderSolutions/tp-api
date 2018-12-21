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

# Deploy eventstore
kubectl create -f eventstore/deployment.yaml
kubectl create -f eventstore/service.yaml

# Expose publicly for testing purposes
kubectl create configmap nginx-es-frontend-conf --from-file=eventstore/frontend.conf
kubectl create -f eventstore/frontend-deployment.yaml
kubectl create -f eventstore/frontend-service.yaml