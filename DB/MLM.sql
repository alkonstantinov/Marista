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

if object_id('pCalcMyProfit') is not null
  drop procedure pCalcMyProfit
go

create procedure pCalcMyProfit @su int as
begin

  declare @mp table
  (
    PyramidId int not null primary key,
    PyramidParentId int null,
    SiteUserId int not null,
    PBV decimal (10,2) not null default 0,
    Prcnt decimal(10,2) not null default 0,
    FromOthers decimal(10,2) not null default 0,
    ToReceive decimal(10,2) not null default 0
  )

  declare @process table
  (
    PyramidId int not null primary key
  )

  print 1
  declare @restart bit = 0
  while 1>0
  begin
    select @restart = 0
    delete from @mp

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

    print 2
    delete from @process

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
    insert into @process
    select tbl.PyramidId 
    from tbl
    join @mp mp on mp.PyramidId = tbl.PyramidId
    join SiteUser su on su.SiteUserId = mp.SiteUserId
    order by level desc
    
    print 3
    while exists (select top 1 1 from @process)
    begin
      --взимаме каквото се вдига отдолу
      update mp
      set FromOthers = PBV + 
        isnull((
          select
          sum( 
            case
              when children.PBV>=100 then children.PBV
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

      --намаляваме процента на децата за да сметнем печалбите
      update mp
      set Prcnt = (select top 1 parent.Prcnt from @mp parent where parent.PyramidId = p.PyramidId) - Prcnt
      from @mp mp
      join @process p on p.PyramidId = mp.PyramidParentId
      
      --изчисляваме печалбите    
      
      update mp
      set ToReceive = PBV*(select top 1 c.Value from constant c where c.ConstantId = 4)/100.00
       + 
        isnull((
          select sum ( 
            case
              when children.PBV>=100 then children.FromOthers*children.Prcnt/100.0
              else 0.0
            end 
            )
          from @mp children
          where children.PyramidParentId = mp.PyramidId
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

        select @restart = 1
        break;
      end
      else
      begin
        delete from @process
        insert into @process
        select mp.PyramidParentId
        from @mp mp 
        join @process prc on prc.PyramidId = mp.PyramidId
        where mp.SiteUserId <> @su

        select * from @process
      end

      if @restart=1 
        break;
    end
    if @restart =0
      break;
  end

  select * from @mp
end
go

declare @su int 


select @su = su.SiteUserId
from SiteUser su 
where su.Username = 'A2.1'

exec pCalcMyProfit @su
set nocount off