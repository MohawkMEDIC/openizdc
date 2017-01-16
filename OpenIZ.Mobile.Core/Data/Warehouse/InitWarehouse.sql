-- DATAMART REGISTRATION TABLE
CREATE TABLE IF NOT EXISTS dw_datamarts (
	uuid  blob(16) not null primary key,
	name varchar(64) not null unique,
	creation_time datetime not null default current_timestamp,
	schema_id blob(16) not null
);

-- DATAMART NAME INDEX
CREATE UNIQUE INDEX IF NOT EXISTS dw_datamarts_name_idx ON dw_datamarts(name);

-- SCHEMA NAMES (ROOT OBJECTS)
CREATE TABLE IF NOT EXISTS dw_schemas (
	uuid blob(16) not null primary key,
	name varchar(64) not null unique
);

-- SCHEMA NAME INDEX
CREATE UNIQUE INDEX IF NOT EXISTS dw_schemas_name_idx ON dw_schemas(name);

-- PROPERTIES TABLE 
CREATE TABLE IF NOT EXISTS dw_properties (
	uuid blob(16) not null primary key,
	cont_id blob(16) not null,
	name varchar(64) not null,
	type int not null,
	attr int not null
);

-- CONTAINER INDEX
CREATE INDEX IF NOT EXISTS dw_properties_cont_idx ON dw_properties(cont_id);