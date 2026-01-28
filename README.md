# Box Office Nightly Split Tracker

A web application for tracking nightly box office ticket sales and estimating net revenue based on distributor percentage cuts.  
Designed for drive-in theaters or venues where **one ticket grants access to all films played in a night**.

Users sign in with Google, enter nightly ticket and film data, and view historical records over time.

---

## Core Concept

Each ticket sold grants admission to **all movies played that night** (double feature, triple feature, etc.), regardless of whether a patron watches every showing.

Because ticket revenue is **not divided per film**, net revenue is calculated by applying **a single distributor cut percentage** to the night’s total ticket revenue.

---

## What Users Enter (Per Night)

- **Date**
- **Number of Films Played**
- **Ticket Data**
  - Number of tickets sold
  - Price per ticket
- **Distributor Cut (choose one method)**
  - **Single % cut for the night**, OR
  - **% cut per film** (used to derive one nightly cut)

---

## Calculations

### Gross Ticket Revenue

grossSales = ticketsSold * ticketPrice

### Net Ticket Revenue (Estimated)

netSales = ticketsSold * ticketPrice * (1 - cutPercent)


Where `cutPercent` is expressed as a decimal  
(e.g., 55% → `0.55`).

**Example**
- Tickets sold: 200
- Ticket price: $12.50
- Distributor cut: 55%

Gross = 200 × 12.50 = $2,500  
Net = 2,500 × (1 - 0.55) = **$1,125**

---

## Determining the Distributor Cut

Users may enter distributor cuts in one of two ways.

### Option A — Cut % Per Night
A single percentage cut applied to the entire night.

nightCutPercent = userEnteredPercent


### Option B — Cut % Per Film
A cut percentage is entered for each film played that night.  
Since tickets grant access to all films, the app derives **one effective nightly cut**.

#### Default Rule: Highest Cut Wins

nightCutPercent = max(filmCutPercents)


This rule is:
- Conservative
- Simple to explain
- Common in multi-feature distributor agreements
- Prevents underestimating distributor obligations

> Future enhancement: allow average or weighted rules via settings.

---

## Features

- **Google Sign-In**
  - Secure OAuth authentication
  - User-specific data isolation

- **Nightly Entry Form**
  - Date picker
  - Ticket count and ticket price
  - Film count
  - Distributor cut input (per night or per film)

- **Automatic Calculations**
  - Gross ticket revenue
  - Estimated net revenue

- **History & Review**
  - View past entries
  - Sort and filter by date
  - View detailed breakdown per night
  - Edit or delete entries (configurable)

---

## Suggested Pages / Routes

- `/` — Dashboard / overview
- `/signin` — Google sign-in
- `/entries/new` — Create nightly entry
- `/entries` — Entry history
- `/entries/{id}` — View/edit entry
- `/settings` — Defaults and calculation rules

---

## Tech Stack (Planned)

The project is expected to use **ASP.NET Core**, but final framework decisions are still open.

### Backend (C#)
- **ASP.NET Core Minimal API** *(lean, fast)*
- **ASP.NET Core MVC**
- **Blazor Server / Blazor WebAssembly**

### Frontend
- Razor Pages (if staying fully in .NET)
- Blazor (C# UI)
- React / Next.js (optional external frontend)

### Authentication
- Google OAuth via ASP.NET Core authentication
- Or external auth provider (Auth0, Firebase, etc.)

### Database (TBD)
- PostgreSQL
- SQL Server
- SQLite (local development)
- MongoDB (document-based alternative)

---

## Data Model (Suggested)

### Entry
Represents one night of ticket sales.

- `Id`
- `UserId`
- `Date`
- `FilmsPlayedCount`
- `TicketsSold`
- `TicketPrice`
- `CutMode` (`PerNight` | `PerFilm`)
- `NightCutPercent`
- `GrossSales`
- `NetSales`
- `CreatedAt`
- `UpdatedAt`

### FilmCut (if CutMode = PerFilm)
- `Id`
- `EntryId`
- `FilmTitle` (optional)
- `CutPercent`

---

## Environment Variables (Example)

Actual names may vary depending on framework choice.

```bash
GOOGLE_CLIENT_ID="..."
GOOGLE_CLIENT_SECRET="..."
DATABASE_CONNECTION_STRING="..."
ASPNETCORE_ENVIRONMENT="Development"

