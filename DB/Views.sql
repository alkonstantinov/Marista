use Marista
go

if OBJECT_ID('vSalesTotal') is not null
  drop view vSalesTotal
go

create view vSalesTotal as
  select 
    s.SaleId,
    s.OnDate,
    c.SiteUserId,
    cu.CustomerName,
    (
      select sum (sd.Price*cast(sd.Quantity as decimal(10,2))) 
      from SaleDetail sd
      where sd.SaleId = s.SaleId
    ) Total
  from Sale s
  join Customer cu on cu.CustomerId = s.CustomerId
  left join Coupon c on c.CouponId = s.CouponId
go

if OBJECT_ID('vMonthSalesPerUser') is not null
  drop view vMonthSalesPerUser
go

create view vMonthSalesPerUser as
  select 
    s.SiteUserId,
    sum(v.Total) total
  from SiteUser s
  join vSalesTotal v on v.SiteUserId = s.SiteUserId
  where Month(v.OnDate)=month(getdate()) and Year(v.OnDate)=Year(getdate())
  group by s.SiteUserId
go

if OBJECT_ID('vMyTeamReport') is not null
  drop view vMyTeamReport
go

create view vMyTeamReport as
  select 
    p.PyramidId,
    p.PyramidParentId,
    p.SiteUserId,
    v.total,
    p.ToReceive,
    p.FromOthers,
    p.PBV,
    bp.BPName,
    bp.EMail
  from Pyramid p
  left join vMonthSalesPerUser v on p.SiteUserId = v.SiteUserId
  join BP on bp.SiteUserId = p.SiteUserId
go

if OBJECT_ID('v3MonthSalesPerUser') is not null
  drop view v3MonthSalesPerUser
go
--select * from v3MonthSalesPerUser
create view v3MonthSalesPerUser as
  select 
    v.SaleId,
    v.OnDate,
    v.SiteUserId,
    v.CustomerName,
    v.Total
  from vSalesTotal v
  where v.OnDate> DATEADD(month,-3,getdate())
go
  
if OBJECT_ID('v3MonthsBonuses') is not null
  drop view v3MonthsBonuses
go
--select * from v3MonthsBonuses
create view v3MonthsBonuses as
  select 
    rh.ResultHistoryId, 
    bp.BPName,
    rh.Bonus,
    p.SiteUserId,
    p2.SiteUserId ParentSiteId,
    rh.Month,
    rh.Year
  from ResultHistory rh
  join bp on bp.BPId = rh.BpId
  join Pyramid p on p.SiteUserId = bp.SiteUserId
  join Pyramid p2 on p2.PyramidId = p.PyramidParentId
  where DATEFROMPARTS(rh.Year, rh.Month, 1)> DATEADD(month,-3,getdate())
go

  
if OBJECT_ID('vBoiko') is not null
  drop view vBoiko
go

create view vBoiko as

with sales(id) as
(
  select s.SaleId
  from Sale s
  where 
    DATEPART(month, s.OnDate)= DATEPART(month, dateadd(month,-1,getdate())) and
    DATEPART(year, s.OnDate)= DATEPART(year, dateadd(month,-1,getdate()))
), products (Id, Barcode, Quantity) as
(
  select p.ProductId, p.Barcode, sum(sd.Quantity)
  from sales s
  join SaleDetail sd on sd.SaleId = s.id
  join Product p on p.ProductId = sd.ProductId
  group by p.ProductId, p.Barcode 
) 
select * from products
go


if OBJECT_ID('vMicroinvest') is not null
  drop view vMicroinvest
go

create view vMicroinvest as
  select 
    s.SaleId,
    s.OnDate,
    c.CustomerName,
    c.City,
    c.Address,
    co.CountryName,
    (
      select sum((sd.Price*sd.Quantity)*(100-sd.Discount)/100)
      from SaleDetail sd 
      where sd.SaleId = s.SaleId
    )  + s.DeliveryPrice Total
  from Sale s
  join Customer c on c.CustomerId = s.CustomerId
  join Country co on co.CountryId= c.CountryId
  where 
    DATEPART(month, s.OnDate)= DATEPART(month, dateadd(month,-1,getdate())) and
    DATEPART(year, s.OnDate)= DATEPART(year, dateadd(month,-1,getdate()))
  
go

