USE [hrmplDB]
GO
/****** Object:  UserDefinedFunction [dbo].[sf_TimeDiff]    Script Date: 30/11/2017 4:29:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[sf_TimeDiff]
(
	-- Add the parameters for the function here
	@startTime time(0),
	@endTime time(0)
)
RETURNS   time
AS
BEGIN
	-- Declare the return variable here
	declare @result as time(0);
	SELECT @result= DATEADD(SECOND, DATEdiff(SECOND, @startTime,@endTime), 0)
	return @result
END
