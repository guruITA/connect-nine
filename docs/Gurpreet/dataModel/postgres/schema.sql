-- Connect, the database of docs/Gurpreet/dataModel/eerd.md as PostgreSQL 16 DDL.
-- 44 tables in third normal form. Every column, type, NOT NULL, default, check, unique index and
-- index is the one in the diagram and the build sheet of eerd.md, by the same names. Foreign keys
-- follow the EF Core convention fk_<table>_<column>. A change to this file is a change to eerd.md.
--
-- Run:  docker compose up -d   (loads this file, then dummydata.sql; see README.md)

BEGIN;

CREATE EXTENSION IF NOT EXISTS citext;
CREATE EXTENSION IF NOT EXISTS pg_trgm;

CREATE TABLE account (
    id uuid NOT NULL,
    email citext NOT NULL,
    password_hash text,
    display_name text NOT NULL,
    role text NOT NULL DEFAULT 'Member',
    email_confirmed_at timestamptz,
    created_at timestamptz NOT NULL,
    deactivated_at timestamptz,
    deactivated_by uuid,
    CONSTRAINT pk_account PRIMARY KEY (id),
    CONSTRAINT ck_account_password_hash_not_empty CHECK (password_hash IS NULL OR password_hash <> ''),
    CONSTRAINT ck_account_role CHECK (role IN ('Member', 'Admin')),
    CONSTRAINT ck_account_deactivation CHECK (deactivated_by IS NULL OR deactivated_at IS NOT NULL),
    CONSTRAINT fk_account_deactivated_by FOREIGN KEY (deactivated_by) REFERENCES account (id) ON DELETE SET NULL
);

CREATE TABLE session (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    token_hash text NOT NULL,
    created_at timestamptz NOT NULL,
    expires_at timestamptz NOT NULL,
    revoked_at timestamptz,
    last_active_at timestamptz NOT NULL,
    CONSTRAINT pk_session PRIMARY KEY (id),
    CONSTRAINT fk_session_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE password_reset (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    token_hash text NOT NULL,
    created_at timestamptz NOT NULL,
    expires_at timestamptz NOT NULL,
    used_at timestamptz,
    CONSTRAINT pk_password_reset PRIMARY KEY (id),
    CONSTRAINT fk_password_reset_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE email_confirmation (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    token_hash text NOT NULL,
    created_at timestamptz NOT NULL,
    expires_at timestamptz NOT NULL,
    used_at timestamptz,
    CONSTRAINT pk_email_confirmation PRIMARY KEY (id),
    CONSTRAINT fk_email_confirmation_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE email_change (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    new_email citext NOT NULL,
    token_hash text NOT NULL,
    created_at timestamptz NOT NULL,
    expires_at timestamptz NOT NULL,
    used_at timestamptz,
    CONSTRAINT pk_email_change PRIMARY KEY (id),
    CONSTRAINT fk_email_change_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE external_login (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    provider text NOT NULL,
    provider_user_id text NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_external_login PRIMARY KEY (id),
    CONSTRAINT ck_external_login_provider CHECK (provider IN ('Google', 'Facebook')),
    CONSTRAINT fk_external_login_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE pending_external_login (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    provider text NOT NULL,
    provider_user_id text NOT NULL,
    token_hash text NOT NULL,
    failed_attempts integer NOT NULL,
    created_at timestamptz NOT NULL,
    expires_at timestamptz NOT NULL,
    CONSTRAINT pk_pending_external_login PRIMARY KEY (id),
    CONSTRAINT fk_pending_external_login_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE log_in_attempt (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    outcome text NOT NULL,
    method text NOT NULL,
    attempted_at timestamptz NOT NULL,
    CONSTRAINT pk_log_in_attempt PRIMARY KEY (id),
    CONSTRAINT fk_log_in_attempt_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE suspicious_activity_alert (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    rule text NOT NULL,
    sent_at timestamptz NOT NULL,
    CONSTRAINT pk_suspicious_activity_alert PRIMARY KEY (id),
    CONSTRAINT fk_suspicious_activity_alert_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE login_failure (
    id uuid NOT NULL,
    email citext NOT NULL,
    failed_at timestamptz NOT NULL,
    CONSTRAINT pk_login_failure PRIMARY KEY (id)
);

CREATE TABLE login_lock (
    id uuid NOT NULL,
    email citext NOT NULL,
    locked_until timestamptz NOT NULL,
    created_at timestamptz NOT NULL,
    warning_handled_at timestamptz,
    CONSTRAINT pk_login_lock PRIMARY KEY (id)
);

CREATE TABLE log_in_puzzle_use (
    id uuid NOT NULL,
    challenge_hash text NOT NULL,
    used_at timestamptz NOT NULL,
    CONSTRAINT pk_log_in_puzzle_use PRIMARY KEY (id)
);

CREATE TABLE audit_entry (
    id uuid NOT NULL,
    actor_account_id uuid,
    action text NOT NULL,
    target_kind text NOT NULL,
    target_id uuid,
    occurred_at timestamptz NOT NULL DEFAULT now(),
    detail text,
    CONSTRAINT pk_audit_entry PRIMARY KEY (id)
);

CREATE TABLE member_profile (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    first_name text NOT NULL,
    age_band text NOT NULL,
    region text NOT NULL,
    bio text,
    created_at timestamptz NOT NULL,
    updated_at timestamptz,
    bio_removed_at timestamptz,
    bio_removed_reason text,
    bio_removed_by uuid,
    CONSTRAINT pk_member_profile PRIMARY KEY (id),
    CONSTRAINT ck_member_profile_age_band CHECK (age_band IN ('From15To17', 'From18To21', 'From22To30')),
    CONSTRAINT ck_member_profile_bio_removed_reason CHECK (bio_removed_reason IS NULL OR bio_removed_reason IN ('NotAllowed', 'PersonalData', 'Spam', 'Other')),
    CONSTRAINT ck_member_profile_bio_removal_complete CHECK ((bio_removed_at IS NULL) = (bio_removed_reason IS NULL)),
    CONSTRAINT fk_member_profile_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE,
    CONSTRAINT fk_member_profile_bio_removed_by FOREIGN KEY (bio_removed_by) REFERENCES account (id) ON DELETE SET NULL
);

CREATE TABLE organisation_profile (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    name text NOT NULL,
    kind text NOT NULL,
    description text NOT NULL,
    website text,
    created_at timestamptz NOT NULL,
    updated_at timestamptz,
    checked_at timestamptz,
    checked_by uuid,
    CONSTRAINT pk_organisation_profile PRIMARY KEY (id),
    CONSTRAINT ck_organisation_profile_kind CHECK (kind IN ('Company', 'School', 'Municipality', 'Club', 'Fund')),
    CONSTRAINT ck_organisation_profile_check CHECK (checked_by IS NULL OR checked_at IS NOT NULL),
    CONSTRAINT fk_organisation_profile_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE,
    CONSTRAINT fk_organisation_profile_checked_by FOREIGN KEY (checked_by) REFERENCES account (id) ON DELETE SET NULL
);

CREATE TABLE organisation_logo (
    organisation_profile_id uuid NOT NULL,
    logo_file_name text NOT NULL,
    logo_width integer NOT NULL,
    logo_height integer NOT NULL,
    CONSTRAINT pk_organisation_logo PRIMARY KEY (organisation_profile_id),
    CONSTRAINT ck_organisation_logo_size CHECK (logo_width > 0 AND logo_height > 0),
    CONSTRAINT fk_organisation_logo_organisation_profile_id FOREIGN KEY (organisation_profile_id) REFERENCES organisation_profile (id) ON DELETE CASCADE
);

CREATE TABLE parent_consent (
    id uuid NOT NULL,
    member_profile_id uuid NOT NULL,
    parent_email citext,
    token_hash text NOT NULL,
    created_at timestamptz NOT NULL,
    expires_at timestamptz NOT NULL,
    given_at timestamptz,
    CONSTRAINT pk_parent_consent PRIMARY KEY (id),
    CONSTRAINT fk_parent_consent_member_profile_id FOREIGN KEY (member_profile_id) REFERENCES member_profile (id) ON DELETE CASCADE
);

CREATE TABLE tag (
    id uuid NOT NULL,
    name citext NOT NULL,
    is_suggested boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_tag PRIMARY KEY (id),
    CONSTRAINT ck_tag_name_length CHECK (char_length(name) BETWEEN 1 AND 30)
);

CREATE TABLE member_profile_tag (
    member_profile_id uuid NOT NULL,
    tag_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_member_profile_tag PRIMARY KEY (member_profile_id, tag_id),
    CONSTRAINT fk_member_profile_tag_member_profile_id FOREIGN KEY (member_profile_id) REFERENCES member_profile (id) ON DELETE CASCADE,
    CONSTRAINT fk_member_profile_tag_tag_id FOREIGN KEY (tag_id) REFERENCES tag (id) ON DELETE RESTRICT
);

CREATE TABLE organisation_profile_tag (
    organisation_profile_id uuid NOT NULL,
    tag_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_organisation_profile_tag PRIMARY KEY (organisation_profile_id, tag_id),
    CONSTRAINT fk_organisation_profile_tag_organisation_profile_id FOREIGN KEY (organisation_profile_id) REFERENCES organisation_profile (id) ON DELETE CASCADE,
    CONSTRAINT fk_organisation_profile_tag_tag_id FOREIGN KEY (tag_id) REFERENCES tag (id) ON DELETE RESTRICT
);

CREATE TABLE connection_request (
    id uuid NOT NULL,
    requester_id uuid NOT NULL,
    receiver_id uuid NOT NULL,
    status text NOT NULL,
    requested_at timestamptz NOT NULL,
    answered_at timestamptz,
    CONSTRAINT pk_connection_request PRIMARY KEY (id),
    CONSTRAINT ck_connection_request_not_self CHECK (requester_id <> receiver_id),
    CONSTRAINT ck_connection_request_status CHECK (status IN ('Requested', 'Accepted', 'Declined')),
    CONSTRAINT fk_connection_request_requester_id FOREIGN KEY (requester_id) REFERENCES member_profile (id) ON DELETE CASCADE,
    CONSTRAINT fk_connection_request_receiver_id FOREIGN KEY (receiver_id) REFERENCES member_profile (id) ON DELETE CASCADE
);

CREATE TABLE member_profile_badge (
    id uuid NOT NULL,
    member_profile_id uuid NOT NULL,
    badge_code text NOT NULL,
    earned_at timestamptz NOT NULL,
    CONSTRAINT pk_member_profile_badge PRIMARY KEY (id),
    CONSTRAINT fk_member_profile_badge_member_profile_id FOREIGN KEY (member_profile_id) REFERENCES member_profile (id) ON DELETE CASCADE
);

CREATE TABLE saved_item (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    kind text NOT NULL,
    item_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_saved_item PRIMARY KEY (id),
    CONSTRAINT ck_saved_item_kind CHECK (kind IN ('Member', 'Organisation', 'Event')),
    CONSTRAINT fk_saved_item_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE project (
    id uuid NOT NULL,
    member_profile_id uuid NOT NULL,
    title text NOT NULL,
    description text NOT NULL,
    created_at timestamptz NOT NULL,
    updated_at timestamptz,
    removed_at timestamptz,
    removed_reason text,
    removed_by uuid,
    removal_notice_dismissed_at timestamptz,
    CONSTRAINT pk_project PRIMARY KEY (id),
    CONSTRAINT ck_project_title_length CHECK (char_length(title) <= 80),
    CONSTRAINT ck_project_description_length CHECK (char_length(description) <= 1000),
    CONSTRAINT ck_project_removed_reason CHECK (removed_reason IS NULL OR removed_reason IN ('NotAllowed', 'PersonalData', 'Spam', 'Other')),
    CONSTRAINT ck_project_removal_complete CHECK ((removed_at IS NULL) = (removed_reason IS NULL)),
    CONSTRAINT ck_project_removal_notice_dismissed CHECK (removal_notice_dismissed_at IS NULL OR removed_at IS NOT NULL),
    CONSTRAINT fk_project_member_profile_id FOREIGN KEY (member_profile_id) REFERENCES member_profile (id) ON DELETE CASCADE,
    CONSTRAINT fk_project_removed_by FOREIGN KEY (removed_by) REFERENCES account (id) ON DELETE SET NULL
);

CREATE TABLE project_image (
    project_id uuid NOT NULL,
    image_file_name text NOT NULL,
    image_width integer NOT NULL,
    image_height integer NOT NULL,
    CONSTRAINT pk_project_image PRIMARY KEY (project_id),
    CONSTRAINT ck_project_image_size CHECK (image_width > 0 AND image_height > 0),
    CONSTRAINT fk_project_image_project_id FOREIGN KEY (project_id) REFERENCES project (id) ON DELETE CASCADE
);

CREATE TABLE removal_review_request (
    id uuid NOT NULL,
    project_id uuid NOT NULL,
    account_id uuid NOT NULL,
    text text NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_removal_review_request PRIMARY KEY (id),
    CONSTRAINT ck_removal_review_request_text_length CHECK (char_length(text) BETWEEN 1 AND 500),
    CONSTRAINT fk_removal_review_request_project_id FOREIGN KEY (project_id) REFERENCES project (id) ON DELETE CASCADE,
    CONSTRAINT fk_removal_review_request_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE opportunity (
    id uuid NOT NULL,
    organisation_profile_id uuid NOT NULL,
    title text NOT NULL,
    description text NOT NULL,
    kind text NOT NULL,
    closing_date date NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_opportunity PRIMARY KEY (id),
    CONSTRAINT ck_opportunity_title_length CHECK (char_length(title) BETWEEN 1 AND 120),
    CONSTRAINT ck_opportunity_description_length CHECK (char_length(description) BETWEEN 1 AND 4000),
    CONSTRAINT ck_opportunity_kind CHECK (kind IN ('Internship', 'Job', 'Project', 'Other')),
    CONSTRAINT fk_opportunity_organisation_profile_id FOREIGN KEY (organisation_profile_id) REFERENCES organisation_profile (id) ON DELETE CASCADE
);

CREATE TABLE opportunity_tag (
    opportunity_id uuid NOT NULL,
    tag_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_opportunity_tag PRIMARY KEY (opportunity_id, tag_id),
    CONSTRAINT fk_opportunity_tag_opportunity_id FOREIGN KEY (opportunity_id) REFERENCES opportunity (id) ON DELETE CASCADE,
    CONSTRAINT fk_opportunity_tag_tag_id FOREIGN KEY (tag_id) REFERENCES tag (id) ON DELETE RESTRICT
);

CREATE TABLE opportunity_member (
    opportunity_id uuid NOT NULL,
    member_profile_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_opportunity_member PRIMARY KEY (opportunity_id, member_profile_id),
    CONSTRAINT fk_opportunity_member_opportunity_id FOREIGN KEY (opportunity_id) REFERENCES opportunity (id) ON DELETE CASCADE,
    CONSTRAINT fk_opportunity_member_member_profile_id FOREIGN KEY (member_profile_id) REFERENCES member_profile (id) ON DELETE CASCADE
);

CREATE TABLE event (
    id uuid NOT NULL,
    organisation_profile_id uuid NOT NULL,
    title text NOT NULL,
    description text NOT NULL,
    place text NOT NULL,
    starts_at timestamptz NOT NULL,
    capacity integer NOT NULL,
    created_at timestamptz NOT NULL,
    updated_at timestamptz,
    cancelled_at timestamptz,
    CONSTRAINT pk_event PRIMARY KEY (id),
    CONSTRAINT ck_event_capacity_positive CHECK (capacity > 0),
    CONSTRAINT fk_event_organisation_profile_id FOREIGN KEY (organisation_profile_id) REFERENCES organisation_profile (id) ON DELETE CASCADE
);

CREATE TABLE event_member (
    id uuid NOT NULL,
    event_id uuid NOT NULL,
    account_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_event_member PRIMARY KEY (id),
    CONSTRAINT fk_event_member_event_id FOREIGN KEY (event_id) REFERENCES event (id) ON DELETE CASCADE,
    CONSTRAINT fk_event_member_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE challenge (
    id uuid NOT NULL,
    organisation_profile_id uuid NOT NULL,
    title text NOT NULL,
    description text NOT NULL,
    deadline date NOT NULL,
    created_at timestamptz NOT NULL,
    updated_at timestamptz,
    removed_at timestamptz,
    CONSTRAINT pk_challenge PRIMARY KEY (id),
    CONSTRAINT fk_challenge_organisation_profile_id FOREIGN KEY (organisation_profile_id) REFERENCES organisation_profile (id) ON DELETE CASCADE
);

CREATE TABLE challenge_member (
    id uuid NOT NULL,
    challenge_id uuid NOT NULL,
    member_profile_id uuid NOT NULL,
    joined_at timestamptz NOT NULL,
    CONSTRAINT pk_challenge_member PRIMARY KEY (id),
    CONSTRAINT fk_challenge_member_challenge_id FOREIGN KEY (challenge_id) REFERENCES challenge (id) ON DELETE CASCADE,
    CONSTRAINT fk_challenge_member_member_profile_id FOREIGN KEY (member_profile_id) REFERENCES member_profile (id) ON DELETE CASCADE
);

CREATE TABLE news_item (
    id uuid NOT NULL,
    author_account_id uuid,
    title text NOT NULL,
    body text NOT NULL,
    published_at timestamptz NOT NULL,
    changed_at timestamptz,
    CONSTRAINT pk_news_item PRIMARY KEY (id),
    CONSTRAINT fk_news_item_author_account_id FOREIGN KEY (author_account_id) REFERENCES account (id) ON DELETE SET NULL
);

CREATE TABLE news_image (
    news_item_id uuid NOT NULL,
    image_file_name text NOT NULL,
    image_alt_text text NOT NULL,
    CONSTRAINT pk_news_image PRIMARY KEY (news_item_id),
    CONSTRAINT ck_news_image_alt_text CHECK (length(btrim(image_alt_text)) > 0),
    CONSTRAINT fk_news_image_news_item_id FOREIGN KEY (news_item_id) REFERENCES news_item (id) ON DELETE CASCADE
);

CREATE TABLE site_page (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    slug text NOT NULL,
    title text NOT NULL,
    body text NOT NULL,
    is_protected boolean NOT NULL DEFAULT false,
    updated_at timestamptz NOT NULL,
    updated_by uuid,
    CONSTRAINT pk_site_page PRIMARY KEY (id),
    CONSTRAINT ck_site_page_slug CHECK (slug ~ '^[a-z0-9-]{1,50}$'),
    CONSTRAINT ck_site_page_title_length CHECK (char_length(title) <= 120),
    CONSTRAINT fk_site_page_updated_by FOREIGN KEY (updated_by) REFERENCES account (id) ON DELETE SET NULL
);

CREATE TABLE site_page_version (
    id uuid NOT NULL,
    site_page_id uuid NOT NULL,
    title text NOT NULL,
    body text NOT NULL,
    updated_at timestamptz NOT NULL,
    updated_by uuid,
    replaced_at timestamptz NOT NULL,
    CONSTRAINT pk_site_page_version PRIMARY KEY (id),
    CONSTRAINT fk_site_page_version_site_page_id FOREIGN KEY (site_page_id) REFERENCES site_page (id) ON DELETE CASCADE,
    CONSTRAINT fk_site_page_version_updated_by FOREIGN KEY (updated_by) REFERENCES account (id) ON DELETE SET NULL
);

CREATE TABLE member_report (
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    reporter_account_id uuid,
    reported_account_id uuid NOT NULL,
    reason text NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_member_report PRIMARY KEY (id),
    CONSTRAINT ck_member_report_reason CHECK (reason IN ('BullyingOrHate', 'SexualMessages', 'PersonalDetailsOrMeeting', 'SharingWithoutAsking', 'PretendingToBeSomeoneElse', 'SellingOrAdvertising', 'Other')),
    CONSTRAINT fk_member_report_reporter_account_id FOREIGN KEY (reporter_account_id) REFERENCES account (id) ON DELETE SET NULL,
    CONSTRAINT fk_member_report_reported_account_id FOREIGN KEY (reported_account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE product (
    id uuid NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    CONSTRAINT pk_product PRIMARY KEY (id)
);

CREATE TABLE product_version (
    id uuid NOT NULL,
    product_id uuid NOT NULL,
    name text NOT NULL,
    description text NOT NULL,
    price_in_cents integer NOT NULL,
    vat_included boolean NOT NULL,
    image_file text,
    image_stored_name text,
    valid_from timestamptz NOT NULL,
    CONSTRAINT pk_product_version PRIMARY KEY (id),
    CONSTRAINT ck_product_version_price_in_cents CHECK (price_in_cents >= 0),
    CONSTRAINT ck_product_version_image_one_source CHECK ((image_file IS NULL) <> (image_stored_name IS NULL)),
    CONSTRAINT fk_product_version_product_id FOREIGN KEY (product_id) REFERENCES product (id) ON DELETE RESTRICT
);

CREATE TABLE basket (
    id uuid NOT NULL,
    account_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    updated_at timestamptz NOT NULL,
    CONSTRAINT pk_basket PRIMARY KEY (id),
    CONSTRAINT fk_basket_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE CASCADE
);

CREATE TABLE basket_item (
    id uuid NOT NULL,
    basket_id uuid NOT NULL,
    product_id uuid NOT NULL,
    quantity integer NOT NULL,
    CONSTRAINT pk_basket_item PRIMARY KEY (id),
    CONSTRAINT ck_basket_item_quantity CHECK (quantity BETWEEN 1 AND 10),
    CONSTRAINT fk_basket_item_basket_id FOREIGN KEY (basket_id) REFERENCES basket (id) ON DELETE CASCADE,
    CONSTRAINT fk_basket_item_product_id FOREIGN KEY (product_id) REFERENCES product (id) ON DELETE RESTRICT
);

CREATE TABLE shop_order (
    id uuid NOT NULL,
    order_number text NOT NULL,
    account_id uuid,
    total_in_cents integer NOT NULL,
    status text NOT NULL,
    provider_payment_id text,
    created_at timestamptz NOT NULL,
    paid_at timestamptz,
    confirmation_sent_at timestamptz,
    CONSTRAINT pk_shop_order PRIMARY KEY (id),
    CONSTRAINT ck_shop_order_total_in_cents CHECK (total_in_cents >= 0),
    CONSTRAINT ck_shop_order_status CHECK (status IN ('Pending', 'Paid', 'Cancelled', 'Failed')),
    CONSTRAINT fk_shop_order_account_id FOREIGN KEY (account_id) REFERENCES account (id) ON DELETE SET NULL
);

CREATE TABLE shop_order_item (
    id uuid NOT NULL,
    shop_order_id uuid NOT NULL,
    product_version_id uuid NOT NULL,
    quantity integer NOT NULL,
    CONSTRAINT pk_shop_order_item PRIMARY KEY (id),
    CONSTRAINT ck_shop_order_item_quantity CHECK (quantity >= 1),
    CONSTRAINT fk_shop_order_item_shop_order_id FOREIGN KEY (shop_order_id) REFERENCES shop_order (id) ON DELETE CASCADE,
    CONSTRAINT fk_shop_order_item_product_version_id FOREIGN KEY (product_version_id) REFERENCES product_version (id) ON DELETE RESTRICT
);

-- Unique and plain indexes, by the names of the build sheet

CREATE UNIQUE INDEX ux_account_email ON account (email);
CREATE INDEX ix_account_created_at ON account (created_at);
CREATE INDEX ix_account_unconfirmed_created_at ON account (created_at) WHERE email_confirmed_at IS NULL;
CREATE INDEX ix_account_deactivated_by ON account (deactivated_by) WHERE deactivated_by IS NOT NULL;
CREATE INDEX ix_account_display_name_trgm ON account USING gin (display_name gin_trgm_ops);
CREATE UNIQUE INDEX ux_session_token_hash ON session (token_hash);
CREATE INDEX ix_session_account_id ON session (account_id);
CREATE INDEX ix_session_expires_at ON session (expires_at);
CREATE UNIQUE INDEX ux_password_reset_token_hash ON password_reset (token_hash);
CREATE INDEX ix_password_reset_account_id ON password_reset (account_id);
CREATE INDEX ix_password_reset_expires_at ON password_reset (expires_at);
CREATE UNIQUE INDEX ux_email_confirmation_token_hash ON email_confirmation (token_hash);
CREATE INDEX ix_email_confirmation_account_id ON email_confirmation (account_id);
CREATE INDEX ix_email_confirmation_expires_at ON email_confirmation (expires_at);
CREATE UNIQUE INDEX ux_email_change_token_hash ON email_change (token_hash);
CREATE INDEX ix_email_change_account_id ON email_change (account_id);
CREATE INDEX ix_email_change_expires_at ON email_change (expires_at);
CREATE UNIQUE INDEX ux_external_login_provider_provider_user_id ON external_login (provider, provider_user_id);
CREATE INDEX ix_external_login_account_id ON external_login (account_id);
CREATE UNIQUE INDEX ux_pending_external_login_token_hash ON pending_external_login (token_hash);
CREATE INDEX ix_pending_external_login_account_id ON pending_external_login (account_id);
CREATE INDEX ix_pending_external_login_expires_at ON pending_external_login (expires_at);
CREATE INDEX ix_log_in_attempt_account_id_attempted_at ON log_in_attempt (account_id, attempted_at);
CREATE INDEX ix_log_in_attempt_attempted_at ON log_in_attempt (attempted_at);
CREATE INDEX ix_suspicious_activity_alert_account_id_rule_sent_at ON suspicious_activity_alert (account_id, rule, sent_at);
CREATE INDEX ix_login_failure_email_failed_at ON login_failure (email, failed_at);
CREATE INDEX ix_login_failure_failed_at ON login_failure (failed_at);
CREATE INDEX ix_login_lock_email_locked_until ON login_lock (email, locked_until);
CREATE INDEX ix_login_lock_warning_handled_at ON login_lock (created_at) WHERE warning_handled_at IS NULL;
CREATE INDEX ix_login_lock_locked_until ON login_lock (locked_until);
CREATE UNIQUE INDEX ux_log_in_puzzle_use_challenge_hash ON log_in_puzzle_use (challenge_hash);
CREATE INDEX ix_log_in_puzzle_use_used_at ON log_in_puzzle_use (used_at);
CREATE INDEX ix_audit_entry_occurred_at ON audit_entry (occurred_at DESC);
CREATE INDEX ix_audit_entry_actor_account_id_occurred_at ON audit_entry (actor_account_id, occurred_at);
CREATE INDEX ix_audit_entry_target_id_occurred_at ON audit_entry (target_id, occurred_at);
CREATE UNIQUE INDEX ux_member_profile_account_id ON member_profile (account_id);
CREATE INDEX ix_member_profile_bio_removed_by ON member_profile (bio_removed_by) WHERE bio_removed_by IS NOT NULL;
CREATE UNIQUE INDEX ux_organisation_profile_account_id ON organisation_profile (account_id);
CREATE INDEX ix_organisation_profile_checked_by ON organisation_profile (checked_by) WHERE checked_by IS NOT NULL;
CREATE UNIQUE INDEX ux_parent_consent_member_profile_id ON parent_consent (member_profile_id);
CREATE UNIQUE INDEX ux_parent_consent_token_hash ON parent_consent (token_hash);
CREATE INDEX ix_parent_consent_expires_at ON parent_consent (expires_at);
CREATE UNIQUE INDEX ux_tag_name ON tag (name);
CREATE INDEX ix_member_profile_tag_tag_id ON member_profile_tag (tag_id);
CREATE INDEX ix_organisation_profile_tag_tag_id ON organisation_profile_tag (tag_id);
CREATE UNIQUE INDEX ux_connection_request_pair ON connection_request (least(requester_id, receiver_id), greatest(requester_id, receiver_id));
CREATE INDEX ix_connection_request_requester_id_status ON connection_request (requester_id, status, requested_at DESC, id);
CREATE INDEX ix_connection_request_receiver_id_status ON connection_request (receiver_id, status, requested_at DESC, id);
CREATE INDEX ix_connection_request_answered_at ON connection_request (answered_at);
CREATE UNIQUE INDEX ux_member_profile_badge_member_profile_id_badge_code ON member_profile_badge (member_profile_id, badge_code);
CREATE UNIQUE INDEX ux_saved_item_account_id_kind_item_id ON saved_item (account_id, kind, item_id);
CREATE INDEX ix_saved_item_account_id_created_at ON saved_item (account_id, created_at);
CREATE INDEX ix_project_member_profile_id ON project (member_profile_id);
CREATE INDEX ix_project_member_profile_id_created_at_id ON project (member_profile_id, created_at DESC, id DESC) WHERE removed_at IS NULL;
CREATE INDEX ix_project_created_at_id ON project (created_at DESC, id DESC) WHERE removed_at IS NULL;
CREATE INDEX ix_project_removed_at ON project (removed_at) WHERE removed_at IS NOT NULL;
CREATE INDEX ix_project_removed_by ON project (removed_by) WHERE removed_by IS NOT NULL;
CREATE UNIQUE INDEX ux_removal_review_request_project_id ON removal_review_request (project_id);
CREATE INDEX ix_removal_review_request_account_id ON removal_review_request (account_id);
CREATE INDEX ix_opportunity_organisation_profile_id ON opportunity (organisation_profile_id);
CREATE INDEX ix_opportunity_created_at_id ON opportunity (created_at DESC, id DESC);
CREATE INDEX ix_opportunity_tag_tag_id ON opportunity_tag (tag_id);
CREATE INDEX ix_opportunity_member_member_profile_id_created_at ON opportunity_member (member_profile_id, created_at DESC);
CREATE INDEX ix_opportunity_member_opportunity_id_created_at ON opportunity_member (opportunity_id, created_at DESC);
CREATE INDEX ix_event_organisation_profile_id ON event (organisation_profile_id);
CREATE INDEX ix_event_starts_at ON event (starts_at);
CREATE UNIQUE INDEX ux_event_member_event_id_account_id ON event_member (event_id, account_id);
CREATE INDEX ix_event_member_account_id ON event_member (account_id);
CREATE INDEX ix_challenge_organisation_profile_id ON challenge (organisation_profile_id);
CREATE INDEX ix_challenge_deadline ON challenge (deadline);
CREATE UNIQUE INDEX ux_challenge_member_challenge_id_member_profile_id ON challenge_member (challenge_id, member_profile_id);
CREATE INDEX ix_challenge_member_member_profile_id ON challenge_member (member_profile_id);
CREATE INDEX ix_challenge_member_challenge_id_joined_at ON challenge_member (challenge_id, joined_at);
CREATE INDEX ix_news_item_published_at_id ON news_item (published_at DESC, id DESC);
CREATE INDEX ix_news_item_author_account_id ON news_item (author_account_id);
CREATE UNIQUE INDEX ux_site_page_slug ON site_page (slug);
CREATE INDEX ix_site_page_updated_by ON site_page (updated_by);
CREATE INDEX ix_site_page_version_site_page_id ON site_page_version (site_page_id);
CREATE INDEX ix_site_page_version_site_page_id_replaced_at ON site_page_version (site_page_id, replaced_at DESC);
CREATE INDEX ix_site_page_version_updated_by ON site_page_version (updated_by);
CREATE UNIQUE INDEX ux_member_report_reporter_account_id_reported_account_id ON member_report (reporter_account_id, reported_account_id);
CREATE INDEX ix_member_report_reported_account_id ON member_report (reported_account_id);
CREATE INDEX ix_member_report_created_at ON member_report (created_at);
CREATE UNIQUE INDEX ux_product_version_product_id_valid_from ON product_version (product_id, valid_from);
CREATE UNIQUE INDEX ux_basket_account_id ON basket (account_id);
CREATE UNIQUE INDEX ux_basket_item_basket_id_product_id ON basket_item (basket_id, product_id);
CREATE INDEX ix_basket_item_product_id ON basket_item (product_id);
CREATE UNIQUE INDEX ux_shop_order_order_number ON shop_order (order_number);
CREATE UNIQUE INDEX ux_shop_order_provider_payment_id ON shop_order (provider_payment_id);
CREATE INDEX ix_shop_order_account_id ON shop_order (account_id);
CREATE INDEX ix_shop_order_item_shop_order_id ON shop_order_item (shop_order_id);
CREATE INDEX ix_shop_order_item_product_version_id ON shop_order_item (product_version_id);

-- audit_entry is append only (task 32.1). Task 120.1 allows one exception: the role
-- connect_audit_cleanup may delete lines older than twelve months.
DO $$ BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'connect_audit_cleanup') THEN
        CREATE ROLE connect_audit_cleanup NOLOGIN;
    END IF;
END $$;
GRANT SELECT, DELETE ON audit_entry TO connect_audit_cleanup;

CREATE FUNCTION audit_entry_append_only() RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
    IF TG_OP = 'DELETE' AND current_user = 'connect_audit_cleanup'
       AND OLD.occurred_at < now() - interval '12 months' THEN
        RETURN OLD;
    END IF;
    RAISE EXCEPTION 'audit_entry is append only';
END $$;

CREATE TRIGGER trg_audit_entry_no_update_delete BEFORE UPDATE OR DELETE ON audit_entry
    FOR EACH ROW EXECUTE FUNCTION audit_entry_append_only();
CREATE TRIGGER trg_audit_entry_no_truncate BEFORE TRUNCATE ON audit_entry
    FOR EACH STATEMENT EXECUTE FUNCTION audit_entry_append_only();

COMMIT;
