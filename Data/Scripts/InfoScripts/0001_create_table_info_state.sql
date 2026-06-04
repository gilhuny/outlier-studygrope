CREATE SCHEMA IF NOT EXISTS info;
 
-- ============================================================
-- INFO TABLES (schema: info)
-- ============================================================
 
-- State (for info tables: Active/Passive)
CREATE TABLE info.info_state (
    id          SERIAL PRIMARY KEY,
    code        INT NOT NULL UNIQUE,
    short_name  VARCHAR(15) NOT NULL,
    full_name   VARCHAR(200) NOT NULL,
    created_user_id UUID NOT NULL,
    created_date_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id UUID,
    modified_date_time TIMESTAMPTZ
);

INSERT INTO info.info_state (code, short_name, full_name, created_user_id) VALUES
(1, 'ACTIVE',   'Active',  '00000000-0000-0000-0000-000000000001'),
(2, 'PASSIVE',  'Passive', '00000000-0000-0000-0000-000000000001');