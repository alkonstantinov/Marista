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


if OBJECT_ID('HCategory') is not null
begin
  exec p_ak_drop_all_foreign_keys 'HCategory'
  drop table HCategory
end
go

create table HCategory 
(
  HCategoryId int not null identity(1,1),
  CategoryName nvarchar(200) not null
  constraint pk_HCategoryId primary key (HCategoryId)
)
go

exec p_ak_create_fk_indeces 'HCategory'
go

insert into HCategory(CategoryName)
values ('horizontal cat one'), ('horizontal cat two')
go


if OBJECT_ID('VCategory') is not null
begin
  exec p_ak_drop_all_foreign_keys 'VCategory'
  drop table VCategory
end
go

create table VCategory 
(
  VCategoryId int not null identity(1,1),
  CategoryName nvarchar(200) not null
  constraint pk_VCategoryId primary key (VCategoryId)
)
go

exec p_ak_create_fk_indeces 'VCategory'
go

insert into VCategory(CategoryName)
values ('vertical cat one'), ('vertical cat two')
go


if OBJECT_ID('Product') is not null
begin
  exec p_ak_drop_all_foreign_keys 'Product'
  drop table Product
end
go

create table Product 
(
  ProductId int not null identity(1,1),
  Name nvarchar(100) not null, --ИМЕ
  Description nvarchar(max) not null, --ОПИСАНИЕ
  Price decimal (10,2) not null, --ЦЕНА
  PromotionalPrice decimal (10,2) null, --ПРОМОЦИОНАЛНА ЦЕНА
  VCategoryId int not null, --ВЕРТИКАЛНА КАТЕГОРИЯ - от таблица VCategory
  HCategoryId int not null, --ХОРИЗОНТАЛНА КАТЕГОРИЯ - от таблица HCategory
  Picture varbinary(max) not null, --СНИМКА
  constraint pk_ProductId primary key (ProductId),
  constraint fk_Product_VCategoryId foreign key (VCategoryId) references VCategory(VCategoryId),
  constraint fk_Product_HCategoryId foreign key (HCategoryId) references HCategory(HCategoryId)
)
go

exec p_ak_create_fk_indeces 'Product'
go


if OBJECT_ID('RelatedProduct') is not null
begin
  exec p_ak_drop_all_foreign_keys 'RelatedProduct'
  drop table RelatedProduct
end
go

create table RelatedProduct 
(
  FromProductId int not null,
  ToProductId int not null,
  constraint pk_RelatedProductId primary key (FromProductId, ToProductId),
  constraint fk_Product_FromProductId foreign key (FromProductId) references Product(ProductId),
  constraint fk_Product_ToProductId foreign key (ToProductId) references Product(ProductId)
)
go

exec p_ak_create_fk_indeces 'RelatedProduct'
go


if OBJECT_ID('SOBonusSize') is not null
begin
  exec p_ak_drop_all_foreign_keys 'SOBonusSize'
  drop table SOBonusSize
end
go

create table SOBonusSize 
(
  SOBonusSizeId int not null identity(1,1),
  FromSO int not null, -- от брой спинофи
  ToSO int not null, -- до брой спинофи
  BonusPercent decimal (10,2) not null, --процент
  constraint pk_SOBonusSizeId primary key (SOBonusSizeId)
)
go

exec p_ak_create_fk_indeces 'SOBonusSize'
go


if OBJECT_ID('Constant') is not null
begin
  exec p_ak_drop_all_foreign_keys 'Constant'
  drop table [Constant]
end
go

create table [Constant]
(
  ConstantId int not null,
  Name nvarchar(100) not null, --ИМЕ НА КОНСТАНТА
  Value decimal (10,2) not null, --СТОЙНОСТ
  constraint pk_ConstantId primary key (ConstantId)
)
go

exec p_ak_create_fk_indeces 'Constant'
go