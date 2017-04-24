CREATE TABLE IF NOT EXISTS dw_datamarts (
	uuid  blob(16) not null primary key,
	name varchar(64) not null unique,
	creation_time bigint not null,
	schema_id blob(16) not null
);

CREATE UNIQUE INDEX IF NOT EXISTS dw_datamarts_name_idx ON dw_datamarts(name);

CREATE TABLE IF NOT EXISTS dw_schemas (
	uuid blob(16) not null primary key,
	name varchar(64) not null unique
);

CREATE UNIQUE INDEX IF NOT EXISTS dw_schemas_name_idx ON dw_schemas(name);

CREATE TABLE IF NOT EXISTS dw_st_query (
	uuid blob(16) not null,
	schema_id blob(16) not null,
	name varchar(64) not null unique,
	constraint pk_dw_st_query primary key (uuid)
);

CREATE TABLE IF NOT EXISTS dw_properties (
	uuid blob(16) not null primary key,
	cont_id blob(16) not null,
	name varchar(64) not null,
	type int not null,
	attr int not null
);

CREATE INDEX IF NOT EXISTS dw_properties_cont_idx ON dw_properties(cont_id);