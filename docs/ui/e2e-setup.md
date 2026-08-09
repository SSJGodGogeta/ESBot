# E2E Test Setup

## Framework chosen

**Cypress** (`^15.18.0`, tested against `15.20.0`), JavaScript, running the Electron
browser bundled with Cypress. Cypress was chosen because:

- The frontend is a small static HTML/JS page (`frontend/index.html` + `frontend/script.js`)
  with no build step, so Cypress's zero-config `cy.visit('/')` + `baseUrl` setup is enough —
  no bundler/dev-server integration is required.
- It ships with automatic retry-until-timeout assertions (`.should(...)`) out of the box,
  which satisfies the "no fixed `sleep()`" requirement without extra wiring.
- The interactive Test Runner (`cypress open`) gives a visual, step-by-step command log
  that is useful for debugging the async API calls this app makes.

The suite currently lives in `frontend/cypress/e2e/user-session.cy.js`.

## Installation

From the `frontend/` folder:

```powershell
cd frontend
npm install
```

This installs `cypress` as a dev dependency (see `frontend/package.json`) and downloads
the Cypress Electron binary on first install. If the binary was not installed
automatically (e.g. npm scripts are restricted in your environment), run:

```powershell
npx cypress install
```

## Starting the application before running tests

Cypress drives a real browser against a real backend + frontend — both must already be
running before the test command is invoked.

1. **Database** (Postgres, via Docker):
   ```powershell
   docker compose up -d db
   ```
2. **Backend API** (from the repository root):
   ```powershell
   cd ESBot.API
   $env:ASPNETCORE_ENVIRONMENT = "Development"
   dotnet run
   ```
   Confirm it is healthy before continuing:
   ```powershell
   curl http://localhost:5243/api/v1/health
   ```
3. **Frontend** (static file server, from `frontend/`):
   ```powershell
   cd frontend
   python -m http.server 3000
   ```
   `cypress.config.js` sets `baseUrl: 'http://localhost:3000'`, so `cy.visit('/')` resolves
   against this server. The backend base URL used by the page under test
   (`http://localhost:5243`) can be overridden with the `API_BASE_URL` Cypress env var if
   the API runs on a different port:
   ```powershell
   npx cypress run --env API_BASE_URL=http://localhost:5243
   ```

## Run commands

| Mode | Command | Notes |
|---|---|---|
| Headless (CI-style) | `npm test` (`cypress run`) | Runs in the headless Electron browser; used by `npm run test:ci`. |
| Headless, explicit | `npm run test:ci` (`cypress run --headless`) | Same as above, explicit flag. |
| Interactive / headed | `npm run test:headed` (`cypress open`) | Opens the Cypress Test Runner UI; pick the Electron/Chrome browser and click the spec to watch it run and to time-travel through each command. |

All commands must be run from `frontend/` with the database, API, and frontend already
running as described above. `cypress.config.js` disables video recording
(`video: false`) but keeps `screenshotOnRunFailure: true`, so a screenshot is written to
`frontend/cypress/screenshots/` automatically whenever a test fails.

## Test coverage

`user-session.cy.js` maps to the "user creation → session creation" flow described in
[`ESBot.Tests/features/UserAuthentication.feature`](../../ESBot.Tests/features/UserAuthentication.feature)
and covers:

1. **Happy path (complete flow):** ping the API health check, create a user with unique
   generated credentials, create a session for that user, and verify the session appears
   in the session list and the chat section becomes visible.
2. **Negative scenario:** attempting to create a session before a user exists is rejected
   with the expected inline error message.
3. **Negative scenario:** submitting the user-creation form with empty fields, and again
   with a too-short password, surfaces the expected validation error messages instead of
   silently failing or creating an invalid user.

All selectors used are the frontend's existing stable element `id`s (e.g. `#username`,
`#createUserButton`, `#sessionStatus`) rather than CSS classes or positional/XPath
selectors, so the suite does not depend on layout or styling changes. See
[`docs/ui/e2e-report.md`](e2e-report.md) for actual run output and current pass/fail
status.
