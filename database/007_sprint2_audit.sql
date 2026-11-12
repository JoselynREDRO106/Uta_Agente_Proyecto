USE EduMonitor;
GO

IF OBJECT_ID(N'dbo.AUDIT_LOG', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AUDIT_LOG
    (
        id BIGINT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_AUDIT_LOG PRIMARY KEY,
        fecha_utc DATETIME2 NOT NULL,
        usuario_id NVARCHAR(200) NULL,
        usuario NVARCHAR(150) NOT NULL,
        rol NVARCHAR(40) NOT NULL,
        tipo NVARCHAR(80) NOT NULL,
        curso_moodle_id INT NULL,
        resultado NVARCHAR(20) NOT NULL,
        detalle NVARCHAR(500) NOT NULL,
        CONSTRAINT CK_AUDIT_LOG_resultado
            CHECK (resultado IN (N'Exito', N'Error', N'Denegado'))
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_AUDIT_LOG_FECHA_TIPO'
      AND object_id = OBJECT_ID(N'dbo.AUDIT_LOG')
)
    CREATE INDEX IX_AUDIT_LOG_FECHA_TIPO
        ON dbo.AUDIT_LOG(fecha_utc DESC, tipo);
GO
