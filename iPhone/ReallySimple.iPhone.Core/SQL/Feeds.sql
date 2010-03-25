CREATE  TABLE "feeds"
(
    "id" GUID NOT NULL,
    "categoryid" GUID,
    "siteid" GUID,
	"url" VARCHAR,
    "type" INTEGER,
    "lastUpdate" DATETIME,
    "cleaner" VARCHAR
)