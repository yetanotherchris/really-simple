CREATE TABLE "categories" 
(
    "id" GUID NOT NULL, 
    "title" VARCHAR
);

CREATE TABLE "sites"
(
    "id" GUID NOT NULL,
    "title" VARCHAR,
    "url" VARCHAR
);

CREATE  TABLE "feeds"
(
    "id" GUID NOT NULL,
    "categoryid" GUID,
    "siteid" GUID,
	"url" VARCHAR,
    "type" INTEGER,
    "lastUpdate" DATETIME,
    "cleaner" VARCHAR
);