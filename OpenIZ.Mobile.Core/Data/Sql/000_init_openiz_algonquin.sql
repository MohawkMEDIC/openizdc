
-- SUPPORTING VIEWS
CREATE VIEW act_participation_view AS 
	SELECT act_participation.*, concept.mnemonic as participationRole_mnemonic 
	FROM act_participation INNER JOIN concept ON (act_participation.participationRole = concept.uuid);

CREATE VIEW act_identifier_view AS
	SELECT act_identifier.*, assigning_authority.domainName AS identifier_domainName, assigning_authority.oid AS identifier_oid, assigning_authority.url AS identifier_url, concept.mnemonic as identifier_type_mnemonic
	FROM act_identifier INNER JOIN assigning_authority ON (act_identifier.authority = assigning_authority.uuid)
	LEFT JOIN identifier_type ON (act_identifier.type = identifier_type.uuid)
	LEFT JOIN concept ON (concept.uuid = identifier_type.typeConcept);

CREATE VIEW act_relationship_view AS
	SELECT act_relationship.*, concept.mnemonic as relationshipType_mnemonic 
	FROM act_relationship INNER JOIN concept ON (act_relationship.relationshipType = concept.uuid);


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
		er.relationshipType as relationship_type,
		er.entity_uuid as relationship_source,
		er.target as relationship_target,
		not(er.kuuid = entity.uuid) as relationship_inversionInd,
		rel_type.mnemonic as relationship_guard,
		rel_entity_name.use as relationship_target_name_use,
		rel_entity_name_comp.type as relationship_target_name_component_type,
		rel_entity_name_comp.value as relationship_target_name_component_value,
		rel_entity_name_comp.phoneticCode as relationship_target_name_component_phoneticCode,
		rel_entity_name_comp.phoneticAlgorithm as relationship_target_name_component_phoneticAlgorithm,
		entity_telecom.use as telecom_use,
		entity_telecom.value as telecom_value,
		rel_entity_telecom.use as relationship_target_telecom_use,
		rel_entity_telecom.value as relationship_target_telecom_value,
		rel_entity_identifier.value as relationship_target_identifier_value
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
		left join (
			select entity_uuid as kuuid, * from entity_relationship
			union 
			select target as kuuid, * from entity_relationship
		) as er on (er.kuuid = entity.uuid)
		left join concept as rel_type on (er.relationshipType = rel_type.uuid)
		left join entity_telecom on (entity_telecom.entity_uuid = entity.uuid)
		left join entity as rel_entity on (rel_entity.uuid = er.target)
		left join entity_name as rel_entity_name on (rel_entity.uuid = rel_entity_name.entity_uuid)
		left join entity_name_comp as rel_entity_name_comp on (rel_entity_name.uuid = rel_entity_name_comp.name_uuid)
		left join entity_telecom as rel_entity_telecom on (rel_entity_telecom.entity_uuid = rel_entity.uuid)
		left join entity_identifier as rel_entity_identifier on (rel_entity_identifier.entity_uuid = rel_entity.uuid);
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
		entity.relationship_guard,
		entity.relationship_type,
		entity.relationship_source,
		entity.relationship_target,
		entity.relationship_inversionInd,
		entity.relationship_target_name_use,
		entity.relationship_target_name_component_type,
		entity.relationship_target_name_component_value,
		entity.relationship_target_name_component_phoneticCode,
		entity.relationship_target_name_component_phoneticAlgorithm,
		entity.telecom_use,
		entity.telecom_value,
		entity.relationship_target_telecom_use,
		entity.relationship_target_telecom_value,
		entity.relationship_target_identifier_value
	from person inner join sqp_Entity as entity on (person.uuid = entity.uuid)
		where entity.classConcept IN (X'46A8E29DF2DDBC4E902E84508C5089EA', X'D8FE046B64C19C46910BF824C2BDA4F0');
	


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
		entity.relationship_guard,
		entity.relationship_type,
		entity.relationship_source,
		entity.relationship_target,
		entity.relationship_inversionInd,
		entity.relationship_target_name_use,
		entity.relationship_target_name_component_type,
		entity.relationship_target_name_component_value,
		entity.relationship_target_name_component_phoneticCode,
		entity.relationship_target_name_component_phoneticAlgorithm,
		entity.relationship_target_telecom_use,
		entity.relationship_target_telecom_value,
		entity.relationship_target_identifier_value,
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
		entity.relationship_guard,
		entity.relationship_type,
		entity.relationship_source,
		entity.relationship_target,
		entity.relationship_inversionInd,
		entity.relationship_target_name_use,
		entity.relationship_target_name_component_type,
		entity.relationship_target_name_component_value,
		entity.relationship_target_name_component_phoneticCode,
		entity.relationship_target_name_component_phoneticAlgorithm,
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
		entity.relationship_guard,
		entity.relationship_type,
		entity.relationship_source,
		entity.relationship_target,
		entity.relationship_inversionInd,
		entity.relationship_target_name_use,
		entity.relationship_target_name_component_type,
		entity.relationship_target_name_component_value,
		entity.relationship_target_name_component_phoneticCode,
		entity.relationship_target_name_component_phoneticAlgorithm,
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
	left join security_user on (security_user.uuid = user.securityUser);

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
		entity.relationship_guard,
		entity.relationship_type,
		entity.relationship_source,
		entity.relationship_target,
		entity.relationship_inversionInd,
		entity.relationship_target_name_use,
		entity.relationship_target_name_component_type,
		entity.relationship_target_name_component_value,
		entity.relationship_target_name_component_phoneticCode,
		entity.relationship_target_name_component_phoneticAlgorithm,
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
		entity.relationship_guard,
		entity.relationship_type,
		entity.relationship_source,
		entity.relationship_target,
		entity.relationship_inversionInd,
		entity.relationship_target_name_use,
		entity.relationship_target_name_component_type,
		entity.relationship_target_name_component_value,
		entity.relationship_target_name_component_phoneticCode,
		entity.relationship_target_name_component_phoneticAlgorithm,
		entity.telecom_use,
		entity.telecom_value
	from application inner join sqp_Entity as entity on (application.uuid = entity.uuid)
	inner join security_application on (application.securityApplication = security_application.uuid)
		where entity.classConcept = X'ADCF9FE21DEC604CA055039A494248AE';


-- ACT SUPPORTING VIEW

CREATE VIEW IF NOT EXISTS sqp_Act AS
SELECT act.*,
	class_concept.mnemonic AS classConcept_mnemonic,
	mood_concept.mnemonic AS moodConcept_mnemonic,
	status_concept.mnemonic AS statusConcept_mnemonic,
	type_concept.mnemonic AS typeConcept_mnemonic,
	reason_concept.mnemonic AS reasonConcept_mnemonic,
	act_identifier_view.value AS identifier_value,
	act_identifier_view.authority AS identifier_authority,
	act_identifier_view.identifier_domainName AS identifier_authority_domainName,
	act_identifier_view.identifier_domainName AS identifier_guard,
	act_identifier_view.identifier_oid AS identifier_authority_oid,
	act_identifier_view.identifier_url AS identifier_authority_url,
	act_identifier_view.type AS identifier_identifierType,
	act_identifier_view.identifier_type_mnemonic AS identifier_identifierType_mnemonic,
	act_participation_view.entity_uuid AS participation_player,
	act_participation_view.participationRole AS participation_participationRole,
	act_participation_view.participationRole_mnemonic AS participation_participationRole_mnemonic,
	act_participation_view.participationRole_mnemonic AS participation_guard,
	act_relationship_view.target AS relationship_target,
	act_relationship_view.relationshipType AS relationship_relationshipType,
	act_relationship_view.relationshipType_mnemonic AS relationship_relationshipType_mnemonic,
	act_relationship_view.relationshipType_mnemonic AS relationship_guard
	FROM 
	act INNER JOIN concept class_concept ON (act.classConcept = class_concept.uuid)
	INNER JOIN concept mood_concept ON (act.moodConcept = mood_concept.uuid)
	INNER JOIN concept status_concept ON (act.statusConcept = status_concept.uuid)
	LEFT JOIN concept type_concept ON (act.typeConcept = type_concept.uuid)
	LEFT JOIN concept reason_concept ON (act.reasonConcept = reason_concept.uuid)
	LEFT JOIN act_identifier_view ON (act.uuid = act_identifier_view.act_uuid)
	LEFT JOIN act_participation_view ON (act.uuid = act_participation_view.act_uuid)
	LEFT JOIN act_relationship_view ON (act.uuid = act_relationship_view.act_uuid);
	

CREATE VIEW IF NOT EXISTS sqp_AssigningAuthority AS
	SELECT assigning_authority.*, assigning_authority_scope.concept as scope_id, concept.mnemonic as scope_mnemomnic
	FROM assigning_authority LEFT JOIN assigning_authority_scope ON (assigning_authority.uuid = assigning_authority_scope.authority)
	LEFT JOIN concept ON (assigning_authority_scope.concept = concept.uuid);

CREATE VIEW IF NOT EXISTS sqp_Material AS
SELECT sqp_entity.*,
	material.expiry as expiryDate,
	material.isAdministrative as isAdministrative,
	material.quantity as quantity,
	formConcept.uuid as formConcept,
	formConcept.mnemonic as formConcept_mnemonic,
	quantityConcept.uuid as quantityConcept,
	quantityConcept.mnemonic as quantityConcept_mnemonic
FROM sqp_entity INNER JOIN material ON (sqp_entity.uuid = material.uuid)
LEFT JOIN concept as formConcept ON (material.form_concept_uuid = formConcept.uuid)
LEFT JOIN concept as quantityConcept ON (material.quantity_concept_uuid = quantityConcept.uuid)
WHERE classConcept IN (X'BE7390D38F0F0E44B8C87034CC138A95', X'86C2FEFAD5890B429085054ACA9D1EEF');

create view if not exists sqp_ManufacturedMaterial as
select sqp_material.*, lotNumber from sqp_material inner join manufactured_material on (sqp_material.uuid = manufactured_material.uuid);

create view if not exists sqp_Provider as 
select sqp_Person.*, 
	provider.*,
	specialty.mnemonic as specialty_mnemonic
	from sqp_Person inner join provider on (sqp_Person.uuid = provider.uuid)
	left join concept specialty on (specialty.uuid = provider.specialty)
	where sqp_Person.classConcept = X'D8FE046B64C19C46910BF824C2BDA4F0';


create index entity_relationship_type on entity_relationship(entity_uuid, target, relationshipType);