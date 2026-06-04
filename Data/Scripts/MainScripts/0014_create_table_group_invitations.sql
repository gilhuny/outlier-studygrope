CREATE TABLE group_invitations (
    id                  BIGSERIAL PRIMARY KEY,
    group_id            UUID NOT NULL REFERENCES study_groups(id),
    invited_by_id       UUID NOT NULL REFERENCES users(id),
    invited_user_id     UUID NOT NULL REFERENCES users(id),
    status_code         INT NOT NULL REFERENCES info.info_status(code), -- Pending, Accepted, Rejected
    message             VARCHAR(500),
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    responded_date_time TIMESTAMPTZ,
    UNIQUE(group_id, invited_user_id)
);
CREATE INDEX ix_group_invitations_group_id ON group_invitations(group_id);
CREATE INDEX ix_group_invitations_invited_user_id ON group_invitations(invited_user_id);
CREATE INDEX ix_group_invitations_status_code ON group_invitations(status_code);