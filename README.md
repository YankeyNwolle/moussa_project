# Helpdesk GROUPE CHK

Système de gestion de tickets Full-Stack — ASP.NET Core 8 + React + SQL Server.

---

## Structure du projet

```
helpdesk/
├── backend/HelpdeskAPI/   ← API REST (C#, EF Core, JWT)
├── frontend/              ← Application React (Vite + Tailwind)
└── database/helpdesk.sql  ← Script de création de la base SQL Server
```

---

## 🗄️ Base de données

1. Ouvrir **SQL Server Management Studio (SSMS)**.
2. Exécuter le fichier `database/helpdesk.sql`.  
   ➜ Crée la base `HelpdeskDB` avec toutes les tables + données initiales.

---

## ⚙️ Backend (ASP.NET Core 8)

### Prérequis
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Configuration
Modifier `backend/HelpdeskAPI/appsettings.json` :
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=<TON_SERVEUR>;Database=HelpdeskDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Démarrage
```bash
cd backend/HelpdeskAPI
dotnet restore
dotnet run
```
API disponible sur **http://localhost:5001**

### Endpoints principaux
| Méthode | Route | Auth | Description |
|---------|-------|------|-------------|
| POST | `/api/auth/register-client` | Public | Inscription client |
| POST | `/api/auth/login-client` | Public | Connexion client |
| POST | `/api/auth/login-agent` | Public | Connexion agent |
| GET | `/api/referentiels` | Public | Catégories & priorités |
| GET | `/api/tickets` | JWT | Liste tickets (filtrée par rôle) |
| POST | `/api/tickets` | Client JWT | Créer un ticket |
| PUT | `/api/tickets/{id}/status` | Agent JWT | Changer le statut |
| GET | `/api/tickets/{id}/history` | JWT | Historique (CHANGER) |
| GET | `/api/tickets/{id}/messages` | JWT | Messages du ticket |
| POST | `/api/tickets/{id}/messages` | JWT | Envoyer un message + pièce jointe |
| GET | `/api/agent/dashboard` | Agent JWT | Stats tableau de bord |
| GET | `/api/agent/tickets` | Agent JWT | Tickets filtrés |

---

## 🌐 Frontend (React + Vite)

### Prérequis
- [Node.js](https://nodejs.org) 18+

### Démarrage
```bash
cd frontend
npm install    # si pas encore fait
npm run dev
```
App disponible sur **http://localhost:5173**

---

## 🔐 Comptes de test

Après exécution du script SQL, des agents de démonstration sont créés.  
Définir leurs vrais mots de passe BCrypt en base, ou créer un nouveau client via `/register`.

**Cycle de vie d'un ticket :**  
`Initialisé` → `En cours` → `Résolu` → `Fermé`

Les changements de statut sont automatiquement historisés dans la table `CHANGER`.

---

## 🔒 Sécurité

- **BCrypt** pour le hachage des mots de passe
- **JWT HS256** (8h d'expiration) pour l'authentification
- **CORS** limité à `http://localhost:5173`
- Routes protégées par rôle : `Client` vs `Agent/Superviseur`
