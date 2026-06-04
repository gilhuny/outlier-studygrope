CREATE TABLE info.info_role (
    id          SERIAL PRIMARY KEY,
    code        INT NOT NULL UNIQUE,
    short_name  VARCHAR(15) NOT NULL,
    full_name   VARCHAR(200) NOT NULL,
    state_code  INT NOT NULL REFERENCES info.info_state(code),
    created_user_id UUID NOT NULL,
    created_date_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id UUID,
    modified_date_time TIMESTAMPTZ
);
CREATE INDEX ix_info_role_state_code ON info.info_role(state_code);

-- Roles
INSERT INTO info.info_role (code, short_name, full_name, state_code, created_user_id) VALUES
(1, 'ADMIN',  'Administrator', 1, '00000000-0000-0000-0000-000000000001'),
(2, 'MEMBER', 'Member',        1, '00000000-0000-0000-0000-000000000001');
 