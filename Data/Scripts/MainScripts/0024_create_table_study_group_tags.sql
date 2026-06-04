CREATE TABLE public.study_group_tags (
    id                BIGSERIAL PRIMARY KEY,
    group_id          UUID NOT NULL REFERENCES study_groups(id),
    tag_code          INT NOT NULL REFERENCES info.info_tag(code),
    created_user_id   UUID NOT NULL REFERENCES users(id),
    created_date_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(group_id, tag_code)
);
CREATE INDEX ix_study_group_tags_group_id ON study_group_tags(group_id);
CREATE INDEX ix_study_group_tags_tag_code ON study_group_tags(tag_code);