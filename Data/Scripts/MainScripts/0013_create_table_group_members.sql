CREATE TABLE group_members (
    id                  BIGSERIAL PRIMARY KEY,
    group_id            UUID NOT NULL REFERENCES study_groups(id),
    user_id             UUID NOT NULL REFERENCES users(id),
    status_code         INT NOT NULL REFERENCES info.info_status(code),
    miss_count          INT NOT NULL DEFAULT 0,
    joined_date_time    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    left_date_time      TIMESTAMPTZ,
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_date_time  TIMESTAMPTZ,
    UNIQUE(group_id, user_id)
);
CREATE INDEX ix_group_members_group_id ON group_members(group_id);
CREATE INDEX ix_group_members_user_id ON group_members(user_id);
CREATE INDEX ix_group_members_status_code ON group_members(status_code);
CREATE INDEX ix_group_members_group_status ON group_members(group_id, status_code);