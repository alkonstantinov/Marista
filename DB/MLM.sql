--пълнене
set nocount on

truncate table pyramid

delete from SiteUser where SiteUserId>1

insert into SiteUser(Username, Password, LevelId)
values ('A1.1', '202cb962ac59075b964b07152d234b70', 2)
insert into SiteUser(Username, Password, LevelId)
values ('A2.1', '202cb962ac59075b964b07152d234b70', 2)
insert into SiteUser(Username, Password, LevelId)
values ('A2.2', '202cb962ac59075b964b07152d234b70', 2)
insert into SiteUser(Username, Password, LevelId)
values ('A3.1', '202cb962ac59075b964b07152d234b70', 2)
insert into SiteUser(Username, Password, LevelId)
values ('A3.2', '202cb962ac59075b964b07152d234b70', 2)
insert into SiteUser(Username, Password, LevelId)
values ('A3.3', '202cb962ac59075b964b07152d234b70', 2)
insert into SiteUser(Username, Password, LevelId)
values ('A4.1', '202cb962ac59075b964b07152d234b70', 2)


insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
select null, null, su.SiteUserId, 100
from SiteUser su
where su.Username = 'A1.1'

insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
select p.PyramidId, null, su.SiteUserId, 100
from SiteUser su
join SiteUser psu on psu.Username = 'A1.1'
join Pyramid p on p.SiteUserId = psu.SiteUserId
where su.Username = 'A2.1'

insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
select p.PyramidId, null, su.SiteUserId, 100
from SiteUser su
join SiteUser psu on psu.Username = 'A1.1'
join Pyramid p on p.SiteUserId = psu.SiteUserId
where su.Username = 'A2.2'

insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
select p.PyramidId, null, su.SiteUserId, 100
from SiteUser su
join SiteUser psu on psu.Username = 'A2.1'
join Pyramid p on p.SiteUserId = psu.SiteUserId
where su.Username = 'A3.1'

insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
select p.PyramidId, null, su.SiteUserId, 100
from SiteUser su
join SiteUser psu on psu.Username = 'A2.1'
join Pyramid p on p.SiteUserId = psu.SiteUserId
where su.Username = 'A3.2'

insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
select p.PyramidId, null, su.SiteUserId, 100
from SiteUser su
join SiteUser psu on psu.Username = 'A2.1'
join Pyramid p on p.SiteUserId = psu.SiteUserId
where su.Username = 'A3.3'

insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
select p.PyramidId, null, su.SiteUserId, 100
from SiteUser su
join SiteUser psu on psu.Username = 'A3.1'
join Pyramid p on p.SiteUserId = psu.SiteUserId
where su.Username = 'A4.1'


--;with tbl as
--(
--  select p.PyramidId, p.PyramidParentId, p.SiteUserId
--  from Pyramid p
--  where p.PyramidParentId is null
--  union all
--  select p.PyramidId, p.PyramidParentId, p.SiteUserId
--  from Pyramid p
--  join tbl on tbl.PyramidId = p.PyramidParentId
--)

--select tbl.PyramidParentId, tbl.PyramidId, su.Username
--from tbl
--join SiteUser su on su.SiteUserId = tbl.SiteUserId





--създаване на временна таблица със собствената пирамида
declare @su int 


select @su = su.SiteUserId
from SiteUser su 
where su.Username = 'A2.1'

declare @mp table
(
  PyramidId int not null primary key,
  PyramidParentId int null,
  SiteUserId int not null,
  PBV decimal (10,2) not null default 0
)



;with tbl as
(
  select p.PyramidId, p.PyramidParentId, p.SiteUserId
  from Pyramid p
  where p.SiteUserId = @su
  union all
  select p.PyramidId, p.PyramidParentId, p.SiteUserId
  from Pyramid p
  join tbl on tbl.PyramidId = p.PyramidParentId
)
insert into @mp (PyramidId, PyramidParentId, PBV, SiteUserId)
select tbl.PyramidId, tbl.PyramidParentId, p.PBV, tbl.SiteUserId
from tbl
join Pyramid p on p.PyramidId = tbl.PyramidId
join SiteUser su on su.SiteUserId = tbl.SiteUserId



;with tbl as
(
  select p.PyramidId, 1 level
  from @mp p
  where p.SiteUserId = @su
  union all
  select p.PyramidId, tbl.level + 1 level
  from Pyramid p
  join tbl on tbl.PyramidId = p.PyramidParentId
)

select tbl.PyramidId, tbl.level, su.Username 
from tbl
join @mp mp on mp.PyramidId = tbl.PyramidId
join SiteUser su on su.SiteUserId = mp.SiteUserId
order by level desc



set nocount off