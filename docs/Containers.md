# Containers

## Infrastructure as Code

This is still a todo.  At the moment the following resources have been created manually in Azure:

- A resource group
- A [Container Registry (ACR)](https://azure.microsoft.com/en-us/services/container-registry/)
- A [Kubernetes Service (AKS)](https://azure.microsoft.com/en-us/services/kubernetes-service/)
  - A Service Principal - required for the Kubernetes Service.  Note that this requires elevated permissions in Azure Active Directory.

Note in development environments to save cost:

- The ACR is created with the Basic SKU, so will require periodic cleaning of unused containers
- The AKS is created using 1 `Standard B2s` node (~£26.12/month), rather than the default 3 `DS2 v2` nodes (3 * ~£74.00/month).

## Resource naming conventions

Having a consitent naming convention for all resources makes automation, building tooling and management of resources easier.

This is still to be decided, but lots of considerations [here](https://docs.microsoft.com/en-us/azure/architecture/best-practices/naming-conventions).

## Deployment

Each of the microservices that makes up the Platform is deployed in a container to a Kubernetes cluster hosted and managed in Azure. We've taken this [tutorial](https://cloudblogs.microsoft.com/opensource/2018/11/27/tutorial-azure-devops-setup-cicd-pipeline-kubernetes-docker-helm/) as inspiration.

Breifly:
- CI (an Azure DevOps Build Pipeline) creates a Docker image and a Helm package and pushes both to an Azure Container Registry
- CD (an Azure DevOps Release Pipeline) performs a Helm upgrade against a specified Azure Kubernetes cluster

To create a Build or Release pipeline follow the instructions in [Azure DevOps Pipeline](Azure-DevOps-Pipelines.md).

## Kubernetes

The Kubernetes cluster can be managed from the Azure Shell [shell.azure.com](https://shell.azure.com) using commands such as those found on the [Kubernetes cheatsheet](https://kubernetes.io/docs/reference/kubectl/cheatsheet/).

Kubernetes also has a dashboard.  To connect to the dashboard, locate the AKS using the Azure portal [potal.azure.com](https://portal.azure.com) and simply click `View Kubernetes dashboard` and follow the connection steps:

1. Open Azure CLI version 2.0.27 or later. This will not work in cloud shell and must be running on your local machine. [How to install the Azure CLI](https://docs.microsoft.com/en-gb/cli/azure/install-azure-cli?view=azure-cli-latest).
1. If you do not already have kubectl installed in your CLI, run the following command:

   `az aks install-cli`
1. Get the credentials for your cluster by running the following command:

   `az aks get-credentials --resource-group <resource-group-name> --name <azure-kubernetes-service-name>`
1. Open the Kubernetes dashboard by running the following command:

   `az aks browse --resource-group <resource-group-name> --name <azure-kubernetes-service-name>`
