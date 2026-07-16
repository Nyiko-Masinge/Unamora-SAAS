# API - Booking

## Purpose
The booking API coordinates the lifecycle of requests, confirmations, reschedules, cancellations, and completion tracking between clients and tradespeople.

## Main Workflow
1. Client creates a booking request.
2. Professional reviews availability and accepts or declines.
3. Booking enters an active state and communication begins.
4. Completion, review, and payment follow-up occur.

## Key States
- pending
- accepted
- declined
- in progress
- completed
- cancelled
- disputed

## Validation Rules
- only eligible professionals should be able to accept certain requests
- bookings must match service availability and permitted pricing rules
- status transitions should be auditable and restricted by role

## Integration Considerations
- notifications should be triggered on transitions
- AI may assist with suggestions but should not override business rules
- disputes should create reviewable cases with full history
