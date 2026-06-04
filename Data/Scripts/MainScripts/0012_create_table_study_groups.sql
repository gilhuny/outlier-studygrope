CREATE TABLE study_groups (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name                VARCHAR(200) NOT NULL,
    description         VARCHAR(4000) NOT NULL,
    creator_id          UUID NOT NULL REFERENCES users(id),
    category_code       INT NOT NULL REFERENCES info.info_category(code),
    privacy_code        INT NOT NULL REFERENCES info.info_group_privacy(code),
    cover_img_id        BIGINT REFERENCES contents(id),  -- ← just this
    join_policy_code    INT NOT NULL REFERENCES info.info_join_policy(code),
    status_code         INT NOT NULL REFERENCES info.info_status(code),
    max_members         INT,                        -- NULL = unlimited
    max_miss_days       INT NOT NULL DEFAULT 3,     -- creator sets allowed miss days
    start_date          DATE NOT NULL,
    end_date            DATE,                       -- NULL = lifetime group
    join_deadline       DATE,                       -- NULL = no deadline (depends on join_policy)
    upload_deadline_hour INT NOT NULL DEFAULT 23,   -- hour of day (0-23) for daily proof
    created_date_time   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_user_id    UUID,
    modified_date_time  TIMESTAMPTZ,
    CONSTRAINT chk_end_date CHECK (end_date IS NULL OR end_date > start_date),
    CONSTRAINT chk_join_deadline CHECK (join_deadline IS NULL OR join_deadline >= start_date),
    CONSTRAINT chk_upload_hour CHECK (upload_deadline_hour BETWEEN 0 AND 23),
    CONSTRAINT chk_max_members CHECK (max_members IS NULL OR max_members > 0),
    CONSTRAINT chk_max_miss_days CHECK (max_miss_days >= 0)
);
CREATE INDEX ix_study_groups_creator_id ON study_groups(creator_id);
CREATE INDEX ix_study_groups_status_code ON study_groups(status_code);
CREATE INDEX ix_study_groups_category_code ON study_groups(category_code);
CREATE INDEX ix_study_groups_privacy_code ON study_groups(privacy_code);
CREATE INDEX ix_study_groups_created_date_time ON study_groups(created_date_time);