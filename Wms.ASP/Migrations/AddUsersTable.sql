-- Migration SQL pour ajouter la table Users à la base de données warehouse
-- Exécutez ce script dans phpMyAdmin ou votre client MySQL

CREATE TABLE IF NOT EXISTS `Users` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Username` VARCHAR(50) NOT NULL,
    `Email` VARCHAR(200) NOT NULL,
    `PasswordHash` VARCHAR(255) NOT NULL,
    `FullName` VARCHAR(200) NOT NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `CreatedAt` DATETIME(6) NOT NULL,
    `UpdatedAt` DATETIME(6) NULL,
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `IX_Users_Username` (`Username`),
    UNIQUE INDEX `IX_Users_Email` (`Email`),
    INDEX `IX_Users_IsActive` (`IsActive`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Insérer l'utilisateur admin par défaut
-- Password hash pour "admin123" (SHA256 en Base64)
-- Note: Le hash sera généré automatiquement par l'application lors du seed
-- Si vous voulez l'insérer manuellement, utilisez ce hash:
-- Pour "admin123": jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=
-- Mais il est recommandé de laisser l'application le faire via SeedDefaultUserAsync

