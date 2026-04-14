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

```bash
dotnet build
```


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
http://localhost:5000/api/test
```


## 10. Häufige Probleme

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

## 11. Projektstruktur

```
ESBot/
│
├── ESBot.API
├── ESBot.Domain
├── ESBot.Infrastructure
├── ESBot.Tests
│
└── README.md
```