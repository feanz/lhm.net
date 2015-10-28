# Large Hadron Migrator .net #

Dev build: [![Build status](https://ci.appveyor.com/api/projects/status/github/feanz/lhm.net?branch=develop&svg=true)](https://ci.appveyor.com/api/projects/status/github/feanz/lhm.net?branch=develop&svg=true)

A port of [large hadron migrator](https://github.com/soundcloud/lhm) for .net.

Database migrations tool such as entity framework or fluent migrator are a useful way to evolve your data schema in an agile manner. Most .net projects start like this, and at first, making changes is fast and easy.  

That is until your tables grow to millions of records. At this point, the locking nature of ALTER TABLE may take your site down for an hour or more while critical tables are migrated. In order to avoid this, developers begin to design around the problem by introducing join tables or moving the data into another layer. Development gets less and less agile as tables grow and grow. To make the problem worse, adding or changing indices to optimize data access becomes just as difficult.

> Side effects may include black holes and universe implosion.

This tool is based on the popular migration tool for Rails and mysql called large hadron migrator developed at soundcloud.  We have ported there implementation to deal with some of the extra issues when trying to apply this pattern to MS SQL Server. 

![LHC](http://farm4.static.flickr.com/3093/2844971993_17f2ddf2a8_z.jpg)


[The Large Hadron collider at CERN](http://en.wikipedia.org/wiki/Large_Hadron_Collider)

## The idea

The basic idea is to perform the migration online while the system is live,
without locking the table. This is done by copying table and using triggers.

## Requirements

Lhm currently only works with MS SQL databases and currently needs to be plugged into an existing migration tool or ran as a custom tool.

First ruff version of Lhm.net

Existing functions

* Add columns
* Rename columns
* Drop column
* Create duplicate copy of table
* Applies migration to table (only supports add column for now)
* Create triggers to keep duplicate upto date
* Copys data into new table with configurable batch size and deplays
* Renames old table to archive name, renames new table to old table name

There is a [sql script](https://github.com/feanz/lhm.net/blob/develop/src/lhm.net/SampleDatabase.sql) in the main project to make a sammple DB. 
