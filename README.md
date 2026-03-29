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
API disponible sur **http://localhost:5000**

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

## 🐳 Exécution avec Docker (recommandé pour une autre machine)

### Prérequis
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installé

### 1) Cloner le projet
```bash
git clone <URL_DU_REPO>
cd helpdesk
```

### 2) Préparer les variables d'environnement
Copier le fichier d'exemple puis ajuster les valeurs :
```bash
cp .env.example .env
```

Dans `.env` :
- `SQL_SA_PASSWORD` : mot de passe SQL Server fort (obligatoire)
- `JWT_KEY` : clé secrète JWT

### 3) Lancer les conteneurs
```bash
docker compose up --build -d
```

Services exposés :
- Frontend : `http://localhost:5173`
- Backend API : `http://localhost:5000`
- SQL Server : `localhost,1433`

### 4) Initialiser la base de données
Le conteneur SQL ne lance pas automatiquement le script `database/helpdesk.sql`.

Après le démarrage de Docker, exécuter le script via SSMS :
1. Se connecter à `localhost,1433` avec login `sa` et le mot de passe `SQL_SA_PASSWORD`
2. Ouvrir `database/helpdesk.sql`
3. Exécuter le script

### 5) Vérifier l'application
- Ouvrir `http://localhost:5173`
- API santé simple : `http://localhost:5000/api/referentiels`

### Commandes utiles
```bash
# Voir les logs
docker compose logs -f backend
docker compose logs -f frontend
docker compose logs -f sqlserver

# Arrêter les conteneurs
docker compose down

# Arrêter + supprimer les volumes (reset complet DB)
docker compose down -v
```

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
