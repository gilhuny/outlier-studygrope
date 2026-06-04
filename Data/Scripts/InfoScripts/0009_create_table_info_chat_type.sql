-- Chat Type (Group, OneToOne)
CREATE TABLE info.info_chat_type (
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
CREATE INDEX ix_info_chat_type_state_code ON info.info_chat_type(state_code);

INSERT INTO info.info_chat_type (code, short_name, full_name, state_code, created_user_id) VALUES
(1, 'GROUP',  'Group Chat',      1, '00000000-0000-0000-0000-000000000001'),
(2, 'ONETONE','One To One Chat', 1, '00000000-0000-0000-0000-000000000001');
 