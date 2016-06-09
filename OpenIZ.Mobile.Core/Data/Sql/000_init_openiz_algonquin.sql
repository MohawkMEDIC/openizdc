CREATE VIEW IF NOT EXISTS FindConceptsByName AS
	SELECT concept.*, concept_name.name as Name, concept_name.lang as Language FROM concept_name INNER JOIN concept ON (concept.uuid = concept_name.concept_uuid)
	WHERE concept.obsoletion_time IS NULL;

INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'5AC073EA5931C848BBCB741911D91CD2', '1.3.6.1.4.1.33349.3.1.5.9.2.0', 'Access Administrative Function', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'CFC10AD86E3D9F42A4A088C0BBBC839D', '1.3.6.1.4.1.33349.3.1.5.9.2.0.1', 'Change Password', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'AC650D9C3E61674A8BC65CE2C0B42160', '1.3.6.1.4.1.33349.3.1.5.9.2.0.2', 'Create Role', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'27C2BC79130DBF4FA83EF2B9FCE34151', '1.3.6.1.4.1.33349.3.1.5.9.2.0.3', 'Alter Role', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'CB4286ABE4289E4EBD7BD6DC72B729B2', '1.3.6.1.4.1.33349.3.1.5.9.2.0.4', 'Create Identity', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'AB965BD16C64004C9A58EA09EEE67D7C', '1.3.6.1.4.1.33349.3.1.5.9.2.1', 'Login', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'360384F6204EC04BB965BAA6D7C80BE3', '1.3.6.1.4.1.33349.3.1.5.9.2.2', 'Unrestricted Clinical Data', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'47AF1DB8A5175E46A5FD706B168B0265', '1.3.6.1.4.1.33349.3.1.5.9.2.2.0', 'Query Clinical Data', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'216927D778A0484395F2ED3CDE83E607', '1.3.6.1.4.1.33349.3.1.5.9.2.2.1', 'Write Clinical Data', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'EE7D022EE4ED3147B7FACB67AE0586BE', '1.3.6.1.4.1.33349.3.1.5.9.2.2.2', 'Delete Clinical Data', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'BF31B75F594E634880BD51757D58EA3B', '1.3.6.1.4.1.33349.3.1.5.9.2.2.3', 'Read Clinical Data', 0, 0);
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'AA91A8DE4D22594881B3C1EB2750067E', '1.3.6.1.4.1.33349.3.1.5.9.2.3', 'Override Disclosure', 0, 0);

-- CREATE USERS ROLE
INSERT INTO security_role (uuid, name, description) VALUES (X'E88AE5F4BD8B3546A6D48A195B143436', 'USERS', 'Group for users who have login access');
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AB965BD16C64004C9A58EFEBEEE67f7C', X'AB965BD16C64004C9A58EA09EEE67D7C', X'E88AE5F4BD8B3546A6D48A195B143436', 2);  -- GRANT Login

-- CREATE ADMINISTRATORS ROLE
INSERT INTO security_role (uuid, name, description) VALUES (X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 'ADMINISTRATORS', 'Group for users who have administrative access');
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'5AC073EA5931C848FEDB741911D91CD2', X'5AC073EA5931C848BBCB741911D91CD2', X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 2);  --GRANT Access Administrative Function
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AB965BD16C64004C9F94EA09EEE67D7C', X'AB965BD16C64004C9A58EA09EEE67D7C', X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 2); -- GRANT Login
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'360384F6204EC04BB965FB36D7C80BE3', X'360384F6204EC04BB965BAA6D7C80BE3', X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 0); -- DENY Unrestricted Clinical Data
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AA91A8DE4D22594881B3C1EF2320067E', X'AA91A8DE4D22594881B3C1EB2750067E', X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 0);  --DENY Override disclosure

-- CREATE ROLE SYSTEM WHICH IS DENIED LOGIN
INSERT INTO security_role (uuid, name, description) VALUES (X'D221AEC323FC3341BA42B0E0A3B817D7', 'SYSTEM', 'Group for user SYSTEM. Identifies the functions that internal system functions have access to. EDITING THIS ROLE MAY CAUSE SYSTEM FAILURE');
INSERT INTO security_user_role (uuid, role_id, user_id) VALUES (X'D221AEC325847341BA42B0E0A3B817D7', X'D221AEC323FC3341BA42B0E0A3B817D7', X'76A0DCFA90366E4AAF9EF1CD68E8C7E8');
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'5AC1254A5931C848BBCB741911D91CD2', X'5AC073EA5931C848BBCB741911D91CD2', X'D221AEC323FC3341BA42B0E0A3B817D7', 2); -- GRANT Access Administrative Function
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AB965BD14582F04C9A58EA09EEE67D7C', X'AB965BD16C64004C9A58EA09EEE67D7C', X'D221AEC323FC3341BA42B0E0A3B817D7', 0); -- DENY Login
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'360384F6204EC04BB965BAAF04F49BE3', X'360384F6204EC04BB965BAA6D7C80BE3', X'D221AEC323FC3341BA42B0E0A3B817D7', 2); -- GRANT Unrestricted Clinical Data
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AA91A8DE4D22594881B3C1EBF3B4B2B1', X'AA91A8DE4D22594881B3C1EB2750067E', X'D221AEC323FC3341BA42B0E0A3B817D7', 0); -- DENY Override disclosure

-- CREATE ROLE ANONYMOUS WHICH IS DENIED ALL CLINICAL 
INSERT INTO security_role (uuid, name, description) VALUES (X'58D8DBDAC513A344AD7D1C44CECAA4B6', 'ANONYMOUS', 'Group for user ANONYMOUS. Identifies the functions that nonlogged in users have access to. EDITING THIS ROLE MAY INTRODUCE SECURITY BREACHES');
INSERT INTO security_user_role (uuid, role_id, user_id) VALUES (X'58D8DBDAC513A344AD7D1C44CECAAFB3', X'58D8DBDAC513A344AD7D1C44CECAA4B6', X'00000000000000000000000000000000');
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'5AC073EA5931C848BBCB7419115B43BC', X'5AC073EA5931C848BBCB741911D91CD2', X'58D8DBDAC513A344AD7D1C44CECAA4B6', 0);  -- DENYAccess Administrative Function
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AB965BD16C64004C9A58EA09EB3D7D7C', X'AB965BD16C64004C9A58EA09EEE67D7C', X'58D8DBDAC513A344AD7D1C44CECAA4B6', 0);  --DENY Login
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'360384F624B3B2BBB965BAA6D7C80BE3', X'360384F6204EC04BB965BAA6D7C80BE3', X'58D8DBDAC513A344AD7D1C44CECAA4B6', 0);  --DENY Unrestricted Clinical Data
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AA91A8DE4D22594881B3C1EB2B4B3D2E', X'AA91A8DE4D22594881B3C1EB2750067E', X'58D8DBDAC513A344AD7D1C44CECAA4B6', 0);  --DENY Override disclosure

INSERT INTO concept_relationship_type (uuid, name, mnemonic) VALUES (X'C2AF4D2C6A56AE419EBC3097D7D22F4A', 'Same as', 'SameAs');
INSERT INTO concept_relationship_type (uuid, name, mnemonic) VALUES (X'3D2927AD3C43754B88D2B5360CD95450', 'Inverse of', 'InverseOf');
INSERT INTO concept_relationship_type (uuid, name, mnemonic) VALUES (X'5BD459A1343C1B4E9B759193A7528CED', 'Member of', 'MemberOf');
INSERT INTO concept_relationship_type (uuid, name, mnemonic) VALUES (X'2F4F8BAE9F000D4EB35E5A89555C5947', 'Negation of', 'NegationOf');

-- Concept Classes
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'5452FD17258CBB4AB246083FBE9AFA15','Classification Concept', 'ClassCode');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'8231B95419FCA24782C6089FD70A4F45', 'Status', 'Status');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'2297A9BBCE239A468FA510DEBA853D35', 'Mood', 'Mood');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'CDFD1DF59B031F4E90BE3CF56AEF8DA4', 'Relationship Type', 'Relationship');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'D300A9A87EA0024EB45F580D09BAF047', 'Route', 'Route');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'4793F61E03EFF74FB3C56334448845E6', 'Unit of Measure', 'UnitOfMeasure');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'39EACD92A3B95B4ABC88A6646C74240D', 'Diagnosis', 'Diagnosis');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'07E245E4A3601A409B81A8AC2479F4A6', 'Findings', 'Finding');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'E6F8D74BB8E4BC4D93A7CF14FBAF9700', 'Problem', 'Problem');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'32BC9CDCEAB84441BEF1DC618E28F4D7', 'Drug or other Material', 'Material');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'39346B0DBEC98044AF39EEB457C052D0', 'Other Classification', 'Other');
INSERT INTO concept_class (uuid, Name, Mnemonic) VALUES (X'4A30D8FFEC43BC4E95FCFB4A4F2338F0', 'Stock control codes', 'Stock');
