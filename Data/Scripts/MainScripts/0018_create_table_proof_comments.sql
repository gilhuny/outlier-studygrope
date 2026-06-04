CREATE TABLE proof_comments (
    id                  BIGSERIAL PRIMARY KEY,
    proof_id            BIGINT NOT NULL REFERENCES daily_proofs(id),
    user_id             UUID NOT NULL REFERENCES users(id),
    comment_text        VARCHAR(1000) NOT NULL,
    status_code         INT NOT NULL REFERENCES info.info_status(code),
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_date_time  TIMESTAMPTZ
);
CREATE INDEX ix_proof_comments_proof_id ON proof_comments(proof_id);
CREATE INDEX ix_proof_comments_user_id ON proof_comments(user_id);
CREATE INDEX ix_proof_comments_status_code ON proof_comments(status_code);