CREATE TABLE info.info_status (
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
CREATE INDEX ix_info_status_state_code ON info.info_status(state_code);

INSERT INTO info.info_status (code, short_name, full_name, state_code, created_user_id) VALUES
(1, 'CREATED',   'Created',   1, '00000000-0000-0000-0000-000000000001'),
(2, 'ACTIVE',    'Active',    1, '00000000-0000-0000-0000-000000000001'),
(3, 'UPDATED',   'Updated',   1, '00000000-0000-0000-0000-000000000001'),
(4, 'ARCHIVED',  'Archived',  1, '00000000-0000-0000-0000-000000000001'),
(5, 'DELETED',   'Deleted',   1, '00000000-0000-0000-0000-000000000001'),
(6, 'PASSIVE',   'Passive',   1, '00000000-0000-0000-0000-000000000001'),
(7, 'COMPLETED', 'Completed', 1, '00000000-0000-0000-0000-000000000001'),
(8, 'PENDING',   'Pending',   1, '00000000-0000-0000-0000-000000000001'),
(9, 'REJECTED',  'Rejected',  1, '00000000-0000-0000-0000-000000000001'),
(10,'ACCEPTED',  'Accepted',  1, '00000000-0000-0000-0000-000000000001'),
(11,'KICKED',    'Kicked',    1, '00000000-0000-0000-0000-000000000001');