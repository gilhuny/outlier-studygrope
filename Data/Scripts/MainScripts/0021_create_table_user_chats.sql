CREATE TABLE user_chats (
    id                  BIGSERIAL PRIMARY KEY,
    chat_id             UUID NOT NULL REFERENCES chats(id),
    user_id             UUID NOT NULL REFERENCES users(id),
    status_code         INT NOT NULL REFERENCES info.info_status(code),
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_date_time  TIMESTAMPTZ,
    UNIQUE(chat_id, user_id)
);
CREATE INDEX ix_user_chats_chat_id ON user_chats(chat_id);
CREATE INDEX ix_user_chats_user_id ON user_chats(user_id);
CREATE INDEX ix_user_chats_status_code ON user_chats(status_code);
CREATE INDEX ix_user_chats_user_status ON user_chats(user_id, status_code);