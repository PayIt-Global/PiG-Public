-- Schema for core_identity
CREATE SCHEMA IF NOT EXISTS core_identity;

-- Table: asp_net_users
CREATE TABLE IF NOT EXISTS core_identity.asp_net_users (
    id SERIAL PRIMARY KEY,
    user_name text,
    normalized_user_name text,
    email text,
    normalized_email text,
    email_confirmed boolean,
    password_hash text,
    security_stamp text,
    concurrency_stamp text,
    phone_number text,
    phone_number_confirmed boolean,
    two_factor_enabled boolean,
    lockout_end timestamp with time zone,
    lockout_enabled boolean,
    access_failed_count integer
);

-- Table: asp_net_role_claims
CREATE TABLE IF NOT EXISTS core_identity.asp_net_role_claims (
    id SERIAL PRIMARY KEY,
    role_id integer,
    claim_type text,
    claim_value text
);

-- Table: asp_net_roles
CREATE TABLE IF NOT EXISTS core_identity.asp_net_roles (
    id SERIAL PRIMARY KEY,
    name text,
    normalized_name text,
    concurrency_stamp text UNIQUE
);

-- Table: asp_net_user_claims
CREATE TABLE IF NOT EXISTS core_identity.asp_net_user_claims (
    id SERIAL PRIMARY KEY,
    user_id integer,
    claim_type text,
    claim_value text
);

-- Table: asp_net_user_logins
CREATE TABLE IF NOT EXISTS core_identity.asp_net_user_logins (
    login_provider text NOT NULL,
    provider_key text NOT NULL,
    provider_display_name text,
    user_id integer,
    PRIMARY KEY (login_provider, provider_key)
);

-- Table: asp_net_user_roles
CREATE TABLE IF NOT EXISTS core_identity.asp_net_user_roles (
    user_id integer NOT NULL,
    role_id integer NOT NULL,
    PRIMARY KEY (user_id, role_id)
);

-- Table: asp_net_user_tokens
CREATE TABLE IF NOT EXISTS core_identity.asp_net_user_tokens (
    user_id integer NOT NULL,
    login_provider text NOT NULL,
    name text NOT NULL,
    value text,
    PRIMARY KEY (user_id, login_provider, name)
);

-- Table: refresh_tokens
CREATE TABLE IF NOT EXISTS core_identity.refresh_tokens (
    id SERIAL PRIMARY KEY,
    user_id integer NOT NULL,
    token text NOT NULL,
    expires timestamp with time zone NOT NULL,
    created timestamp with time zone DEFAULT now() NOT NULL,
    created_by_ip text,
    revoked timestamp with time zone,
    revoked_by_ip text,
    replaced_by_token text,
    reason_revoked text
);

-- Foreign Key Constraints
ALTER TABLE core_identity.asp_net_role_claims ADD CONSTRAINT fk_role_id FOREIGN KEY (role_id) REFERENCES core_identity.asp_net_roles(id) ON DELETE CASCADE;
ALTER TABLE core_identity.asp_net_user_claims ADD CONSTRAINT fk_user_id_claims FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;
ALTER TABLE core_identity.asp_net_user_logins ADD CONSTRAINT fk_user_id_logins FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;
ALTER TABLE core_identity.asp_net_user_roles ADD CONSTRAINT fk_user_id_roles FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;
ALTER TABLE core_identity.asp_net_user_roles ADD CONSTRAINT fk_role_id_roles FOREIGN KEY (role_id) REFERENCES core_identity.asp_net_roles(id) ON DELETE CASCADE;
ALTER TABLE core_identity.asp_net_user_tokens ADD CONSTRAINT fk_user_id_tokens FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;
ALTER TABLE core_identity.refresh_tokens ADD CONSTRAINT fk_user_id_refresh_tokens FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;
