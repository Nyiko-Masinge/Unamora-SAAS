# Deployment - Azure

## Purpose
This document defines the deployment direction for Unamora on Azure, covering hosting, observability, security, and operational resilience.

## Recommended Services
- Azure App Service or Container Apps for the API and background workers
- Azure Static Web Apps or CDN for the frontend
- Azure SQL Database for relational data
- Azure Blob Storage for documents and media
- Azure Service Bus for async messaging
- Azure Cache for Redis for performance and rate limiting
- Azure AI Search and AI services for intelligent features
- Azure Key Vault for secrets

## Operational Goals
- deploy independently for frontend, API, and worker services
- support staging and production environments
- maintain monitoring and alerting for platform health
- ensure backup, restore, and disaster recovery readiness

## Security and Reliability
- managed identity and secure secret access
- private networking where feasible
- health probes and rollback readiness
- centralized logging and application monitoring
