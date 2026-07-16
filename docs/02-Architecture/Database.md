# Database Design

## Overview
The database layer stores the domain model that powers profiles, bookings, trust signals, payments, messaging, and operational workflows.

## Core Entity Groups
### Identity and Access
- users
- roles and permissions
- authentication records
- profile verification state

### Marketplace Entities
- services and categories
- profiles and portfolios
- availability schedules
- service areas and location metadata

### Booking and Operations
- bookings
- quotes and offers
- work status history
- cancellations and reschedules

### Trust and Reputation
- reviews
- ratings
- verification requests
- dispute records

### AI and Automation Support
- search indexes and metadata
- recommendation input/output snapshots
- queue and job state records
- notification history

## Relationship Patterns
- one user can own one or more profiles depending on role
- a booking links a client, a professional, and a service record
- reviews are tied to completed bookings and trusted user identities
- verification artifacts belong to a specific user or business profile

## Design Principles
- use relational tables for transactional integrity
- preserve immutable history for auditability
- store derived AI signals separately from canonical business data
- support soft deletion or state change history where appropriate

## Migration Strategy
- apply incremental schema migrations
- version schemas for large changes
- test migrations against representative data volumes
- preserve referential integrity across onboarding and booking changes
