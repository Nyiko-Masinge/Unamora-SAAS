# Phases 11–15 Architecture Blueprint

## Foundation

Extend the existing React/Vite frontend and ASP.NET Core clean-architecture backend. AI provides suggestions, extraction, and risk signals only: deterministic services and authorised staff remain the source of truth for identity, money movement, enforcement, and policy exceptions. Every feature is tenant-aware, privacy-scoped, authorised, auditable, versioned, rate-limited, observable, and has a non-AI fallback.

## Phase 11 — AI

### 26. AI architecture

The AI gateway is the only application boundary permitted to call model providers. It performs authentication, data minimisation/redaction, prompt/model selection, schema validation, safety checks, quotas, retries, caching, cost accounting, and immutable audit events. Specialised services behind it are retrieval, embedding, recommendations, document intelligence, trust/risk, and AI evaluation.

Flow: application use case authorises the request; gateway creates minimal approved context and correlation ID; specialised service returns structured output with confidence, sources, version, cost, and expiry; application applies deterministic rules; UI shows an explanation and staff can review when required.

Maintain an AI feature registry containing owner, purpose, permitted inputs, output schema, risk class, retention, model/prompt/index versions, evaluation threshold, and escalation path. Version prompts, indexes, policies, and rankings; test prompt injection, data leakage, unsafe output, bias, and fraud bypasses before release. Log hashes/references rather than raw sensitive data where possible.

### 27. Smart Search

Natural-language search converts a request such as “licensed emergency plumber near Sandton tonight under R1,500” into service, urgency, location/radius, availability, price, language, verification, and quality filters, visibly showing the inferred filters.

Use hybrid retrieval: lexical search for exact terms, service names, locations, and licence numbers; semantic/vector search for meaning; then fuse candidates, enforce hard eligibility and visibility constraints, and rank. Search only public/approved profiles, service packages, portfolios, public jobs, categories, FAQs, and policies. Never embed chats, payments, raw IDs, or unapproved documents. Index metadata includes tenant, status, category, geospatial cell, language, verification, price band, availability, freshness, and permissions. Re-index idempotently from domain events, with a dead-letter queue and a database-backed keyword fallback.

Rank explanations should state user-visible reasons (“serves your area”, “available today”). Measure zero-result/reformulation rates, relevance, click-through, contact/booking conversion, latency, and multilingual performance. Paid promotion, if introduced, is clearly labelled and cannot bypass suitability or trust constraints.

### 28. Recommendation engine

Recommend eligible tradespeople on search/home, relevant open jobs to eligible tradespeople, and complementary related services. Begin with explainable rules plus weighted scoring: service/job fit, distance, availability, verified status, quality, response reliability, budget fit, and relevance. Enforce privacy, filter, visibility, safety, and fairness constraints before ranking. Capture consented impressions, clicks, saves, contacts, bids, bookings, cancellations, completions, and satisfaction. Introduce learned ranking only after clean feedback data and formal offline/online evaluation. Give every card a short explanation and “not interested” feedback; do not infer or use protected characteristics.

### 29. OCR and document intelligence

Support certificates, licences, IDs, proof of address, business registrations, tax/compliance records, and insurance evidence. Upload to quarantined private storage using short-lived signed URLs; validate type/size, malware-scan, hash/deduplicate, classify, OCR/extract, and validate per-document fields such as expiry, issuer format, checksum/MRZ where lawful, name match, and registration format. Identify quality, tampering, duplicate/perceptual-hash, and account-consistency issues.

OCR produces proposed data only. Users may correct it; low-risk high-confidence auto-acceptance is allowed only by policy, and all uncertain/high-risk cases go to verification staff with image, extraction, confidence, and reasons. Encrypt storage, restrict access, log review actions, and purge on the retention schedule. No biometric matching or automated identity denial without a specific legal/privacy/human-review programme.

### 30. AI chatbot

The assistant supports FAQs, policies, profile guidance, booking/payment status explanations, booking changes within policy, dispute intake, and human escalation. Use retrieval-augmented generation over approved versioned help/policy content and cite in-product source titles/dates. Authenticate before account-specific queries; use a narrow tool allowlist and read-only booking/payment tools. Cancellation, refund, payout, or account changes go through deterministic workflows with explicit confirmation.

Untrusted user documents and links cannot override policy. Escalate on low confidence, safety/legal/financial topics, disputes, fraud, policy exceptions, payment failures, or repeated unresolved/angry users. Preserve transcript, sources, and a handover summary. Disclose AI use and offer human support. Track groundedness, policy accuracy, containment, escalation quality, CSAT, latency, tool errors, and unsafe output.

### 31. AI fraud detection

Use layered deterministic rules, anomaly scoring, and staff review; scores are case-management input, not irreversible automatic punishment. Detect fake reviews via booking eligibility, duplicate text/embeddings, bursts and reviewer-provider graph anomalies; duplicate accounts via permitted identifier/device/payment/address/timing links; fake documents via OCR/tampering/issuer/duplicate/expiry signals; location fraud via impossible travel and repeated mismatches; suspicious behaviour via velocity, payment/booking anomalies, spam, chargeback links, and enforcement evasion.

Responses are step-up verification, rate limits, temporary holds, moderation queues, and appealable enforcement. Retain feature provenance, score/rule version, reasons, reviewer decision, and appeal outcome. Tune thresholds for false-positive harm, especially new or low-connectivity providers.

## Phase 12 — Automation (Prompt 32)

Use Hangfire with durable SQL storage. API processes enqueue only after database commit through an outbox; dedicated workers execute. Restrict the dashboard by SSO, admin role, network controls, and audit logs. Configure idempotent handlers, correlation IDs, bounded exponential retry with jitter, dead-letter queues, concurrency controls, timezone-aware schedules, and external-provider idempotency keys.

Automate booking reminders; post-completion review reminders; registration/resend email verification with expiry/rate limits; subscription renewal, payment retry, grace-period entitlement changes; daily/monthly operational reports; and SLA/dispute/verification/fraud escalation. Each job has a business idempotency key and stops if the state no longer qualifies.

## Phase 13 — Notifications (Prompt 33)

A notification orchestrator consumes domain events, resolves consent/preferences, quiet hours, locale, priority, template version, and permitted channels, then emits channel-specific queue messages. Provide email for detailed transactional records, SMS for urgent alerts, push for timely app engagement, and durable in-app inbox/history. Abstract providers and use in-app or another authorised channel as fallback for non-critical messages.

Templates are named, versioned, localisable, previewable, approval-controlled, and validate their variables. Categorise transactional, security, operational, and marketing messages; enforce subscription/consent law by category. Store event, user, template, rendered schema, provider message ID, status, timing, and error. De-duplicate by event/channel/recipient/purpose, coalesce low priority notifications, rate-limit, process callbacks for delivery/bounces/complaints/opt-outs, and alert on queue age, failures, bounce spikes, and dead letters.

## Phase 14 — Frontend

### 34. React foundation

Use this feature-first structure:

```text
src/
  app/          bootstrap, providers, route composition
  layouts/      public, auth, client, tradesperson, admin shells
  routes/       definitions, guards, lazy/error boundaries
  features/     auth, search, booking, payments, chat, reviews, profiles, verification
  pages/        route-level composition by audience
  components/   shared UI primitives/composites
  services/     typed API, realtime, notifications
  state/        session, server cache, UI state
  hooks/ styles/ assets/ types/
```

Apply route-level lazy loading, typed API contracts, server-state caching/invalidation, and a small authentication context for session/sign-in/out/refresh. Prefer HTTP-only secure session cookies plus CSRF protection rather than persistent browser tokens. Public routes include marketing, search, profiles, FAQ, legal, and authentication. Shared authenticated routes include account, notifications, chat, support, saved items, and payment methods. Client, tradesperson, and admin routes enforce role, claims, tenant, status, and entitlement then show a dedicated forbidden/review state when unsuitable. API enforcement remains authoritative.

Use semantic tokens for theme, light/dark/system preference, WCAG 2.2 AA, keyboard/focus/reduced-motion support, and locale-aware date/currency/language formatting.

### 35–38. Product surfaces

Client dashboard: next booking/status timeline, messages needing response, payment/receipt status, saved providers, recent searches, review prompts, support, notification preferences, and useful empty states.

Tradesperson dashboard: matched jobs/leads, upcoming bookings, unread chat, profile/verification completeness, availability, earnings/payout status, reputation, and transparent subscription/lead costs; distinguish AI recommendations from confirmed work.

Admin dashboard: role-scoped overview, verification/document review, disputes, content/review moderation, fraud cases, users/support, notifications/job health, and reports. High-impact actions require reasons, optional dual control, and immutable case timelines.

Marketing: landing, discovery, pricing, FAQ, about, contact, terms, privacy. Pricing states currency, interval, taxes/fees, renewal/cancellation and inclusions. Legal content is versioned with acceptance records. Public pages are accessible, performant, SEO-ready, and spam-protected.

## Phase 15 — DevOps (Prompt 39)

Containerise frontend, API, and worker independently using multi-stage builds, non-root users, immutable tags, health/readiness checks, managed secret references, minimal bases, and vulnerability scans. Local compose is development-only; production uses managed database/cache/storage/messaging.

GitHub Actions: pull requests run formatting/lint, unit and contract tests, frontend build, secret/dependency scans, and image scans. Main runs integration tests, publishes immutable artefacts/SBOM, deploys staging, and smoke-tests. Production uses approval, immutable artefacts, migration plan, smoke checks, monitored rollout, and rollback. Use Azure OIDC/workload identity, protected branches, reviewed changes, and commit-pinned actions.

Azure topology: Static Web Apps/CDN for React; independent Container Apps or App Service deployments for API and Hangfire workers; Azure SQL for transactional/Hangfire data; Blob Storage for documents/media; Service Bus for durable events; Redis for cache/rate limits; AI Search for hybrid retrieval; Key Vault; and approved AI/document services behind application interfaces. Use managed identities, WAF/TLS, private networking where feasible, and data-residency-aware regions.

Instrument frontend, API, workers, integrations, AI, queues, and database with OpenTelemetry and Azure Monitor/Application Insights. Redact secrets/tokens/documents/payment data. Dashboard/alert user-impacting SLOs: API, booking/payment success, notification timeliness, job completion/queue age, search, AI cost/quality/safety, and security events. Use Azure SQL PITR/long-term retention, Blob versioning/soft delete/immutable retention where policy requires, configuration-as-code backups, and quarterly tested restore/DR exercises with documented RPO/RTO.

Delivery order: observability/identity/outbox/worker/notifications; frontend shell/auth/routes/dashboards; document verification; hybrid search; explainable recommendation rules; grounded support chat; then fraud scoring/moderation and controlled rollouts.
