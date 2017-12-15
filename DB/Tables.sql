use marista
go


if OBJECT_ID('Level') is not null
begin
  exec p_ak_drop_all_foreign_keys 'Level'
  drop table Level
end
go

create table Level 
(
  LevelId int not null,
  Name nvarchar(50) not null,
  constraint pk_LevelId primary key (LevelId)
)
go

exec p_ak_create_fk_indeces 'Level'
go

insert into Level values (1,'Administrator'), (2,'BrandPromoter')
go


if OBJECT_ID('SiteUser') is not null
begin
  exec p_ak_drop_all_foreign_keys 'SiteUser'
  drop table SiteUser
end
go

create table SiteUser 
(
  SiteUserId int not null identity(1,1),
  Username nvarchar(50) not null,
  Password nvarchar(32) not null,
  LevelId int not null,
  constraint pk_SiteUserId primary key (SiteUserId),
  constraint fk_SiteUser_LevelId foreign key (LevelId) references Level(LevelId)
)
go

exec p_ak_create_fk_indeces 'SiteUser'
go

insert into SiteUser(Username, Password, LevelId)
values ('admin', '202cb962ac59075b964b07152d234b70', 1)
go