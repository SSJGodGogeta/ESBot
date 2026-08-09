# E2E Testing Setup

This document covers the end-to-end (E2E) testing setup for the ESBot Frontend.

## Framework

This project uses **Cypress** (version 15.18.0) for E2E testing. Cypress was chosen for its:

- Easy setup and configuration
- Excellent developer experience with interactive test runner
- Built-in assertions and waiting mechanisms
- Real browser testing capabilities
- Screenshot and video recording on failures

## Installation

Install Cypress as a dev dependency:

```bash
npm install --save-dev cypress
```

Or if you're setting up the project for the first time:

```bash
npm install
```

## Configuration

The Cypress configuration is defined in `cypress.config.js`:

- **Base URL**: `http://localhost:3000` (the frontend application)
- **Test type**: E2E tests
- **Video recording**: Disabled (to save disk space)
- **Screenshots**: Enabled on test failures

## Running Tests

### Prerequisites

Before running the E2E tests, you need to start both the backend API and the frontend application:

#### 1. Start the Backend API

Ensure the ESBot backend API is running on `http://localhost:5243`.

Refer to the backend documentation for startup instructions.

#### 2. Start the Frontend Application

From the `frontend` folder, start a local static server:

**Using Python:**

```bash
cd frontend
python -m http.server 3000
```

**Using Node.js (npx):**

```bash
cd frontend
npx serve . -l 3000
```

The frontend should now be accessible at `http://localhost:3000`.

### Running the Tests

Once both the backend and frontend are running, execute the tests:

**Headless mode (CI/production):**

```bash
npm test
```

or

```bash
npx cypress run
```

**Interactive mode (development):**

```bash
npm run test:headed
```

or

```bash
npx cypress open
```

**Headless mode (explicit):**

```bash
npm run test:ci
```

## Test Structure

Tests are located in `cypress/e2e/`:

- `user-session.cy.js` - Tests for user creation, session creation, and chat functionality

## Environment Variables

The tests use the following default values:

- **Frontend URL**: `http://localhost:3000` (configured in `cypress.config.js`)
- **Backend API URL**: `http://localhost:5243` (can be overridden via `API_BASE_URL` environment variable)

To override the API base URL:

```bash
CYPRESS_API_BASE_URL=http://localhost:5000 npx cypress run
```

## NixOS Users

If you're running NixOS, you may encounter issues running `npm test` or `npx cypress` commands due to library dependencies. In this case, use the Cypress binary directly:

```bash
cypress run
```

For interactive mode:

```bash
cypress open
```

Make sure Cypress is installed in your Nix environment or available in your system path.

## Troubleshooting

### Tests fail with "API is not reachable"

- Ensure the backend API is running on `http://localhost:5243`
- Check that the `/api/v1/health` endpoint responds successfully

### Tests fail with "Cannot find element"

- Ensure the frontend is running on `http://localhost:3000`
- Check browser console for JavaScript errors
- Verify the frontend files are being served correctly

### Cypress fails to start (missing libraries)

On Linux systems, Cypress may require additional system dependencies. Refer to the [Cypress documentation on system requirements](https://on.cypress.io/required-dependencies).

## Package Scripts

| Command | Description |
|---------|-------------|
| `npm test` | Run tests in headless mode |
| `npm run test:headed` | Open Cypress interactive test runner |
| `npm run test:ci` | Run tests in headless mode (CI) |
