You are running the **Feature Discovery Interview** for FinClaude. Gather enough information to fully specify a new feature and decompose it into tasks.

## Before you start

Read silently:
1. `SPECIFICATION.md` — avoid duplicating existing features
2. `ImplementationPlan/progress.md` — existing tasks and status
3. `ImplementationPlan/dependencies.md` — dependency graph for placing new tasks

Only open a specific task file if `dependencies.md` is insufficient to determine connections.

Use any description in the `/feature` message as your starting point.

## Interview — two rounds

Ask each round as a single message.

### Round 1 — Business context
1. What is the feature?
2. What user problem does it solve?
3. Walk me through the user journey — trigger, actions, outcome.
4. What does success look like?
5. Any known non-goals or constraints?

### Round 2 — Technical context

Ask only what applies:
- New data required? (entities or fields)
- New or changed API endpoints?
- New or changed mobile screens?
- Which existing tasks does it depend on?
- Breaking changes to existing behavior?
- Validation rules or business constraints?
- Edge cases?

## After the interview

**1. Summarise**

```
Feature: [name]
Problem: ...
User journey: ...
Data model changes: ...
New endpoints: ...
New screens: ...
Dependencies: ...
Validation / business rules: ...
Edge cases: ...
Out of scope: ...
```

Ask: *"Is this accurate? Shall I proceed?"*

**2. On confirmation**, spawn `analytic-consultant` with the full summary. The agent produces exactly:
1. New task file — `ImplementationPlan/tasks/<Px-TASK-NN>.md`
2. Updated `ImplementationPlan/progress.md` — new row, status `Waiting`
3. Updated `ImplementationPlan/dependencies.md` — new entry in correct phase block

## Rules
- Do not modify any files yourself — `analytic-consultant` does that
- No confirmation = no action
- If the feature already exists in SPECIFICATION.md, say so and stop

