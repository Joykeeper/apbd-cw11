IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Doctor] (
    [IdDoctor] int NOT NULL IDENTITY,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Doctor] PRIMARY KEY ([IdDoctor])
);

CREATE TABLE [Medicament] (
    [IdMedicament] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    [Type] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Medicament] PRIMARY KEY ([IdMedicament])
);

CREATE TABLE [Patient] (
    [IdPatient] int NOT NULL IDENTITY,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Birthdate] datetime2 NOT NULL,
    CONSTRAINT [PK_Patient] PRIMARY KEY ([IdPatient])
);

CREATE TABLE [Prescription] (
    [IdPrescription] int NOT NULL IDENTITY,
    [Date] datetime2 NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [IdPatient] int NOT NULL,
    [IdDoctor] int NOT NULL,
    CONSTRAINT [PK_Prescription] PRIMARY KEY ([IdPrescription]),
    CONSTRAINT [FK_Prescription_Doctor_IdDoctor] FOREIGN KEY ([IdDoctor]) REFERENCES [Doctor] ([IdDoctor]) ON DELETE CASCADE,
    CONSTRAINT [FK_Prescription_Patient_IdPatient] FOREIGN KEY ([IdPatient]) REFERENCES [Patient] ([IdPatient]) ON DELETE CASCADE
);

CREATE TABLE [Prescription_Medicament] (
    [IdMedicament] int NOT NULL,
    [IdPrescription] int NOT NULL,
    [Dose] int NOT NULL,
    [Details] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Prescription_Medicament] PRIMARY KEY ([IdMedicament], [IdPrescription]),
    CONSTRAINT [FK_Prescription_Medicament_Medicament_IdMedicament] FOREIGN KEY ([IdMedicament]) REFERENCES [Medicament] ([IdMedicament]) ON DELETE CASCADE,
    CONSTRAINT [FK_Prescription_Medicament_Prescription_IdPrescription] FOREIGN KEY ([IdPrescription]) REFERENCES [Prescription] ([IdPrescription]) ON DELETE CASCADE
);

CREATE INDEX [IX_Prescription_IdDoctor] ON [Prescription] ([IdDoctor]);

CREATE INDEX [IX_Prescription_IdPatient] ON [Prescription] ([IdPatient]);

CREATE INDEX [IX_Prescription_Medicament_IdPrescription] ON [Prescription_Medicament] ([IdPrescription]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250519182852_InitialCreate', N'9.0.5');

COMMIT;
GO

