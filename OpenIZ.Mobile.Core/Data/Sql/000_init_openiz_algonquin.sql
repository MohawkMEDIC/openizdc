CREATE VIEW IF NOT EXISTS FindConceptsByName AS
	SELECT concept.*, concept_name.name as Name, concept_name.lang as Language FROM concept_name INNER JOIN concept ON (concept.uuid = concept_name.concept_uuid)
	WHERE concept.obsoletion_time IS NULL;
