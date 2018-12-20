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

## Deployment

Each of the microservices that makes up the Platform is deployed in a container to a Kubernetes cluster hosted and managed in Azure. We've taken this [tutorial](https://cloudblogs.microsoft.com/opensource/2018/11/27/tutorial-azure-devops-setup-cicd-pipeline-kubernetes-docker-helm/) as inspiration.

Breifly:
- CI (an Azure DevOps Build Pipeline) creates a Docker image and a Helm package and pushes both to an Azure Container Registry
- CD (an Azure DevOps Release Pipeline) performs a Helm upgrade against a specified Azure Kubernetes cluster

This is subject to change depending on how we'd like to structure container registries.  It may be that we have a seperate container registery per environment or per client in production, and therefore the push to the registry (from an artifact store) is part of the CD pipeline.

To create a Build or Release pipeline follow the instructions in [Azure DevOps Pipeline](Azure-DevOps-Pipelines.md).

## Versioning strategy

We intend to use [SemVer 2.0](https://semver.org/spec/v2.0.0.html) to consistently version our microservices and their corresponding containers.

.Net Core allows the setting of a `VersionPrefix` e.g. 1.2.3 and a `VersionSuffix` e.g. alpha to form a `Version` e.g. 1.2.3-alpha (more details [here](https://andrewlock.net/version-vs-versionsuffix-vs-packageversion-what-do-they-all-mean/)).

The Azure DevOps Build Pipeline currently creates the `VersionPrefix` (major.minor.patch).  The major.minor portion is controlled by a repository file (`version.txt`, so we can change it on branches etc) and the patch is controlled by an automatic counter in Azure DevOps.

This is achieved with two jobs in the pipeline:

- one to read the `version.txt` file, exporting a variable `majorMinor` in future jobs
- the next job uses the `majorMinor` variable within a counter expression (so if the major minor version changes, the counter starts from zero again). This job then has a step to set the build number which is used as a parameter in various dotnet, docker and helm steps.

## Kubernetes

The Kubernetes cluster can be managed from the Azure Shell [shell.azure.com](https://shell.azure.com) using commands such as those found on the [Kubernetes cheatsheet](https://kubernetes.io/docs/reference/kubectl/cheatsheet/).

Kubernetes also has a dashboard.  To connect to the dashboard, locate the AKS using the Azure portal [portal.azure.com](https://portal.azure.com) and simply click `View Kubernetes dashboard` and follow the connection steps:

1. Open Azure CLI version 2.0.27 or later. This will not work in cloud shell and must be running on your local machine. [How to install the Azure CLI](https://docs.microsoft.com/en-gb/cli/azure/install-azure-cli?view=azure-cli-latest).
1. If you do not already have kubectl installed in your CLI, run the following command:

   `az aks install-cli`
1. Get the credentials for your cluster by running the following command:

   `az aks get-credentials --resource-group <resource-group-name> --name <azure-kubernetes-service-name>`
1. Open the Kubernetes dashboard by running the following command:

   `az aks browse --resource-group <resource-group-name> --name <azure-kubernetes-service-name>`
