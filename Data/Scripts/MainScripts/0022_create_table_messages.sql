CREATE TABLE messages (
    id                  BIGSERIAL PRIMARY KEY,
    chat_id             UUID NOT NULL REFERENCES chats(id),
    from_user_id        UUID NOT NULL REFERENCES users(id),
    message_text        VARCHAR(4000) NOT NULL,
    reply_to_message_id BIGINT REFERENCES messages(id),
    read_at             TIMESTAMPTZ,               -- for 1-on-1; NULL = unread
    status_code         INT NOT NULL REFERENCES info.info_status(code),
    created_user_id     UUID NOT NULL REFERENCES users(id),
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id    UUID,
    modified_date_time  TIMESTAMPTZ
);
CREATE INDEX ix_messages_chat_id ON messages(chat_id);
CREATE INDEX ix_messages_from_user_id ON messages(from_user_id);
CREATE INDEX ix_messages_status_code ON messages(status_code);
CREATE INDEX ix_messages_reply_to ON messages(reply_to_message_id);
CREATE INDEX ix_messages_chat_created ON messages(chat_id, created_date_time);
CREATE INDEX ix_messages_chat_status ON messages(chat_id, status_code);