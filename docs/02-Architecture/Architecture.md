# Architecture Overview

## Summary
Unamora is implemented as a modular, cloud-ready SaaS platform with a React frontend, an ASP.NET Core backend, and supporting services for AI, background processing, storage, and observability.

## Core Architectural Principles
- clean separation between domain logic, application services, infrastructure, and presentation
- deterministic workflows for sensitive operations such as payments, identity, and enforcement
- AI used as an assistive layer, not as the sole authority for critical decisions
- tenant-aware, auditable, and secure-by-design components

## System Components
### Frontend
- React + Vite single-page application
- role-based experience for clients, tradespeople, and administrators
- route-based navigation and reusable UI components

### Backend API
- ASP.NET Core Web API
- modular application layers for controllers, use cases, domain entities, and infrastructure services
- authentication and authorization integrated at the API boundary

### Data and Persistence
- relational database for transactional data
- background worker processes for automation and notification delivery
- blob storage for verification documents and media assets

### AI and Intelligence Services
- matching engine
- OCR and document intelligence
- recommendation ranking
- chatbot and support assistance
- fraud and risk evaluation

## Communication Patterns
- RESTful API endpoints for core business operations
- domain event-based communication for asynchronous workflows
- queue-based processing for reminders, notifications, and heavy operations

## Security and Reliability Considerations
- identity and access control enforced server-side
- secrets managed through secure configuration mechanisms
- audit logs for important actions
- rate-limiting and retry policies for resilience

## Target Deployment Shape
- frontend hosted as a modern web application
- API and worker services deployed independently
- Azure-based hosting for storage, messaging, monitoring, and AI services
