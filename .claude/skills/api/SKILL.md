---
name: api
description: API-level rules for authentication, authorization, error mapping, pagination, and soft-delete behavior in the FinClaude backend. Ensures consistent HTTP behavior and predictable API responses across all endpoints.
---
## Authentication Rules
- JWT must contain the `AccountId` claim.
- Every authenticated request must extract `AccountId` from the JWT.
- All data access must be scoped by `AccountId`.
- Refresh token rotation:
  - Every refresh returns a **new** refresh token.
  - Old refresh token becomes invalid.

## Account Setup Guard
- If `Account.SnapshotStartDate` is `null`, the API must return:
  - **403 Forbidden**
  - Problem Details type: `account-setup-required`
- Guard applies to all authenticated routes **except**:
  - `/api/v1/auth/*`
  - `PUT /api/v1/account`

## Error Mapping Rules
- `Error.Validation` → **422 Unprocessable Entity**
- `Error.NotFound` → **404 Not Found**
- `Error.Conflict` → **409 Conflict**
- `Error.Unexpected` → **500 Internal Server Error**

## Pagination Rules
- Pagination uses offset-based parameters:
  - `?page=1&pageSize=20`
- API responses must follow this structure:
```json
{
  "items": [...],
  "totalCount": number,
  "page": number,
  "pageSize": number
}
```
## REST Conventions
- Base path: `/api/v1/`
- CRUD mapping:
  - List → GET `/resource`
  - Detail → GET `/resource/{id}`
  - Create → POST `/resource`
  - Update → PUT `/resource/{id}`
  - Delete → DELETE `/resource/{id}`

## Soft-delete Behavior
- Soft-deleted entities must:
  - Be excluded from all list endpoints.
  - Still appear in historical snapshot data.
  - Soft-delete must **never** remove or alter historical records.

## Authorization Rules
- Cross-account access must always return **404 Not Found**.
- Handlers must validate ownership before performing any operation.
- Steps must enforce authorization before domain logic.

## General API Behavior
- Controllers must be thin:
  - No business logic.
  - Map HTTP → command/query → handler → HTTP.
- All endpoints must return Problem Details for errors.
- All endpoints must use consistent casing and naming conventions.
- All error responses must use RFC 9457 Problem Details format.
