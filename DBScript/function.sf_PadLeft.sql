USE [hrmplDB]
GO
/****** Object:  UserDefinedFunction [dbo].[sf_PadLeft]    Script Date: 30/11/2017 4:29:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[sf_PadLeft]
(
	-- Add the parameters for the function here
	@value varchar(10),
	@len int
)
RETURNS varchar(10)
AS
BEGIN
	-- Declare the return variable here
	declare @result varchar(10);
	set @result=REPLACE(STR(@value, @len), SPACE(1), '0')
	return @result

END
