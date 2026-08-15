-- Crear esquema
CREATE SCHEMA IF NOT EXISTS seguridad;
-- ============================================================================
-- Service.Seguridad - Script de Creación de Esquema y Tablas
-- Motor: PostgreSQL
-- ============================================================================

-- ============================================================================
-- Tabla: usuario
-- ============================================================================
CREATE TABLE seguridad.usuario (
    id_usuario         UUID           NOT NULL,
    correo             VARCHAR(150)   NOT NULL,
    nombres            VARCHAR(100)   NOT NULL,
    apellidos          VARCHAR(100)   NOT NULL,
    metodo_registro    VARCHAR(20)    NOT NULL,
    contrasena_hash    VARCHAR(255)   NULL,
    google_id          VARCHAR(255)   NULL,
    correo_verificado  BOOLEAN        NOT NULL DEFAULT FALSE,
    token_verificacion VARCHAR(255)   NULL,
    expira_token       TIMESTAMP      NULL,
    ultimo_acceso      TIMESTAMP      NULL,
    creado_por         VARCHAR(150)   NULL,
    fecha_creacion     TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modificado_por     VARCHAR(150)   NULL,
    fecha_modificacion TIMESTAMP      NULL,
    activo             BOOLEAN        NOT NULL DEFAULT TRUE,
    CONSTRAINT pk_usuario PRIMARY KEY (id_usuario)
);

-- Índices únicos
CREATE UNIQUE INDEX ux_usuario_correo ON seguridad.usuario (correo);
CREATE UNIQUE INDEX ux_usuario_google_id ON seguridad.usuario (google_id);

-- ============================================================================
-- Tabla: token_refresco
-- ============================================================================
CREATE TABLE seguridad.token_refresco (
    id_token           UUID           NOT NULL,
    id_usuario         UUID           NOT NULL,
    token              VARCHAR(255)   NOT NULL,
    fecha_creacion     TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_expiracion   TIMESTAMP      NOT NULL,
    es_revocado        BOOLEAN        NOT NULL DEFAULT FALSE,
    ip_origen          VARCHAR(45)    NULL,
    agente_usuario     VARCHAR(255)   NULL,
    creado_por         VARCHAR(150)   NULL,
    modificado_por     VARCHAR(150)   NULL,
    fecha_modificacion TIMESTAMP      NULL,
    activo             BOOLEAN        NOT NULL DEFAULT TRUE,
    CONSTRAINT pk_token_refresco PRIMARY KEY (id_token),
    CONSTRAINT fk_token_refresco_usuario FOREIGN KEY (id_usuario)
        REFERENCES seguridad.usuario (id_usuario)
        ON DELETE CASCADE
);

-- Índices
CREATE UNIQUE INDEX ux_token_refresco_token ON seguridad.token_refresco (token);
CREATE INDEX ix_token_refresco_usuario ON seguridad.token_refresco (id_usuario);
CREATE INDEX ix_token_refresco_expiracion ON seguridad.token_refresco (fecha_expiracion);
