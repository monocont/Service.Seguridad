-- Crear esquema
CREATE SCHEMA IF NOT EXISTS seguridad;
-- ============================================================================
-- Service.Seguridad - Script de Creación de Tablas Rol y Usuario_Rol
-- Motor: PostgreSQL
-- ============================================================================

-- ============================================================================
-- Tabla: rol
-- ============================================================================
CREATE TABLE seguridad.rol (
    id_rol             UUID           NOT NULL,
    codigo_rol         VARCHAR(10)    NOT NULL,
    nombre_rol         VARCHAR(50)    NOT NULL,
    creado_por         VARCHAR(150)   NULL,
    fecha_creacion     TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modificado_por     VARCHAR(150)   NULL,
    fecha_modificacion TIMESTAMP      NULL,
    activo             BOOLEAN        NOT NULL DEFAULT TRUE,
    CONSTRAINT pk_rol PRIMARY KEY (id_rol)
);

-- Índices únicos
CREATE UNIQUE INDEX ux_rol_codigo ON seguridad.rol (codigo_rol);

-- ============================================================================
-- Tabla: usuario_rol
-- ============================================================================
CREATE TABLE seguridad.usuario_rol (
    id_usuario         UUID           NOT NULL,
    id_rol             UUID           NOT NULL,
    creado_por         VARCHAR(150)   NULL,
    fecha_creacion     TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modificado_por     VARCHAR(150)   NULL,
    fecha_modificacion TIMESTAMP      NULL,
    activo             BOOLEAN        NOT NULL DEFAULT TRUE,
    CONSTRAINT pk_usuario_rol PRIMARY KEY (id_usuario, id_rol),
    CONSTRAINT fk_usuario_rol_usuario FOREIGN KEY (id_usuario)
        REFERENCES seguridad.usuario (id_usuario)
        ON DELETE CASCADE,
    CONSTRAINT fk_usuario_rol_rol FOREIGN KEY (id_rol)
        REFERENCES seguridad.rol (id_rol)
        ON DELETE CASCADE
);

-- Índices
CREATE INDEX ix_usuario_rol_rol ON seguridad.usuario_rol (id_rol);

-- ============================================================================
-- Seed: Roles iniciales
-- ============================================================================
INSERT INTO seguridad.rol (id_rol, codigo_rol, nombre_rol, fecha_creacion, activo)
VALUES
    (gen_random_uuid(), 'USER', 'Usuario', CURRENT_TIMESTAMP, TRUE),
    (gen_random_uuid(), 'ADMIN', 'Administrador', CURRENT_TIMESTAMP, TRUE);
