# API - Authentication

## Purpose
Authentication ensures that clients, tradespeople, and administrators access only the parts of the platform appropriate to their identity and role.

## Core Endpoints
- sign in and sign out
- refresh session or token state
- register new account
- request password reset or verification resend

## Security Model
- secure password handling and identity verification
- role-based authorization for client, tradesperson, and admin access
- server-side enforcement of permissions for protected operations

## Business Rules
- only verified users may access premium or sensitive workflows
- admin-only actions require elevated authorization
- authentication events should be logged for auditability

## Notes for Implementation
Use the existing identity infrastructure in the backend and enforce authorization consistently in controllers and services.
