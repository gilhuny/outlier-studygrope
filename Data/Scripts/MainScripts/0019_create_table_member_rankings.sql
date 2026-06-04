CREATE TABLE member_rankings (
    id                      BIGSERIAL PRIMARY KEY,
    group_id                UUID NOT NULL REFERENCES study_groups(id),
    user_id                 UUID NOT NULL REFERENCES users(id),
    total_score             DECIMAL(10,2) NOT NULL DEFAULT 0,
    avg_rating              DECIMAL(3,2) NOT NULL DEFAULT 0,
    upload_count            INT NOT NULL DEFAULT 0,
    miss_count              INT NOT NULL DEFAULT 0,
    current_streak          INT NOT NULL DEFAULT 0,   -- consecutive upload days
    longest_streak          INT NOT NULL DEFAULT 0,
    rank_position           INT,                       -- calculated rank in group
    last_calculated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(group_id, user_id)
);
CREATE INDEX ix_member_rankings_group_id ON member_rankings(group_id);
CREATE INDEX ix_member_rankings_user_id ON member_rankings(user_id);
CREATE INDEX ix_member_rankings_total_score ON member_rankings(group_id, total_score DESC);