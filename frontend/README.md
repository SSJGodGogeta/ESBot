# ESBot Frontend

A lightweight frontend for the ESBot API. It supports:

- Creating a user
- Creating a session for the user
- Sending chat messages for a session
- Listing sessions for the current user

## Run locally

From the `frontend` folder, start a local static server.

Using Python:

```powershell
cd frontend
python -m http.server 3000
```

If you prefer Node.js and have `npx` available:

```powershell
cd frontend
npx serve . -l 3000
```

Then open:

```text
http://localhost:3000
```

## API URL

The page includes a backend base URL field. Use the URL where the API is running, for example:

```text
http://localhost:5243
```
