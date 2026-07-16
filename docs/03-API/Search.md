# API - Search

## Purpose
The search API allows users to discover suitable professionals and services using natural language or structured filters.

## Inputs
- service category
- location and radius
- availability window
- price band
- verification level
- user language preferences

## Search Strategy
- lexical matching for exact service and location terms
- semantic ranking for intent and relevance
- hard filters for eligibility, visibility, and trust constraints

## Output Expectations
- ranked result cards with explainable reasons
- profile visibility rules enforced server-side
- fallback to deterministic results if AI ranking is unavailable

## Business Rules
- only approved and visible profiles should be returned
- sensitive or non-public data must not be exposed in search results
- search quality should be measured by relevance and conversion metrics
