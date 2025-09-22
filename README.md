# Docker & Kubernetes Deployment Demo  
**From Visual Studio 2022 → Azure Container Registry (ACR) → Azure Kubernetes Service (AKS)**

## 📌 Overview
This project demonstrates a complete containerized deployment workflow using **Docker** and **Kubernetes**, starting from local development in **Visual Studio 2022** and ending with a live application running on **Azure Kubernetes Service (AKS)**.

The deployment pipeline covers:
- Building and containerizing the application with **Docker**
- Publishing the image to **Azure Container Registry (ACR)**
- Deploying to **AKS** using Kubernetes manifests
- Exposing the application via a public endpoint for demo purposes  
  http://hans-dockerk8s-demo.australiaeast.cloudapp.azure.com/api/products/ (via Postman)  
  http://hans-dockerk8s-demo.australiaeast.cloudapp.azure.com/api/products/79c4e77d-fff4-4345-ac34-3a46137b1e92
- Restart AKS pod  
  kubectl rollout restart deployment dockerk8sdemoapiapp -n default-1758294228254
