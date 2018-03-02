use marista
go
alter table ProductPicture
  add IsVideo bit not null default 0
go