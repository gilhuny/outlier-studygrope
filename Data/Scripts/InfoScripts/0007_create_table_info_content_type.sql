CREATE TABLE info.info_content_type (
    id          SERIAL PRIMARY KEY,
    code        INT NOT NULL UNIQUE,
    short_name  VARCHAR(15) NOT NULL,
    full_name   VARCHAR(200) NOT NULL,
    type_name   VARCHAR(100) NOT NULL UNIQUE,  -- MIME type e.g. image/jpeg
    state_code  INT NOT NULL REFERENCES info.info_state(code),
    created_user_id UUID NOT NULL,
    created_date_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id UUID,
    modified_date_time TIMESTAMPTZ
);
CREATE INDEX ix_info_content_type_state_code ON info.info_content_type(state_code);


INSERT INTO info.info_content_type (code, short_name, full_name, type_name, state_code, created_user_id) VALUES
(1, 'JPG',  'JPEG Image',       'image/jpeg',       1, '00000000-0000-0000-0000-000000000001'),
(2, 'PNG',  'PNG Image',        'image/png',        1, '00000000-0000-0000-0000-000000000001'),
(3, 'PDF',  'PDF Document',     'application/pdf',  1, '00000000-0000-0000-0000-000000000001'),
(4, 'TXT',  'Plain Text',       'text/plain',       1, '00000000-0000-0000-0000-000000000001'),
(5, 'WEBP', 'WebP Image',       'image/webp',       1, '00000000-0000-0000-0000-000000000001');