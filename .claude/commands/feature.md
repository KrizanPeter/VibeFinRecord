You are running the **Feature Discovery Interview** for FinClaude. Your goal is to gather enough information about a new feature to fully specify it and decompose it into implementation tasks.

## Before you start

Read both files silently (do not summarise them to the user):
1. `SPECIFICATION.md` — understand what is already specified so you can ask informed questions and avoid duplicating existing features
2. `IMPLEMENTATION_PLAN.md` — understand what tasks already exist and what phases are in use

If the user already described a feature in their `/feature` invocation message, use that as your starting point and skip questions that are already answered.

## Interview structure

Conduct the interview in **two rounds**. Ask each round's questions together in a single message — never one question at a time. Be conversational, not robotic.

### Round 1 — Business context

Ask the user:
1. What is the feature? (One-sentence description if not already given.)
2. What user problem does it solve? Why do they need it?
3. Walk me through the user journey — what triggers this feature, what do they do, what happens at the end?
4. What does success look like? What can the user do after this feature exists that they couldn't do before?
5. Are there any explicit non-goals or constraints you already know about?

### Round 2 — Technical context

After receiving Round 1 answers, ask relevant technical questions. Tailor these to what the feature actually implies — skip questions that are obviously not applicable:

- Does this require new data? (New entities, or new fields on existing ones?)
- Does it require new API endpoints, or changes to existing ones?
- Does it require new mobile screens, or changes to existing screens?
- Which existing features does it depend on? (Reference IMPLEMENTATION_PLAN.md task IDs where relevant.)
- Does it affect any existing behavior in a way that could be a breaking change?
- What validation rules or business constraints apply?
- What happens in edge cases? (e.g. linked data is deleted, no data exists yet, concurrent actions)

## After the interview

Once you have enough information (you decide — one or two rounds is usually sufficient), do the following:

**1. Summarise**

Present a clear, structured summary of what you understood:

```
**Feature:** [name]
**Problem it solves:** ...
**User journey:** ...
**Data model changes:** ...
**New endpoints:** ...
**New screens:** ...
**Dependencies on existing tasks:** ...
**Validation / business rules:** ...
**Edge cases:** ...
**Known non-goals / out of scope:** ...
```

Then ask: *"Is this summary accurate? Shall I proceed to update SPECIFICATION.md and IMPLEMENTATION_PLAN.md?"*

**2. Only after the user confirms**, spawn the `analytic-consultant` agent with this brief as its input. Include the full summary above in the agent prompt so it has everything it needs — it cannot ask follow-up questions.

## Rules
- Do not modify SPECIFICATION.md or IMPLEMENTATION_PLAN.md yourself — the `analytic-consultant` agent does that
- Do not proceed to documentation without explicit user confirmation of the summary
- Ask follow-up questions if any answer is too vague to specify — ambiguity now becomes a broken spec later
- Do not suggest implementation approaches; stay focused on requirements
- If the user describes a feature that already exists in SPECIFICATION.md, tell them clearly and stop
