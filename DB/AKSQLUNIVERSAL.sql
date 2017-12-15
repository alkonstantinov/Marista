
if object_id ('f_ak_split_string') is not null
  drop function f_ak_split_string
go

create function f_ak_split_string (@str nvarchar (max), @delim nvarchar (max)) 
returns 
  @SlittedStrings table 
  (
    id int identity (1,1) primary key,
    line nvarchar (max)
  ) as
begin

  ;with tbl as
  (
    select 
      case
        when charindex (@delim, @str)=0 then @str 
        when charindex (@delim, @str)>0 then substring (@str,1,charindex (@delim, @str)-1)        
      end item,
      case
        when charindex (@delim, @str)=0 then null 
        when charindex (@delim, @str)>0 then substring (@str,charindex (@delim, @str)+1,len(@str))
        else null
      end rest
    union all
    select 
      case
        when charindex (@delim, tbl.rest)=0 then tbl.rest 
        when charindex (@delim, tbl.rest)>0 then substring (tbl.rest,1,charindex (@delim, tbl.rest)-1)
        else null
      end item,
      case
        when charindex (@delim, tbl.rest)=0 and tbl.rest!='' then null 
        when charindex (@delim, tbl.rest)>0 then substring (tbl.rest,charindex (@delim, tbl.rest)+1,len(@str))
        else null
      end rest
      from tbl
      where tbl.rest is not null
      
  )
  insert into @SlittedStrings(line)
  select item from tbl 
  where item is not null
  option (MAXRECURSION 10000);

  return
end
go

if OBJECT_ID('p_ak_drop_all_foreign_keys') is not null 
  DROP PROCEDURE p_ak_drop_all_foreign_keys 
go

CREATE PROCEDURE p_ak_drop_all_foreign_keys @tableName nvarchar (max) as 
BEGIN
  CREATE TABLE #statements 
  ( 
    statement nvarchar (max) 
  ) 
  
  ;WITH ForeignKeyInfo as 
  ( 
    SELECT 
      OBJECT_NAME(f.parent_object_id) AS TableName, 
      f.name as fkname, 
      OBJECT_NAME (f.referenced_object_id) AS ReferenceTableName 
    FROM sys.foreign_keys AS f 
    WHERE OBJECT_NAME (f.referenced_object_id) = @tableName 
  ) 
  INSERT INTO #statements (statement) 
    SELECT 
      'alter TABLE '+TableName+' DROP constraint '+fkname 
    FROM ForeignKeyInfo 
  
  DECLARE @q nvarchar (max) 
  DECLARE crsStatements cursor for SELECT statement FROM #statements 
  OPEN crsStatements 
  FETCH next FROM crsStatements INTO @q 
  WHILE @@FETCH_STATUS=0 
  BEGIN 
    EXECUTE (@q) 
    FETCH next FROM crsStatements INTO @q 
  END 
  CLOSE crsStatements 
  DEALLOCATE crsStatements 
  DROP TABLE #statements 
END
go

IF OBJECT_ID('p_ak_create_fk_indeces') IS NOT NULL
  DROP PROCEDURE p_ak_create_fk_indeces
GO
-- _p_create_fk_indexes 'COURTS'
CREATE PROCEDURE p_ak_create_fk_indeces @tableName nvarchar (max) AS
BEGIN
  CREATE TABLE #statements 
  ( 
    statement nvarchar (max) 
  ) 
  
  ;WITH ForeignKeyInfo as 
  ( 
    SELECT 
      OBJECT_NAME(f.parent_object_id) AS TableName, 
      COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName, 
      OBJECT_NAME (f.referenced_object_id) AS ReferenceTableName, 
      COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferenceColumnName, 
      'idx_'+OBJECT_NAME(f.parent_object_id)+'_'+COL_NAME(fc.parent_object_id, fc.parent_column_id) IndexName 
    FROM sys.foreign_keys AS f 
    INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id 
    WHERE OBJECT_NAME (f.parent_object_id) = @tableName 
  ) 
  INSERT INTO #statements (statement) 
    SELECT 
    'if exists (SELECT * FROM sysindexes WHERE id=object_id('''+TableName+''') and name='''+Indexname+''') '+ 
    'DROP index '+indexname+' on '+tablename+'; '+ 
    'CREATE index '+indexname+' on '+tablename+'('+columnname+'); ' FROM ForeignKeyInfo 

  DECLARE @q nvarchar (max) 
  DECLARE crsStatements cursor for SELECT statement FROM #statements 
  OPEN crsStatements 
  FETCH next FROM crsStatements INTO @q 
  WHILE @@FETCH_STATUS=0 
  BEGIN 
  --print @q
    EXECUTE (@q) 
    FETCH next FROM crsStatements INTO @q 
  END 
  CLOSE crsStatements 
  DEALLOCATE crsStatements 
  DROP TABLE #statements 
END
go



IF OBJECT_ID('p_ak_create_file_system') IS NOT NULL
  DROP PROCEDURE p_ak_create_file_system
GO
-- _p_create_fk_indexes 'COURTS'
CREATE PROCEDURE p_ak_create_file_system AS
BEGIN
  DECLARE @FileSystem int
  DECLARE @RetCode int
  EXECUTE @RetCode = sp_OACreate 'Scripting.FileSystemObject' , @FileSystem OUTPUT
  IF (@@ERROR|@RetCode > 0 Or @FileSystem < 0)
    RAISERROR ('could not create FileSystemObject',16,1)
  
  RETURN @FileSystem
END
GO

IF OBJECT_ID('p_ak_create_file') IS NOT NULL
  DROP PROCEDURE p_ak_create_file
GO
-- _p_create_fk_indexes 'COURTS'
CREATE PROCEDURE p_ak_create_file @FileSystem int, @Filename nvarchar (max) AS
BEGIN
  DECLARE @FileHandle int
  DECLARE @RetCode int
      
  EXECUTE @RetCode = sp_OAMethod @FileSystem , 'OpenTextFile' , @FileHandle OUTPUT , @Filename, 2, 1
  IF (@@ERROR|@RetCode > 0 Or @FileHandle < 0)
    RAISERROR ('could not create File',16,1)
  RETURN @FileHandle
END
GO


IF OBJECT_ID('p_ak_rename_file') IS NOT NULL
  DROP PROCEDURE p_ak_rename_file
GO
-- p_ak_rename_file 'COURTS'
CREATE PROCEDURE p_ak_rename_file @FileSystem int, @Filename nvarchar (max), @NewFileName nvarchar (max) AS
BEGIN
  DECLARE @FileHandle int
  declare @OnSuccess int
  DECLARE @RetCode int
  EXEC @RetCode = sp_OAMethod @FileSystem, 'RenameFileOrDir', @OnSuccess OUT, @FileName, @NewFileName
  IF (@@ERROR|@RetCode > 0 Or @FileHandle < 0)
    RAISERROR ('could not rename File',16,1)
  RETURN @FileHandle
END
GO


IF OBJECT_ID('p_ak_write_to_file') IS NOT NULL
  DROP PROCEDURE p_ak_write_to_file
GO
-- _p_create_fk_indexes 'COURTS'
CREATE PROCEDURE p_ak_write_to_file @FileHandle int, @text nvarchar(max) AS
BEGIN
  DECLARE @RetCode int
      
  EXECUTE @RetCode = sp_OAMethod @FileHandle , 'Write' , NULL , @text
      IF (@@ERROR|@RetCode > 0 )
           RAISERROR ('could not write to File',16,1)
  
END
GO

IF OBJECT_ID('p_ak_delete_file') IS NOT NULL
  DROP PROCEDURE p_ak_delete_file
GO
-- _p_create_fk_indexes 'COURTS'
CREATE PROCEDURE p_ak_delete_file @FileSystem int, @Filename nvarchar(max) AS
BEGIN
  DECLARE @RetCode int
      
  EXECUTE @RetCode = sp_OAMethod @FileSystem , 'DeleteFile' , NULL , @Filename
      IF (@@ERROR|@RetCode > 0 )
           RAISERROR ('could not delete File',16,1)
  
END
GO


IF OBJECT_ID('p_ak_close_file') IS NOT NULL
  DROP PROCEDURE p_ak_close_file
GO
-- p_ak_close_file 'COURTS'
CREATE PROCEDURE p_ak_close_file @FileHandle int AS
BEGIN
  DECLARE @RetCode int
  EXECUTE @RetCode = sp_OAMethod @FileHandle , 'Close'
    IF (@@ERROR|@RetCode > 0)
      RAISERROR ('Could not close file ',16,1)
  EXEC sp_OADestroy @filehandle
    IF (@@ERROR|@RetCode > 0)
      RAISERROR ('Could not destroy file object',16,1)
  
END
GO

IF OBJECT_ID('p_ak_close_file_system') IS NOT NULL
  DROP PROCEDURE p_ak_close_file_system
GO
-- p_ak_close_file 'COURTS'
CREATE PROCEDURE p_ak_close_file_system @FileSystem int AS
BEGIN
  EXEC sp_OADestroy @FileSystem
  
END
GO


if object_id ('f_ak_trim') is not null
  drop function f_ak_trim
go

create function f_ak_trim (@str nvarchar (max)) 
returns nvarchar(max)
begin
  if @str is null 
    return @str
  declare @res nvarchar(max) = @str
  
  while LEN (@res)>0 and UNICODE (SUBSTRING(@res,1,1))<33
    select @res = SUBSTRING (@res,2,LEN(@res)-1)
    
  while LEN (@res)>0 and UNICODE (SUBSTRING(@res,1,LEN(@res)))<33
    select @res = SUBSTRING (@res,1,LEN(@res)-1)  
    
    
  return @res  
  
end
go
