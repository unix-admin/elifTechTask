
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

DROP TABLE IF EXISTS data;
CREATE TABLE data (id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR (255), estimated DOUBLE, parent_id INTEGER DEFAULT (0))
INSERT INTO data (id, name, estimated, parent_id) VALUES (1, 'Company1', 25, 0);
INSERT INTO data (id, name, estimated, parent_id) VALUES (2, 'Company2', 13, 1);
INSERT INTO data (id, name, estimated, parent_id) VALUES (3, 'Company3', 5, 2);
INSERT INTO data (id, name, estimated, parent_id) VALUES (4, 'Company4', 10, 1);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
