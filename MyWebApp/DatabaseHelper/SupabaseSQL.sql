CREATE TABLE tickets (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,

    status VARCHAR(20) NOT NULL DEFAULT 'Open'
        CHECK (status IN ('Open', 'In Progress', 'Resolved', 'Closed')),

    priority VARCHAR(20) NOT NULL DEFAULT 'Medium'
        CHECK (priority IN ('Low', 'Medium', 'High', 'Critical')),

    ticket_type VARCHAR(20) NOT NULL DEFAULT 'Task'
        CHECK (ticket_type IN ('Task', 'Bug', 'Story', 'Improvement')),

    created_by UUID,
    assigned_to UUID,

    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    due_date TIMESTAMP WITH TIME ZONE
);

INSERT INTO tickets (title, description, status, priority, ticket_type, created_by, assigned_to, due_date)
VALUES 
-- 1. Un bug crítico que requiere atención inmediata
('Error de login en la app móvil', 'Los usuarios reciben un error 500 al intentar iniciar sesión con Google.', 'Open', 'Critical', 'Bug', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '1 day'),

-- 2. Una tarea simple ya asignada y en progreso
('Actualizar términos de servicio', 'Modificar la sección de privacidad según las nuevas regulaciones de 2026.', 'In Progress', 'Medium', 'Task', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '7 days'),

-- 3. Una historia de usuario (Story) sin asignar aún
('Crear panel de administración', 'Como admin, quiero ver los reportes mensuales de ventas en un gráfico interactivo.', 'Open', 'High', 'Story', gen_random_uuid(), NULL, NOW() + INTERVAL '14 days'),

-- 4. Una mejora ya resuelta
('Optimizar queries de reportes', 'Se indexó la tabla de transacciones para mejorar el tiempo de carga del dashboard.', 'Resolved', 'Medium', 'Improvement', gen_random_uuid(), gen_random_uuid(), NOW() - INTERVAL '2 days'),

-- 5. Un bug de baja prioridad ya cerrado
('Falta tooltip en botón de guardado', 'El botón de guardar no muestra el texto de ayuda al pasar el cursor.', 'Closed', 'Low', 'Bug', gen_random_uuid(), gen_random_uuid(), NOW() - INTERVAL '5 days'),

-- 6. Una tarea de alta prioridad urgente
('Renovar certificados SSL', 'Los certificados del servidor de producción expiran la próxima semana.', 'Open', 'High', 'Task', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '3 days'),

-- 7. Una mejora en progreso
('Implementar modo oscuro', 'Agregar switch en la configuración del perfil para cambiar el tema de la interfaz.', 'In Progress', 'Low', 'Improvement', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '10 days'),

-- 8. Una historia de usuario compleja
('Integración con pasarela de pagos Stripe', 'Permitir pagos recurrentes para las suscripciones premium.', 'Open', 'High', 'Story', gen_random_uuid(), NULL, NOW() + INTERVAL '30 days'),

-- 9. Un bug en progreso
('Fuga de memoria en el contenedor de NodeJS', 'El uso de RAM sube constantemente tras 24 horas encendido el servicio.', 'In Progress', 'Critical', 'Bug', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '2 days'),

-- 10. Una tarea rutinaria completada
('Backup mensual de la base de datos', 'Validar y almacenar el dump de producción en el bucket S3 seguro.', 'Closed', 'Medium', 'Task', gen_random_uuid(), gen_random_uuid(), NOW() - INTERVAL '1 day');

CREATE TABLE comments (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    ticket_id BIGINT NOT NULL,   
    commment_text TEXT,
    created_by UUID,    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()    
);

-- Insert 1: Un comentario estándar de soporte
INSERT INTO comments (ticket_id, comment_text, created_by, created_at)
VALUES (
    1, 
    'El usuario reporta que no puede acceder al módulo de facturación. Se procede a revisar permisos.', 
    'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 
    NOW()
);

-- Insert 2: Comentario breve usando el valor por defecto de created_at
INSERT INTO comments (ticket_id, comment_text, created_by)
VALUES (
    1, 
    'Permisos verificados. El problema persiste.', 
    'b2cc7632-1234-4567-89ab-cdef01234567'
);

-- Insert 3: Comentario con una marca de tiempo específica en el pasado
INSERT INTO comments (ticket_id, comment_text, created_by, created_at)
VALUES (
    1, 
    'Incidencia escalada al equipo de DevOps debido a una caída intermitente en el servidor de base de datos.', 
    'fa3b1824-8854-4603-9f57-1941d3b38c29', 
    '2026-06-15 14:30:00+00'
);

-- Insert 4: Comentario simulando la respuesta del cliente
INSERT INTO comments (ticket_id, comment_text, created_by)
VALUES (
    1, 
    'Ya puedo ver el botón de descarga, gracias por la rápida solución.', 
    'c9a646d3-9021-4b12-a123-bcdef456789a'
);

INSERT INTO comments (ticket_id, comment_text, created_by)
VALUES (
    2, 
    'Que pasa que no me resuelven nada???', 
    'c9a646d3-9021-4b12-a123-bcdef456789a'
);


SELECT * FROM comments

CREATE POLICY "Allow anonymous inserts" 
ON public.comments 
FOR INSERT 
TO anon 
WITH CHECK (true);

