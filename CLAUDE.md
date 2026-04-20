# FinClaude — Claude Code Guide

Personal finance tracker. Users snapshot asset values periodically; the app builds a net-worth timeline and tracks goals. Full spec: `SPECIFICATION.md`.

## Repository layout

```
FinClaude/
├── SPECIFICATION.md          — product source of truth
├── ImplementationPlan/
│   ├── progress.md           — task status (start here each session)
│   ├── dependencies.md       — dependency graph
│   └── tasks/                — Px-TASK-NN[-DONE].md per task
├── design/
│   ├── assets/tokens.css     — canonical design tokens
│   ├── screens/              — per-screen HTML mockups
│   └── style-guide/
├── .claude/agents/           — agent definitions
├── .claude/skills/           — skill SKILL.md files (canonical rules)
└── src/                      — backend (.NET) + mobile (React Native)
```

## Tech stack

| Layer | Technology |
|---|---|
| Mobile | React Native (iOS + Android) |
| Backend | .NET (C#), RESTful, Clean Architecture |
| Database | SQLite via EF Core |
| Auth | ASP.NET Core Identity + JWT |

Backend layers: `Domain` ← `Application` ← `Infrastructure` ← `Api`

### Mobile libraries

| Concern | Library |
|---|---|
| Navigation | React Navigation (Stack + Bottom Tabs) |
| HTTP | Axios (JWT inject + silent refresh interceptors) |
| Server state | TanStack Query |
| Global state | Zustand |
| Forms | React Hook Form |

## Mobile architecture rules

- API calls via TanStack Query only — no raw `useEffect` + `useState` for server data.
- Auth tokens in `authStore`, account state in `accountStore` (Zustand).
- All forms use React Hook Form — no per-field `useState`.
- JWT injection and `401` silent refresh in Axios interceptors (`src/api`) only.
- Route guards in `src/navigation`, not in screens.
- Financial values: JetBrains Mono, `fontVariant: ['tabular-nums']`.

## Agents

| Agent | When to use |
|---|---|
| `analytic-consultant` | Adding a feature not in SPECIFICATION.md |
| `spec-guardian` | Product decision changed or added |
| `plan-reviewer` | Start of session — find next task |
| `backend-builder` | Starting a backend vertical slice |
| `design-consultant` | Implementing or auditing a mobile screen |
| `code-reviewer` | After completing a task |

**`/feature`** — starts feature discovery interview, then spawns `analytic-consultant`.

## Key spec decisions

- Refresh tokens in ASP.NET Identity `UserTokens` table, rotated on use.
- `SnapshotStartDate` + `SnapshotPeriodicity` locked once any Snapshot exists (409).
- Goal progress with no snapshots → `currentValue: null, progressPercent: null`.

