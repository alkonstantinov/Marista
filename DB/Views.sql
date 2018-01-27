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

  