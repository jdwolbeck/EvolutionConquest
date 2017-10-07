drop table Ancestors
go
create table Ancestors
(
	TableKey int Identity(1,1) not null,
	SessionID int not null,
	CreatureID int not null,
	AncestorID int not null,
	AncestorName varchar(200) not null
)