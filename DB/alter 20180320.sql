use marista

alter table product
add 
  LongDescription nvarchar(max) not null default '',
  Benefits nvarchar(max) not null default ''


  select * from product