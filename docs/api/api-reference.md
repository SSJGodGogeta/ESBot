# ESBot API — Referenz

- Basis-URL und notwendige Umgebungsvariablen
- Endpoint-Tabelle für alle implementierten Ressourcen (Methode, Pfad, Beschreibung, Header, Body-Schema, Erfolgscode, Beispielantwort)
- Fehlerantworten (Beispiele für 404, 422, 500)
- Setup-Anleitung für einen frischen Checkout (Abhängigkeiten installieren, Umgebungsvariablen setzen, Backend starten, Health-Check prüfen)

Hinweis: Dieses Dokument beschreibt die aktuell im Repository implementierten Controller in `ESBot.API/Controllers/v1`. Die Controller verwenden die Routenbasis `/v1/[controller]` (z. B. `/v1/Users`).

## Basis-URL

- Lokal: http://localhost:5243
- Swagger UI: http://localhost:5243/swagger/index.html

Die genaue URL kann je nach Umgebung und Launch-Settings variieren. Beim lokalen Start mit `dotnet run` werden in der Regel die oben genannten Ports verwendet.

## Wichtige "Umgebungsvariablen" (appsettings.json)

- ConnectionStrings__DefaultConnection — (erforderlich) PostgreSQL-Verbindungszeichenfolge. Beispiel:
  - `Host=localhost;Port=5432;Database=esbot;Username=postgres;Password=postgres`

## Endpoints — Übersicht

Alle im Projekt vorhandenen v1-Controller folgen dem Pattern `/v1/{resource}`. Für die meisten Ressourcen sind die CRUD-ähnlichen Endpunkte implementiert: Filter (GET mit Query), Create (POST), Delete (DELETE mit Query `id`), Update (PUT mit Query `id` und Body).

Die folgenden Ressourcen sind implementiert (jeweils Controller im Verzeichnis `ESBot.API/Controllers/v1`):


- EvaluationResults (/v1/EvaluationResults)
- Messages (/v1/Messages)
- QuizItems (/v1/QuizItems)
- QuizRequests (/v1/QuizRequests)
- Sessions (/v1/Sessions)
- SubmittedAnswers (/v1/SubmittedAnswers)
- Users (/v1/Users)

Für jede Ressource gelten die folgenden Endpoints:

- GET /v1/{Resource}
  - Beschreibung: Filtern / Auflisten von Entitäten. Die Filter-Parameter werden als Query-Parameter entgegengenommen (je Ressource gibt es einen Filter-DTO, z. B. `UserFilter`).
  - Erforderliche Header: `Accept: application/json`
  - Request Body: N/A
  - Erfolgsstatus: 200 OK
  - Beispielantwort (Array):

```json
[
  {
    "id": "4b2a4dcf-3a47-4a40-a7db-9e6d8af95939",
    "username": "alice",
    "email": "alice@esbot.com",
    "hashedPassword": "hashed1",
    "createdAt": "2026-04-15T09:36:11.897619Z",
    "sessions": []
  },
  {
    "id": "3adb43bc-570d-40b9-bd6b-a0ada7ce6523",
    "username": "bob",
    "email": "bob@esbot.com",
    "hashedPassword": "hashed2",
    "createdAt": "2026-04-15T09:36:11.897804Z",
    "sessions": []
  }
]
```

- POST /v1/{Resource}
  - Beschreibung: Erstellen einer neuen Entität. Body enthält die Entität als JSON.
  - Erforderliche Header: `Content-Type: application/json`, `Accept: application/json`
  - Request Body: JSON-Repräsentation der Entität (siehe Beispiel unten)
  - Erfolgsstatus: 201 Created
  - Beispiel Request (User):

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "username": "string",
  "email": "user@example.com",
  "hashedPassword": "stringst"
}
```

  - Beispielantwort (erstellte Entität):

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "username": "string",
  "email": "user@example.com",
  "hashedPassword": "stringst",
  "createdAt": "2026-06-16T12:00:00Z"
}
```

- DELETE /v1/{Resource}
  - Beschreibung: Löschen einer Entität. Der Parameter `id` wird als Query-Parameter erwartet (Controller-Methode erwartet `Guid id`).
  - Beispiel: `DELETE /v1/Users?id=11111111-2222-3333-4444-555555555555`
  - Erforderliche Header: `Accept: application/json`
  - Request Body: N/A
  - Erfolgsstatus: 200 OK
  - Beispielantwort: Deleted entity with ID: {id}

- PUT /v1/{Resource}
  - Beschreibung: Update einer existierenden Entität. `id` als Query-Parameter, Body enthält die neue Entität im JSON-Format.
  - Beispiel: `PUT /v1/Users?id=11111111-2222-3333-4444-555555555555`
  - Erforderliche Header: `Content-Type: application/json`, `Accept: application/json`
  - Request Body: JSON-Repräsentation der Entität mit den zu setzenden Feldern
  - Erfolgsstatus: 200 OK
  - Beispielantwort (aktualisierte Entität):

```json
{
  "id": "11111111-2222-3333-4444-555555555555",
  "name": "Alice Updated",
  "email": "alice@example.com"
}
```

### Anmerkung zu Pfaden und Binding

Die Controller verwenden die Klassen-Route `[Route("/v1/[controller]")]`. Die Methoden `Delete(Guid id)` und `Update(Guid id, ...)` deklarieren das `id`-Argument ohne Route-Template. ASP.NET Core bindet primitive Parameter standardmäßig aus Route, Form oder Query — in der Praxis muss das `id` daher als Query-Parameter übergeben werden, z. B. `?id=...`.

## Fehlerantworten — Beispiele

- 404 Not Found

```json
{
  "status": 404,
  "error": "Not Found",
  "message": "{typeof(TEntity).Name} with ID {id} not found."
}
```

- 422 Unprocessable Entity (z. B. ModelState invalid, fehlende Pflichtfelder)

```json
{
  "status": 422,
  "error": "Unprocessable Entity",
  "message": "JSON-Schema validierungsfehler",
  "details": [
    {
      "field": "email",
      "error": "Das Feld 'email' ist erforderlich."
    }
  ]
}
```

- 500 Internal Server Error

```json
{
  "status": 500,
  "error": "Internal Server Error",
  "message": "Could not create/update/delete entity of type {typeof(TEntity).Name}\nAn Exception occurred: Type - {e.GetType()}, Message - {e.Message}"
}
```


## Setup-Anleitung (frischer Checkout)

1) Repository klonen

```powershell
git clone https://github.com/SSJGodGogeta/ESBot/
cd ESBot/ESBot
```

2) .NET SDK installieren

- Stellen Sie sicher, dass .NET SDK (z. B. 6.0/7.0, je nach Projekt-Target) installiert ist. Prüfen mit:

```powershell
dotnet --info
```

3) Abhängigkeiten wiederherstellen und bauen

```powershell
dotnet restore
dotnet build --configuration Debug
```

4) Backend starten

```powershell
dotnet run --project ESBot.API
```

5) Health-Check prüfen

```powershell
Invoke-RestMethod -Method Get -Uri "http://localhost:5243/api/v1/health" | ConvertTo-Json
```
oder im Browser: http://localhost:5243/api/v1/health

Erwartete Ausgabe: 

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.2246175",
  "entries": {
    "database": {
      "data": {

      },
      "duration": "00:00:00.2024769",
      "status": "Healthy",
      "tags": []
    },
    "postgres": {
      "data": {

      },
      "duration": "00:00:00.0396389",
      "status": "Healthy",
      "tags": []
    }
  }
}
```

7) API testen via Swagger

Öffne http://localhost:5243/swagger/index.html im Browser — dort sind die Endpoints interaktiv dokumentiert und testbar.