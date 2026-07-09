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
INSERT INTO comments (ticket_id, commment_text, created_by, created_at)
VALUES (
    1, 
    'El usuario reporta que no puede acceder al módulo de facturación. Se procede a revisar permisos.', 
    'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 
    NOW()
);

-- Insert 2: Comentario breve usando el valor por defecto de created_at
INSERT INTO comments (ticket_id, commment_text, created_by)
VALUES (
    1, 
    'Permisos verificados. El problema persiste.', 
    'b2cc7632-1234-4567-89ab-cdef01234567'
);

-- Insert 3: Comentario con una marca de tiempo específica en el pasado
INSERT INTO comments (ticket_id, commment_text, created_by, created_at)
VALUES (
    1, 
    'Incidencia escalada al equipo de DevOps debido a una caída intermitente en el servidor de base de datos.', 
    'fa3b1824-8854-4603-9f57-1941d3b38c29', 
    '2026-06-15 14:30:00+00'
);

-- Insert 4: Comentario simulando la respuesta del cliente
INSERT INTO comments (ticket_id, commment_text, created_by)
VALUES (
    1, 
    'Ya puedo ver el botón de descarga, gracias por la rápida solución.', 
    'c9a646d3-9021-4b12-a123-bcdef456789a'
);

INSERT INTO comments (ticket_id, commment_text, created_by)
VALUES (
    2, 
    'Que pasa que no me resuelven nada???', 
    'c9a646d3-9021-4b12-a123-bcdef456789a'
);


SELECT * FROM comments

DELETE FROM comments

CREATE POLICY "Allow anonymous inserts" 
ON public.comments 
FOR INSERT 
TO anon 
WITH CHECK (true);

create policy "Enable delete for users based on user_id"
on "public"."comments"
as PERMISSIVE
for DELETE
to public
using (
    true
);


-- =============================================================================
-- Supabase Schema for the Angular TaskFlow App
-- Run this in your Supabase SQL Editor (Database → SQL Editor → New query).
-- =============================================================================

-- ─── Extensions ──────────────────────────────────────────────────────────────
create extension if not exists "uuid-ossp";

-- =============================================================================
-- 1. PROFILES TABLE
--    One row per authenticated user, automatically created on sign-up.
-- =============================================================================
create table if not exists public.profiles (
  id          uuid primary key references auth.users(id) on delete cascade,
  email       text not null,
  full_name   text,
  avatar_url  text,
  created_at  timestamptz not null default now(),
  updated_at  timestamptz not null default now()
);

-- Auto-insert a profile row when a new user registers.
create or replace function public.handle_new_user()
returns trigger
language plpgsql
security definer set search_path = public
as $$
begin
  insert into public.profiles (id, email, full_name)
  values (
    new.id,
    new.email,
    new.raw_user_meta_data ->> 'full_name'
  );
  return new;
end;
$$;

drop trigger if exists on_auth_user_created on auth.users;
create trigger on_auth_user_created
  after insert on auth.users
  for each row execute procedure public.handle_new_user();

-- ─── Row Level Security ───────────────────────────────────────────────────────
alter table public.profiles enable row level security;

create policy "Users can view their own profile"
  on public.profiles for select
  using (auth.uid() = id);

create policy "Users can update their own profile"
  on public.profiles for update
  using (auth.uid() = id);


-- =============================================================================
-- 2. TASKS TABLE
-- =============================================================================
create type public.task_status   as enum ('todo', 'in_progress', 'done');
create type public.task_priority as enum ('low', 'medium', 'high');

create table if not exists public.tasks (
  id          uuid primary key default uuid_generate_v4(),
  user_id     uuid not null references public.profiles(id) on delete cascade,
  title       text not null check (char_length(title) between 1 and 120),
  description text,
  status      public.task_status   not null default 'todo',
  priority    public.task_priority not null default 'medium',
  due_date    timestamptz,
  created_at  timestamptz not null default now(),
  updated_at  timestamptz not null default now()
);

-- Keep updated_at current automatically.
create or replace function public.set_updated_at()
returns trigger
language plpgsql
as $$
begin
  new.updated_at = now();
  return new;
end;
$$;

create trigger tasks_updated_at
  before update on public.tasks
  for each row execute procedure public.set_updated_at();

-- ─── Indexes ──────────────────────────────────────────────────────────────────
create index if not exists tasks_user_id_idx    on public.tasks (user_id);
create index if not exists tasks_status_idx     on public.tasks (status);
create index if not exists tasks_due_date_idx   on public.tasks (due_date);

-- ─── Row Level Security ───────────────────────────────────────────────────────
alter table public.tasks enable row level security;

create policy "Users can manage their own tasks"
  on public.tasks
  using (auth.uid() = user_id)
  with check (auth.uid() = user_id);

-- ─── Realtime ─────────────────────────────────────────────────────────────────
-- Enable Postgres changes publication for the tasks table so the Angular
-- real-time subscription receives INSERT / UPDATE / DELETE events.
alter publication supabase_realtime add table public.tasks;

-- 1. Corrige la FK en tasks para que apunte a auth.users (no a profiles)
ALTER TABLE public.tasks
  DROP CONSTRAINT IF EXISTS tasks_user_id_fkey,
  ADD CONSTRAINT tasks_user_id_fkey
    FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE;

-- 2. Backfill: crea el perfil de cualquier usuario que no lo tenga todavía
INSERT INTO public.profiles (id, email, full_name)
SELECT id, email, raw_user_meta_data->>'full_name'
FROM auth.users
WHERE id NOT IN (SELECT id FROM public.profiles)
ON CONFLICT (id) DO NOTHING;