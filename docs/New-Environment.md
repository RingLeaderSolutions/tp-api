# How to setup a new environment

This document will eventually list the steps to creating a new Theta platform environment (either internal or a production environment for a client).

## Prerequistes

1. Install the Azure CLI version 2.0.27 or later. [How to install the Azure CLI](https://docs.microsoft.com/en-gb/cli/azure/install-azure-cli?view=azure-cli-latest).
1. Install [helm](https://github.com/helm/helm/releases) and add it to your path.  Make sure the version of helm is the same one used in the Azure DevOps Release pipelines. 

## Infrastructure Steps

TODO: Terraform / Azure Resource Manager template documentation

In `theta-platform\infra\`:

1. Add the new enviornment name to `environment-names\common.ps1`
1. Run `create-key-vault.ps1` to create a Key Vault
1. Run `create-k8s.ps1` to create the Azure Kubernetes Service and Azure Container Registry
1. Run `configure-k8s-managed-service-identity.ps1` to allow the k8s pods to get and list secrets in the Key Vault via a Managed Service Identity

## Application Steps

TODO: First deployment

## Client Data steps

TODO: Client Data
