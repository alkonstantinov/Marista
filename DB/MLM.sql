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
insert into SiteUser(Username, Password, LevelId)
values ('B1.1', '202cb962ac59075b964b07152d234b70', 2)



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


insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
select null, p.PyramidId, su.SiteUserId, 100
from SiteUser su
join SiteUser psu on psu.Username = 'A1.1'
join Pyramid p on p.SiteUserId = psu.SiteUserId
where su.Username = 'B1.1'


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

if object_id('pCalcMyProfit') is not null
  drop procedure pCalcMyProfit
go

create procedure pCalcMyProfit @pu int as
begin

  declare @mp table
  (
    PyramidId int not null primary key,
    PyramidParentId int null,
    SiteUserId int not null,
    PBV decimal (10,2) not null default 0,
    Prcnt decimal(10,2) not null default 0,
    ParentPrcnt decimal(10,2) not null default 0,
    SoPrcnt decimal(10,2) not null default 0,
    SoParentPrcnt decimal(10,2) not null default 0,
    FromOthers decimal(10,2) not null default 0,
    ToReceive decimal(10,2) not null default 0
  )

  declare @process table
  (
    PyramidId int not null primary key
  )

  declare @allLevels table
  (
    PyramidId int not null primary key,
    Level int
  )



  declare @restart bit = 0
  
  ;with tbl as
  (
    select p.PyramidId, p.PyramidParentId, p.SiteUserId
    from Pyramid p
    where p.PyramidId = @pu
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
  

  declare @level int 
  
  
  ;with tbl as
  (
    select p.PyramidId, 1 level
    from @mp p
    where p.PyramidId = @pu
    union all
    select p.PyramidId, tbl.level + 1 level
    from Pyramid p
    join tbl on tbl.PyramidId = p.PyramidParentId
  )
  insert into @allLevels(PyramidId, Level)
  select tbl.PyramidId, tbl.level 
  from tbl
  join @mp mp on mp.PyramidId = tbl.PyramidId
  
  
  select @level = max (level) from @allLevels
  
  while @level>=1
  begin
    delete from @process

    insert into @process
    select PyramidId from @allLevels where level = @level

    
    --взимаме каквото се вдига отдолу
    update mp
    set FromOthers =  
      isnull((
        select
        sum( 
          case
            when children.PBV>=100 then children.PBV+children.FromOthers
            else 0
          end
          ) 
        from @mp children
        where children.PyramidParentId = mp.PyramidId
      ),0)
    from @mp mp
    join @process p on p.PyramidId = mp.PyramidId 
       
    --оправяме процента
    update mp
    set Prcnt = bs.BonusPercent
    from @mp mp
    join @process p on p.PyramidId = mp.PyramidId 
    join BonusSize bs on bs.FromBonus<=mp.FromOthers and bs.ToBonus>=mp.FromOthers


    --оправяме спиноф процента
    update mp
    set SoPrcnt = bs.BonusPercent
    from @mp mp
    join @process p on p.PyramidId = mp.PyramidId 
    join SoBonusSize bs on 
      (
        select count (1) from Pyramid sop where sop.PyramidSpinoffParentId = mp.PyramidId
      ) between bs.FromSO and bs.ToSO


    --Сетваме процентите от родителите
    update mp
    set 
      ParentPrcnt = (select top 1 parent.Prcnt from @mp parent where parent.PyramidId = p.PyramidId),
      SoParentPrcnt = (select top 1 parent.SoPrcnt from @mp parent where parent.PyramidId = p.PyramidId)
    from @mp mp
    join @process p on p.PyramidId = mp.PyramidParentId
      
    --изчисляваме печалбите    
      
    update mp
    set ToReceive = PBV*(select top 1 c.Value from constant c where c.ConstantId = 4)/100.00
      + 
      isnull((
        select sum ( 
          case
            when children.PBV>=100 then children.FromOthers*(children.ParentPrcnt - children.Prcnt)/100.0
            else 0.0
          end 
          )
        from @mp children
        where children.PyramidParentId = mp.PyramidId
      ),0)+
      isnull((
        select sum (         
          case
            when children.PBV>=100 then children.FromOthers*children.SoParentPrcnt/100.0
            else 0.0
          end 
          )
        from Pyramid children
        where children.PyramidSpinoffParentId = mp.PyramidId
      ),0)
    from @mp mp
    join @process p on p.PyramidId = mp.PyramidId 

    --проверяваме дали има спиноф
    
    if exists (
      select top 1 1 
      from @mp mp
      join @process p on p.PyramidId = mp.PyramidId        
      where mp.Prcnt = (select max(bs.BonusPercent) from BonusSize bs)
    ) 
    begin
      -- ако да - спиноф и повтаряме
      update Pyramid
      set 
        PyramidSpinoffParentId = p.PyramidParentId,
        PyramidParentId = null
      from Pyramid p
      join  @mp mp on mp.PyramidId = p.PyramidId
      join @process prc on prc.PyramidId = mp.PyramidId        
      where mp.Prcnt = (select max(bs.BonusPercent) from BonusSize bs)

      return 1;
    end
    else
    begin
      select @level = @level - 1
      
    end

    --select * from @mp    
  end
    
  update p
  set
    p.FromOthers = mp.FromOthers,
    p.Prcnt = mp.Prcnt,
    p.SoPrcnt = mp.SoPrcnt,
    p.ToReceive = mp.ToReceive
  from Pyramid p
  join @mp mp on mp.PyramidId = p.PyramidId

  
end
go

declare @process table
(
  id int not null identity (1,1) primary key,
  PyramidId int not null 
)

declare @su int 
declare @repeat bit = 0
declare @calcResult int = 0
while 1 > 0
begin
  delete from @process
  select @repeat = 0

  ;with roots as
  (
    select p.pyramidid id
    from Pyramid p
    where p.PyramidParentId is null
  ), id2root as
  (
    select r.id id, r.id root
    from roots r
    union all
    select p.PyramidId id, i2r.root
    from Pyramid p
    join id2root i2r on i2r.id = p.PyramidParentId
  ), so as
  (
    select r.id, 1 level
    from roots r
    join Pyramid p on p.PyramidId = r.id
    where p.PyramidSpinoffParentId is null
    union all
    select r.id, so.level + 1 level
    from roots r
    join Pyramid p on p.PyramidId = r.id
    join id2root i2r on p.PyramidSpinoffParentId = i2r.id
    join so on so.id = i2r.root
  

  )
  insert into @process (PyramidId)
  select id from so order by level desc


  declare crs cursor for select Pyramidid from @process order by id
  open crs
  fetch next from crs into @su

  while @@FETCH_STATUS=0 
  begin
    exec @calcResult = pCalcMyProfit @su
    
    if @calcResult = 1
    begin
      select @repeat = 1;
      break;
    end

    fetch next from crs into @su
  end
  close crs
  deallocate crs
  if @repeat = 0 break
end

select * from Pyramid
--exec pCalcMyProfit @su
set nocount off