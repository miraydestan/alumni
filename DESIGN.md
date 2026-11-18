# DESIGN.md – ALUMNI Design System & Visual Specification

> **Official Design System & Implementation Specification for ALUMNI**
> **Brand Name:** ALUMNI
> **English Description:** University Alumni Network
> **Turkish Description:** Üniversite Mezunlar Ağı
> **Design Direction:** Premium Academic – Elegant – Institutional – Editorial
> **Document Status:** Authoritative Single Source of Truth (SSOT) — Reviewed & Refined

---

## Document Purpose & Codebase Implementation Status

This document is the authoritative **Single Source of Truth (SSOT)** for the visual identity, UI design tokens, component architecture, and styling rules across the **ALUMNI** platform.

To maintain engineering integrity, this specification explicitly differentiates between **currently implemented code** in the repository and **proposed design standards** approved for ongoing and future UI development.

### Implementation Status Legend
* **`[Implemented in Codebase (main)]`**: Physically present and active in the current `main` branch (`wwwroot/css/site.css`, Razor Views, and Shared Layout).
* **`[Prototyped on feature/homepage-design]`**: Implemented and verified on the `feature/homepage-design` branch as part of the homepage redesign prototype, pending merge into `main`.
* **`[Proposed Design Standard]`**: The target visual standard established by the project leadership (`brand.md` and this document) that must guide all upcoming refactorings, new views, and component migrations.

---

## 1. Visual Theme & Atmosphere

### 1.1 Aesthetic Philosophy
The visual direction of ALUMNI is **Premium Academic – Elegant – Institutional – Editorial**.

The platform is designed to evoke the timeless prestige, intellectual dignity, and enduring fellowship of distinguished collegiate institutions (such as Oxford, Cambridge, Harvard, or Princeton). It deliberately avoids:
* Generic technology startup aesthetics and SaaS dashboard patterns.
* Saturated, bright blue buttons, electric purple highlights, or neon gradients.
* Excessive, bubbly cards with exaggerated border-radii.
* Artificial metrics, fabricated alumni testimonials, or synthetic counters.

### 1.2 Core Atmosphere Attributes
* **Academic Gravitas:** Collegiate Deep Navy forms the visual foundation, supported by Warm Off-White backgrounds and subtle, restrained Gold hairline accents.
* **Editorial Dignity:** Generous spatial margins, refined classical serif headlines (`Playfair Display`), crisp sans-serif body copy (`Plus Jakarta Sans`), and authentic campus architectural photography.
* **Photography-Driven Hero:** A full-width, high-resolution architectural campus photograph with a cinematic dark navy overlay replaces standard tech hero blocks.
* **Authenticity Over Artificiality:** Only real database records (`User`, `Announcement`, `JobPosting`) are displayed. When data is absent, a dignified academic empty state is shown; fake metrics are never fabricated.
* **Restrained Polish:** Hairline borders (1px) and soft ambient elevation replace heavy drop shadows and decorative clutter.

---

## 2. Color Palette & Semantic Tokens

### 2.1 Color Tokens & Implementation Status

The color system is strictly restrained. Below is the exact verification between the current codebase on `main` and the approved academic design standards:

| Token Name | Hex Code | Status | Role & Usage |
| :--- | :--- | :---: | :--- |
| `--ats-navy` | `#142B4A` | `[Proposed / Prototyped]` | **Primary Brand Color.** Headers, solid navbar state, primary cards, buttons. *(Note: `main` currently uses `#0f2942`)* |
| `--ats-navy-dark` | `#0D1B2A` | `[Proposed / Prototyped]` | **Dark Institutional Base.** Hero overlay base, closing banner, institutional footer. *(Note: `main` currently uses `#0a1b2b`)* |
| `--ats-warm-offwhite` | `#F7F5F0` | `[Proposed / Prototyped]` | **Editorial Cream Canvas.** Primary background tone providing warmth and readability. *(Note: `main` currently uses `#f8fafc`)* |
| `--ats-white` | `#FFFFFF` | `[Implemented in main]` | **Surface White.** Card surfaces, alternate section backgrounds, contrast button text. |
| `--ats-gold` | `#C5A46D` | `[Proposed / Prototyped]` | **Restrained Accent.** Overlines, active indicators, emblems, hairline dividers, primary CTA. |
| `--ats-gold-hover` | `#B39158` | `[Proposed / Prototyped]` | Interactive hover state for gold buttons and links. |
| `--ats-gold-light` | `#F4EEDA` | `[Proposed / Prototyped]` | Subtle tinted background for badges and highlighted quote containers. |
| `--ats-gray-muted` | `#737B86` | `[Proposed / Prototyped]` | Secondary text, timestamps, metadata labels, footer text. *(Note: `main` currently uses `#475569`)* |
| `--ats-gray-light` | `#E8E6DF` | `[Proposed / Prototyped]` | Card borders, table dividers, subtle hairline rules. *(Note: `main` currently uses `#e2e8f0`)* |

### 2.2 Gold Restraint Rule (Critical Guideline)
Gold (`#C5A46D`) is an **accent color**, not a primary background or text color:
* **Permitted Uses:** Hairline dividers (`.editorial-divider`), small uppercase section overlines (`.academic-overline`), active navigation indicators, subtle badge outlines, and selective primary CTA buttons (`.btn-academic-gold`).
* **Prohibited Uses:** Never use gold for large section backgrounds, modal dialog backgrounds, large blocks of body copy, or secondary buttons. Avoid placing multiple gold CTA buttons in the same viewport.

### 2.3 Verified WCAG Contrast Analysis
Rather than assuming accessibility compliance, color combinations have been mathematically calculated according to the **WCAG 2.1 relative luminance specification**:

$$\text{Contrast Ratio} = \frac{L_1 + 0.05}{L_2 + 0.05}$$

| Combination | Foreground | Background | Calculated Ratio | WCAG 2.1 Compliance Level | Usage Rule & Assessment |
| :--- | :--- | :--- | :---: | :---: | :--- |
| **Navy on White** | `#142B4A` | `#FFFFFF` | **14.26 : 1** | **Passes AAA** (Exceeds 7.0:1) | Ideal for primary headings, navigation, and body reading text. |
| **Navy on Warm Off-White** | `#142B4A` | `#F7F5F0` | **13.08 : 1** | **Passes AAA** (Exceeds 7.0:1) | Ideal for section titles and card headings on editorial cream sections. |
| **Dark Navy on Warm Off-White** | `#0D1B2A` | `#F7F5F0` | **15.65 : 1** | **Passes AAA** (Exceeds 7.0:1) | Maximum contrast for high-legibility institutional text. |
| **Dark Navy on Gold Button** | `#0D1B2A` | `#C5A46D` | **7.06 : 1** | **Passes AAA** (Exceeds 7.0:1) | **Required for `.btn-academic-gold`.** Text on gold buttons MUST be Dark Navy. |
| **Gold on Dark Navy** | `#C5A46D` | `#0D1B2A` | **7.06 : 1** | **Passes AAA** (Exceeds 7.0:1) | Permitted for overlines, icons, and badges in the footer and dark banners. |
| **Gold on Deep Navy** | `#C5A46D` | `#142B4A` | **5.95 : 1** | **Passes AA** (Exceeds 4.5:1) | Permitted for active nav links and accents on scrolled navy navbar. |
| **Muted Gray on White** | `#737B86` | `#FFFFFF` | **4.29 : 1** | **Passes AA Large Only** (Fails 4.5:1 for <18pt) | **Contrast Warning:** `#737B86` passes for large text (>=18pt or 14pt bold, threshold 3.0:1) but falls slightly short of 4.5:1 for normal body text. Primary body reading text must use `--ats-navy` (`#142B4A`). Reserve `#737B86` for secondary dates, metadata, and captions. |
| **Gold on White** | `#C5A46D` | `#FFFFFF` | **2.40 : 1** | **FAILS WCAG AA** (Below 4.5:1) | **Strictly Prohibited for Text:** Gold text on white or cream backgrounds is unreadable. Never render body copy or standard titles in gold on light backgrounds. |
| **White on Gold** | `#FFFFFF` | `#C5A46D` | **2.40 : 1** | **FAILS WCAG AA** (Below 4.5:1) | **Strictly Prohibited:** Never use white text on gold buttons or badges. Always use Dark Navy (`#0D1B2A`). |

---

## 3. Typography Rules

### 3.1 Font Stacks
* **Currently Implemented on `main`:**
  ```css
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
  ```
* **Proposed Academic Standard (`brand.md` & `feature/homepage-design`):**
  * **Headings & Editorial Titles (`var(--ats-font-serif)`):**
    `'Playfair Display', Georgia, 'Times New Roman', serif` (Weights: 600 SemiBold, 700 Bold).
  * **Body, UI & Navigation (`var(--ats-font-sans)`):**
    `'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif` (Weights: 400 Regular, 500 Medium, 600 SemiBold, 700 Bold).

### 3.2 Typographic Scale & Hierarchy
| Role | Recommended Font | Size | Weight | Line Height | Tracking | Usage |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Hero Display** | Playfair Display | `clamp(2.5rem, 5.5vw, 4.25rem)` | 700 | 1.15 | `-0.02em` | Homepage hero headline |
| **Section Title (H2)** | Playfair Display | `clamp(2rem, 3.5vw, 2.75rem)` | 600 / 700 | 1.25 | `-0.015em` | Major section headers |
| **Subsection (H3)** | Playfair Display | `clamp(1.35rem, 2.5vw, 1.75rem)` | 600 | 1.30 | `-0.01em` | Card titles, modal titles |
| **Section Overline** | Plus Jakarta Sans | `0.785rem` (12.5px) | 700 | 1.30 | `0.15em` (Caps) | Above section titles (`.academic-overline`) |
| **Editorial Lead** | Plus Jakarta Sans | `1.125rem` (18px) | 400 | 1.75 | `normal` | Hero subtext, section introductory leads |
| **Body Regular** | Plus Jakarta Sans / System | `1rem` (16px) | 400 | 1.65 – 1.70 | `normal` | Standard body paragraphs |
| **Metadata / Dates** | Plus Jakarta Sans / System | `0.85rem` (13.6px) | 500 / 600 | 1.40 | `0.05em` | Timestamps, badge labels, card info |
| **Caption / Micro** | Plus Jakarta Sans / System | `0.75rem` (12px) | 500 / 600 | 1.30 | `0.08em` | Archival photo captions |

---

## 4. Component Styling

### 4.1 Navigation Bar

#### Current Implementation (`main`):
* Class: `.navbar-custom` with light background (`#ffffff !important`), bottom border (`#e2e8f0`), `.navbar-brand-custom`, and `.brand-icon-box`.
* Links: Home, About, Users, Announcements, Job Postings, Admin, Alumni, plus Platform Info action button.

#### Proposed Academic Standard (`feature/homepage-design`):
* **Dual-State Adaptive Navbar (`.navbar-academic`):**
  * **Hero State (`.navbar-transparent`):** Positioned over the campus photography hero; transparent background with white navigation links and a subtle hairline border (`rgba(255, 255, 255, 0.12)`).
  * **Scrolled / Inner Page State (`.navbar-solid` / `.navbar-scrolled`):** Transitions smoothly to solid Deep Navy (`#142B4A`) with a gold hairline border (`rgba(197, 164, 109, 0.25)`) and soft elevation.
* **Brand Wordmark:**
  * Square emblem with mortarboard icon and gold hairline border (`1.5px solid var(--ats-gold)`).
  * Text block: Brand title `ALUMNI` (Playfair Display, bold, uppercase) and subtitle `University Alumni Network` / `Üniversite Mezunlar Ağı`.
* **Flexible Route Architecture:**
  * Navigation links represent core platform sections (e.g., Home, About, Alumni Directory, Announcements, Career Opportunities).
  * *Design Rule:* Avoid arbitrary restrictions on link count. Navigation items should reflect current functional site capabilities, with clear hierarchy and consistent padding.
* **Header Actions:**
  * Bilingual toggle (`TR | EN`).
  * Institutional portal action (`Login`) triggering the Single Sign-On (SSO) information modal (`#loginNoticeModal`).

### 4.2 Buttons & Actions

All interactive actions utilize a dignified, non-bubbly **4px border-radius standard**.

1. **Primary Gold Action (`.btn-academic-gold`) `[Proposed / Prototyped]`:**
   * Background: `--ats-gold: #C5A46D`
   * Text: `--ats-navy-dark: #0D1B2A !important` *(Ensures 7.06:1 WCAG AAA contrast; white text is prohibited)*
   * Hover: `--ats-gold-hover: #B39158`, transform `translateY(-1px)`.
2. **Academic Outline Action (`.btn-academic-outline`) `[Proposed / Prototyped]`:**
   * Background: `transparent`
   * Border: `1.5px solid rgba(255, 255, 255, 0.7)`
   * Text: `#FFFFFF !important`
   * Hover: `rgba(255, 255, 255, 0.12)` background, `#FFFFFF` border.
3. **Primary Custom Action (`.btn-primary-custom`) `[Implemented in main]`:**
   * `main`: Background `#2563eb` (SaaS blue).
   * *Proposed Refinement:* Map to Deep Navy (`#142B4A`) with gold hover state for backwards compatibility across existing inner views.
4. **Outline Custom Action (`.btn-outline-custom`) `[Implemented in main]`:**
   * `main`: Background `#ffffff`, border `#e2e8f0`, text `#0f172a`.
   * *Proposed Refinement:* Map to white background with `#142B4A` text and gold hover transition.

### 4.3 Cards & Content Containers

1. **Editorial Record Card (`.editorial-record-card`) `[Proposed / Prototyped]`:**
   * Background: `#FFFFFF`
   * Border: `1px solid var(--ats-gray-light)` with `3px solid var(--ats-navy)` top accent border.
   * Corner Radius: `4px`
   * Hover: Top border transitions to `--ats-gold`, subtle `translateY(-3px)` lift.
2. **Editorial Job Card (`.editorial-job-card`) `[Proposed / Prototyped]`:**
   * Background: `--ats-warm-offwhite: #F7F5F0`
   * Border: `1px solid var(--ats-gray-light)`
   * Corner Radius: `4px`
   * Hover: Background brightens to `#FFFFFF`, border transitions to subtle gold.
3. **Archival Image Frame (`.editorial-image-frame`) `[Proposed / Prototyped]`:**
   * Matted white frame (`padding: 0.5rem; background: #FFFFFF; border: 1px solid var(--ats-gray-light)`).
   * Contains real campus or archival photography with centered italic caption (`.editorial-caption`).
4. **Existing Cards (`.feature-card`, `.info-card`) `[Implemented in main]`:**
   * Currently active in `main` across `Views/User/Index.cshtml`, `Views/Announcement/Index.cshtml`, and `Views/Home/About.cshtml`.
   * Retained in CSS for full backwards compatibility; stylized with 4px border-radius and subtle hairline borders in the refined academic style.

### 4.4 Authentic Empty States (`.academic-empty-state`) `[Proposed / Prototyped]`
* Rather than fabricating mock records or artificial counts when tables are empty, pages must render a dignified empty container:
  * Border: `1px dashed var(--ats-gray-light)`
  * Corner Radius: `4px`
  * Background: `rgba(255, 255, 255, 0.6)`
  * Icon: Restrained gold academic icon (`bi-inbox`, `bi-people`, or `bi-megaphone`).
  * Text: Clear explanation and dignified invitation to contribute.

### 4.5 Modals & Dialogs (`#loginNoticeModal`) `[Prototyped on feature/homepage-design]`
* Background: `--ats-navy-dark: #0D1B2A`
* Text: White with subtle gold overline and divider accents.
* Informs visitors that member authentication is being integrated via institutional Single Sign-On (SSO), while keeping public views accessible.

### 4.6 Institutional Footer

#### Current Implementation (`main`):
* Class: `.footer-custom` with light background (`#ffffff`), `.footer-brand`, and horizontal links.

#### Proposed Academic Standard (`brand.md` & `feature/homepage-design`):
* Class: `.footer-academic` with Dark Navy (`#0D1B2A`) background and gold top hairline border.
* **Clean 3-Column Layout:**
  1. **Brand & Identity:** ALUMNI wordmark, subtitle (*University Alumni Network*), and institutional mission statement.
  2. **Institutional Navigation:** Essential public links (*Home*, *About the Platform*, *Alumni Directory*, *Campus Bulletins*, *Career Opportunities*).
  3. **Language Selection:** Prominent, accessible `English (EN)` and `Türkçe (TR)` selector buttons.
* **Strict Footer Sanitization:** Public footers must never expose technical backend implementation details (e.g. `ASP.NET Core`, `PostgreSQL`, Swagger, or direct administrator CRUD links).
* **Bottom Bar:** Separated by hairline border; contains copyright and accessible links to *Institutional Ethics* and *Privacy Policy*.

---

## 5. Layout Principles & Spacing System

### 5.1 8-Point Spatial Grid
Margins, paddings, and component dimensions align with standard 8px increments:
* `4px` (0.25rem) – Hairlines, text block gaps, badge vertical padding.
* `8px` (0.5rem) – Icon margins, button-to-icon gaps.
* `12px` (0.75rem) – Compact input padding, label-to-control spacing.
* `16px` (1rem) – Base component padding, standard grid gaps.
* `24px` (1.5rem) – Card body interior padding, column gutters (`g-4`).
* `32px` (2rem) – Section sub-block margins, large card padding.
* `48px` (3rem) – Major container vertical padding, tablet section margins.
* `64px` – `80px` (4rem – 5rem) – Standard desktop section vertical padding.
* `96px` (6rem) – Hero section padding and large desktop footer padding.

### 5.2 Section Background Alternation (Design Guideline)
Section background alternation serves as a **recommended visual rhythm guideline** to maintain visual interest across lengthy content, rather than an inflexible requirement:
* **Recommended Flow:**
  1. **Hero:** Full-width campus photograph with Dark Navy cinematic overlay.
  2. **Primary Section (e.g., About / Overview):** Warm Off-White (`#F7F5F0`).
  3. **Secondary Section (e.g., Directory / Community):** Pure White (`#FFFFFF`).
  4. **Tertiary Section (e.g., Bulletins / Announcements):** Warm Off-White (`#F7F5F0`).
  5. **Quaternary Section (e.g., Careers / Opportunities):** Pure White (`#FFFFFF`).
  6. **Closing Invitation Banner:** Deep Navy (`#142B4A`) or Dark Navy (`#0D1B2A`).
  7. **Footer:** Dark Navy (`#0D1B2A`).
* **Flexibility Rule:** Designers and developers may group consecutive sections under a uniform background (e.g., when presenting closely related content) or use dark accent containers when narrative focus requires emphasis.

### 5.3 Container Width Standards
* **Standard Bootstrap Container:** `container` (`max-width: 1200px` on wide screens).
* **Hero Content Max Width:** `max-width: 900px` (ensures optimal line length for display typography).
* **Editorial Reading Max Width:** `max-width: 680px – 720px` (used for editorial lead texts, details views, and form cards).

---

## 6. Depth, Borders & Elevation

ALUMNI prioritizes crisp architectural definition and subtle tactile depth over heavy dropshadows:

### 6.1 Shadow Scale
* `--ats-shadow-card`: `0 4px 16px -2px rgba(13, 27, 42, 0.05)` (Resting cards).
* `--ats-shadow-academic`: `0 10px 30px -10px rgba(13, 27, 42, 0.08)` (Card hover states and framed imagery).
* **Modal Elevation:** `0 20px 40px rgba(13, 27, 42, 0.4)` (Dialogs and overlays).

### 6.2 Border Radii Standards
* **4px Standard:** Cards, buttons, input fields, image frames, and modal dialogs. Conveys structured academic permanence.
* **2px Inner Framing:** Inner photograph boundaries within archival image frames.
* **50rem (Pill):** Strictly reserved for small status badges and category chips.

### 6.3 Dividers & Hairlines
* **Editorial Hairline Divider (`.editorial-divider`):** `width: 60px; height: 2px; background-color: var(--ats-gold); margin: 1.5rem 0;`.
* **Subtle Hairline Borders:** `1px solid var(--ats-gray-light)` (`#E8E6DF`) or `rgba(20, 43, 74, 0.08)`.

---

## 7. Do's and Don'ts

| Category | DO | DON'T |
| :--- | :--- | :--- |
| **Aesthetic Tone** | Maintain a dignified, prestigious academic editorial tone. | Never introduce SaaS startup tropes, emojis in headlines, or tech marketing jargon. |
| **Color Usage** | Use Deep Navy (`#142B4A`) and Warm Off-White (`#F7F5F0`) as the primary canvas. | Never use electric blue, neon gradients, purple highlights, or saturated green badges. |
| **Gold Restraint** | Treat Gold (`#C5A46D`) as a delicate hairline and accent color. | Never use gold for large backgrounds, body copy, or multiple competing buttons in one view. |
| **Contrast & WCAG** | Ensure all text meets verified WCAG 2.1 AA standards (Navy text on gold buttons = 7.06:1). | Never put white text on gold buttons (2.40:1, fails AA) or gold text on white surfaces (2.40:1, fails AA). |
| **Typography** | Use Playfair Display for headings and Plus Jakarta Sans for body and UI. | Never use playful display fonts, unstyled serif defaults, or bubbly rounded sans-serifs. |
| **Data Integrity** | Render authentic database properties or dignified empty states. | Never invent fake statistics (*"15,000+ Alumni Worldwide"*), fake quotes, or synthetic profiles. |
| **Layout Rhythm** | Use alternating warm off-white and white backgrounds as a rhythm guide. | Do not treat background alternation as a rigid, unyielding constraint when context dictates otherwise. |
| **Navigation** | Provide clear navigation to primary functional sections with adaptive hero states. | Do not enforce arbitrary restrictions like "exactly five links"; adapt links to site functionality. |
| **Footer** | Present an institutional footer with brand identity, verified links, and language toggles. | Never display backend technical stack details (`PostgreSQL`, `ASP.NET Core`, Swagger) to public visitors. |
| **Localization** | Provide complete, authentic English and Turkish translations for all UI chrome. | Never mix English titles with Turkish descriptions in the same view. |

---

## 8. Responsive Design Rules

### 8.1 Breakpoint Standards
* **Mobile (`< 768px`):**
  * Grid reflows to single-column (`col-12`).
  * Navbar collapses into a solid Deep Navy container with an accessible hamburger toggle.
  * Hero padding adjusts to `4.5rem 0 3rem`; display headlines scale via `clamp()` to approximately `2.35rem`.
* **Tablet (`768px – 991px`):**
  * Grid reflows to two columns (`col-md-6`).
  * Archival image frames adjust height to `300px`.
* **Desktop (`992px – 1199px`):**
  * Three-column grid layout (`col-lg-4`).
  * Horizontal navbar with transparent hero overlay and active indicators.
* **Wide Desktop (`>= 1200px`):**
  * Full editorial layout, `1200px` container width, `420px` archival image frame height.

### 8.2 Touch Target Standards
* All interactive touch targets (buttons, links, language pills) must provide a minimum clickable area of **44px × 44px**.
* Mobile navigation items must feature a minimum of `0.75rem` vertical padding to prevent accidental mis-taps.

---

## 9. Localization Implementation Standards

### 9.1 Architecture & Project Conventions
* **Current State in `main`:** The codebase currently renders static strings, with some views exhibiting mixed language copy. No formal runtime localization framework (`IStringLocalizer`) is currently registered in `Program.cs`.
* **Prototype State on `feature/homepage-design`:** The redesign branch tested a lightweight client-side bilingual approach utilizing dual attributes:
  ```html
  <span data-i18n-en="University Alumni Network"
        data-i18n-tr="Üniversite Mezunlar Ağı">
      University Alumni Network
  </span>
  ```
  coupled with a client-side JavaScript toggle and C# server-side helper methods for database model fallbacks.
* **Design System Standard:** Rather than mandating a single frontend implementation mechanism (e.g. strictly requiring `data-i18n` attributes), the design system defines the **linguistic and content quality standards**:
  1. Every user-facing UI element (headers, buttons, navigation, empty states, labels) must have authentic English and Turkish phrasing.
  2. Language selection must be synchronized: switching to English must translate all UI chrome to English; switching to Turkish must translate all UI chrome to Turkish.
  3. Date representations must follow locale conventions:
     * English: `MMMM dd, yyyy` (e.g., *November 18, 2026*)
     * Turkish: `dd MMMM yyyy` (e.g., *18 Kasım 2026*)
  4. Whichever technical architecture is adopted (client-side attributes, ASP.NET Core `IViewLocalizer`, or route-based culture), developers must preserve these linguistic rules without breaking existing conventions.

---

## 10. AI Agent Implementation Guidelines

When generating or editing views, controllers, or CSS files, autonomous AI coding agents must adhere to the following checklist:

### 10.1 Implementation Checklist
- [ ] **Brand Consistency:** Site title is strictly **ALUMNI**; Turkish description is *Üniversite Mezunlar Ağı*; English description is *University Alumni Network*.
- [ ] **No Generic SaaS UI:** Do not reintroduce SaaS badge widgets, bright blue buttons (`#2563eb`), or colorful gradients.
- [ ] **Verified WCAG Contrast:** Ensure text has adequate contrast. Never place white text on gold buttons; always use Dark Navy (`#0D1B2A`). Never place gold text on white backgrounds.
- [ ] **Authentic Database Records:** Bind real database properties (`item.Title`, `item.CreatedDate`). If a collection is empty, render `.academic-empty-state` rather than generating mock cards.
- [ ] **Clean Footer:** Do not add Swagger, developer documentation, or database infrastructure links to public footers.
- [ ] **Clean Build:** Run `dotnet build` to verify 0 warnings and 0 errors after code changes.
- [ ] **UTF-8 Encoding:** Ensure all Turkish characters (ç, ğ, ı, İ, ö, ş, ü, Ç, Ğ, Ö, Ş, Ü) are saved in clean UTF-8 without corruption.

---

## 11. Codebase Verification Audit Matrix

The following table provides an exhaustive reconciliation of every documented CSS token, font family, and reusable class against the actual repository source files:

| Element / Identifier | Type | Found in `main` (`wwwroot/css/site.css`) | Found in `feature/homepage-design` | Status & Guidance |
| :--- | :---: | :---: | :---: | :--- |
| `--ats-navy` | CSS Var | Yes (`#0f2942`) | Yes (`#142B4A`) | **Implemented.** `#142B4A` is the proposed academic standard. |
| `--ats-navy-dark` | CSS Var | Yes (`#0a1b2b`) | Yes (`#0D1B2A`) | **Implemented.** `#0D1B2A` is the proposed academic standard. |
| `--ats-primary` | CSS Var | Yes (`#1e40af`) | No (replaces with navy) | **Legacy Token in `main`.** Deprecate in favor of `--ats-navy`. |
| `--ats-accent` | CSS Var | Yes (`#2563eb`) | No (replaces with gold) | **Legacy Token in `main`.** Deprecate in favor of `--ats-gold`. |
| `--ats-accent-hover` | CSS Var | Yes (`#1d4ed8`) | No | **Legacy Token in `main`.** Deprecate in favor of `--ats-gold-hover`. |
| `--ats-bg` | CSS Var | Yes (`#f8fafc`) | Yes (`var(--ats-warm-offwhite)`) | **Implemented.** Maps to warm off-white in academic standard. |
| `--ats-card-border` | CSS Var | Yes (`#e2e8f0`) | Yes (`rgba(20,43,74,0.08)`) | **Implemented.** Hairline border format in academic standard. |
| `--ats-text-main` | CSS Var | Yes (`#0f172a`) | Yes (`#142B4A`) | **Implemented.** Maps to Deep Navy in academic standard. |
| `--ats-text-muted` | CSS Var | Yes (`#475569`) | Yes (`#737B86`) | **Implemented.** Maps to Muted Gray in academic standard. |
| `--ats-warm-offwhite` | CSS Var | No | Yes (`#F7F5F0`) | **Prototyped.** Core token of proposed academic palette. |
| `--ats-gold` | CSS Var | No | Yes (`#C5A46D`) | **Prototyped.** Core accent token of proposed academic palette. |
| `--ats-gold-hover` | CSS Var | No | Yes (`#B39158`) | **Prototyped.** Interactive accent token. |
| `--ats-gold-light` | CSS Var | No | Yes (`#F4EEDA`) | **Prototyped.** Tinted badge/quote token. |
| `--ats-gray-light` | CSS Var | No | Yes (`#E8E6DF`) | **Prototyped.** Card border and divider token. |
| `-apple-system...` | Font Stack | Yes (in `body`) | Yes (as sans fallback) | **Implemented in `main`.** Default system font. |
| `Playfair Display` | Font Family | No | Yes (in `_Layout.cshtml`) | **Prototyped.** Headings standard for academic identity. |
| `Plus Jakarta Sans` | Font Family | No | Yes (in `_Layout.cshtml`) | **Prototyped.** Body standard for academic identity. |
| `.navbar-custom` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** Legacy light navbar. |
| `.navbar-academic` | CSS Class | No | Yes | **Prototyped.** Adaptive transparent-to-solid navbar. |
| `.navbar-transparent` | CSS Class | No | Yes | **Prototyped.** Hero state for academic navbar. |
| `.navbar-solid` | CSS Class | No | Yes | **Prototyped.** Scrolled/inner state for academic navbar. |
| `.btn-primary-custom` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** Backwards compatibility retained. |
| `.btn-outline-custom` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** Backwards compatibility retained. |
| `.btn-academic-gold` | CSS Class | No | Yes | **Prototyped.** Primary CTA for academic identity. |
| `.btn-academic-outline`| CSS Class | No | Yes | **Prototyped.** Outline CTA for dark hero sections. |
| `.feature-card` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** Feature card on inner/home views. |
| `.info-card` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** Info card on About and Users views. |
| `.icon-box` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** Icon container. |
| `.card-heading` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** Card title class. |
| `.card-description` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** Card description class. |
| `.track-list` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** List with icons on About view. |
| `.management-banner` | CSS Class | Yes | Yes (compatibility) | **Implemented in `main`.** Banner card on About view. |
| `.editorial-record-card`| CSS Class| No | Yes | **Prototyped.** Refined card with top navy accent line. |
| `.editorial-job-card` | CSS Class | No | Yes | **Prototyped.** Refined cream-surfaced career card. |
| `.editorial-image-frame`| CSS Class| No | Yes | **Prototyped.** Matted white archival photograph frame. |
| `.academic-empty-state` | CSS Class| No | Yes | **Prototyped.** Dignified empty table/list state. |
| `.academic-overline` | CSS Class | No | Yes | **Prototyped.** Uppercase gold section category tag. |
| `.academic-heading` | CSS Class | No | Yes | **Prototyped.** Playfair Display section title class. |
| `.editorial-divider` | CSS Class | No | Yes | **Prototyped.** 60px gold hairline accent bar. |
| `.footer-custom` | CSS Class | Yes | No (replaced) | **Implemented in `main`.** Legacy light footer. |
| `.footer-academic` | CSS Class | No | Yes | **Prototyped.** Dark Navy academic footer. |
