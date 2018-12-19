# How to setup a new environment

This document will eventually list the steps to creating a new Theta platform environment (either internal or a production environment for a client).

## Infrastructure Steps

TODO: Terraform / Azure Resource Manager template documentation

1. Add the new enviornment name to `theta-platform\infra\environment-names\common.ps1`
2. Run `theta-platform\infra\create-key-vault.ps1` to create a Key Vault
3. Run `configure-k8s-managed-service-identity.ps1` to allow the k8s pods to get and list secrets in the Key Vault via a Managed Service Identity

## Application Steps

TODO: First deployment

## Client Data steps

TODO: Client Data
