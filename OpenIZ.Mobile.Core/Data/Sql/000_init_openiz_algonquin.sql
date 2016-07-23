-- DEFAULT DATA SECTION
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
INSERT INTO security_policy (uuid, oid, name, is_public, can_override) VALUES (X'B3D3DE2A7D22594881B3C1EB2750067E', '1.3.6.1.4.1.33349.3.1.5.9.2.10', 'Local Administrator', 0, 0);

-- CREATE USERS ROLE
INSERT INTO security_role (uuid, name, description) VALUES (X'E88AE5F4BD8B3546A6D48A195B143436', 'USERS', 'Group for users who have login access');
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AB965BD16C64004C9A58EFEBEEE67f7C', X'AB965BD16C64004C9A58EA09EEE67D7C', X'E88AE5F4BD8B3546A6D48A195B143436', 2);  -- GRANT Login

-- CREATE ADMINISTRATORS ROLE
INSERT INTO security_role (uuid, name, description) VALUES (X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 'ADMINISTRATORS', 'Group for users who have administrative access');
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'5AC073EA5931C848FEDB741911D91CD2', X'5AC073EA5931C848BBCB741911D91CD2', X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 2);  --GRANT Access Administrative Function
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AB965BD16C64004C9F94EA09EEE67D7C', X'AB965BD16C64004C9A58EA09EEE67D7C', X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 2); -- GRANT Login
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'360384F6204EC04BB965FB36D7C80BE3', X'360384F6204EC04BB965BAA6D7C80BE3', X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 0); -- DENY Unrestricted Clinical Data
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AA91A8DE4D22594881B3C1EF2320067E', X'AA91A8DE4D22594881B3C1EB2750067E', X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 0);  --DENY Override disclosure
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AB91A8DE4D22594881B3C1EF2320067E', X'B3D3DE2A7D22594881B3C1EB2750067E', X'1DBAD2F6B55BE341B7FB2EC32418B2E1', 2);  --GRANT Local Administrator

-- CREATE LOCAL ADMINS
INSERT INTO security_role (uuid, name, description) VALUES (X'8DF6D7E8B55BE341B7FB2EC32418B2E1', 'LOCAL ADMINISTRATORS', 'Group for users who have local administrative access');
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'5AC073EA23423848FEDB741911D91CD2', X'5AC073EA5931C848BBCB741911D91CD2', X'8DF6D7E8B55BE341B7FB2EC32418B2E1', 0);  --DENY Access Administrative Function
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AB965BD16234242C9F94EA09EEE67D7C', X'AB965BD16C64004C9A58EA09EEE67D7C', X'8DF6D7E8B55BE341B7FB2EC32418B2E1', 2); -- GRANT Login
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'AA91F8134234324881B3C1EF2320067E', X'B3D3DE2A7D22594881B3C1EB2750067E', X'8DF6D7E8B55BE341B7FB2EC32418B2E1', 2);  --GRANT Local Administrator

-- CREATE ROLE SYSTEM WHICH IS DENIED LOGIN
INSERT INTO security_role (uuid, name, description) VALUES (X'D221AEC323FC3341BA42B0E0A3B817D7', 'SYSTEM', 'Group for user SYSTEM. Identifies the functions that internal system functions have access to. EDITING THIS ROLE MAY CAUSE SYSTEM FAILURE');
INSERT INTO security_user_role (uuid, role_id, user_id) VALUES (X'D221AEC325847341BA42B0E0A3B817D7', X'D221AEC323FC3341BA42B0E0A3B817D7', X'76A0DCFA90366E4AAF9EF1CD68E8C7E8');
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'5AC1254A5931C848BBCB741911D91CD2', X'5AC073EA5931C848BBCB741911D91CD2', X'D221AEC323FC3341BA42B0E0A3B817D7', 2); -- GRANT Access Administrative Function
INSERT INTO security_role_policy (uuid, policy_id, role_id, grant_type) VALUES (X'5AC1254A5931C848BFDB741911D91CD2', X'B3D3DE2A7D22594881B3C1EB2750067E', X'D221AEC323FC3341BA42B0E0A3B817D7', 2); -- GRANT Access Local Administrative Function
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

-- QUERY HELPERS

-- QUERY FOR CONCEPTS
create view if not exists sqp_Concept AS 
	select concept.*, 
		status.mnemonic as statusConcept_mnemonic,
		concept_class.mnemonic as class_mnemonic, 
		concept_class.name as class_name,
		concept_name.language as name_language,
		concept_name.value as name_value,
		concept_name.phoneticCode as name_phoneticCode,
		concept_name.phoneticAlgorithm as name_phoneticAlgorithm,
		concept_set.mnemonic as conceptSet_mnemonic,
		concept_set.name as conceptSet_name,
		concept_set.oid as conceptSet_oid,
		concept_set.uuid as conceptSet_id
		from concept left join concept as status on (concept.statusConcept = status.uuid)
		inner join concept_class on (concept_class.uuid = concept.class)
		left join concept_name on (concept_name.concept_uuid = concept.uuid)
		left join concept_concept_set on (concept_concept_set.concept_uuid = concept.uuid)
		left join concept_set on (concept_set.uuid = concept_concept_set.concept_set_uuid);


-- ENTITY VIEW HELPER
create view if not exists sqp_Entity as 
	select entity.*,
		class.mnemonic as classConcept_mnemonic,
		determiner.mnemonic as determinerConcept_mnemonic,
		type.mnemonic as typeConcept_mnemonic,
		status.mnemonic as statusConcept_mnemonic,
		entity_address.use as address_use,
		entity_address_concept.mnemonic as address_use_mnemonic,
		entity_address_concept.mnemonic as address_guard,
		entity_address_comp.type as address_component_type,
		entity_address_comp.value as address_component_value,
		entity_address_comp_type.mnemonic as address_component_type_mnemonic,
		entity_address_comp_type.mnemonic as address_component_guard,
		entity_extension.extensionType as extension_type,
		extension_type.name as extension_type_name,
		entity_identifier.value as identifier_value,
		assigning_authority.name as identifier_authority_name,
		assigning_authority.domainName as identifier_authority_domainName,
		assigning_authority.domainName as identifier_guard,
		assigning_authority.oid as identifier_authority_oid,
		entity_name.use as name_use,
		entity_name_concept.mnemonic as name_use_mnemonic,
		entity_name_concept.mnemonic as name_guard,
		entity_name_comp.type as name_component_type,
		entity_name_comp.value as name_component_value,
		entity_name_comp.phoneticCode as name_component_phoneticCode,
		entity_name_comp.phoneticAlgorithm as name_component_phoneticAlgorithm,
		entity_name_comp_type.mnemonic as name_component_type_mnemonic,
		entity_name_comp_type.mnemonic as name_component_guard,
		entity_relationship.relationshipType as relationship_type,
		rel_entity_name.use as relationship_name_use,
		rel_entity_name_comp.type as relationship_name_component_type,
		rel_entity_name_comp.value as relationship_name_component_value,
		rel_entity_name_comp.phoneticCode as relationship_name_component_phoneticCode,
		rel_entity_name_comp.phoneticAlgorithm as relationship_name_component_phoneticAlgorithm,
		entity_telecom.use as telecom_use,
		entity_telecom.value as telecom_value
	from entity inner join concept as class on (class.uuid = entity.classConcept)
		inner join concept as determiner on (determiner.uuid = entity.determinerConcept)
		left join concept as type on (type.uuid = entity.typeConcept)
		left join concept as status on (status.uuid = entity.statusConcept)
		left join entity_address on (entity.uuid = entity_address.entity_uuid)
		left join concept as entity_address_concept on (entity_address_concept.uuid = entity_address.use)
		left join entity_address_comp on (entity_address.uuid = entity_address_comp.address_uuid)
		left join concept as entity_address_comp_type on (entity_address_comp_type.uuid = entity_address_comp.type)
		left join entity_extension on (entity.uuid = entity_extension.entity_uuid)
		left join extension_type on (entity_extension.extensionType = extension_type.uuid)
		left join entity_identifier on (entity.uuid = entity_identifier.entity_uuid)
		left join assigning_authority on (entity_identifier.authority = assigning_authority.uuid)
		left join entity_name on (entity.uuid = entity_name.entity_uuid)
		left join concept as entity_name_concept on (entity_name_concept.uuid = entity_name.use)
		left join entity_name_comp on (entity_name.uuid = entity_name_comp.name_uuid)
		left join concept as entity_name_comp_type on (entity_name_comp_type.uuid = entity_name_comp.type)
		left join entity_relationship on (entity_relationship.entity_uuid = entity.uuid)
		left join entity_telecom on (entity_telecom.entity_uuid = entity.uuid)
		left join entity as rel_entity on (rel_entity.uuid = entity_relationship.entity_uuid)
		left join entity_name as rel_entity_name on (rel_entity.uuid = rel_entity_name.entity_uuid)
		left join entity_name_comp as rel_entity_name_comp on (rel_entity_name.uuid = rel_entity_name_comp.name_uuid)
		left join entity_telecom as rel_entity_telecom on (rel_entity_telecom.entity_uuid = rel_entity.uuid);
	
-- PERSON VIEW
create view if not exists sqp_Person as
select person.*,
		entity.classConcept_mnemonic,
		entity.determinerConcept_mnemonic,
		entity.typeConcept_mnemonic,
		entity.statusConcept_mnemonic,
		entity.classConcept,
		entity.createdBy,
		entity.creationTime,
		entity.determinerConcept,
		entity.obsoletedBy,
		entity.obsoletionTime,
		entity.replace_version_uuid,
		entity.statusConcept,
		entity.typeConcept,
		entity.updatedBy,
		entity.updatedTime,
		entity.version_uuid,
		entity.address_use,
		entity.address_guard,
		entity.address_use_mnemonic,
		entity.address_component_type,
		entity.address_component_guard,
		entity.address_component_type_mnemonic,
		entity.address_component_value,
		entity.extension_type,
		entity.extension_type_name,
		entity.identifier_value,
		entity.identifier_authority_name,
		entity.identifier_guard,
		entity.identifier_authority_domainName,
		entity.identifier_authority_oid,
		entity.name_use,
		entity.name_use_mnemonic,
		entity.name_guard,
		entity.name_component_type,
		entity.name_component_type_mnemonic,
		entity.name_component_guard,
		entity.name_component_value,
		entity.name_component_phoneticCode,
		entity.name_component_phoneticAlgorithm,
		entity.relationship_type,
		entity.relationship_name_use,
		entity.relationship_name_component_type,
		entity.relationship_name_component_value,
		entity.relationship_name_component_phoneticCode,
		entity.relationship_name_component_phoneticAlgorithm,
		entity.telecom_use,
		entity.telecom_value
	from person inner join sqp_Entity as entity on (person.uuid = entity.uuid)
		where entity.classConcept = X'46A8E29DF2DDBC4E902E84508C5089EA';
	


-- PATIENT VIEW HELPER
create view if not exists sqp_Patient as 
	select patient.*,
		person.*,
		concept.mnemonic as genderConcept_mnemonic,
				entity.classConcept_mnemonic,
		entity.determinerConcept_mnemonic,
		entity.typeConcept_mnemonic,
		entity.statusConcept_mnemonic,
		entity.classConcept,
		entity.createdBy,
		entity.creationTime,
		entity.determinerConcept,
		entity.obsoletedBy,
		entity.obsoletionTime,
		entity.replace_version_uuid,
		entity.statusConcept,
		entity.typeConcept,
		entity.updatedBy,
		entity.updatedTime,
		entity.version_uuid,
		entity.address_use,
		entity.address_guard,
		entity.address_use_mnemonic,
		entity.address_component_type,
		entity.address_component_guard,
		entity.address_component_type_mnemonic,
		entity.address_component_value,
		entity.extension_type,
		entity.extension_type_name,
		entity.identifier_value,
		entity.identifier_authority_name,
		entity.identifier_guard,
		entity.identifier_authority_domainName,
		entity.identifier_authority_oid,
		entity.name_use,
		entity.name_use_mnemonic,
		entity.name_guard,
		entity.name_component_type,
		entity.name_component_type_mnemonic,
		entity.name_component_guard,
		entity.name_component_value,
		entity.name_component_phoneticCode,
		entity.name_component_phoneticAlgorithm,
		entity.relationship_type,
		entity.relationship_name_use,
		entity.relationship_name_component_type,
		entity.relationship_name_component_value,
		entity.relationship_name_component_phoneticCode,
		entity.relationship_name_component_phoneticAlgorithm,
		entity.telecom_use,
		entity.telecom_value
	from patient inner join sqp_Entity as entity on (patient.uuid = entity.uuid)
	inner join person on (patient.uuid = person.uuid)
		inner join concept on (concept.uuid = patient.genderConcept)
		where entity.classConcept = X'6F9CCDBAA93F1E48963637457962804D';
	
-- ORGANIZATION VIEW
create view if not exists sqp_Organization as
select organization.*,
	concept.mnemonic as industryConcept_mnemonic,
				entity.classConcept_mnemonic,
		entity.determinerConcept_mnemonic,
		entity.typeConcept_mnemonic,
		entity.statusConcept_mnemonic,
		entity.classConcept,
		entity.createdBy,
		entity.creationTime,
		entity.determinerConcept,
		entity.obsoletedBy,
		entity.obsoletionTime,
		entity.replace_version_uuid,
		entity.statusConcept,
		entity.typeConcept,
		entity.updatedBy,
		entity.updatedTime,
		entity.version_uuid,
		entity.address_use,
		entity.address_guard,
		entity.address_use_mnemonic,
		entity.address_component_type,
		entity.address_component_guard,
		entity.address_component_type_mnemonic,
		entity.address_component_value,
		entity.extension_type,
		entity.extension_type_name,
		entity.identifier_value,
		entity.identifier_authority_name,
		entity.identifier_guard,
		entity.identifier_authority_domainName,
		entity.identifier_authority_oid,
		entity.name_use,
		entity.name_use_mnemonic,
		entity.name_guard,
		entity.name_component_type,
		entity.name_component_type_mnemonic,
		entity.name_component_guard,
		entity.name_component_value,
		entity.name_component_phoneticCode,
		entity.name_component_phoneticAlgorithm,
		entity.relationship_type,
		entity.relationship_name_use,
		entity.relationship_name_component_type,
		entity.relationship_name_component_value,
		entity.relationship_name_component_phoneticCode,
		entity.relationship_name_component_phoneticAlgorithm,
		entity.telecom_use,
		entity.telecom_value
	from organization inner join sqp_Entity as entity on (organization.uuid = entity.uuid)
	left join concept on (organization.industryConcept = concept.uuid)
		where entity.classConcept = X'55BD087C424DCD4992F86388D6C4183F';

-- PLACE VIEW
create view if not exists sqp_Place as
select place.*,
		place_service.serviceConcept as service_serviceConcept,
		concept.mnemonic as service_serviceConcept_mnemonic,
				entity.classConcept_mnemonic,
		entity.determinerConcept_mnemonic,
		entity.typeConcept_mnemonic,
		entity.statusConcept_mnemonic,
		entity.classConcept,
		entity.createdBy,
		entity.creationTime,
		entity.determinerConcept,
		entity.obsoletedBy,
		entity.obsoletionTime,
		entity.replace_version_uuid,
		entity.statusConcept,
		entity.typeConcept,
		entity.updatedBy,
		entity.updatedTime,
		entity.version_uuid,
		entity.address_use,
		entity.address_guard,
		entity.address_use_mnemonic,
		entity.address_component_type,
		entity.address_component_guard,
		entity.address_component_type_mnemonic,
		entity.address_component_value,
		entity.extension_type,
		entity.extension_type_name,
		entity.identifier_value,
		entity.identifier_authority_name,
		entity.identifier_guard,
		entity.identifier_authority_domainName,
		entity.identifier_authority_oid,
		entity.name_use,
		entity.name_use_mnemonic,
		entity.name_guard,
		entity.name_component_type,
		entity.name_component_type_mnemonic,
		entity.name_component_guard,
		entity.name_component_value,
		entity.name_component_phoneticCode,
		entity.name_component_phoneticAlgorithm,
		entity.relationship_type,
		entity.relationship_name_use,
		entity.relationship_name_component_type,
		entity.relationship_name_component_value,
		entity.relationship_name_component_phoneticCode,
		entity.relationship_name_component_phoneticAlgorithm,
		entity.telecom_use,
		entity.telecom_value
	from place inner join sqp_Entity as entity on (place.uuid = entity.uuid)
	    left join place_service on (place_service.entity_uuid = place.uuid)
		left join concept on (concept.uuid = place_service.serviceConcept)
		where entity.classConcept IN (X'754FDD79E8682247A7F58BC2E08F5CD6',
			X'B3FFB248DB07BA47AD73FC8FB8502471',
			X'7DEEEF6EF5DFD346A6A7171EF93879C7',
			X'28E5ACC77D27FB4E94EEBFD1278F40F7',
			X'7378AB21F38E784D9C194582B3C40631',
			X'F4643C0B0D22E544BDC64BFAD6120963',
			X'A7DF34FFD3C68B4FBC9F14BCDC13BA6C',
			X'B0B0F48CE584224185FE6AFA8240C218',
			X'285C1A4DB7DE1E41B75FD524F90DFA63',
			X'9632CD7458CB84499750D077A130CE05');

-- USER ENTITY VIEW
create view if not exists sqp_UserEntity as
select user.*,
	security_user.username as securityUser_userName,
	security_user.email as securityUser_email,
	security_user.phone_number as securityUser_email,
	security_user.locked as securityUser_lockout,
	sqp_Person.*
	from sqp_Person inner join user  on (user.uuid = sqp_Person.uuid)
	inner join security_user on (security_user.uuid = user.securityUser);
-- DEVICE VIEW
create view if not exists sqp_DeviceEntity as
select device.*,
	security_device.public_id as securityDevice_name,
				entity.classConcept_mnemonic,
		entity.determinerConcept_mnemonic,
		entity.typeConcept_mnemonic,
		entity.statusConcept_mnemonic,
		entity.classConcept,
		entity.createdBy,
		entity.creationTime,
		entity.determinerConcept,
		entity.obsoletedBy,
		entity.obsoletionTime,
		entity.replace_version_uuid,
		entity.statusConcept,
		entity.typeConcept,
		entity.updatedBy,
		entity.updatedTime,
		entity.version_uuid,
		entity.address_use,
		entity.address_guard,
		entity.address_use_mnemonic,
		entity.address_component_type,
		entity.address_component_guard,
		entity.address_component_type_mnemonic,
		entity.address_component_value,
		entity.extension_type,
		entity.extension_type_name,
		entity.identifier_value,
		entity.identifier_authority_name,
		entity.identifier_guard,
		entity.identifier_authority_domainName,
		entity.identifier_authority_oid,
		entity.name_use,
		entity.name_use_mnemonic,
		entity.name_guard,
		entity.name_component_type,
		entity.name_component_type_mnemonic,
		entity.name_component_guard,
		entity.name_component_value,
		entity.name_component_phoneticCode,
		entity.name_component_phoneticAlgorithm,
		entity.relationship_type,
		entity.relationship_name_use,
		entity.relationship_name_component_type,
		entity.relationship_name_component_value,
		entity.relationship_name_component_phoneticCode,
		entity.relationship_name_component_phoneticAlgorithm,
		entity.telecom_use,
		entity.telecom_value
	from device inner join sqp_Entity as entity on (device.uuid = entity.uuid)
	inner join security_device on (device.securityDevice = security_device.uuid)
		where entity.classConcept = X'04FF7313EFA60A42B1D04A07465FE8E8';

-- APPLICATION ENTITY
create view if not exists sqp_ApplicationEntity as
select application.*,
	security_application.public_id as securityApplication_name,
				entity.classConcept_mnemonic,
		entity.determinerConcept_mnemonic,
		entity.typeConcept_mnemonic,
		entity.statusConcept_mnemonic,
		entity.classConcept,
		entity.createdBy,
		entity.creationTime,
		entity.determinerConcept,
		entity.obsoletedBy,
		entity.obsoletionTime,
		entity.replace_version_uuid,
		entity.statusConcept,
		entity.typeConcept,
		entity.updatedBy,
		entity.updatedTime,
		entity.version_uuid,
		entity.address_use,
		entity.address_guard,
		entity.address_use_mnemonic,
		entity.address_component_type,
		entity.address_component_guard,
		entity.address_component_type_mnemonic,
		entity.address_component_value,
		entity.extension_type,
		entity.extension_type_name,
		entity.identifier_value,
		entity.identifier_authority_name,
		entity.identifier_guard,
		entity.identifier_authority_domainName,
		entity.identifier_authority_oid,
		entity.name_use,
		entity.name_use_mnemonic,
		entity.name_guard,
		entity.name_component_type,
		entity.name_component_type_mnemonic,
		entity.name_component_guard,
		entity.name_component_value,
		entity.name_component_phoneticCode,
		entity.name_component_phoneticAlgorithm,
		entity.relationship_type,
		entity.relationship_name_use,
		entity.relationship_name_component_type,
		entity.relationship_name_component_value,
		entity.relationship_name_component_phoneticCode,
		entity.relationship_name_component_phoneticAlgorithm,
		entity.telecom_use,
		entity.telecom_value
	from application inner join sqp_Entity as entity on (application.uuid = entity.uuid)
	inner join security_application on (application.securityApplication = security_application.uuid)
		where entity.classConcept = X'ADCF9FE21DEC604CA055039A494248AE';