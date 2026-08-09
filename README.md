[![.NET Core Build and Test](https://github.com/SSJGodGogeta/ESBot/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/SSJGodGogeta/ESBot/actions/workflows/dotnet-desktop.yml)

# ESBot Setup Anleitung

## 1. Voraussetzungen

Stelle sicher, dass .NET 9.0 auf deinen lokalen PC installiert ist.

[Download Link](<https://dotnet.microsoft.com/en-us/download/dotnet/9.0>)

Oder installiere [Docker Desktop](<https://docs.docker.com/desktop/>) und führe den unten stehenden Befehl aus.

# Docker Setup

Führe den folgenden Befehl aus, nachdem die Installation von Docker Desktop abgeschlossen ist.

```
docker compose up --build
```

---

# Lokaler Setup 

Prüfen:

```bash
dotnet --version
```

### PostgreSQL (lokal installiert)

Download: [https://www.postgresql.org/download/](https://www.postgresql.org/download/)

Wichtige Einstellungen:

* Host: localhost
* Port: 5432
* Username: postgres
* Password: merken oder setzen


### Weitere Tools

* pgAdmin oder DBeaver (für besseres UI)
* Visual Studio / Rider / VS Code
* Git

## 2. Projekt klonen

```bash
git clone https://github.com/SSJGodGogeta/ESBot.git
cd ESBot
```


## 3. Connection String prüfen

Datei:
`ESBot.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=esbot;Username=postgres;Password=postgres"
  }
}
```

Falls dein Passwort anders ist, entsprechend anpassen.


## 4. Dependencies installieren

```bash
dotnet restore
```


## 5. Projekt bauen

Beim Erstellen des Projekts (Build) werden durch eine MSBuild-Konfiguration automatisch alle Unit-Tests und BDD-Tests (Behavior-Driven Development mit **Reqnroll** / Gherkin) im Hintergrund ausgeführt.

```bash
dotnet build
```

Falls einer der Tests fehlschlägt, bricht der Build-Prozess mit einer Fehlermeldung ab.


## 6. Backend starten

```bash
dotnet run --project ESBot.API
```

## 7. Automatische Migration (im Code)

Im `Program.cs` ist folgende Logik enthalten:

```csharp
db.Database.Migrate();
```

Diese sorgt dafür, dass beim Start:

* Datenbank erstellt wird (falls sie nicht existiert)
* Tabellen erstellt werden
* Migrationen automatisch angewendet werden

## 8. Datenbank Migration manuell ausführen

Falls die Datenbank trotzdem nicht existiert oder leer ist:

```bash
dotnet ef database update --project ESBot.Infrastructure --startup-project ESBot.API
```

Falls dotnet ef nicht installiert ist:

```bash
dotnet tool install --global dotnet-ef
```

## 9. API testen

Browser oder Postman:

```
http://localhost:5243/v1/health
```

## 10. Frontend starten

Nach dem Backend kannst du das Frontend starten.

Aus dem `frontend/` Ordner:

**Mit Python:**

```bash
cd frontend
python -m http.server 3000
```

**Mit Node.js und `npx`:**

```bash
cd frontend
npx serve . -l 3000
```

Dann öffne im Browser:

```
http://localhost:3000
```

### Frontend-Features

- Benutzer erstellen
- Sessions (Lerngruppen) erstellen
- Chat-Nachrichten senden und anzeigen
- Session-Verwaltung

Das Frontend stellt sich automatisch auf die Backend-URL `http://localhost:5243` ein, aber du kannst die URL im Frontend anpassen, falls das Backend auf einer anderen URL erreichbar ist.

## 11. Häufige Probleme

### Problem: Connection refused (5432)

Ursache: PostgreSQL läuft nicht

Lösung:

* PostgreSQL Dienst starten
* oder pgAdmin öffnen


### Problem: falsches Passwort

Lösung:
Connection String in `appsettings.json` anpassen


### Problem: dotnet ef nicht gefunden

```bash
dotnet tool install --global dotnet-ef
```

### Problem: Migration schlägt fehl

```bash
dotnet build
dotnet ef migrations add Fix
dotnet ef database update
```

## 12. Projektstruktur

```
ESBot/
│
├── ESBot.API                 # Backend (ASP.NET Core)
├── ESBot.Domain              # Domain Models und Enums
├── ESBot.Infrastructure       # Database und Repositories
├── ESBot.Application          # Business Logic (ChatService, etc.)
├── ESBot.Tests                # Unit und Integration Tests
│
├── frontend/                  # Frontend (HTML/CSS/JS)
|
└── README.md
```

## 13. Static Code Analysis

### SonarQube

```sh
sonar-scanner
```

oder mit der VSCode Extenstion:

- “Analyze all files”

### Rider (Built-In Tools)

Run coverage via UI:

- “Cover All Tests”
