CREATE TABLE chats (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    chat_type_code      INT NOT NULL REFERENCES info.info_chat_type(code),  -- Group or OneToOne
    name                VARCHAR(200),               -- for group chats
    group_id            UUID REFERENCES study_groups(id),  -- NULL for 1-on-1
    status_code         INT NOT NULL REFERENCES info.info_status(code),
    created_user_id     UUID NOT NULL REFERENCES users(id),
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id    UUID,
    modified_date_time  TIMESTAMPTZ
);
CREATE INDEX ix_chats_status_code ON chats(status_code);
CREATE INDEX ix_chats_group_id ON chats(group_id);
CREATE INDEX ix_chats_chat_type_code ON chats(chat_type_code);
 