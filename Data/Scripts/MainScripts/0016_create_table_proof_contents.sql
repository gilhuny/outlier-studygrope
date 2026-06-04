-- Proof Contents (files attached to proofs — images, PDFs)
CREATE TABLE proof_contents (
    id                  BIGSERIAL PRIMARY KEY,
    proof_id            BIGINT NOT NULL REFERENCES daily_proofs(id),
    content_id          BIGINT NOT NULL REFERENCES contents(id),
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX ix_proof_contents_proof_id ON proof_contents(proof_id);
CREATE INDEX ix_proof_contents_content_id ON proof_contents(content_id);