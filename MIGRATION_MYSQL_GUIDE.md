# Guide de Migration vers MySQL - Base de données "warehouse"

## ✅ Modifications Effectuées

1. ✅ Packages NuGet mis à jour : `Pomelo.EntityFrameworkCore.MySql` 8.0.2
2. ✅ Chaînes de connexion mises à jour pour MySQL
3. ✅ Configuration DbContext mise à jour pour MySQL
4. ✅ Entity Framework Core mis à jour à la version 8.0.2

## 📋 Configuration Actuelle

**Chaîne de connexion MySQL** (dans `appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=warehouse;User=root;Password=;Port=3306;"
  }
}
```

⚠️ **IMPORTANT**: Si votre MySQL a un mot de passe pour l'utilisateur `root`, modifiez la chaîne de connexion :
```json
"DefaultConnection": "Server=localhost;Database=warehouse;User=root;Password=VOTRE_MOT_DE_PASSE;Port=3306;"
```

## 🚀 Étapes pour Finaliser la Migration

### Option 1 : Création Automatique (Recommandé pour Démarrage Rapide)

L'application créera automatiquement les tables lors du premier lancement grâce à `EnsureCreatedAsync()`.

**Étapes :**

1. **Vérifiez que MySQL est démarré** :
   - Vérifiez que le service MySQL est en cours d'exécution
   - Vérifiez que la base de données `warehouse` existe (vous l'avez déjà créée dans phpMyAdmin)

2. **Mettez à jour le mot de passe si nécessaire** :
   - Ouvrez `Wms.ASP/appsettings.json`
   - Ouvrez `Warehouse Management System/appsettings.json`
   - Modifiez `Password=` avec votre mot de passe MySQL

3. **Lancez l'application** :
   ```bash
   # Pour l'application Web
   cd Wms.ASP
   dotnet run
   
   # OU pour l'application WinForms
   cd "Warehouse Management System"
   dotnet run
   ```

4. **Vérifiez dans phpMyAdmin** :
   - Rafraîchissez phpMyAdmin
   - Vous devriez voir toutes les tables créées dans la base `warehouse`

### Option 2 : Utilisation des Migrations EF Core (Recommandé pour Production)

Si vous préférez utiliser les migrations EF Core :

1. **Installez les outils EF Core** :
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. **Créez la migration initiale** :
   ```bash
   cd Wms.ASP
   dotnet ef migrations add InitialMySqlMigration --project ../Wms.Infrastructure --startup-project .
   ```

3. **Appliquez la migration** :
   ```bash
   dotnet ef database update --project ../Wms.Infrastructure --startup-project .
   ```

## 📊 Tables qui seront créées

L'application créera automatiquement les tables suivantes dans votre base `warehouse` :

- **Items** - Articles/Produits
- **Warehouses** - Entrepôts
- **Locations** - Emplacements de stockage
- **Lots** - Lots/Batch
- **Stock** - Niveaux de stock
- **Movements** - Historique des mouvements

## 🔍 Vérification

Après le premier lancement :

1. **Ouvrez phpMyAdmin**
2. **Sélectionnez la base `warehouse`**
3. **Vérifiez que les tables sont créées** dans l'onglet "Structure"
4. **Vérifiez que les données initiales sont insérées** dans l'onglet "SQL" :
   ```sql
   SELECT * FROM Items;
   SELECT * FROM Locations;
   SELECT * FROM Warehouses;
   ```

## ⚙️ Configuration Avancée

### Changer le Port MySQL

Si MySQL n'utilise pas le port 3306 par défaut :
```json
"DefaultConnection": "Server=localhost;Database=warehouse;User=root;Password=;Port=3307;"
```

### Utiliser un Utilisateur Différent

```json
"DefaultConnection": "Server=localhost;Database=warehouse;User=wms_user;Password=password123;Port=3306;"
```

### Connexion à un Serveur Distant

```json
"DefaultConnection": "Server=192.168.1.100;Database=warehouse;User=wms_user;Password=password123;Port=3306;"
```

## 🐛 Dépannage

### Erreur : "Unable to connect to any of the specified MySQL hosts"

**Solution** :
- Vérifiez que MySQL est démarré
- Vérifiez que le port est correct (3306 par défaut)
- Vérifiez les paramètres de connexion dans `appsettings.json`

### Erreur : "Access denied for user 'root'@'localhost'"

**Solution** :
- Vérifiez le mot de passe dans la chaîne de connexion
- Ou créez un utilisateur MySQL dédié :
  ```sql
  CREATE USER 'wms_user'@'localhost' IDENTIFIED BY 'votre_mot_de_passe';
  GRANT ALL PRIVILEGES ON warehouse.* TO 'wms_user'@'localhost';
  FLUSH PRIVILEGES;
  ```

### Erreur : "Unknown database 'warehouse'"

**Solution** :
- La base de données doit exister avant de lancer l'application
- Créez-la dans phpMyAdmin ou via MySQL :
  ```sql
  CREATE DATABASE warehouse CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
  ```

### Les tables ne sont pas créées

**Solution** :
- Vérifiez les logs de l'application pour les erreurs
- Vérifiez que l'utilisateur MySQL a les permissions nécessaires
- Essayez de créer manuellement une table pour tester les permissions

## 📝 Notes Importantes

1. **La base de données `warehouse` doit exister** avant le premier lancement
2. **Les données initiales** (sample data) seront automatiquement insérées au premier lancement
3. **Les migrations futures** peuvent être créées avec `dotnet ef migrations add`
4. **Pour la production**, utilisez un utilisateur MySQL dédié avec des permissions limitées

## ✅ Prochaines Étapes

1. ✅ Modifiez le mot de passe dans `appsettings.json` si nécessaire
2. ✅ Lancez l'application : `dotnet run`
3. ✅ Vérifiez dans phpMyAdmin que les tables sont créées
4. ✅ Testez l'application avec les données initiales

---

**Tout est prêt !** Il ne reste plus qu'à lancer l'application et vérifier dans phpMyAdmin que tout fonctionne correctement.

