-- ============================================================
--  Helpdesk GROUPE CHK  –  Script de création de base de données
--  SQL Server
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HelpdeskDB')
    CREATE DATABASE HelpdeskDB;
GO

USE HelpdeskDB;
GO

-- --------------------------------------------------------
-- ROLE
-- --------------------------------------------------------
CREATE TABLE ROLE (
    id_Rol  INT           NOT NULL IDENTITY(1,1),
    libelle NVARCHAR(50)  NOT NULL,
    CONSTRAINT PK_ROLE PRIMARY KEY (id_Rol)
);

-- --------------------------------------------------------
-- EQUIPE
-- --------------------------------------------------------
CREATE TABLE EQUIPE (
    ref_Equ INT           NOT NULL IDENTITY(1,1),
    libelle NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_EQUIPE PRIMARY KEY (ref_Equ)
);

-- --------------------------------------------------------
-- PRIORITE
-- --------------------------------------------------------
CREATE TABLE PRIORITE (
    id_Pri  INT           NOT NULL IDENTITY(1,1),
    libelle NVARCHAR(50)  NOT NULL,
    niveau  INT           NOT NULL DEFAULT 1, -- 1=Faible, 2=Moyenne, 3=Haute, 4=Critique
    CONSTRAINT PK_PRIORITE PRIMARY KEY (id_Pri)
);

-- --------------------------------------------------------
-- STATUT
-- --------------------------------------------------------
CREATE TABLE STATUT (
    id_Sta  INT           NOT NULL IDENTITY(1,1),
    libelle NVARCHAR(50)  NOT NULL,
    CONSTRAINT PK_STATUT PRIMARY KEY (id_Sta)
);

-- --------------------------------------------------------
-- CATEGORIE
-- --------------------------------------------------------
CREATE TABLE CATEGORIE (
    cod_Cat INT           NOT NULL IDENTITY(1,1),
    libelle NVARCHAR(100) NOT NULL,
    ref_Equ INT           NOT NULL,
    CONSTRAINT PK_CATEGORIE PRIMARY KEY (cod_Cat),
    CONSTRAINT FK_CATEGORIE_EQUIPE FOREIGN KEY (ref_Equ) REFERENCES EQUIPE(ref_Equ)
);

-- --------------------------------------------------------
-- CLIENT
-- --------------------------------------------------------
CREATE TABLE CLIENT (
    id_Cli  INT           NOT NULL IDENTITY(1,1),
    nom     NVARCHAR(100) NOT NULL,
    prenom  NVARCHAR(100) NOT NULL,
    email   NVARCHAR(150) NOT NULL UNIQUE,
    tel     NVARCHAR(20)  NULL,
    login   NVARCHAR(100) NOT NULL UNIQUE,
    mdp     NVARCHAR(255) NOT NULL,  -- BCrypt hash
    CONSTRAINT PK_CLIENT PRIMARY KEY (id_Cli)
);

-- --------------------------------------------------------
-- AGENT
-- --------------------------------------------------------
CREATE TABLE AGENT (
    id_Agt  INT           NOT NULL IDENTITY(1,1),
    nom     NVARCHAR(100) NOT NULL,
    prenom  NVARCHAR(100) NOT NULL,
    email   NVARCHAR(150) NOT NULL UNIQUE,
    login   NVARCHAR(100) NOT NULL UNIQUE,
    mdp     NVARCHAR(255) NOT NULL,  -- BCrypt hash
    ref_Equ INT           NOT NULL,
    id_Rol  INT           NOT NULL,
    CONSTRAINT PK_AGENT PRIMARY KEY (id_Agt),
    CONSTRAINT FK_AGENT_EQUIPE FOREIGN KEY (ref_Equ) REFERENCES EQUIPE(ref_Equ),
    CONSTRAINT FK_AGENT_ROLE  FOREIGN KEY (id_Rol)  REFERENCES ROLE(id_Rol)
);

-- --------------------------------------------------------
-- TICKET
-- --------------------------------------------------------
CREATE TABLE TICKET (
    num_Tic INT            NOT NULL IDENTITY(1,1),
    titre   NVARCHAR(200)  NOT NULL,
    descr   NVARCHAR(MAX)  NOT NULL,
    datecre DATETIME2      NOT NULL DEFAULT GETDATE(),
    id_Cli  INT            NOT NULL,
    cod_Cat INT            NOT NULL,
    id_Pri  INT            NOT NULL,
    id_Sta  INT            NOT NULL,
    CONSTRAINT PK_TICKET   PRIMARY KEY (num_Tic),
    CONSTRAINT FK_TICKET_CLIENT    FOREIGN KEY (id_Cli)  REFERENCES CLIENT(id_Cli),
    CONSTRAINT FK_TICKET_CATEGORIE FOREIGN KEY (cod_Cat) REFERENCES CATEGORIE(cod_Cat),
    CONSTRAINT FK_TICKET_PRIORITE  FOREIGN KEY (id_Pri)  REFERENCES PRIORITE(id_Pri),
    CONSTRAINT FK_TICKET_STATUT    FOREIGN KEY (id_Sta)  REFERENCES STATUT(id_Sta)
);

-- --------------------------------------------------------
-- MESSAGE
-- --------------------------------------------------------
CREATE TABLE MESSAGE (
    cod_Mes INT           NOT NULL IDENTITY(1,1),
    cont    NVARCHAR(MAX) NOT NULL,
    date_Mes DATETIME2   NOT NULL DEFAULT GETDATE(),
    image   NVARCHAR(500) NULL,    -- chemin du fichier joint
    num_Tic INT           NOT NULL,
    id_Cli  INT           NULL,    -- null si envoyé par agent
    id_Agt  INT           NULL,    -- null si envoyé par client
    CONSTRAINT PK_MESSAGE PRIMARY KEY (cod_Mes),
    CONSTRAINT FK_MESSAGE_TICKET FOREIGN KEY (num_Tic) REFERENCES TICKET(num_Tic),
    CONSTRAINT FK_MESSAGE_CLIENT FOREIGN KEY (id_Cli)  REFERENCES CLIENT(id_Cli),
    CONSTRAINT FK_MESSAGE_AGENT  FOREIGN KEY (id_Agt)  REFERENCES AGENT(id_Agt),
    CONSTRAINT CHK_MESSAGE_SENDER CHECK (
        (id_Cli IS NOT NULL AND id_Agt IS NULL) OR
        (id_Agt IS NOT NULL AND id_Cli IS NULL)
    )
);

-- --------------------------------------------------------
-- AFFECTER  (affectation ticket → agent)
-- --------------------------------------------------------
CREATE TABLE AFFECTER (
    num_Tic         INT       NOT NULL,
    id_Agt         INT       NOT NULL,
    date_affectation DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_AFFECTER PRIMARY KEY (num_Tic, id_Agt),
    CONSTRAINT FK_AFFECTER_TICKET FOREIGN KEY (num_Tic) REFERENCES TICKET(num_Tic),
    CONSTRAINT FK_AFFECTER_AGENT  FOREIGN KEY (id_Agt)  REFERENCES AGENT(id_Agt)
);

-- --------------------------------------------------------
-- CHANGER  (historique des changements de statut)
-- --------------------------------------------------------
CREATE TABLE CHANGER (
    id_Cha      INT       NOT NULL IDENTITY(1,1),
    num_Tic     INT       NOT NULL,
    id_Sta_avant INT      NULL,    -- null pour la création
    id_Sta_apres INT      NOT NULL,
    date_change DATETIME2 NOT NULL DEFAULT GETDATE(),
    id_Agt      INT       NULL,    -- null si changement système (création)
    CONSTRAINT PK_CHANGER PRIMARY KEY (id_Cha),
    CONSTRAINT FK_CHANGER_TICKET      FOREIGN KEY (num_Tic)      REFERENCES TICKET(num_Tic),
    CONSTRAINT FK_CHANGER_STATUT_AVANT FOREIGN KEY (id_Sta_avant) REFERENCES STATUT(id_Sta),
    CONSTRAINT FK_CHANGER_STATUT_APRES FOREIGN KEY (id_Sta_apres) REFERENCES STATUT(id_Sta),
    CONSTRAINT FK_CHANGER_AGENT       FOREIGN KEY (id_Agt)        REFERENCES AGENT(id_Agt)
);

-- ============================================================
--  DONNÉES INITIALES (Seed)
-- ============================================================

-- Rôles
INSERT INTO ROLE (libelle) VALUES ('Agent'), ('Superviseur');

-- Priorités
INSERT INTO PRIORITE (libelle, niveau) VALUES
    ('Faible',    1),
    ('Moyenne',   2),
    ('Haute',     3),
    ('Critique',  4);

-- Statuts (cycle de vie du ticket)
INSERT INTO STATUT (libelle) VALUES
    ('Initialisé'),
    ('En cours'),
    ('Résolu'),
    ('Fermé');

-- Équipes
INSERT INTO EQUIPE (libelle) VALUES
    ('Support Informatique'),
    ('Réseau & Infrastructure'),
    ('Ressources Humaines'),
    ('Comptabilité & Finance');

-- Catégories (liées aux équipes)
INSERT INTO CATEGORIE (libelle, ref_Equ) VALUES
    ('Panne matérielle',        1),
    ('Problème logiciel',       1),
    ('Accès & Permissions',     1),
    ('Problème réseau',         2),
    ('VPN & Connexion distante',2),
    ('Demande de congés',       3),
    ('Paie & Salaires',         4);

-- Agents de démonstration (mdp = BCrypt de "Agent123!")
-- IMPORTANT : remplacer les hashes par de vrais hashes BCrypt en production
INSERT INTO AGENT (nom, prenom, email, login, mdp, ref_Equ, id_Rol) VALUES
    ('Dupont',  'Jean',  'jean.dupont@chk.com',  'jdupont',  '$2a$11$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA', 1, 1),
    ('Martin',  'Alice', 'alice.martin@chk.com', 'amartin',  '$2a$11$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA', 2, 1),
    ('Bernard', 'Paul',  'paul.bernard@chk.com', 'pbernard', '$2a$11$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA', 3, 1);

GO
SELECT * FROM AGENT;