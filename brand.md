# ALUMNI – Brand & Design Guidelines

- **Official Brand Name:** ALUMNI
- **Turkish Description:** Üniversite Mezunlar Ağı
- **English Description:** University Alumni Network

## Approved Design Direction: Premium Academic & Institutional Editorial

This document specifies the official visual identity, color tokens, typography, architectural layout rules, and accessibility standards for the **ALUMNI** university platform.

---

## 1. Visual Philosophy & Design Identity

The visual identity embodies the prestige, dignity, and timeless heritage of an internationally distinguished collegiate institution. It rejects generic technology startup and SaaS dashboard aesthetics in favor of a **sophisticated academic editorial aesthetic**.

### Key Brand Pillars:
- **Academic Prestige:** Inspired by world-renowned collegiate institutions, featuring deep navy and subtle warm gold accents.
- **Institutional Dignity:** Generous whitespace, refined serif headlines, and architectural photography create trust and credibility.
- **Lifelong Connection:** Bringing together graduates, faculty, and current students through shared intellectual heritage and authentic opportunities.
- **Clarity & Truth:** Rejection of fabricated metrics, artificial social proof, and superfluous decorative widgets.

---

## 2. Approved Color Palette & Design Tokens

The color system is strictly defined to ensure a restrained, elegant, and unified institutional presentation.

### 2.1 Color Tokens
| Name | Hex Code | Role & Usage |
| :--- | :--- | :--- |
| **Deep Navy** | `#142B4A` | Dominant brand color, primary headers, solid navbar, key action surfaces |
| **Dark Navy** | `#0D1B2A` | Background for hero overlay, closing banner, and institutional footer |
| **Warm Off-White** | `#F7F5F0` | Primary alternating section background providing warmth and editorial rhythm |
| **White** | `#FFFFFF` | Card surfaces, clean contrast section backgrounds, button highlights |
| **Elegant Gold** | `#C5A46D` | Subtle accent for overlines, active indicators, borders, and emblems |
| **Muted Gray** | `#737B86` | Secondary body text, timestamps, metadata labels, footer text |

### 2.2 Functional Accents
| Token | Value | Context |
| :--- | :--- | :--- |
| `--ats-gold-hover` | `#b39158` | Interactive gold hover state |
| `--ats-gold-subtle` | `rgba(197, 164, 109, 0.15)` | Subtle gold hairline borders and badges |
| `--ats-navy-subtle` | `rgba(20, 43, 74, 0.08)` | Minimal border dividers on light surfaces |
| `--ats-overlay` | `rgba(13, 27, 42, 0.68)` | Cinematic hero image darkening overlay |

---

## 3. Typography System

### 3.1 Font Stack
- **Headings & Institutional Display:**
  `'Playfair Display', Georgia, 'Times New Roman', serif`
  - Evokes classical academic gravitas, high-contrast letterforms, and editorial elegance.
- **Body & Functional UI:**
  `'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif`
  - Ensures effortless legibility across all viewport sizes, clean numerical rendering, and crisp contrast.

### 3.2 Typographic Hierarchy
| Role | Font Family | Size | Weight | Line Height | Letter Spacing |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Hero Display** | Playfair Display | `clamp(2.5rem, 5.5vw, 4.25rem)` | 700 / Bold | 1.15 | `-0.02em` |
| **Section Title** | Playfair Display | `clamp(2rem, 3.5vw, 2.75rem)` | 600 / SemiBold | 1.25 | `-0.01em` |
| **Editorial Lead** | Plus Jakarta Sans | `1.15rem` (18.4px) | 400 / Regular | 1.7 | `normal` |
| **Section Overline**| Plus Jakarta Sans | `0.785rem` (12.5px) | 700 / Bold | 1.3 | `0.15em` (Caps) |
| **Body Text** | Plus Jakarta Sans | `1rem` (16px) | 400 / Regular | 1.65 | `normal` |
| **Metadata / Dates**| Plus Jakarta Sans | `0.85rem` (13.6px) | 500 / Medium | 1.4 | `0.02em` |

---

## 4. Navigation Architecture

### 4.1 Transparent-to-Solid Scroll Transition
- **Hero State (Top of Page):**
  - Completely transparent background positioned directly over the hero image.
  - High-contrast white typography with gold accent highlights.
  - Minimalistic `ALUMNI` typographic wordmark.
- **Scrolled State (> 50px):**
  - Smooth transition (`transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1)`) to solid Deep Navy (`#142B4A`).
  - Subtle gold bottom hairline divider (`1px solid rgba(197, 164, 109, 0.25)`).
  - Maintained accessibility and legibility.
- **Internal Pages:**
  - Sticky solid Deep Navy navbar across all non-hero pages (`/about`, `/User`, `/Announcement`, `/JobPosting`, `/AnnouncementAdmin`).

### 4.2 Navigation Item Layout
- **Left:** `ALUMNI` institutional wordmark with university subtitle.
- **Center:**
  - `Home` (`/`)
  - `About` (`/about`)
  - `Alumni` (`/User`)
  - `Announcements` (`/Announcement`)
  - `Jobs` (`/JobPosting`)
- **Right:**
  - `TR | EN` interactive language switcher.
  - `Login` institutional portal action.

---

## 5. Homepage Structure & Composition

The homepage follows an editorial academic narrative:

1. **Section 1 – Hero:**
   - Full-width architectural campus photography (`/images/campus-hero.jpg`).
   - Dark navy overlay ensuring optimal text contrast.
   - Headline: *"Connecting Generations. Inspiring Futures."* / *"Kuşakları Birleştiren, Geleceğe İlham Veren."*
   - Concise supporting text: *"Bringing graduates and students together through shared experiences, meaningful connections, and new opportunities."*
   - Two refined CTAs: Primary Gold button (*Explore Alumni*) and Gold Outline (*Discover Opportunities*).
   - Zero two-column tech layouts, zero fake badges, zero fabricated profiles.

2. **Section 2 – About the Platform:**
   - Warm Off-White (`#F7F5F0`) background.
   - Editorial presentation with institutional narrative and archival photography (`/images/campus-about.jpg`).
   - Clear focus on heritage, continuous institutional engagement, and academic excellence.

3. **Section 3 – Alumni Connections:**
   - Pure White (`#FFFFFF`) background.
   - Introduction to the global multidisciplinary graduate directory.
   - Editorial structure encouraging visitors to explore `/User` without fabricated statistics.

4. **Section 4 – Campus Announcements:**
   - Warm Off-White (`#F7F5F0`) background.
   - Direct database integration (`_db.Announcements`).
   - Displays real titles, real dates, and real descriptions.
   - Dignified empty state when no records exist.

5. **Section 5 – Career Opportunities:**
   - Pure White (`#FFFFFF`) background.
   - Direct database integration (`_db.JobPostings`).
   - Displays only existing model properties (`Title`, `Company`, `User.Name`).
   - Professional corporate styling linking to `/JobPosting`.

6. **Section 6 – Closing Institutional Call:**
   - Deep Navy (`#142B4A`) background with subtle gold hairline frame.
   - Dignified invitation to participate in university alumni initiatives.

7. **Section 7 – Institutional Footer:**
   - Dark Navy (`#0D1B2A`) background.
   - Clean 3-column layout: Brand & narrative, Institutional links, and Language selection.
   - Verified system links, copyright, and language options without technical clutter.

---

## 6. Bilingual Support Standards

- The site provides comprehensive, synchronized support for **English (EN)** and **Turkish (TR)**.
- Language selection is persisted across page navigation via client-side cookie (`alumni_language`) and immediate DOM translation without full page reloads.
- All headings, subheadings, buttons, empty states, and navigation labels feature authentic, idiomatic translations.
- Database-backed content (e.g. user-submitted titles) is presented authentically without artificial translation.
