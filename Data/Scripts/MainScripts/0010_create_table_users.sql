CREATE TABLE users (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username            VARCHAR(50) NOT NULL UNIQUE,
    email               VARCHAR(200) NOT NULL UNIQUE,
    password_hash       VARCHAR(500) NOT NULL,
    first_name          VARCHAR(100) NOT NULL,
    last_name           VARCHAR(100) NOT NULL,
    bio                 VARCHAR(1000),              -- ← move bio here too
    img_id              BIGINT REFERENCES contents(id),  -- ← profile image
    role_code           INT NOT NULL REFERENCES info.info_role(code),
    status_code         INT NOT NULL REFERENCES info.info_status(code),
    created_user_id     UUID NOT NULL,
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id    UUID,
    modified_date_time  TIMESTAMPTZ
);
CREATE INDEX ix_users_role_code ON users(role_code);
CREATE INDEX ix_users_status_code ON users(status_code);
CREATE INDEX ix_users_username ON users(username);
CREATE INDEX ix_users_email ON users(email);