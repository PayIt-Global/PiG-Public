--
-- PostgreSQL database dump
--

-- Dumped from database version 15.7 (Debian 15.7-0+deb12u1)
-- Dumped by pg_dump version 15.4

-- Started on 2024-07-07 10:52:05

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 6 (class 2615 OID 16498)
-- Name: core_identity; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA core_identity;


ALTER SCHEMA core_identity OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 223 (class 1259 OID 16582)
-- Name: asp_net_role_claims; Type: TABLE; Schema: core_identity; Owner: postgres
--

CREATE TABLE core_identity.asp_net_role_claims (
    id integer NOT NULL,
    role_id character varying(450),
    claim_type text,
    claim_value text
);


ALTER TABLE core_identity.asp_net_role_claims OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 16581)
-- Name: asp_net_role_claims_id_seq; Type: SEQUENCE; Schema: core_identity; Owner: postgres
--

CREATE SEQUENCE core_identity.asp_net_role_claims_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE core_identity.asp_net_role_claims_id_seq OWNER TO postgres;

--
-- TOC entry 3414 (class 0 OID 0)
-- Dependencies: 222
-- Name: asp_net_role_claims_id_seq; Type: SEQUENCE OWNED BY; Schema: core_identity; Owner: postgres
--

ALTER SEQUENCE core_identity.asp_net_role_claims_id_seq OWNED BY core_identity.asp_net_role_claims.id;


--
-- TOC entry 216 (class 1259 OID 16515)
-- Name: asp_net_roles; Type: TABLE; Schema: core_identity; Owner: postgres
--

CREATE TABLE core_identity.asp_net_roles (
    id character varying(450) NOT NULL,
    name text,
    normalized_name text,
    concurrency_stamp text
);


ALTER TABLE core_identity.asp_net_roles OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16544)
-- Name: asp_net_user_claims; Type: TABLE; Schema: core_identity; Owner: postgres
--

CREATE TABLE core_identity.asp_net_user_claims (
    id integer NOT NULL,
    user_id character varying(450),
    claim_type text,
    claim_value text
);


ALTER TABLE core_identity.asp_net_user_claims OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 16543)
-- Name: asp_net_user_claims_id_seq; Type: SEQUENCE; Schema: core_identity; Owner: postgres
--

CREATE SEQUENCE core_identity.asp_net_user_claims_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE core_identity.asp_net_user_claims_id_seq OWNER TO postgres;

--
-- TOC entry 3415 (class 0 OID 0)
-- Dependencies: 218
-- Name: asp_net_user_claims_id_seq; Type: SEQUENCE OWNED BY; Schema: core_identity; Owner: postgres
--

ALTER SEQUENCE core_identity.asp_net_user_claims_id_seq OWNED BY core_identity.asp_net_user_claims.id;


--
-- TOC entry 220 (class 1259 OID 16557)
-- Name: asp_net_user_logins; Type: TABLE; Schema: core_identity; Owner: postgres
--

CREATE TABLE core_identity.asp_net_user_logins (
    login_provider text NOT NULL,
    provider_key text NOT NULL,
    provider_display_name text,
    user_id character varying(450)
);


ALTER TABLE core_identity.asp_net_user_logins OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16526)
-- Name: asp_net_user_roles; Type: TABLE; Schema: core_identity; Owner: postgres
--

CREATE TABLE core_identity.asp_net_user_roles (
    user_id character varying(450) NOT NULL,
    role_id character varying(450) NOT NULL
);


ALTER TABLE core_identity.asp_net_user_roles OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16569)
-- Name: asp_net_user_tokens; Type: TABLE; Schema: core_identity; Owner: postgres
--

CREATE TABLE core_identity.asp_net_user_tokens (
    user_id character varying(450) NOT NULL,
    login_provider text NOT NULL,
    name text NOT NULL,
    value text
);


ALTER TABLE core_identity.asp_net_user_tokens OWNER TO postgres;

--
-- TOC entry 215 (class 1259 OID 16502)
-- Name: asp_net_users; Type: TABLE; Schema: core_identity; Owner: postgres
--

CREATE TABLE core_identity.asp_net_users (
    id character varying(450) NOT NULL,
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


ALTER TABLE core_identity.asp_net_users OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 16596)
-- Name: refresh_tokens; Type: TABLE; Schema: core_identity; Owner: postgres
--

CREATE TABLE core_identity.refresh_tokens (
    id integer NOT NULL,
    user_id character varying(450) NOT NULL,
    token text NOT NULL,
    expires timestamp with time zone NOT NULL,
    created timestamp with time zone DEFAULT now() NOT NULL,
    created_by_ip text,
    revoked timestamp with time zone,
    revoked_by_ip text,
    replaced_by_token text,
    reason_revoked text
);


ALTER TABLE core_identity.refresh_tokens OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 16595)
-- Name: refresh_tokens_id_seq; Type: SEQUENCE; Schema: core_identity; Owner: postgres
--

CREATE SEQUENCE core_identity.refresh_tokens_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE core_identity.refresh_tokens_id_seq OWNER TO postgres;

--
-- TOC entry 3416 (class 0 OID 0)
-- Dependencies: 224
-- Name: refresh_tokens_id_seq; Type: SEQUENCE OWNED BY; Schema: core_identity; Owner: postgres
--

ALTER SEQUENCE core_identity.refresh_tokens_id_seq OWNED BY core_identity.refresh_tokens.id;


--
-- TOC entry 3231 (class 2604 OID 16585)
-- Name: asp_net_role_claims id; Type: DEFAULT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_role_claims ALTER COLUMN id SET DEFAULT nextval('core_identity.asp_net_role_claims_id_seq'::regclass);


--
-- TOC entry 3230 (class 2604 OID 16547)
-- Name: asp_net_user_claims id; Type: DEFAULT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_claims ALTER COLUMN id SET DEFAULT nextval('core_identity.asp_net_user_claims_id_seq'::regclass);


--
-- TOC entry 3232 (class 2604 OID 16599)
-- Name: refresh_tokens id; Type: DEFAULT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.refresh_tokens ALTER COLUMN id SET DEFAULT nextval('core_identity.refresh_tokens_id_seq'::regclass);


--
-- TOC entry 3257 (class 2606 OID 16589)
-- Name: asp_net_role_claims asp_net_role_claims_pkey; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_role_claims
    ADD CONSTRAINT asp_net_role_claims_pkey PRIMARY KEY (id);


--
-- TOC entry 3243 (class 2606 OID 16525)
-- Name: asp_net_roles asp_net_roles_concurrency_stamp_key; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_roles
    ADD CONSTRAINT asp_net_roles_concurrency_stamp_key UNIQUE (concurrency_stamp);


--
-- TOC entry 3245 (class 2606 OID 16523)
-- Name: asp_net_roles asp_net_roles_normalized_name_key; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_roles
    ADD CONSTRAINT asp_net_roles_normalized_name_key UNIQUE (normalized_name);


--
-- TOC entry 3247 (class 2606 OID 16521)
-- Name: asp_net_roles asp_net_roles_pkey; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_roles
    ADD CONSTRAINT asp_net_roles_pkey PRIMARY KEY (id);


--
-- TOC entry 3251 (class 2606 OID 16551)
-- Name: asp_net_user_claims asp_net_user_claims_pkey; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_claims
    ADD CONSTRAINT asp_net_user_claims_pkey PRIMARY KEY (id);


--
-- TOC entry 3235 (class 2606 OID 16514)
-- Name: asp_net_users asp_net_users_concurrency_stamp_key; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_users
    ADD CONSTRAINT asp_net_users_concurrency_stamp_key UNIQUE (concurrency_stamp);


--
-- TOC entry 3237 (class 2606 OID 16512)
-- Name: asp_net_users asp_net_users_normalized_email_key; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_users
    ADD CONSTRAINT asp_net_users_normalized_email_key UNIQUE (normalized_email);


--
-- TOC entry 3239 (class 2606 OID 16510)
-- Name: asp_net_users asp_net_users_normalized_user_name_key; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_users
    ADD CONSTRAINT asp_net_users_normalized_user_name_key UNIQUE (normalized_user_name);


--
-- TOC entry 3241 (class 2606 OID 16508)
-- Name: asp_net_users asp_net_users_pkey; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_users
    ADD CONSTRAINT asp_net_users_pkey PRIMARY KEY (id);


--
-- TOC entry 3253 (class 2606 OID 16563)
-- Name: asp_net_user_logins pk_asp_net_user_logins; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_logins
    ADD CONSTRAINT pk_asp_net_user_logins PRIMARY KEY (login_provider, provider_key);


--
-- TOC entry 3249 (class 2606 OID 16532)
-- Name: asp_net_user_roles pk_asp_net_user_roles; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_roles
    ADD CONSTRAINT pk_asp_net_user_roles PRIMARY KEY (user_id, role_id);


--
-- TOC entry 3255 (class 2606 OID 16575)
-- Name: asp_net_user_tokens pk_asp_net_user_tokens; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_tokens
    ADD CONSTRAINT pk_asp_net_user_tokens PRIMARY KEY (user_id, login_provider, name);


--
-- TOC entry 3259 (class 2606 OID 16604)
-- Name: refresh_tokens refresh_tokens_pkey; Type: CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.refresh_tokens
    ADD CONSTRAINT refresh_tokens_pkey PRIMARY KEY (id);


--
-- TOC entry 3265 (class 2606 OID 16590)
-- Name: asp_net_role_claims fk_asp_net_role_claims_asp_net_roles_role_id; Type: FK CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_role_claims
    ADD CONSTRAINT fk_asp_net_role_claims_asp_net_roles_role_id FOREIGN KEY (role_id) REFERENCES core_identity.asp_net_roles(id) ON DELETE CASCADE;


--
-- TOC entry 3262 (class 2606 OID 16552)
-- Name: asp_net_user_claims fk_asp_net_user_claims_asp_net_users_user_id; Type: FK CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_claims
    ADD CONSTRAINT fk_asp_net_user_claims_asp_net_users_user_id FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;


--
-- TOC entry 3263 (class 2606 OID 16564)
-- Name: asp_net_user_logins fk_asp_net_user_logins_asp_net_users_user_id; Type: FK CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_logins
    ADD CONSTRAINT fk_asp_net_user_logins_asp_net_users_user_id FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;


--
-- TOC entry 3260 (class 2606 OID 16538)
-- Name: asp_net_user_roles fk_asp_net_user_roles_asp_net_roles_role_id; Type: FK CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_roles
    ADD CONSTRAINT fk_asp_net_user_roles_asp_net_roles_role_id FOREIGN KEY (role_id) REFERENCES core_identity.asp_net_roles(id) ON DELETE CASCADE;


--
-- TOC entry 3261 (class 2606 OID 16533)
-- Name: asp_net_user_roles fk_asp_net_user_roles_asp_net_users_user_id; Type: FK CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_roles
    ADD CONSTRAINT fk_asp_net_user_roles_asp_net_users_user_id FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;


--
-- TOC entry 3264 (class 2606 OID 16576)
-- Name: asp_net_user_tokens fk_asp_net_user_tokens_asp_net_users_user_id; Type: FK CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.asp_net_user_tokens
    ADD CONSTRAINT fk_asp_net_user_tokens_asp_net_users_user_id FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;


--
-- TOC entry 3266 (class 2606 OID 16605)
-- Name: refresh_tokens fk_refresh_tokens_asp_net_users_user_id; Type: FK CONSTRAINT; Schema: core_identity; Owner: postgres
--

ALTER TABLE ONLY core_identity.refresh_tokens
    ADD CONSTRAINT fk_refresh_tokens_asp_net_users_user_id FOREIGN KEY (user_id) REFERENCES core_identity.asp_net_users(id) ON DELETE CASCADE;


-- Completed on 2024-07-07 10:52:06

--
-- PostgreSQL database dump complete
--

