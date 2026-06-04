CREATE TABLE daily_proofs (
    id                  BIGSERIAL PRIMARY KEY,
    group_id            UUID NOT NULL REFERENCES study_groups(id),
    user_id             UUID NOT NULL REFERENCES users(id),
    proof_date          DATE NOT NULL,             -- the day this proof is for
    proof_type_code     INT NOT NULL REFERENCES info.info_proof_type(code),
    text_content        VARCHAR(4000),             -- for text proofs
    status_code         INT NOT NULL REFERENCES info.info_status(code),
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_date_time  TIMESTAMPTZ,
    UNIQUE(group_id, user_id, proof_date),        -- one proof per user per day per group
    CONSTRAINT chk_text_content CHECK (
        proof_type_code != 3 OR text_content IS NOT NULL  -- type 3 = Text must have content
    )
);
CREATE INDEX ix_daily_proofs_group_id ON daily_proofs(group_id);
CREATE INDEX ix_daily_proofs_user_id ON daily_proofs(user_id);
CREATE INDEX ix_daily_proofs_proof_date ON daily_proofs(proof_date);
CREATE INDEX ix_daily_proofs_status_code ON daily_proofs(status_code);
CREATE INDEX ix_daily_proofs_group_date ON daily_proofs(group_id, proof_date);