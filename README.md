This project was created for the purpose recreating a SpringBoot Command line into a C# Command line:

- Read Balances.csv 
- Create Summary.csv
- Created on a MAC with Visual Studio for Mac Community Version 8.8.6, C#, and PostgreSQL
- Implements Command Line and Npgsql (5.0.3)
- Program is writing to console for debugging


I am running a postgreSQL server on my localhost.  I created a database mydb.
User danielb was created as a superuser.

The PostgreSQL table used to store and summarize the data.

```
create table balances
(
	balances_id serial,
	first_name varchar(50),
	last_name varchar(50),
	address varchar(100),
	city varchar(50),
	state varchar(50),
	zip varchar(50),
	phone varchar(50),
	balance numeric(20,2),
	createtime timestamp not null,
	filename varchar(100)
);

create index createfileidx on balances 
(createtime, filename);
```

I had to rename the folders before commit from vsDemoCmdLn to csDemoCmdLn.  Git did not want to upload the files inside the folder.  When I renamed it, it worked.

