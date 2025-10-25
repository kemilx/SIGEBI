/*
    Script de creación de la base de datos SIGEBI para Microsoft SQL Server.
    Generado a partir del modelo de dominio e infraestructura del proyecto.
*/

IF DB_ID(N'SIGEBI') IS NULL
BEGIN
    CREATE DATABASE [SIGEBI];
END
GO

USE [SIGEBI];
GO

-- Eliminación preventiva de tablas existentes en orden de dependencias
IF OBJECT_ID(N'dbo.UsuarioRoles', N'U') IS NOT NULL DROP TABLE dbo.UsuarioRoles;
IF OBJECT_ID(N'dbo.Penalizaciones', N'U') IS NOT NULL DROP TABLE dbo.Penalizaciones;
IF OBJECT_ID(N'dbo.Notificaciones', N'U') IS NOT NULL DROP TABLE dbo.Notificaciones;
IF OBJECT_ID(N'dbo.Prestamos', N'U') IS NOT NULL DROP TABLE dbo.Prestamos;
IF OBJECT_ID(N'dbo.Libros', N'U') IS NOT NULL DROP TABLE dbo.Libros;
IF OBJECT_ID(N'dbo.Configuraciones', N'U') IS NOT NULL DROP TABLE dbo.Configuraciones;
IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NOT NULL DROP TABLE dbo.Usuarios;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

-- ========================================
--  Tabla: Roles
-- ========================================
CREATE TABLE dbo.Roles
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Roles_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc DATETIME2(7) NULL
);
GO

CREATE UNIQUE INDEX IX_Roles_Nombre ON dbo.Roles(Nombre);
GO

-- ========================================
--  Tabla: Usuarios
-- ========================================
CREATE TABLE dbo.Usuarios
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(120) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Tipo INT NOT NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
    CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Usuarios_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc DATETIME2(7) NULL
);
GO

CREATE UNIQUE INDEX IX_Usuarios_Email ON dbo.Usuarios(Email);
GO

-- ========================================
--  Tabla intermedia: UsuarioRoles (muchos-a-muchos)
-- ========================================
CREATE TABLE dbo.UsuarioRoles
(
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    RolId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_UsuarioRoles PRIMARY KEY (UsuarioId, RolId),
    CONSTRAINT FK_UsuarioRoles_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_UsuarioRoles_Roles FOREIGN KEY (RolId) REFERENCES dbo.Roles(Id) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_UsuarioRoles_RolId ON dbo.UsuarioRoles(RolId);
GO

CREATE INDEX IX_UsuarioRoles_UsuarioId ON dbo.UsuarioRoles(UsuarioId);
GO

-- ========================================
--  Tabla: Libros
-- ========================================
CREATE TABLE dbo.Libros
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Libros PRIMARY KEY,
    Titulo NVARCHAR(250) NOT NULL,
    Autor NVARCHAR(200) NOT NULL,
    Isbn NVARCHAR(40) NULL,
    Ubicacion NVARCHAR(100) NULL,
    EjemplaresTotales INT NOT NULL,
    EjemplaresDisponibles INT NOT NULL,
    Estado INT NOT NULL,
    FechaPublicacion DATETIME2(7) NULL,
    CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Libros_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc DATETIME2(7) NULL
);
GO

CREATE INDEX IX_Libros_Titulo ON dbo.Libros(Titulo);
GO

CREATE INDEX IX_Libros_Autor ON dbo.Libros(Autor);
GO

CREATE INDEX IX_Libros_Estado ON dbo.Libros(Estado);
GO

CREATE UNIQUE INDEX IX_Libros_Isbn ON dbo.Libros(Isbn) WHERE Isbn IS NOT NULL;
GO

-- ========================================
--  Tabla: Prestamos
-- ========================================
CREATE TABLE dbo.Prestamos
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Prestamos PRIMARY KEY,
    LibroId UNIQUEIDENTIFIER NOT NULL,
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    Estado INT NOT NULL CONSTRAINT DF_Prestamos_Estado DEFAULT (1),
    Observaciones NVARCHAR(500) NULL,
    FechaEntregaRealUtc DATETIME2(7) NULL,
    FechaInicioUtc DATETIME2(7) NOT NULL,
    FechaFinCompromisoUtc DATETIME2(7) NOT NULL,
    CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Prestamos_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc DATETIME2(7) NULL,
    CONSTRAINT FK_Prestamos_Libros FOREIGN KEY (LibroId) REFERENCES dbo.Libros(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Prestamos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Prestamos_UsuarioId ON dbo.Prestamos(UsuarioId);
GO

CREATE INDEX IX_Prestamos_LibroId ON dbo.Prestamos(LibroId);
GO

CREATE INDEX IX_Prestamos_Estado ON dbo.Prestamos(Estado);
GO

-- ========================================
--  Tabla: Penalizaciones
-- ========================================
CREATE TABLE dbo.Penalizaciones
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Penalizaciones PRIMARY KEY,
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    PrestamoId UNIQUEIDENTIFIER NULL,
    Monto DECIMAL(12,2) NOT NULL,
    FechaInicioUtc DATETIME2(7) NOT NULL,
    FechaFinUtc DATETIME2(7) NOT NULL,
    Motivo NVARCHAR(500) NOT NULL,
    Activa BIT NOT NULL CONSTRAINT DF_Penalizaciones_Activa DEFAULT (1),
    CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Penalizaciones_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc DATETIME2(7) NULL,
    CONSTRAINT FK_Penalizaciones_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Penalizaciones_Prestamos FOREIGN KEY (PrestamoId) REFERENCES dbo.Prestamos(Id) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Penalizaciones_UsuarioId ON dbo.Penalizaciones(UsuarioId);
GO

CREATE INDEX IX_Penalizaciones_Activa ON dbo.Penalizaciones(Activa);
GO

-- ========================================
--  Tabla: Notificaciones
-- ========================================
CREATE TABLE dbo.Notificaciones
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Notificaciones PRIMARY KEY,
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    Titulo NVARCHAR(150) NOT NULL,
    Mensaje NVARCHAR(1000) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL,
    Leida BIT NOT NULL CONSTRAINT DF_Notificaciones_Leida DEFAULT (0),
    FechaLecturaUtc DATETIME2(7) NULL,
    CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Notificaciones_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc DATETIME2(7) NULL,
    CONSTRAINT FK_Notificaciones_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id) ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Notificaciones_UsuarioId ON dbo.Notificaciones(UsuarioId);
GO

CREATE INDEX IX_Notificaciones_Leida ON dbo.Notificaciones(Leida);
GO

CREATE INDEX IX_Notificaciones_Tipo ON dbo.Notificaciones(Tipo);
GO

-- ========================================
--  Tabla: Configuraciones
-- ========================================
CREATE TABLE dbo.Configuraciones
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Configuraciones PRIMARY KEY,
    Clave NVARCHAR(120) NOT NULL,
    Valor NVARCHAR(500) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Configuraciones_Activo DEFAULT (1),
    CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Configuraciones_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc DATETIME2(7) NULL
);
GO

CREATE UNIQUE INDEX IX_Configuraciones_Clave ON dbo.Configuraciones(Clave);
GO

CREATE INDEX IX_Configuraciones_Activo ON dbo.Configuraciones(Activo);
GO

-- ========================================
--  Datos semilla opcionales (roles base)
-- ========================================
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Nombre = N'Bibliotecario')
BEGIN
    INSERT INTO dbo.Roles (Id, Nombre, Descripcion)
    VALUES (NEWID(), N'Bibliotecario', N'Rol para el personal de biblioteca');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Nombre = N'Administrador')
BEGIN
    INSERT INTO dbo.Roles (Id, Nombre, Descripcion)
    VALUES (NEWID(), N'Administrador', N'Rol administrativo del sistema');
END
GO

PRINT 'Esquema SIGEBI creado correctamente.';
GO
