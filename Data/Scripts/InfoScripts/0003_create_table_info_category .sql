CREATE TABLE info.info_category (
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
CREATE INDEX ix_info_category_state_code ON info.info_category(state_code);

INSERT INTO info.info_category (code, short_name, full_name, state_code, created_user_id) VALUES
(1, 'MATH',     'Mathematics',       1, '00000000-0000-0000-0000-000000000001'),
(2, 'LANG',     'Language Learning', 1, '00000000-0000-0000-0000-000000000001'),
(3, 'CODE',     'Coding',            1, '00000000-0000-0000-0000-000000000001'),
(4, 'SCIENCE',  'Science',           1, '00000000-0000-0000-0000-000000000001'),
(5, 'HISTORY',  'History',           1, '00000000-0000-0000-0000-000000000001'),
(6, 'ART',      'Art & Design',      1, '00000000-0000-0000-0000-000000000001'),
(7, 'MUSIC',    'Music',             1, '00000000-0000-0000-0000-000000000001'),
(8, 'FITNESS',  'Fitness & Health',  1, '00000000-0000-0000-0000-000000000001'),
(9, 'BUSINESS', 'Business',          1, '00000000-0000-0000-0000-000000000001'),
(10,'OTHER',    'Other',             1, '00000000-0000-0000-0000-000000000001');