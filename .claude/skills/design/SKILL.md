---
name: design
description: Design system rules for implementing FinClaude mobile screens in React Native. Covers tokens, typography, components, and screen-specific mockup references. Load this skill whenever implementing or auditing any mobile screen.
---

## Source files
- Tokens: `design/assets/tokens.css` — single source of truth for all values
- Components & principles: `design/style-guide/style-guide.html`
- Per-screen mockups: `design/screens/<screen>.html`

## Screen → mockup mapping
| Task | Mockup file |
|------|------------|
| TASK-20 (Auth) | `design/screens/auth.html` |
| TASK-21 (Account Setup) | `design/screens/account-setup.html` |
| TASK-22 / TASK-23 (Snapshots) | `design/screens/snapshots.html` |
| TASK-24 (Assets) | `design/screens/assets.html` |
| TASK-25 (Groups) | `design/screens/groups.html` |
| TASK-26 (Goals) | `design/screens/goals.html` |
| TASK-30 / TASK-31 (Dashboard) | `design/screens/dashboard.html` |

**Always read the relevant mockup file before implementing a screen.**

## Design principles
- **Clarity over decoration** — generous whitespace, no visual noise.
- **Trust through restraint** — calm palette; feels like a bank, not a trading app.
- **Low-friction data entry** — forms are large, fast and forgiving.
- **Long-term perspective** — emphasise trends, totals, direction of travel.

## Color tokens
| Token | Hex | Use |
|-------|-----|-----|
| `--brand-500` | `#1F8A5E` | Primary CTA, active states |
| `--brand-600` | `#186E4B` | Pressed state |
| `--brand-100` | `#D6EADD` | Focus ring, tinted backgrounds |
| `--ink-900` | `#0F1720` | Body text |
| `--ink-600` | `#4A5765` | Secondary text |
| `--ink-500` | `#6B7685` | Placeholder, captions |
| `--ink-300` | `#C3C9D2` | Borders (unfocused) |
| `--ink-200` | `#E1E5EB` | Dividers |
| `--ink-100` | `#EEF1F4` | Chip/tag backgrounds |
| `--ink-50` | `#F7F8FA` | Page background |
| `--surface` | `#FFFFFF` | Card / input backgrounds |
| `--danger` | `#D64545` | Errors, destructive actions |
| `--warning` | `#E9A53D` | Warnings |
| `--success` | `#1F8A5E` | Positive feedback |

Data-viz accents (charts only): `--accent-blue` `#3A7BD5`, `--accent-amber` `#E9A53D`, `--accent-coral` `#E26B5C`, `--accent-violet` `#7E6BD1`.

## Typography
| Role | Size | Weight | Notes |
|------|------|--------|-------|
| display | 32 | 800 | Net-worth total, hero numbers |
| h1 | 26 | 700 | Screen title |
| h2 | 22 | 700 | Section heading |
| h3 | 18 | 600 | Card title |
| body | 16 | 400 | Default text |
| small | 14 | 400 | Captions, metadata |
| tiny | 12 | 600 | Uppercase labels (letterSpacing: 0.06em) |
| mono | 15 | 500 | Financial values, codes |

- UI font: **Inter**
- Financial / numeric font: **JetBrains Mono** + `fontVariant: ['tabular-nums']`
- Letter spacing: display `−0.03em`, h1 `−0.02em`, h2 `−0.01em`

## Spacing (4 px base grid)
`sp1=4`, `sp2=8`, `sp3=12`, `sp4=16`, `sp5=20`, `sp6=24`, `sp7=32`, `sp8=40`, `sp9=48`, `sp10=64`

Standard screen horizontal padding: **sp6 (24px)**. Standard section gap: **sp7 (32px)**.

## Border radius
| Token | Value | Use |
|-------|-------|-----|
| r-sm | 6 | Chips, tags |
| r-md | 10 | Inputs, buttons, small cards |
| r-lg | 14 | Cards |
| r-xl | 20 | Bottom sheets, large cards |
| r-pill | 999 | Progress bars, badges |

## Elevation (shadows)
Use sparingly — only for floating/interactive surfaces.
- `shadow-xs` — subtle card lift
- `shadow-sm` — default card
- `shadow-md` — active/focused card
- `shadow-lg` — modal, bottom sheet

## Core components

### Buttons
- **Primary**: `bg=brand-500`, white text, `r-md`, `py=12 px=22`, weight 600, size 15
- **Secondary**: `bg=ink-100`, `ink-900` text
- **Ghost**: transparent, `brand-600` text
- **Danger**: `bg=danger`, white text
- Full-width on mobile forms.

### Inputs
- Border: `ink-200`, `r-md`, `py=12 px=16`, size 15
- Focus: border `brand-500`, shadow `0 0 0 3px brand-100`
- Error: border `danger`
- Label above input, `tiny` style, `ink-600`
- Error message below, `small`, `danger`

### Chips / Badges
- Default: `bg=brand-50`, `brand-700` text, `r-pill`, `py=5 px=12`, size 12, weight 600
- Neutral: `bg=ink-100`, `ink-700`
- Warning: `bg=#FEF3E0`, `#8A5A10`
- Danger: `bg=#FDE8E8`, `#8A2A2A`

### Progress bar
- Track: `bg=ink-100`, height 8, `r-pill`
- Fill: gradient `brand-400 → brand-500`

### Cards
- `bg=surface`, border `ink-200`, `r-lg`, `shadow-sm`
- Inner padding: `sp6 (24px)`
