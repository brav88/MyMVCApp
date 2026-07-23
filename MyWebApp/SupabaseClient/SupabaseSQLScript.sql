-- =============================================================================
-- Supabase Schema for the Angular TaskFlow App
-- Run this in your Supabase SQL Editor (Database → SQL Editor → New query).
-- =============================================================================

-- ─── 1. EXTENSIONS & ENUMS ───────────────────────────────────────────────────
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TYPE public.task_status   AS ENUM ('todo', 'in_progress', 'done');
CREATE TYPE public.task_priority AS ENUM ('low', 'medium', 'high');

-- ─── 2. PROFILES TABLE & TRIGGERS ────────────────────────────────────────────
-- One row per authenticated user, automatically created on sign-up.
CREATE TABLE IF NOT EXISTS public.profiles (
  id          UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
  email       TEXT NOT NULL,
  full_name   TEXT,
  avatar_url  TEXT,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Auto-insert a profile row when a new user registers.
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER 
LANGUAGE plpgsql
SECURITY DEFINER SET search_path = public
AS $$
BEGIN
  INSERT INTO public.profiles (id, email, full_name)
  VALUES (
    NEW.id,
    NEW.email,
    NEW.raw_user_meta_data ->> 'full_name'
  );
  RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;
CREATE TRIGGER on_auth_user_created
  AFTER INSERT ON auth.users
  FOR EACH ROW EXECUTE PROCEDURE public.handle_new_user();

-- ─── 3. TASKS TABLE & TRIGGERS ───────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS public.tasks (
  id          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id     UUID NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE, -- Corregido a auth.users según tu nota
  title       TEXT NOT NULL CHECK (char_length(title) BETWEEN 1 AND 120),
  description TEXT,
  status      public.task_status   NOT NULL DEFAULT 'todo',
  priority    public.task_priority NOT NULL DEFAULT 'medium',
  due_date    TIMESTAMPTZ,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Keep updated_at current automatically.
CREATE OR REPLACE FUNCTION public.set_updated_at()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS tasks_updated_at ON public.tasks;
CREATE TRIGGER tasks_updated_at
  BEFORE UPDATE ON public.tasks
  FOR EACH ROW EXECUTE PROCEDURE public.set_updated_at();

-- ─── 4. TICKETS TABLE ────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS public.tickets (
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

-- ─── 5. COMMENTS TABLE ───────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS public.comments (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    ticket_id BIGINT NOT NULL REFERENCES public.tickets(id) ON DELETE CASCADE,   
    comment_text TEXT,
    created_by UUID,    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()    
);

-- ─── 6. INDEXES ──────────────────────────────────────────────────────────────
CREATE INDEX IF NOT EXISTS tasks_user_id_idx    ON public.tasks (user_id);
CREATE INDEX IF NOT EXISTS tasks_status_idx     ON public.tasks (status);
CREATE INDEX IF NOT EXISTS tasks_due_date_idx   ON public.tasks (due_date);

-- ─── 7. ROW LEVEL SECURITY (RLS) & POLICIES ──────────────────────────────────
ALTER TABLE public.profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.tasks ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.comments ENABLE ROW LEVEL SECURITY; -- Habilitada para consistencia

-- Profiles Policies
CREATE POLICY "Users can view their own profile"
  ON public.profiles FOR SELECT USING (auth.uid() = id);

CREATE POLICY "Users can update their own profile"
  ON public.profiles FOR UPDATE USING (auth.uid() = id);

-- Tasks Policies
CREATE POLICY "Users can manage their own tasks"
  ON public.tasks USING (auth.uid() = user_id) WITH CHECK (auth.uid() = user_id);

-- Comments Policies
CREATE POLICY "Allow anonymous inserts" 
  ON public.comments FOR INSERT TO anon WITH CHECK (true);

CREATE POLICY "Enable delete for users based on user_id"
  ON public.comments AS PERMISSIVE FOR DELETE TO public USING (true);

-- Realtime Configuration
ALTER PUBLICATION supabase_realtime ADD TABLE public.tasks;

-- ─── 8. BACKFILL & MAINTENANCE ───────────────────────────────────────────────
-- Backfill: crea el perfil de cualquier usuario que no lo tenga todavía
INSERT INTO public.profiles (id, email, full_name)
SELECT id, email, raw_user_meta_data->>'full_name'
FROM auth.users
WHERE id NOT IN (SELECT id FROM public.profiles)
ON CONFLICT (id) DO NOTHING;

-- ─── 9. SEED DATA (INSERTS DE PRUEBA) ────────────────────────────────────────

-- Inserts en TICKETS
INSERT INTO public.tickets (title, description, status, priority, ticket_type, created_by, assigned_to, due_date)
VALUES 
  ('Error de login en la app móvil', 'Los usuarios reciben un error 500 al intentar iniciar sesión con Google.', 'Open', 'Critical', 'Bug', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '1 day'),
  ('Actualizar términos de servicio', 'Modificar la sección de privacidad según las nuevas regulaciones de 2026.', 'In Progress', 'Medium', 'Task', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '7 days'),
  ('Crear panel de administración', 'Como admin, quiero ver los reportes mensuales de ventas en un gráfico interactivo.', 'Open', 'High', 'Story', gen_random_uuid(), NULL, NOW() + INTERVAL '14 days'),
  ('Optimizar queries de reportes', 'Se indexó la tabla de transacciones para mejorar el tiempo de carga del dashboard.', 'Resolved', 'Medium', 'Improvement', gen_random_uuid(), gen_random_uuid(), NOW() - INTERVAL '2 days'),
  ('Falta tooltip en botón de guardado', 'El botón de guardar no muestra el texto de ayuda al pasar el cursor.', 'Closed', 'Low', 'Bug', gen_random_uuid(), gen_random_uuid(), NOW() - INTERVAL '5 days'),
  ('Renovar certificados SSL', 'Los certificados del servidor de producción expiran la próxima semana.', 'Open', 'High', 'Task', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '3 days'),
  ('Implementar modo oscuro', 'Agregar switch en la configuración del perfil para cambiar el tema de la interfaz.', 'In Progress', 'Low', 'Improvement', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '10 days'),
  ('Integración con pasarela de pagos Stripe', 'Permitir pagos recurrentes para las suscripciones premium.', 'Open', 'High', 'Story', gen_random_uuid(), NULL, NOW() + INTERVAL '30 days'),
  ('Fuga de memoria en el contenedor de NodeJS', 'El uso de RAM sube constantemente tras 24 horas encendido el servicio.', 'In Progress', 'Critical', 'Bug', gen_random_uuid(), gen_random_uuid(), NOW() + INTERVAL '2 days'),
  ('Backup mensual de la base de datos', 'Validar y almacenar el dump de producción en el bucket S3 seguro.', 'Closed', 'Medium', 'Task', gen_random_uuid(), gen_random_uuid(), NOW() - INTERVAL '1 day');

-- Inserts en COMMENTS
INSERT INTO public.comments (ticket_id, comment_text, created_by, created_at)
VALUES 
  (1, 'El usuario reporta que no puede acceder al módulo de facturación. Se procede a revisar permisos.', 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', NOW()),
  (1, 'Permisos verificados. El problema persiste.', 'b2cc7632-1234-4567-89ab-cdef01234567', NOW()),
  (1, 'Incidencia escalada al equipo de DevOps debido a una caída intermitente en el servidor de base de datos.', 'fa3b1824-8854-4603-9f57-1941d3b38c29', '2026-06-15 14:30:00+00'),
  (1, 'Ya puedo ver el botón de descarga, gracias por la rápida solución.', 'c9a646d3-9021-4b12-a123-bcdef456789a', NOW()),
  (2, 'Que pasa que no me resuelven nada???', 'c9a646d3-9021-4b12-a123-bcdef456789a', NOW());