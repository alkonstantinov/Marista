use marista
go
delete from SaleDetail
delete from sale
delete from customer
delete from Pyramid
delete from ResultHistory
delete from bp
delete from ChatItem
delete from chat
delete from Coupon
delete from siteuser where SiteUserId>1

if OBJECT_ID('pTestAddPyramid') is not null
  drop procedure  pTestAddPyramid 
go
  
create procedure pTestAddPyramid @parentSiteUser int as
begin  
  declare @name nvarchar(10)
  select @name='user'+cast (count(1) as nvarchar(max)) from SiteUser
  declare @siteuserid int
  insert into SiteUser(Username, Password, LevelId)
  values (@name, '202cb962ac59075b964b07152d234b70', 2)
  select @siteuserid = @@IDENTITY

  if @parentSiteUser is not null
  insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
  select p.PyramidId, null, @siteuserid, 25
  from Pyramid p
  where p.SiteUserId = @parentSiteUser
  else
  insert into Pyramid (PyramidParentId, PyramidSpinoffParentId, SiteUserId, PBV)
  values(null, null, @siteuserid, 25)
  

  return @siteuserid
end
go

if OBJECT_ID('pTestAdd4') is not null
  drop procedure  pTestAdd4
go
  
create procedure pTestAdd4 as
begin  
  declare @tbl table (id int)
  insert into @tbl(id)
  select p1.siteuserid
  from Pyramid p1
  left join Pyramid p2 on p2.PyramidParentId = p1.PyramidId
  where p2.PyramidId is null
  declare crs cursor for 
  select id from @tbl

  declare @su int
  open crs
  fetch next from crs into @su
  while @@FETCH_STATUS=0
  begin
    print @su
    execute pTestAddPyramid @su
    execute pTestAddPyramid @su
    execute pTestAddPyramid @su
    execute pTestAddPyramid @su
    fetch next from crs into @su
  end
  close crs
  deallocate crs

end
go

exec pTestAddPyramid null --1
exec pTestAdd4  --4
exec pTestAdd4  --16
exec pTestAdd4  --64
exec pTestAdd4  --256


select * from pyramid 