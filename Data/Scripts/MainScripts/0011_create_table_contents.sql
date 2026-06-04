CREATE TABLE contents (
    id                  BIGSERIAL PRIMARY KEY,
    file_id             UUID NOT NULL UNIQUE,
    name                VARCHAR(200) NOT NULL,
    folder              VARCHAR(200) NOT NULL,
    content_type_code   INT NOT NULL REFERENCES info.info_content_type(code),
    status_code         INT NOT NULL REFERENCES info.info_status(code),
    created_user_id     UUID NOT NULL,
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id    UUID,
    modified_date_time  TIMESTAMPTZ
);
CREATE INDEX ix_contents_status_code ON contents(status_code);
CREATE INDEX ix_contents_content_type_code ON contents(content_type_code);