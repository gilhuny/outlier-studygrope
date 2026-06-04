CREATE TABLE message_read_receipts (
    id                  BIGSERIAL PRIMARY KEY,
    message_id          BIGINT NOT NULL REFERENCES messages(id),
    user_id             UUID NOT NULL REFERENCES users(id),
    read_at             TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(message_id, user_id)
);
CREATE INDEX ix_message_read_receipts_message_id ON message_read_receipts(message_id);
CREATE INDEX ix_message_read_receipts_user_id ON message_read_receipts(user_id);