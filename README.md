# lhm.net
A port of large hadron migrator for .net

First ruff version of Lhm.net
Existing functions

* Add columns
* Create duplicate copy of table
* Applies migration to table (only supports add column for now)
* Create triggers to keep duplicate upto date
* Copys data into new table with configurable batch size and deplays
* Renames old table to archive name, renames new table to old table name

This verions only includes the ability to add columns in terms of
migrations that you can do.

Also needs to add support for custom indexes and foreign keys to the
CreateDestinationTable function.

There is a sql script in the main project to make a sammple DB. 
