# LedgerPay

## Transactional Order & Payment Processing System

LedgerPay is a **backend-only** production-style system built with **ASP.NET Core**, **RabbitMQ**, **PostgreSQL**, and **Redis**, designed to demonstrate **reliable transaction processing, idempotency, and failure-tolerant backend architecture**.

This repository contains **no frontend code**. The service is intended to be consumed by external clients such as web applications, mobile apps, or other backend services.

The project models how **real-world payment systems** are designed, focusing on correctness, durability, and explicit architectural trade-offs rather than simple CRUD flows.

---

## Table of Contents

- Overview
- System Goals
- Architecture
- Core Flows
- Tech Stack
- Project Structure
- Idempotency & Reliability
- Failure Handling
- API Design
- Local Setup
- Running the System
- Environment Variables
- Trade-offs
- Future Improvements

---

## Overview

Payment systems must remain **correct under failure**.

LedgerPay is designed to:
- Separate **state changes** from **side effects**
- Process payments **asynchronously**
- Safely handle retries and duplicate requests
- Maintain a clear audit trail

The API remains fast and responsive, while all risky operations (payments, notifications, audits) are processed in background workers.

---

## System Goals

- Strong transactional consistency for order state
- Reliable message delivery using RabbitMQ
- Idempotent APIs and consumers
- Clear failure recovery paths
- Explicit auditability

### Non-Goals

- Real-time payment confirmation
- Distributed saga orchestration
- Microservices architecture
- Event streaming with Kafka

---

## Architecture Overview

```
+---------+       +---------------------+
| Client  | ----> | ASP.NET Core API    |
+---------+       +---------------------+
                           |
                           v
                  +---------------------+
                  | PostgreSQL (ACID)   |
                  +---------------------+
                           |
                           v
                  +---------------------+
                  | RabbitMQ (Durable)  |
                  +---------------------+
                           |
                           v
                  +---------------------+
                  | Payment Worker      |
                  +---------------------+
                           |
                           v
                  +---------------------+
                  | Payment Provider    |
                  +---------------------+
```

---

## Core Flows

### Order Creation Flow

```
Client
  ↓
API validation
  ↓
Begin DB transaction
  ↓
Create Order (PENDING)
  ↓
Commit transaction
  ↓
Publish PaymentRequested event
  ↓
Return 202 Accepted
```

### Payment Processing Flow

```
RabbitMQ
  ↓
Payment Worker
  ↓
Check idempotency key (Redis)
  ↓
Call external payment provider
  ↓
Update order & payment status
  ↓
Write audit log
```

---

## Tech Stack

| Category | Technology |
|--------|------------|
| Runtime | ASP.NET Core (.NET 8) |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Message Broker | RabbitMQ |
| Cache | Redis |
| Authentication | JWT + Refresh Tokens |
| Infrastructure | Docker & Docker Compose |

---

## Project Structure

```
src/
 ├── Api/
 ├── Application/
 ├── Domain/
 ├── Infrastructure/
 └── Workers/
```

---

## Idempotency & Reliability

- Idempotency keys stored in Redis
- Durable queues with acknowledgements
- Retry and DLQ strategies
- Safe handling of duplicate requests

---

## Failure Handling

Handled scenarios:
- Worker crashes
- Message redelivery
- Duplicate API requests
- External provider failures

---

## API Design (Sample)

POST /api/orders

```json
{
  "amount": 199.99,
  "currency": "USD"
}
```

---

## Local Setup

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose

---

## Running the System

```bash
docker-compose up -d
dotnet run --project src/Api
dotnet run --project src/Workers/PaymentProcessor
```

---

## Environment Variables

- DB_CONNECTION_STRING
- RABBITMQ_HOST
- REDIS_HOST
- JWT_SECRET

---

## Trade-offs

- Async payments = delayed confirmation
- RabbitMQ adds operational complexity
- Strong consistency limits throughput

---

## Future Improvements

- Outbox pattern
- Payment reconciliation
- OpenTelemetry
- Metrics & dashboards

---