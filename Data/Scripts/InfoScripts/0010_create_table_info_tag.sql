CREATE TABLE info.info_tag (
    id                SERIAL PRIMARY KEY,
    code              INT NOT NULL UNIQUE,
    short_name        VARCHAR(15) NOT NULL,
    full_name         VARCHAR(200) NOT NULL,
    state_code        INT NOT NULL REFERENCES info.info_state(code),
    created_user_id   UUID NOT NULL,
    created_date_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id  UUID,
    modified_date_time TIMESTAMPTZ
);
CREATE INDEX ix_info_tag_state_code ON info.info_tag(state_code);

INSERT INTO info.info_tag (code, short_name, full_name, state_code, created_user_id) VALUES
(1,  'BEGINNER',  'Beginner Friendly',  1, '00000000-0000-0000-0000-000000000001'),
(2,  'ADVANCED',  'Advanced Level',     1, '00000000-0000-0000-0000-000000000001'),
(3,  'DAILY',     'Daily Practice',     1, '00000000-0000-0000-0000-000000000001'),
(4,  'OLYMPIAD',  'Olympiad Prep',      1, '00000000-0000-0000-0000-000000000001'),
(5,  'EXAM',      'Exam Prep',          1, '00000000-0000-0000-0000-000000000001'),
(6,  'COMPETE',   'Competitive',        1, '00000000-0000-0000-0000-000000000001'),
(7,  'CASUAL',    'Casual Pace',        1, '00000000-0000-0000-0000-000000000001'),
(8,  'INTENSE',   'Intense Focus',      1, '00000000-0000-0000-0000-000000000001'),
(9,  'STREAKBSD', 'Streak Based',       1, '00000000-0000-0000-0000-000000000001'),
(10, 'TEAMWORK',  'Teamwork Focus',     1, '00000000-0000-0000-0000-000000000001'),
(11, 'SELFPACED', 'Self Paced',         1, '00000000-0000-0000-0000-000000000001'),
(12, 'MENTORED',  'Mentored',           1, '00000000-0000-0000-0000-000000000001');