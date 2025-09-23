# Docker & Kubernetes Deployment Demo  
**From Visual Studio 2022 → Azure Container Registry (ACR) → Azure Kubernetes Service (AKS)**

## 📌 Overview
This project demonstrates a complete containerized deployment workflow using **Docker** and **Kubernetes**, starting from local development in **Visual Studio 2022** and ending with a live application running on **Azure Kubernetes Service (AKS)**.

The deployment pipeline covers:
- Building and containerizing the application with **Docker**
- Publishing the image to **Azure Container Registry (ACR)**
- Deploying to **AKS** using Kubernetes manifests
- Exposing the application via a public endpoint for demo purposes
- The deployment yaml file is in the root folder of the project
- Restart and deploy AKS pod after new code publishment to ACR  
  kubectl rollout restart deployment dockerk8sdemoapiapp -n default-1758294228254
