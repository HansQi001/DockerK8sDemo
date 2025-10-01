# Docker & Kubernetes Deployment Demo  
- ## Automation from GitHub to ACR then AKS  
1. **Create a Kubernetes cluster named "mini-API-demo" under resource group "k8s-group"**
2. **Create a Service Principal with secret:**  
```Powershell
az ad sp create-for-rbac --name "gh-actions" /
--role AcrPush /
--scopes $(az acr show --name DockerK8sDemoAPIApp20250919115743 --query id -o tsv)
```

3. **Get the kubelet identity of your AKS cluster**  
```Powershell
az aks show --resource-group k8s-group /
--name mini-API-demo /
--query identityProfile.kubeletidentity.clientId -o tsv
```

3. **Assign AcrPull role to that identity on your ACR**  
```Powershell
az role assignment create /
--assignee <kubelet-id> /
--role AcrPull /
--scope $(az acr show --name DockerK8sDemoAPIApp20250919115743 --query id -o tsv) /
--subscription <subscription-id>
```

4. **Create an Azure Role-Based Access Control (RBAC) assignment**  
```Powershell
az role assignment create /
--assignee $(az ad sp list --display-name gh-actions --query "[0].id" -o tsv) /
--role "Azure Kubernetes Service Cluster User Role" /
--scope $(az aks show --name mini-API-demo --resource-group k8s-group --query id -o tsv)
```

5. **Merge your AKS cluster credentials into ~/.kube/config**  
```Powershell
az aks get-credentials --resource-group k8s-group --name mini-API-demo --overwrite-existing
```

- ## From Visual Studio 2022 → Azure Container Registry (ACR) → Azure Kubernetes Service (AKS)

### 📌 Overview
This project demonstrates a complete containerized deployment workflow using **Docker** and **Kubernetes**, starting from local development in **Visual Studio 2022** and ending with a live application running on **Azure Kubernetes Service (AKS)**.

The deployment pipeline covers:
- Building and containerizing the application with **Docker**
- Publishing the image to **Azure Container Registry (ACR)**
- Deploying to **AKS** using Kubernetes manifests
- Exposing the application via a public endpoint for demo purposes
- The deployment yaml file is in the root folder of the project
- Restart and deploy AKS pod after new code publishment to ACR  
  kubectl rollout restart deployment dockerk8sdemoapiapp -n default-1758294228254
