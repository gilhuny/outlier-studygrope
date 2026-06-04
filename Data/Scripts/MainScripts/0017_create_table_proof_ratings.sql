CREATE TABLE proof_ratings (
    id                  BIGSERIAL PRIMARY KEY,
    proof_id            BIGINT NOT NULL REFERENCES daily_proofs(id),
    rated_by_user_id    UUID NOT NULL REFERENCES users(id),
    rating              SMALLINT NOT NULL,
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(proof_id, rated_by_user_id),           -- one rating per user per proof
    CONSTRAINT chk_rating CHECK (rating BETWEEN 1 AND 5)
);
CREATE INDEX ix_proof_ratings_proof_id ON proof_ratings(proof_id);
CREATE INDEX ix_proof_ratings_rated_by ON proof_ratings(rated_by_user_id);