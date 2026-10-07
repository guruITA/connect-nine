-- Connect, dummy data for schema.sql.
-- A small, made up Young Amstelland: one admin, eight young people, three organisations and one
-- account that never finished signing up. Every one of the 44 tables gets rows, so every table can
-- be looked at in pgAdmin and every relationship of the EERD has an example.
--
-- Every person, address and text here is invented. All email addresses use example.org.
-- Times are relative to now(), so the data always looks recent.
--
-- The ids follow one pattern so a row is easy to recognise in pgAdmin:
--   10000000-...-0000000000NN  account NN             20000000-...  member_profile
--   30000000-...               organisation_profile  40000000-...  tag
--   50000000-...               project               60000000-...  opportunity
--   70000000-...               event                 80000000-...  challenge
--   90000000-...               news_item             a0000000-...  product
--   b0000000-...               product_version       c0000000-...  shop_order
--   d0000000-...               every other table

\set ON_ERROR_STOP 1
BEGIN;

-- ---------------------------------------------------------------- account
-- 01 admin, 02 to 09 young people, 10 to 12 organisations, 13 never finished signing up.
-- 08 Emma logs in with Google only, so she has no password. 09 Ayoub was switched off by the admin.
INSERT INTO account (id, email, password_hash, display_name, role, email_confirmed_at, created_at, deactivated_at, deactivated_by) VALUES
 ('10000000-0000-4000-8000-000000000001', 'admin@example.org',      '$argon2id$dummy-hash-01', 'Foundation admin',  'Admin',  now() - interval '120 days', now() - interval '120 days', null, null),
 ('10000000-0000-4000-8000-000000000002', 'lotte@example.org',      '$argon2id$dummy-hash-02', 'Lotte',             'Member', now() - interval '60 days',  now() - interval '60 days',  null, null),
 ('10000000-0000-4000-8000-000000000003', 'daan@example.org',       '$argon2id$dummy-hash-03', 'Daan',              'Member', now() - interval '55 days',  now() - interval '55 days',  null, null),
 ('10000000-0000-4000-8000-000000000004', 'sara@example.org',       '$argon2id$dummy-hash-04', 'Sara',              'Member', now() - interval '50 days',  now() - interval '50 days',  null, null),
 ('10000000-0000-4000-8000-000000000005', 'milan@example.org',      '$argon2id$dummy-hash-05', 'Milan',             'Member', now() - interval '45 days',  now() - interval '45 days',  null, null),
 ('10000000-0000-4000-8000-000000000006', 'yara@example.org',       '$argon2id$dummy-hash-06', 'Yara',              'Member', now() - interval '40 days',  now() - interval '40 days',  null, null),
 ('10000000-0000-4000-8000-000000000007', 'noah@example.org',       '$argon2id$dummy-hash-07', 'Noah',              'Member', now() - interval '35 days',  now() - interval '35 days',  null, null),
 ('10000000-0000-4000-8000-000000000008', 'emma@example.org',       null,                      'Emma',              'Member', now() - interval '30 days',  now() - interval '30 days',  null, null),
 ('10000000-0000-4000-8000-000000000009', 'ayoub@example.org',      '$argon2id$dummy-hash-09', 'Ayoub',             'Member', now() - interval '25 days',  now() - interval '25 days',  now() - interval '3 days', '10000000-0000-4000-8000-000000000001'),
 ('10000000-0000-4000-8000-000000000010', 'info@svamstel.example.org',   '$argon2id$dummy-hash-10', 'SV Amstel',         'Member', now() - interval '90 days', now() - interval '90 days', null, null),
 ('10000000-0000-4000-8000-000000000011', 'jongeren@gemeente.example.org','$argon2id$dummy-hash-11', 'Gemeente Aalsmeer', 'Member', now() - interval '80 days', now() - interval '80 days', null, null),
 ('10000000-0000-4000-8000-000000000012', 'hello@techstart.example.org', '$argon2id$dummy-hash-12', 'TechStart',         'Member', now() - interval '70 days', now() - interval '70 days', null, null),
 ('10000000-0000-4000-8000-000000000013', 'new@example.org',        '$argon2id$dummy-hash-13', 'new',               'Member', null,                        now() - interval '2 hours',  null, null);

-- ---------------------------------------------------------------- member_profile
-- Noah's short line was removed by the admin because it held personal data.
INSERT INTO member_profile (id, account_id, first_name, age_band, region, bio, created_at, updated_at, bio_removed_at, bio_removed_reason, bio_removed_by) VALUES
 ('20000000-0000-4000-8000-000000000002', '10000000-0000-4000-8000-000000000002', 'Lotte', 'From15To17', 'Amstelveen',   'I love football and want to learn to code.', now() - interval '60 days', null, null, null, null),
 ('20000000-0000-4000-8000-000000000003', '10000000-0000-4000-8000-000000000003', 'Daan',  'From18To21', 'Aalsmeer',     'Studying ICT, looking for an internship.',   now() - interval '55 days', now() - interval '10 days', null, null, null),
 ('20000000-0000-4000-8000-000000000004', '10000000-0000-4000-8000-000000000004', 'Sara',  'From22To30', 'Uithoorn',     'Graphic designer, happy to help a club.',    now() - interval '50 days', null, null, null, null),
 ('20000000-0000-4000-8000-000000000005', '10000000-0000-4000-8000-000000000005', 'Milan', 'From15To17', 'Ouder-Amstel', null,                                         now() - interval '45 days', null, null, null, null),
 ('20000000-0000-4000-8000-000000000006', '10000000-0000-4000-8000-000000000006', 'Yara',  'From18To21', 'Amstelveen',   'Music, photography and volunteering.',       now() - interval '40 days', null, null, null, null),
 ('20000000-0000-4000-8000-000000000007', '10000000-0000-4000-8000-000000000007', 'Noah',  'From22To30', 'Diemen',       'Call me on my number',                       now() - interval '35 days', null, now() - interval '5 days', 'PersonalData', '10000000-0000-4000-8000-000000000001'),
 ('20000000-0000-4000-8000-000000000008', '10000000-0000-4000-8000-000000000008', 'Emma',  'From18To21', 'Amstelveen',   'Gamer and future game designer.',            now() - interval '30 days', null, null, null, null),
 ('20000000-0000-4000-8000-000000000009', '10000000-0000-4000-8000-000000000009', 'Ayoub', 'From15To17', 'Uithoorn',     null,                                         now() - interval '25 days', null, null, null, null);

-- ---------------------------------------------------------------- organisation_profile and organisation_logo
-- SV Amstel was checked by the foundation, the other two not yet.
INSERT INTO organisation_profile (id, account_id, name, kind, description, website, created_at, updated_at, checked_at, checked_by) VALUES
 ('30000000-0000-4000-8000-000000000010', '10000000-0000-4000-8000-000000000010', 'SV Amstel',         'Club',         'A football club that trains young coaches.', 'https://svamstel.example.org', now() - interval '90 days', null, now() - interval '85 days', '10000000-0000-4000-8000-000000000001'),
 ('30000000-0000-4000-8000-000000000011', '10000000-0000-4000-8000-000000000011', 'Gemeente Aalsmeer', 'Municipality', 'The youth desk of the municipality.',        null,                           now() - interval '80 days', null, null, null),
 ('30000000-0000-4000-8000-000000000012', '10000000-0000-4000-8000-000000000012', 'TechStart',         'Company',      'A small software company with internships.', 'https://techstart.example.org', now() - interval '70 days', now() - interval '20 days', null, null);
INSERT INTO organisation_logo (organisation_profile_id, logo_file_name, logo_width, logo_height) VALUES
 ('30000000-0000-4000-8000-000000000010', '7c1e0b5a9d3f4e2a8b6c1d0e9f7a3b2c.png', 400, 400),
 ('30000000-0000-4000-8000-000000000012', '2f9a1c3e5b7d4f6a8c0e1b3d5f7a9c1e.png', 600, 300);

-- ---------------------------------------------------------------- parent_consent
-- Lotte is under 16: her parent agreed, so the address is emptied and given_at stays as proof.
-- Milan's request still waits for an answer.
INSERT INTO parent_consent (id, member_profile_id, parent_email, token_hash, created_at, expires_at, given_at) VALUES
 ('d0000000-0000-4000-8000-000000000101', '20000000-0000-4000-8000-000000000002', null,                   repeat('a1', 32), now() - interval '60 days', now() - interval '46 days', now() - interval '59 days'),
 ('d0000000-0000-4000-8000-000000000102', '20000000-0000-4000-8000-000000000005', 'parent@example.org',   repeat('a2', 32), now() - interval '2 days',  now() + interval '12 days', null);

-- ---------------------------------------------------------------- tag and the three tag link tables
INSERT INTO tag (id, name, is_suggested, created_at) VALUES
 ('40000000-0000-4000-8000-000000000001', 'Football',       true,  now() - interval '120 days'),
 ('40000000-0000-4000-8000-000000000002', 'Programming',    true,  now() - interval '120 days'),
 ('40000000-0000-4000-8000-000000000003', 'Design',         true,  now() - interval '120 days'),
 ('40000000-0000-4000-8000-000000000004', 'Music',          true,  now() - interval '120 days'),
 ('40000000-0000-4000-8000-000000000005', 'Volunteering',   true,  now() - interval '120 days'),
 ('40000000-0000-4000-8000-000000000006', 'Photography',    true,  now() - interval '120 days'),
 ('40000000-0000-4000-8000-000000000007', 'Game design',    false, now() - interval '30 days'),
 ('40000000-0000-4000-8000-000000000008', 'Sustainability', false, now() - interval '20 days');
INSERT INTO member_profile_tag (member_profile_id, tag_id, created_at) VALUES
 ('20000000-0000-4000-8000-000000000002', '40000000-0000-4000-8000-000000000001', now() - interval '60 days'),
 ('20000000-0000-4000-8000-000000000002', '40000000-0000-4000-8000-000000000002', now() - interval '60 days'),
 ('20000000-0000-4000-8000-000000000003', '40000000-0000-4000-8000-000000000002', now() - interval '55 days'),
 ('20000000-0000-4000-8000-000000000004', '40000000-0000-4000-8000-000000000003', now() - interval '50 days'),
 ('20000000-0000-4000-8000-000000000006', '40000000-0000-4000-8000-000000000004', now() - interval '40 days'),
 ('20000000-0000-4000-8000-000000000006', '40000000-0000-4000-8000-000000000005', now() - interval '40 days'),
 ('20000000-0000-4000-8000-000000000006', '40000000-0000-4000-8000-000000000006', now() - interval '40 days'),
 ('20000000-0000-4000-8000-000000000008', '40000000-0000-4000-8000-000000000007', now() - interval '30 days');
INSERT INTO organisation_profile_tag (organisation_profile_id, tag_id, created_at) VALUES
 ('30000000-0000-4000-8000-000000000010', '40000000-0000-4000-8000-000000000001', now() - interval '90 days'),
 ('30000000-0000-4000-8000-000000000010', '40000000-0000-4000-8000-000000000005', now() - interval '90 days'),
 ('30000000-0000-4000-8000-000000000011', '40000000-0000-4000-8000-000000000008', now() - interval '80 days'),
 ('30000000-0000-4000-8000-000000000012', '40000000-0000-4000-8000-000000000002', now() - interval '70 days');

-- ---------------------------------------------------------------- connection_request
-- Lotte and Daan are connected, Yara still waits for Sara, Emma declined Milan.
INSERT INTO connection_request (id, requester_id, receiver_id, status, requested_at, answered_at) VALUES
 ('d0000000-0000-4000-8000-000000000201', '20000000-0000-4000-8000-000000000002', '20000000-0000-4000-8000-000000000003', 'Accepted', now() - interval '20 days', now() - interval '19 days'),
 ('d0000000-0000-4000-8000-000000000202', '20000000-0000-4000-8000-000000000006', '20000000-0000-4000-8000-000000000004', 'Requested', now() - interval '2 days', null),
 ('d0000000-0000-4000-8000-000000000203', '20000000-0000-4000-8000-000000000005', '20000000-0000-4000-8000-000000000008', 'Declined', now() - interval '9 days', null);

-- ---------------------------------------------------------------- project, project_image, removal_review_request
-- Milan's project was removed as spam; he asked the foundation to look again.
INSERT INTO project (id, member_profile_id, title, description, created_at, updated_at, removed_at, removed_reason, removed_by, removal_notice_dismissed_at) VALUES
 ('50000000-0000-4000-8000-000000000001', '20000000-0000-4000-8000-000000000002', 'Training app for my team', 'A small app that shows the training schedule of my football team.', now() - interval '30 days', null, null, null, null, null),
 ('50000000-0000-4000-8000-000000000002', '20000000-0000-4000-8000-000000000004', 'New logo for a club',      'A logo and a poster I made for a local club.',                      now() - interval '25 days', now() - interval '3 days', null, null, null, null),
 ('50000000-0000-4000-8000-000000000003', '20000000-0000-4000-8000-000000000006', 'Photo series Amstelveen',  'Twelve photos of the city at night.',                               now() - interval '15 days', null, null, null, null, null),
 ('50000000-0000-4000-8000-000000000004', '20000000-0000-4000-8000-000000000005', 'Buy cheap sneakers',       'Visit my shop for cheap sneakers.',                                 now() - interval '10 days', null, now() - interval '8 days', 'Spam', '10000000-0000-4000-8000-000000000001', null);
INSERT INTO project_image (project_id, image_file_name, image_width, image_height) VALUES
 ('50000000-0000-4000-8000-000000000002', '5a7c9e1b3d5f4a6c8e0b2d4f6a8c0e2b.jpg', 1200, 800),
 ('50000000-0000-4000-8000-000000000003', '9e1b3d5f7a4c4e6a8c0e2b4d6f8a0c2e.jpg', 1600, 1067);
INSERT INTO removal_review_request (id, project_id, account_id, text, created_at) VALUES
 ('d0000000-0000-4000-8000-000000000301', '50000000-0000-4000-8000-000000000004', '10000000-0000-4000-8000-000000000005', 'It was a joke, I will change the text.', now() - interval '7 days');

-- ---------------------------------------------------------------- member_profile_badge
INSERT INTO member_profile_badge (id, member_profile_id, badge_code, earned_at) VALUES
 ('d0000000-0000-4000-8000-000000000401', '20000000-0000-4000-8000-000000000002', 'FirstProject', now() - interval '30 days'),
 ('d0000000-0000-4000-8000-000000000402', '20000000-0000-4000-8000-000000000004', 'FirstProject', now() - interval '25 days'),
 ('d0000000-0000-4000-8000-000000000403', '20000000-0000-4000-8000-000000000006', 'FirstProject', now() - interval '15 days');

-- ---------------------------------------------------------------- opportunity, opportunity_tag, opportunity_member
-- The one of the municipality is closed already.
INSERT INTO opportunity (id, organisation_profile_id, title, description, kind, closing_date, created_at) VALUES
 ('60000000-0000-4000-8000-000000000001', '30000000-0000-4000-8000-000000000012', 'Internship web developer',  'Five months, building our customer portal with a mentor.', 'Internship', current_date + 30, now() - interval '14 days'),
 ('60000000-0000-4000-8000-000000000002', '30000000-0000-4000-8000-000000000010', 'Youth coach under 10',      'Two afternoons a week, training our youngest players.',    'Job',        current_date + 21, now() - interval '10 days'),
 ('60000000-0000-4000-8000-000000000003', '30000000-0000-4000-8000-000000000011', 'Design a sustainable park', 'Think along about the new park in the centre.',             'Project',    current_date - 5,  now() - interval '40 days');
INSERT INTO opportunity_tag (opportunity_id, tag_id, created_at) VALUES
 ('60000000-0000-4000-8000-000000000001', '40000000-0000-4000-8000-000000000002', now() - interval '14 days'),
 ('60000000-0000-4000-8000-000000000002', '40000000-0000-4000-8000-000000000001', now() - interval '10 days'),
 ('60000000-0000-4000-8000-000000000002', '40000000-0000-4000-8000-000000000005', now() - interval '10 days'),
 ('60000000-0000-4000-8000-000000000003', '40000000-0000-4000-8000-000000000008', now() - interval '40 days'),
 ('60000000-0000-4000-8000-000000000003', '40000000-0000-4000-8000-000000000003', now() - interval '40 days');
INSERT INTO opportunity_member (opportunity_id, member_profile_id, created_at) VALUES
 ('60000000-0000-4000-8000-000000000001', '20000000-0000-4000-8000-000000000003', now() - interval '12 days'),
 ('60000000-0000-4000-8000-000000000001', '20000000-0000-4000-8000-000000000008', now() - interval '6 days'),
 ('60000000-0000-4000-8000-000000000002', '20000000-0000-4000-8000-000000000002', now() - interval '8 days'),
 ('60000000-0000-4000-8000-000000000003', '20000000-0000-4000-8000-000000000004', now() - interval '30 days');

-- ---------------------------------------------------------------- event and event_member
-- One upcoming, one in the past, one cancelled. The open day has 30 places, 4 taken.
INSERT INTO event (id, organisation_profile_id, title, description, place, starts_at, capacity, created_at, updated_at, cancelled_at) VALUES
 ('70000000-0000-4000-8000-000000000001', '30000000-0000-4000-8000-000000000010', 'Open training day',      'Try a training as a youth coach.', 'Sportpark De Amstel, Amstelveen', now() + interval '10 days', 30, now() - interval '20 days', now() - interval '2 days', null),
 ('70000000-0000-4000-8000-000000000002', '30000000-0000-4000-8000-000000000012', 'Code night',             '',                                 'Library Amstelveen',             now() - interval '7 days',  15, now() - interval '30 days', null, null),
 ('70000000-0000-4000-8000-000000000003', '30000000-0000-4000-8000-000000000011', 'Youth council meeting',  'Your ideas for the town.',         'Town hall Aalsmeer',             now() + interval '20 days', 40, now() - interval '15 days', null, now() - interval '1 day');
INSERT INTO event_member (id, event_id, account_id, created_at) VALUES
 ('d0000000-0000-4000-8000-000000000501', '70000000-0000-4000-8000-000000000001', '10000000-0000-4000-8000-000000000002', now() - interval '15 days'),
 ('d0000000-0000-4000-8000-000000000502', '70000000-0000-4000-8000-000000000001', '10000000-0000-4000-8000-000000000003', now() - interval '14 days'),
 ('d0000000-0000-4000-8000-000000000503', '70000000-0000-4000-8000-000000000001', '10000000-0000-4000-8000-000000000006', now() - interval '5 days'),
 ('d0000000-0000-4000-8000-000000000504', '70000000-0000-4000-8000-000000000001', '10000000-0000-4000-8000-000000000009', now() - interval '20 days'),
 ('d0000000-0000-4000-8000-000000000505', '70000000-0000-4000-8000-000000000002', '10000000-0000-4000-8000-000000000003', now() - interval '25 days'),
 ('d0000000-0000-4000-8000-000000000506', '70000000-0000-4000-8000-000000000002', '10000000-0000-4000-8000-000000000008', now() - interval '24 days');

-- ---------------------------------------------------------------- challenge and challenge_member
INSERT INTO challenge (id, organisation_profile_id, title, description, deadline, created_at, updated_at, removed_at) VALUES
 ('80000000-0000-4000-8000-000000000001', '30000000-0000-4000-8000-000000000012', 'Build a chatbot for our help desk', 'Make a first version that answers five common questions.', current_date + 45, now() - interval '12 days', null, null),
 ('80000000-0000-4000-8000-000000000002', '30000000-0000-4000-8000-000000000011', 'Less litter in the park',            'Come up with an idea that young people will really use.',    current_date + 14, now() - interval '25 days', now() - interval '4 days', null),
 ('80000000-0000-4000-8000-000000000003', '30000000-0000-4000-8000-000000000010', 'Old challenge',                      'Removed by the club.',                                         current_date - 10, now() - interval '60 days', null, now() - interval '20 days');
INSERT INTO challenge_member (id, challenge_id, member_profile_id, joined_at) VALUES
 ('d0000000-0000-4000-8000-000000000601', '80000000-0000-4000-8000-000000000001', '20000000-0000-4000-8000-000000000003', now() - interval '10 days'),
 ('d0000000-0000-4000-8000-000000000602', '80000000-0000-4000-8000-000000000001', '20000000-0000-4000-8000-000000000008', now() - interval '9 days'),
 ('d0000000-0000-4000-8000-000000000603', '80000000-0000-4000-8000-000000000002', '20000000-0000-4000-8000-000000000006', now() - interval '20 days');

-- ---------------------------------------------------------------- saved_item
-- kind says which table item_id points to: a member profile, an organisation profile or an event.
INSERT INTO saved_item (id, account_id, kind, item_id, created_at) VALUES
 ('d0000000-0000-4000-8000-000000000701', '10000000-0000-4000-8000-000000000002', 'Organisation', '30000000-0000-4000-8000-000000000010', now() - interval '18 days'),
 ('d0000000-0000-4000-8000-000000000702', '10000000-0000-4000-8000-000000000003', 'Event',        '70000000-0000-4000-8000-000000000001', now() - interval '14 days'),
 ('d0000000-0000-4000-8000-000000000703', '10000000-0000-4000-8000-000000000006', 'Member',       '20000000-0000-4000-8000-000000000004', now() - interval '3 days'),
 ('d0000000-0000-4000-8000-000000000704', '10000000-0000-4000-8000-000000000012', 'Member',       '20000000-0000-4000-8000-000000000003', now() - interval '11 days');

-- ---------------------------------------------------------------- news_item and news_image
INSERT INTO news_item (id, author_account_id, title, body, published_at, changed_at) VALUES
 ('90000000-0000-4000-8000-000000000001', '10000000-0000-4000-8000-000000000001', 'Welcome to Connect',        'Connect is open for every young person in Amstelland.', now() - interval '100 days', null),
 ('90000000-0000-4000-8000-000000000002', '10000000-0000-4000-8000-000000000001', 'Three new organisations',   'SV Amstel, TechStart and the municipality of Aalsmeer joined.', now() - interval '20 days', now() - interval '19 days');
INSERT INTO news_image (news_item_id, image_file_name, image_alt_text) VALUES
 ('90000000-0000-4000-8000-000000000001', '3b5d7f9a1c4e4a6c8e0b2d4f6a8c0e2d.jpg', 'Young people laughing at a table with laptops');

-- ---------------------------------------------------------------- site_page and site_page_version
-- The privacy statement was saved twice, so there is one older version of it.
INSERT INTO site_page (id, slug, title, body, is_protected, updated_at, updated_by) VALUES
 ('d0000000-0000-4000-8000-000000000801', 'privacy',     'Privacy statement', '# Privacy statement' || chr(10) || 'We only keep what Connect needs.', true,  now() - interval '30 days', '10000000-0000-4000-8000-000000000001'),
 ('d0000000-0000-4000-8000-000000000802', 'house-rules', 'House rules',       '# House rules' || chr(10) || 'Be kind to each other.',           true,  now() - interval '100 days', null),
 ('d0000000-0000-4000-8000-000000000803', 'about',       'About Connect',     '# About Connect' || chr(10) || 'Made with Young Amstelland.',    false, now() - interval '50 days', '10000000-0000-4000-8000-000000000001');
INSERT INTO site_page_version (id, site_page_id, title, body, updated_at, updated_by, replaced_at) VALUES
 ('d0000000-0000-4000-8000-000000000811', 'd0000000-0000-4000-8000-000000000801', 'Privacy statement', 'This text is not written yet. The foundation adds it before Connect opens.', now() - interval '100 days', null, now() - interval '30 days');

-- ---------------------------------------------------------------- member_report
-- Yara reported Ayoub, after which the admin switched his account off.
INSERT INTO member_report (id, reporter_account_id, reported_account_id, reason, created_at) VALUES
 ('d0000000-0000-4000-8000-000000000901', '10000000-0000-4000-8000-000000000006', '10000000-0000-4000-8000-000000000009', 'BullyingOrHate', now() - interval '4 days'),
 ('d0000000-0000-4000-8000-000000000902', '10000000-0000-4000-8000-000000000002', '10000000-0000-4000-8000-000000000009', 'BullyingOrHate', now() - interval '4 days');

-- ---------------------------------------------------------------- the log in tables
INSERT INTO session (id, account_id, token_hash, created_at, expires_at, revoked_at, last_active_at) VALUES
 ('d0000000-0000-4000-8000-000000001001', '10000000-0000-4000-8000-000000000001', repeat('b1', 32), now() - interval '2 hours',  now() + interval '22 hours', null, now() - interval '5 minutes'),
 ('d0000000-0000-4000-8000-000000001002', '10000000-0000-4000-8000-000000000002', repeat('b2', 32), now() - interval '1 hour',   now() + interval '23 hours', null, now() - interval '1 minute'),
 ('d0000000-0000-4000-8000-000000001003', '10000000-0000-4000-8000-000000000003', repeat('b3', 32), now() - interval '3 days',   now() - interval '2 days',   null, now() - interval '2 days'),
 ('d0000000-0000-4000-8000-000000001004', '10000000-0000-4000-8000-000000000008', repeat('b4', 32), now() - interval '5 hours',  now() + interval '19 hours', now() - interval '4 hours', now() - interval '4 hours'),
 ('d0000000-0000-4000-8000-000000001005', '10000000-0000-4000-8000-000000000012', repeat('b5', 32), now() - interval '30 minutes', now() + interval '23 hours', null, now());
INSERT INTO password_reset (id, account_id, token_hash, created_at, expires_at, used_at) VALUES
 ('d0000000-0000-4000-8000-000000001101', '10000000-0000-4000-8000-000000000002', repeat('c1', 32), now() - interval '12 days', now() - interval '12 days' + interval '1 hour', now() - interval '12 days' + interval '10 minutes'),
 ('d0000000-0000-4000-8000-000000001102', '10000000-0000-4000-8000-000000000007', repeat('c2', 32), now() - interval '20 minutes', now() + interval '40 minutes', null);
INSERT INTO email_confirmation (id, account_id, token_hash, created_at, expires_at, used_at) VALUES
 ('d0000000-0000-4000-8000-000000001201', '10000000-0000-4000-8000-000000000002', repeat('d1', 32), now() - interval '60 days', now() - interval '59 days', now() - interval '60 days' + interval '5 minutes'),
 ('d0000000-0000-4000-8000-000000001202', '10000000-0000-4000-8000-000000000006', repeat('d2', 32), now() - interval '40 days', now() - interval '39 days', now() - interval '40 days' + interval '2 minutes'),
 ('d0000000-0000-4000-8000-000000001203', '10000000-0000-4000-8000-000000000013', repeat('d3', 32), now() - interval '2 hours',  now() + interval '22 hours', null);
INSERT INTO email_change (id, account_id, new_email, token_hash, created_at, expires_at, used_at) VALUES
 ('d0000000-0000-4000-8000-000000001301', '10000000-0000-4000-8000-000000000007', 'noah.new@example.org', repeat('e1', 32), now() - interval '1 hour', now() + interval '23 hours', null);
INSERT INTO external_login (id, account_id, provider, provider_user_id, created_at) VALUES
 ('d0000000-0000-4000-8000-000000001401', '10000000-0000-4000-8000-000000000008', 'Google',   'google-sub-100200300', now() - interval '30 days'),
 ('d0000000-0000-4000-8000-000000001402', '10000000-0000-4000-8000-000000000003', 'Facebook', 'facebook-sub-400500', now() - interval '50 days');
INSERT INTO pending_external_login (id, account_id, provider, provider_user_id, token_hash, failed_attempts, created_at, expires_at) VALUES
 ('d0000000-0000-4000-8000-000000001501', '10000000-0000-4000-8000-000000000004', 'Google', 'google-sub-700800900', repeat('f1', 32), 1, now() - interval '3 minutes', now() + interval '7 minutes');
INSERT INTO log_in_attempt (id, account_id, outcome, method, attempted_at) VALUES
 ('d0000000-0000-4000-8000-000000001601', '10000000-0000-4000-8000-000000000002', 'WrongPassword', 'Password', now() - interval '1 hour 2 minutes'),
 ('d0000000-0000-4000-8000-000000001602', '10000000-0000-4000-8000-000000000002', 'Succeeded',     'Password', now() - interval '1 hour'),
 ('d0000000-0000-4000-8000-000000001603', '10000000-0000-4000-8000-000000000008', 'Succeeded',     'Google',   now() - interval '5 hours'),
 ('d0000000-0000-4000-8000-000000001604', '10000000-0000-4000-8000-000000000003', 'WrongPassword', 'Password', now() - interval '2 days');
INSERT INTO suspicious_activity_alert (id, account_id, rule, sent_at) VALUES
 ('d0000000-0000-4000-8000-000000001701', '10000000-0000-4000-8000-000000000003', 'ManyLocks', now() - interval '2 days');
-- these two key on the typed address, which may belong to no account at all
INSERT INTO login_failure (id, email, failed_at) VALUES
 ('d0000000-0000-4000-8000-000000001801', 'daan@example.org',     now() - interval '2 days'),
 ('d0000000-0000-4000-8000-000000001802', 'daan@example.org',     now() - interval '2 days' + interval '1 minute'),
 ('d0000000-0000-4000-8000-000000001803', 'nobody@example.org',   now() - interval '6 hours');
INSERT INTO login_lock (id, email, locked_until, created_at, warning_handled_at) VALUES
 ('d0000000-0000-4000-8000-000000001901', 'daan@example.org', now() - interval '2 days' + interval '15 minutes', now() - interval '2 days', now() - interval '2 days');
INSERT INTO log_in_puzzle_use (id, challenge_hash, used_at) VALUES
 ('d0000000-0000-4000-8000-000000002001', repeat('9f', 32), now() - interval '1 hour'),
 ('d0000000-0000-4000-8000-000000002002', repeat('8e', 32), now() - interval '5 hours');

-- ---------------------------------------------------------------- audit_entry
-- detail holds only fixed words or a number, never a name or an address.
INSERT INTO audit_entry (id, actor_account_id, action, target_kind, target_id, occurred_at, detail) VALUES
 ('d0000000-0000-4000-8000-000000002101', '10000000-0000-4000-8000-000000000002', 'session.started',   'account',          '10000000-0000-4000-8000-000000000002', now() - interval '1 hour',  null),
 ('d0000000-0000-4000-8000-000000002102', '10000000-0000-4000-8000-000000000001', 'project.removed',   'project',          '50000000-0000-4000-8000-000000000004', now() - interval '8 days',  'Spam'),
 ('d0000000-0000-4000-8000-000000002103', '10000000-0000-4000-8000-000000000001', 'member.bio_removed','member_profile',   '20000000-0000-4000-8000-000000000007', now() - interval '5 days',  'PersonalData'),
 ('d0000000-0000-4000-8000-000000002104', '10000000-0000-4000-8000-000000000001', 'account.deactivated','account',         '10000000-0000-4000-8000-000000000009', now() - interval '3 days',  null),
 ('d0000000-0000-4000-8000-000000002105', null,                                   'account.locked',    'account',          '10000000-0000-4000-8000-000000000003', now() - interval '2 days',  '15');

-- ---------------------------------------------------------------- webshop
-- The T shirt went up from 15.00 to 18.00. Daan bought two before the change: his order still
-- points at the old version and so still shows 15.00 each. The cap has an uploaded picture.
INSERT INTO product (id, is_active) VALUES
 ('a0000000-0000-4000-8000-000000000001', true),
 ('a0000000-0000-4000-8000-000000000002', true),
 ('a0000000-0000-4000-8000-000000000003', false);
INSERT INTO product_version (id, product_id, name, description, price_in_cents, vat_included, image_file, image_stored_name, valid_from) VALUES
 ('b0000000-0000-4000-8000-000000000011', 'a0000000-0000-4000-8000-000000000001', 'Young Amstelland T shirt, size M', 'Black, with the logo on the front.', 1500, true, 't-shirt.jpg', null, now() - interval '90 days'),
 ('b0000000-0000-4000-8000-000000000012', 'a0000000-0000-4000-8000-000000000001', 'Young Amstelland T shirt, size M', 'Black, with the logo on the front.', 1800, true, 't-shirt.jpg', null, now() - interval '5 days'),
 ('b0000000-0000-4000-8000-000000000021', 'a0000000-0000-4000-8000-000000000002', 'Young Amstelland cap',             'One size.',                         1200, true, null, '4c6e8a0b2d4f4a6c8e0b2d4f6a8c0e2f.jpg', now() - interval '60 days'),
 ('b0000000-0000-4000-8000-000000000031', 'a0000000-0000-4000-8000-000000000003', 'Sticker set',                      'No longer for sale.',                300, true, 'stickers.jpg', null, now() - interval '90 days');
INSERT INTO basket (id, account_id, created_at, updated_at) VALUES
 ('d0000000-0000-4000-8000-000000002201', '10000000-0000-4000-8000-000000000006', now() - interval '1 day', now() - interval '2 hours');
INSERT INTO basket_item (id, basket_id, product_id, quantity) VALUES
 ('d0000000-0000-4000-8000-000000002211', 'd0000000-0000-4000-8000-000000002201', 'a0000000-0000-4000-8000-000000000001', 1),
 ('d0000000-0000-4000-8000-000000002212', 'd0000000-0000-4000-8000-000000002201', 'a0000000-0000-4000-8000-000000000002', 2);
INSERT INTO shop_order (id, order_number, account_id, total_in_cents, status, provider_payment_id, created_at, paid_at, confirmation_sent_at) VALUES
 ('c0000000-0000-4000-8000-000000000001', 'CN-2026-0001', '10000000-0000-4000-8000-000000000003', 4200, 'Paid',    'tr_dummy0001', now() - interval '20 days', now() - interval '20 days', now() - interval '20 days'),
 ('c0000000-0000-4000-8000-000000000002', 'CN-2026-0002', '10000000-0000-4000-8000-000000000004', 1800, 'Pending', null,           now() - interval '10 minutes', null, null);
INSERT INTO shop_order_item (id, shop_order_id, product_version_id, quantity) VALUES
 ('d0000000-0000-4000-8000-000000002301', 'c0000000-0000-4000-8000-000000000001', 'b0000000-0000-4000-8000-000000000011', 2),
 ('d0000000-0000-4000-8000-000000002302', 'c0000000-0000-4000-8000-000000000001', 'b0000000-0000-4000-8000-000000000021', 1),
 ('d0000000-0000-4000-8000-000000002303', 'c0000000-0000-4000-8000-000000000002', 'b0000000-0000-4000-8000-000000000012', 1);

COMMIT;
