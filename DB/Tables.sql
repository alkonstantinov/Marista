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
  CurrentSessionId nvarchar(50) null,
  constraint pk_SiteUserId primary key (SiteUserId),
  constraint fk_SiteUser_LevelId foreign key (LevelId) references Level(LevelId)
)
go

exec p_ak_create_fk_indeces 'SiteUser'
go

create index ix_SessionId on SiteUser (CurrentSessionId asc)
go

insert into SiteUser(Username, Password, LevelId)
values ('admin', '202cb962ac59075b964b07152d234b70', 1), ('testOne', '202cb962ac59075b964b07152d234b70', 1)
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


if OBJECT_ID('BonusSize') is not null
begin
  exec p_ak_drop_all_foreign_keys 'BonusSize'
  drop table BonusSize
end
go

create table BonusSize 
(
  BonusSizeId int not null identity(1,1),
  FromBonus decimal (10,2) not null, -- От цена
  ToBonus decimal (10,2) not null, -- До цена
  BonusPercent decimal (10,2) not null, -- процент
  constraint pk_BonusSizeId primary key (BonusSizeId)
)
go

exec p_ak_create_fk_indeces 'BonusSize'


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


if OBJECT_ID('Chat') is not null
begin
  exec p_ak_drop_all_foreign_keys 'Chat'
  drop table Chat
end
go

create table Chat 
(
  ChatId int not null identity(1,1), --идентификатор на чат
  SiteUserId int not null, -- собственик на чата
  constraint pk_ChatId primary key (ChatId),
  constraint fk_Chat_SiteUserId foreign key (SiteUserId) references SiteUser(SiteUserId)
)
go

exec p_ak_create_fk_indeces 'Chat'
go

if OBJECT_ID('ChatItem') is not null
begin
  exec p_ak_drop_all_foreign_keys 'ChatItem'
  drop table ChatItem
end
go


insert into Chat(SiteUserId) values (1)
go
create table ChatItem 
(
  ChatItemId int not null identity(1,1), -- Идентификатор на реплика
  ChatId int not null, -- Идентификатор на чат 
  SiteUserId int not null, -- Кой е казал репликата
  OnDate datetime2 not null, -- в колко часа
  Said nvarchar(max), -- реплика
  Attachment varbinary(max), -- файл
  constraint pk_ChatItemId primary key (ChatItemId),  
  constraint fk_ChatItem_ChatId foreign key (ChatId) references Chat(ChatId),
  constraint fk_ChatItem_SiteUserId foreign key (SiteUserId) references SiteUser(SiteUserId)
)
go
exec p_ak_create_fk_indeces 'ChatItem'
go

if OBJECT_ID('Coupon') is not null
begin
  exec p_ak_drop_all_foreign_keys 'Coupon'
  drop table Coupon
end
go

create table Coupon
(
  CouponId int not null identity(1,1),
  UniqueId nvarchar (10) not null, -- уникален код на купона
  SiteUserId int not null, --кой генерира купона
  Expires datetime2 not null, -- на коя дата изтича валидността на купона
  HCategoryId int null,  --ИЛИ всички продукти от тази категория са с намаление
  VCategoryId int null,  -- ИЛИ всички продукти от тази категория  са с намаление
  ProductId int null, --ИЛИ  само този продукт е с намаление
  ForAll bit not null default 0, -- ИЛИ абсолютно всички продукти са с намаление
  Discount int not null default 0, -- размер на намалението в проценти от 1 до 23
  Used bit not null default 0, -- Дали е използван купонът
  Img varbinary (max), -- Изображение на купона
  constraint pk_CouponId primary key (CouponId),  
  constraint fk_Coupon_SiteUserId foreign key (SiteUserId) references SiteUser(SiteUserId),
  constraint fk_Coupon_HCategoryId foreign key (HCategoryId) references HCategory(HCategoryId),
  constraint fk_Coupon_VCategoryId foreign key (VCategoryId) references VCategory(VCategoryId),
  constraint fk_Coupon_ProductId foreign key (ProductId) references Product(ProductId)
)
go

exec p_ak_create_fk_indeces 'Coupon'
go



if OBJECT_ID('Pyramid') is not null
begin
  exec p_ak_drop_all_foreign_keys 'Pyramid'
  drop table Pyramid
end
go

create table Pyramid 
(
  PyramidId int not null identity(1,1),
  PyramidParentId int null,
  PyramidSpinoffParentId int null,
  SiteUserId int not null,
  PBV decimal (10,2) not null default 0,  
  Prcnt decimal(10,2) not null default 0,
  ParentPrcnt decimal(10,2) not null default 0,
  SoParentPrcnt decimal(10,2) not null default 0,
  SoPrcnt decimal(10,2) not null default 0,
  FromOthers decimal(10,2) not null default 0,
  ToReceive decimal(10,2) not null default 0
  constraint pk_PyramidId primary key (PyramidId),
  constraint fk_Pyramid_PyramidParentId foreign key (PyramidParentId) references Pyramid(PyramidId),
  constraint fk_Pyramid_PyramidSpinoffParentId foreign key (PyramidSpinoffParentId) references Pyramid(PyramidId),
  constraint fk_Pyramid_SiteUserId foreign key (SiteUserId) references SiteUser(SiteUserId)
  

)
go

exec p_ak_create_fk_indeces 'Pyramid'
go

if OBJECT_ID('MarketingMaterial') is not null
begin
  exec p_ak_drop_all_foreign_keys 'MarketingMaterial'
  drop table MarketingMaterial
end
go

create table MarketingMaterial 
(
  MarketingMaterialId int not null identity(1,1),
  Title nvarchar(200) not null,
  FileName nvarchar(500) not null,
  Content varbinary(max) not null,
  constraint pk_MarketingMaterialId primary key (MarketingMaterialId)
)
go

exec p_ak_create_fk_indeces 'MarketingMaterial'
go


if OBJECT_ID('Country') is not null
begin
  exec p_ak_drop_all_foreign_keys 'Country'
  drop table Country
end
go

create table Country
(
  CountryId int not null,
  CountryName nvarchar(200) not null,
  constraint pk_CountryId primary key (CountryId)
)
go

exec p_ak_create_fk_indeces 'Country'
go

insert into Country(CountryId, CountryName) values (1, 'Bulgaria'), (2, 'Germany')
go


if OBJECT_ID('BP') is not null
begin
  exec p_ak_drop_all_foreign_keys 'BP'
  drop table BP
end
go

create table BP
(
  BPId int not null identity(1,1),
  BPName nvarchar(200) not null,
  EMail nvarchar(200) not null,
  PayPal nvarchar(200) not null,
  Address nvarchar(max) not null,
  CountryId int not null,
  SiteUserId int not null,
  Files varbinary(max) not null,
  FileName nvarchar(200) not null,
  Active bit not null default 0,
  constraint pk_BPId primary key (BPId),
  constraint fk_BP_CountryId foreign key (CountryId) references Country(CountryId),
  constraint fk_BP_SiteUserId foreign key (SiteUserId) references SiteUser(SiteUserId)
)
go

exec p_ak_create_fk_indeces 'BP'
go
