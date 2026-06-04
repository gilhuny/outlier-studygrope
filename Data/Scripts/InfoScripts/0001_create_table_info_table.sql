CREATE TABLE info.info_table (
    id                SERIAL PRIMARY KEY,
    code              INT NOT NULL UNIQUE,
    short_name        VARCHAR(15) NOT NULL,
    full_name         VARCHAR(200) NOT NULL,
    table_name        VARCHAR(200) NOT NULL UNIQUE,
    state_code        INT NOT NULL REFERENCES info.info_state(code),
    created_user_id   UUID NOT NULL,
    created_date_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id  UUID,
    modified_date_time TIMESTAMPTZ
);

INSERT INTO info.info_table (code, short_name, full_name, table_name, state_code, created_user_id) VALUES
(1,  'STATE',    'State',         'info_state',         1, '00000000-0000-0000-0000-000000000001'),
(2,  'ROLE',     'Role',          'info_role',          1, '00000000-0000-0000-0000-000000000001'),
(3,  'CATEGORY', 'Category',      'info_category',      1, '00000000-0000-0000-0000-000000000001'),
(4,  'PRIVACY',  'Group Privacy', 'info_group_privacy', 1, '00000000-0000-0000-0000-000000000001'),
(5,  'JOINPOL',  'Join Policy',   'info_join_policy',   1, '00000000-0000-0000-0000-000000000001'),
(6,  'STATUS',   'Status',        'info_status',        1, '00000000-0000-0000-0000-000000000001'),
(7,  'CONTTYPE', 'Content Type',  'info_content_type',  1, '00000000-0000-0000-0000-000000000001'),
(8,  'PROOFTYPE','Proof Type',    'info_proof_type',    1, '00000000-0000-0000-0000-000000000001'),
(9,  'CHATTYPE', 'Chat Type',     'info_chat_type',     1, '00000000-0000-0000-0000-000000000001'),
(10, 'TAG',      'Tag',           'info_tag',           1, '00000000-0000-0000-0000-000000000001'),
(11, 'TABLE',    'Info Table',    'info_table',         1, '00000000-0000-0000-0000-000000000001');