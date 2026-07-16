# AI - Matching Engine

## Purpose
The matching engine helps connect clients with the most suitable professionals based on service fit, availability, locality, verification status, and historical quality signals.

## Matching Inputs
- requested service type
- location and distance
- provider availability
- pricing fit
- verification level
- reputation and response rate

## Ranking Strategy
The system should prioritize candidates that are:
- relevant to the service request
- available within the requested window
- geographically appropriate
- trusted and verified
- likely to deliver a strong client experience

## Guardrails
- protect user privacy
- do not use protected characteristics
- keep recommendations explainable to users
- allow staff review for edge cases or policy-sensitive decisions

## Evaluation Metrics
- match-to-booking conversion
- response rate
- cancellation rate
- client satisfaction
- false-positive and false-negative quality
